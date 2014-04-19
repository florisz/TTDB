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
    class WorkbenchException : Exception
    {
        public WorkbenchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    class WorkbenchConversionException : WorkbenchException
    {
        public WorkbenchConversionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    class Dsl2Its
    {
        static internal CaseFileSpecification LoadCaseFileSpecification(CaseFileModelSpec modelSpec)
        {
            CaseFileSpecification spec = new CaseFileSpecification();

            spec.Link = new CaseFileSpecificationLink[2];

            spec.Link[0] = new CaseFileSpecificationLink() {rel = CaseFileSpecificationLinkRel.objectmodel, href = modelSpec.ModelRoot.ObjectModelSpec.Self };
            spec.Link[1] = new CaseFileSpecificationLink() { rel = CaseFileSpecificationLinkRel.self, href = "" };

            spec.Name = modelSpec.Name;
            spec.UriTemplate = modelSpec.UriTemplate;

            spec.Structure = new CaseFileSpecificationStructure();

            spec.Structure.Entity = LoadCaseFileEntity(modelSpec.CaseFileRootEntity);

            return spec;
        }

        private static CaseFileSpecificationEntity LoadCaseFileEntity(CaseFileEntity caseFileEntity)
        {
            // Load entity
            var entity = new CaseFileSpecificationEntity()
            {
                Name = caseFileEntity.Name,
                Type = caseFileEntity.ModelEntity.Name,
            };

            // Load relation
            List<CaseFileSpecificationRelation> relations = new List<CaseFileSpecificationRelation>();
            foreach (CaseFileRelation r in caseFileEntity.ChildCaseFileRelations)
            {
                relations.Add(new CaseFileSpecificationRelation()
                {
                    Name = r.Name,
                    Type = r.ModelRelation.Name,
                    Entity = LoadCaseFileEntity(r.ChildCaseFileEntity),
                });
            }
            entity.Relation = relations.ToArray();

            return entity;
        }

        static internal ObjectModel LoadObjectModel(ModelRoot fromRoot)
        {
            ObjectModel model = new ObjectModel();

            model.Link = new ObjectModelLink() { href = "", rel = ObjectModelLinkRel.self };
            model.Name = fromRoot.ObjectModelSpec.Name;

            // add definition (entity / relation)
            List<ObjectDefinition> od = new List<ObjectDefinition>();

            var allTypes = ObjectModelSpecHasTypes.GetLinksToTypes(fromRoot.ObjectModelSpec);
            foreach (ModelEntity t in allTypes.Where(t => t.ModelType is ModelEntity).Select(t => t.ModelType))
            {
                od.Add(new ObjectDefinition()
                {
                    Name = t.Name,
                    ObjectType = ObjectType.entity,
                    Properties = LoadObjectDefinitionProperty(t),
                    ComplexProperties = LoadComplexProperties(t)
                });
            }
            foreach (ModelRelation t in allTypes.Where(x => x.ModelType is ModelRelation).Select(y => y.ModelType))
            {
                od.Add(new ObjectDefinition()
                {
                    Name = t.Name,
                    ObjectType = ObjectType.relation,
                    Properties = LoadObjectDefinitionProperty(t),
                    ComplexProperties = LoadComplexProperties(t)
                });
            }
            model.ObjectDefinitions = od.ToArray();


            // Add Relations
            var or = new List<ObjectRelation>();
            foreach (ModelEntity e in ObjectModelSpecHasTypes.GetLinksToTypes(fromRoot.ObjectModelSpec).Where(x => x.ModelType is ModelEntity).Select(y => y.ModelType))
            {
                foreach (var link in EntityHasRelations.GetLinksToToRelations(e))
                {
                    or.Add(new ObjectRelation()
                    {
                        Source = link.ModelEntity.Name,
                        Target = link.ModelRelation.Name,
                        MaxOccurs = "1"//TODO maxoccurs
                    });
                }
            }
            foreach (ModelRelation relation in ObjectModelSpecHasTypes.GetLinksToTypes(fromRoot.ObjectModelSpec).Where(x => x.ModelType is ModelRelation).Select(y => y.ModelType))
            {
                or.Add(new ObjectRelation()
                    {
                        Source = relation.Name,
                        Target = relation.Entity.Name,
                        MaxOccurs = "1"//TODO maxoccurs
                    });
            }

            model.ObjectRelations = or.ToArray();
            return model;
        }

        private static ObjectDefinitionComplexProperty[] LoadComplexProperties(ModelType t)
        {
            var p = new List<ObjectDefinitionComplexProperty>();

            foreach (ModelComplexType xt in t.ModelComplexTypes)
            {
                var link = ModelTypeReferencesModelComplexTypes.GetLink(t, xt);

                string containerName = link.ContainerName == string.Empty ? xt.Name : link.ContainerName;
                // Add a container like: <adressen> <adres/> ... </adressen>
                p.Add(new ObjectDefinitionComplexProperty()
                {
                    Name = containerName,
                    MinOccurs = 0,
                    MaxOccurs = "1",
                    ComplexProperties = new[]{new ObjectDefinitionComplexProperty()
                    {
                        Name = xt.Name,
                        MinOccurs = 0,
                        MaxOccurs = link.IsCollection ?  "unbounded" : "1",
                        Properties = LoadObjectDefinitionProperty(xt),
                        ComplexProperties = LoadComplexProperties(xt)
                    }}
                });

            }

            return p.ToArray();
        }

        private static ObjectDefinitionProperty[] LoadObjectDefinitionProperty(ModelType mt)
        {
            var p = new List<ObjectDefinitionProperty>();

            foreach (ModelAttribute attribute in ModelTypeHasAttributes.GetLinksToAttributes(mt).Select(a => a.ModelAttribute))
            {
                p.Add(new ObjectDefinitionProperty()
                {
                    Name = attribute.Name,
                    Type = attribute.Type,
                    RequiredSpecified = attribute.Required,
                    Required = attribute.Required
                });
            }

            return p.ToArray();
        }


    }
}
