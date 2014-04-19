using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams.GraphObject;
using Microsoft.VisualStudio.Modeling.Diagrams;

using Luminis.Its.Client.Model;

namespace Luminis.Its.Workbench.DslPackage
{
    class Its2Dsl
    {
        static internal CaseFileModelSpec LoadCaseFileSpecInDsl(ModelRoot toRoot, CaseFileSpecification fromItsModel)
        {
            Store store = toRoot.Store;
            var tx = store.TransactionManager.BeginTransaction();

            CaseFileModelSpec caseFile = HasCaseFileModelSpec.GetCaseFileModelSpecs(toRoot).Where(s => s.Name == fromItsModel.Name).FirstOrDefault();
            if (caseFile == null)
            {
                // load only when not already loaded
                caseFile = new CaseFileModelSpec(store)
                {
                    Name = fromItsModel.Name,
                    ModelRoot = toRoot,
                    Self = fromItsModel.Link.Where(x => x.rel == CaseFileSpecificationLinkRel.self).FirstOrDefault().href,
                    ObjectModelSpec = toRoot.ObjectModelSpec.Self,
                    UriTemplate = fromItsModel.UriTemplate,
                };
                caseFile.CaseFileRootEntity = LoadCaseFileEntity(caseFile, fromItsModel.Structure.Entity);
            }

            tx.Commit();

            return caseFile;
        }

        private static CaseFileEntity LoadCaseFileEntity(CaseFileModelSpec caseFile, CaseFileSpecificationEntity itsEntity)
        {
            if (itsEntity != null)
            {
                // load entity
                CaseFileEntity entity = new CaseFileEntity(caseFile.Store)
                {
                    CaseFileModelSpec = caseFile,
                    Name = itsEntity.Name,
                };

                ObjectModelSpec om = caseFile.ModelRoot.ObjectModelSpec;
                var tl = ObjectModelSpecHasTypes.GetLinksToTypes(om).FirstOrDefault(t => t.ModelType.Name == itsEntity.Type);
                entity.ModelEntity = tl.ModelType as ModelEntity;

                // load Relations
                if (itsEntity.Relation != null)
                {
                    foreach (CaseFileSpecificationRelation r in itsEntity.Relation)
                    {
                        LoadCaseFileRelation(caseFile, entity, r);
                    }
                }
                return entity;
            }
            return null;
        }

        private static void LoadCaseFileRelation(CaseFileModelSpec caseFile, CaseFileEntity entity, CaseFileSpecificationRelation r)
        {
            // load relation
            CaseFileRelation relation = new CaseFileRelation(entity.Store)
            {
                CaseFileModelSpec = caseFile,
                Name = r.Name,
                ParentCaseFileEntity = entity,
            };

            ObjectModelSpec om = caseFile.ModelRoot.ObjectModelSpec;
            var tl = ObjectModelSpecHasTypes.GetLinksToTypes(om).FirstOrDefault(t => t.ModelType.Name == r.Type);
            relation.ModelRelation = tl.ModelType as ModelRelation;

            var childEntity = LoadCaseFileEntity(caseFile, r.Entity);
            childEntity.ParentCaseFileRelation = relation;
        }


