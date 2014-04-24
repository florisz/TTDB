using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using TimeTraveller.Services.Data;
using TimeTraveller.Services.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Unity;

namespace TimeTraveller.Services.ObjectModels.Impl
{
    public class ObjectModelService : AbstractTimeLineService<ObjectModel>, IObjectModelService
    {
        #region Private Properties
        private static readonly string _objectModelXsd = "objectmodel.xsd";
        private static readonly string _objectModelXsdResourceName = string.Format("{0}.{1}", typeof(IObjectModelService).Namespace, _objectModelXsd);
        private const string _xmlSchemaNamespacePrefix = "xs";
        private const string _xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        #endregion

        #region Constructors
        public ObjectModelService(ILogger logger, IUnity container, IDataService dataservice)
            : base(ItemType.ObjectModel, logger, container, dataservice)
        {
        }
        #endregion

        #region AbstractTimeLineService Members
        public override string XmlSchemaName
        {
            get { return _objectModelXsd; }
        }

        public override string XmlSchemaResourceName
        {
            get { return _objectModelXsdResourceName; }
        }

        public override ObjectModel Convert(IBaseObjectValue objectValue, Uri baseUri)
        {
            ObjectModel result = Convert(objectValue.Text, Encoding.UTF8);
            result.BaseObjectValue = objectValue;
            result.SelfUri = result.GetUri(baseUri, UriType.Version);

            return result;
        }