        static internal void loadObjectModelInDsl(ModelRoot toRoot, ObjectModel fromItsModel)
        {
            Store store = toRoot.Store;
            var tx = store.TransactionManager.BeginTransaction();


            if (toRoot.ObjectModelSpec != null) toRoot.ObjectModelSpec.Delete();
            ObjectModelSpec om = new ObjectModelSpec(store)
            {
                ModelRoot = toRoot,
                Name = fromItsModel.Name,
                Self = fromItsModel.Link.href,
            };

            toRoot.CaseFileModelSpecs.Clear();

            foreach (ObjectDefinition od in fromItsModel.ObjectDefinitions)
            {
                switch (od.ObjectType)
                {
                    case ObjectType.entity:
                        ModelEntity entity;
                        entity = new ModelEntity(store)
                        {
                            Name = od.Name,
                            ObjectModelSpec = om,
                        };
                        LoadModelAttribute(store, entity, od.Properties);
                        LoadModelComplexType(store, om, entity, od.ComplexProperties);

                        break;

                    case ObjectType.relation:
                        ModelRelation relation;
                        relation = new ModelRelation(store)
                        {
                            Name = od.Name,
                            ObjectModelSpec = om,
                        };
                        LoadModelAttribute(store, relation, od.Properties);
                        LoadModelComplexType(store, om, relation, od.ComplexProperties);
                        break;

                    default:
                        break;
                }
            }

            // object relations
            var types = ObjectModelSpecHasTypes.GetLinksToTypes(om);
            foreach (ObjectRelation relation in fromItsModel.ObjectRelations)
            {
                var source = types.First(n => n.ModelType.Name == relation.Source);
                var target = types.First(n => n.ModelType.Name == relation.Target);

                // E->R
                if (source.ModelType is ModelEntity)
                {
                    new EntityHasRelations(source.ModelType as ModelEntity, target.ModelType as ModelRelation);
                }

                // R->E
                if (source.ModelType is ModelRelation)
                {
                    new RelationHasEntity(source.ModelType as ModelRelation, target.ModelType as ModelEntity);
                }
            }

            tx.Commit();
        }


        private static void LoadModelAttribute(Store store, ModelType entity, ObjectDefinitionProperty[] properties)
        {
            if (properties != null)
            {
                ModelAttribute attribute;
                foreach (ObjectDefinitionProperty prop in properties)
                {
                    attribute = new ModelAttribute(store) { Name = prop.Name, Type = prop.Type, ModelType = entity };
                }
            }
        }
        private static void LoadModelComplexType(Store store, ObjectModelSpec om, ModelType parent, ObjectDefinitionComplexProperty[] complexProperties)
        {
            if (complexProperties != null)
            {
                ModelComplexType complexType;
                foreach (ObjectDefinitionComplexProperty p in complexProperties)
                {
                    if (IsContainter(p))
                    {
                        // special case this is a generated complex properite that is not a ModelComplexType but a named reference

                        // Add branch below (only one complex type)
                        ObjectDefinitionComplexProperty pp = p.ComplexProperties[0];
                        complexType = new ModelComplexType(store) { Name = pp.Name, ObjectModelSpec = om };
                        LoadModelComplexType(store, om, complexType, pp.ComplexProperties);

                        // Make relation and name it
                        var link = new ModelTypeReferencesModelComplexTypes(parent, complexType);
                        link.ContainerName = p.Name;
                        link.IsCollection = pp.MaxOccurs == "unbounded";
                    }
                    else
                    {
                        // normal case, Add  the complex type
                        var tl = ObjectModelSpecHasTypes.GetLinksToTypes(om).FirstOrDefault(t => t.ModelType.Name == p.Name);
                        if (tl == null)
                        {
                            // Add complex type
                            complexType = new ModelComplexType(store) { Name = p.Name, ObjectModelSpec = om };

                            LoadModelAttribute(store, complexType, p.Properties);
                            LoadModelComplexType(store, om, complexType, p.ComplexProperties);
                        }
                        else
                        {
                            // take reference to existing
                            complexType = tl.ModelType as ModelComplexType;
                        }
                        // todo relate to entity / relation
                        var link = new ModelTypeReferencesModelComplexTypes(parent, complexType);
                        link.ContainerName = p.Name;
                        link.IsCollection = p.MaxOccurs == "unbounded";
                    }
                }
            }
        }

        private static bool IsContainter(ObjectDefinitionComplexProperty p)
        {
            // this complex property is only allowed to have 1 complex property
            return (
                ((p != null) && (p.MaxOccurs == "1")) &&
                ((p.Properties == null) || (p.Properties != null && p.Properties.Length == 0)) &&
                (p.ComplexProperties != null && p.ComplexProperties.Length == 1));
        }
    }
}