        public override void WriteDetailedSummaryInfo(ObjectModel item, Uri baseUri, XmlWriter xmlWriter)
        {
            IBaseObjectType casefilespecificationType = DataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification));
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", casefilespecificationType.Name);
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, casefilespecificationType.RelativeUri, item.ExtId, "/"));
            xmlWriter.WriteEndElement(); // Link

            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, BaseObjectType.RelativeUri, item.ExtId, ".xsd"));
            xmlWriter.WriteEndElement(); // Link
        }
        #endregion

        #region IObjectModel Members
        public override ObjectModel Convert(string objectModelXml, Encoding encoding)
        {
            try
            {
                ObjectModel result = base.Convert(objectModelXml, encoding);
                ConnectRelationSourceAndTarget(result);

                ConnectEntityRelations(result);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public string GetXmlSchema(ObjectModel objectmodel, Encoding encoding)
        {
            try
            {
                Logger.DebugFormat("GetXmlSchema({0}, {1})", objectmodel.Name, encoding.HeaderName);

                XmlSchema xmlSchema = new XmlSchema();
                xmlSchema.Id = objectmodel.Name;
                xmlSchema.AttributeFormDefault = XmlSchemaForm.Unqualified;
                xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;

                foreach (ObjectDefinition objectDefinition in objectmodel.ObjectDefinitions)
                {
                    XmlSchemaComplexType complexType = CreateXmlSchemaComplexType(objectDefinition);
                    xmlSchema.Items.Add(complexType);
                }

                StringBuilder resultXml = new StringBuilder();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace(_xmlSchemaNamespacePrefix, _xmlSchemaNamespace);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = encoding;
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);
                xmlSchema.Write(xmlWriter, nsmgr);
                xmlWriter.Close();

                Logger.DebugFormat("Schema=\r\n{0}", resultXml);

                string result = resultXml.ToString();

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }

        }
        #endregion

        #region Private Methods
        private static void ConnectEntityRelations(ObjectModel objectModel, ObjectDefinition sourceObjectDefinition)
        {
            // Select all object definitions where the ObjectType=entity or relation
            var objectDefinitionNames = (from definition in objectModel.ObjectDefinitions
                                             select definition.Name);

            // Select all relations where the sourceObjectDefinition is the "Source" and the "Target" is an entity of relation
            var relations = (from relation in objectModel.ObjectRelations
                             where relation.Source.Equals(sourceObjectDefinition.Name)
                             && objectDefinitionNames.Contains(relation.Target)
                             select relation);

            sourceObjectDefinition.EntityRelations = relations.ToArray();
        }

        private static void ConnectEntityRelations(ObjectModel objectModel)
        {
            if (objectModel.ObjectRelations != null)
            {
                foreach (ObjectDefinition objectDefinition in objectModel.ObjectDefinitions)
                {
                    ConnectEntityRelations(objectModel, objectDefinition);
                }
            }
        }

        private static void ConnectRelationSourceAndTarget(ObjectModel objectModel, ObjectRelation objectRelation)
        {
            objectRelation.SourceObjectDefinition = (from definition in objectModel.ObjectDefinitions
                                                     where definition.Name.Equals(objectRelation.Source)
                                                     select definition).First();

            objectRelation.TargetObjectDefinition = (from definition in objectModel.ObjectDefinitions
                                                     where definition.Name.Equals(objectRelation.Target)
                                                     select definition).First();
        }

        private static void ConnectRelationSourceAndTarget(ObjectModel objectModel)
        {
            if (objectModel.ObjectRelations != null)
            {
                foreach (ObjectRelation objectRelation in objectModel.ObjectRelations)
                {
                    ConnectRelationSourceAndTarget(objectModel, objectRelation);
                }
            }
        }

        private static XmlSchemaComplexType CreateXmlSchemaComplexType(ObjectDefinition objectDefinition)
        {
            XmlSchemaComplexType result = new XmlSchemaComplexType();
            result.Name = objectDefinition.Name;

            XmlSchemaAll all = new XmlSchemaAll();
            result.Particle = all;

            if (objectDefinition.Properties != null)
            {
                foreach (ObjectDefinitionProperty property in objectDefinition.Properties)
                {
                    XmlSchemaElement propertyElement = CreateXmlSchemaElement(property);
                    all.Items.Add(propertyElement);
                }
            }
            if (objectDefinition.ComplexProperties != null)
            {
                foreach (ObjectDefinitionComplexProperty property in objectDefinition.ComplexProperties)
                {
                    XmlSchemaElement complexPropertyElement = CreateXmlSchemaElement(property);
                    all.Items.Add(complexPropertyElement);
                }
            }

            return result;
        }

        private static XmlSchemaElement CreateXmlSchemaElement(ObjectDefinitionComplexProperty complexProperty)
        {
            XmlSchemaElement result = new XmlSchemaElement();
            result.Name = complexProperty.Name;
            result.MinOccurs = complexProperty.MinOccurs;
            result.MaxOccursString = complexProperty.MaxOccurs;

            XmlSchemaComplexType innerComplexType = new XmlSchemaComplexType();
            result.SchemaType = innerComplexType;

            XmlSchemaAll all = new XmlSchemaAll();
            innerComplexType.Particle = all;

            if (complexProperty.Properties != null)
            {
                foreach (ObjectDefinitionProperty property in complexProperty.Properties)
                {
                    XmlSchemaElement propertyElement = CreateXmlSchemaElement(property);
                    all.Items.Add(propertyElement);
                }
            }
            if (complexProperty.ComplexProperties != null)
            {
                foreach (ObjectDefinitionComplexProperty property in complexProperty.ComplexProperties)
                {
                    XmlSchemaElement complexPropertyElement = CreateXmlSchemaElement(property);
                    all.Items.Add(complexPropertyElement);
                }
            }

            return result;
        }

        private static XmlSchemaElement CreateXmlSchemaElement(ObjectDefinitionProperty property)
        {
            XmlSchemaElement result = new XmlSchemaElement();
            result.Name = property.Name;
            result.SchemaTypeName = new XmlQualifiedName(property.Type, _xmlSchemaNamespace);
            if (property.RequiredSpecified && property.Required)
            {
                result.MinOccurs = 1;
                result.MaxOccurs = 1;
            }
            else
            {
                result.MinOccurs = 0;
                result.MaxOccurs = 1;
            }

            return result;
        }

        #endregion
    }
}
