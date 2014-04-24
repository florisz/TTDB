using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;

using Microsoft.CSharp;

using TimeTraveller.Services.Impl;
using TimeTraveller.Services.Data;
using TimeTraveller.Services.ObjectModels;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.General.Xml;

namespace TimeTraveller.Services.CaseFileSpecifications.Impl
{
    public class CaseFileSpecificationService: AbstractTimeLineService<CaseFileSpecification>, ICaseFileSpecificationService
    {
        #region Private Properties
        private static readonly string _caseFileSpecificationXsd = "casefilespecification.xsd";
        private static readonly string _caseFileSpecificationXsdResourceName = string.Format("{0}.{1}", typeof(ICaseFileSpecificationService).Namespace, _caseFileSpecificationXsd);
        private const string _specificationsTemplate = "{0}/";
        private const string _xmlSchemaNamespacePrefix = "xs";
        private const string _xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        private IObjectModelService _objectModelService;
        #endregion

        #region Constructors
        public CaseFileSpecificationService(ILogger logger, IUnity container, IObjectModelService objectModelService, IDataService dataService)
            : base(ItemType.CaseFileSpecification, logger, container, dataService)
        {
            _objectModelService = objectModelService;
        }
        #endregion

        #region AbstractTimeLineService Members
        public override string XmlSchemaName
        {
            get { return _caseFileSpecificationXsd; }
        }

        public override string XmlSchemaResourceName
        {
            get { return _caseFileSpecificationXsdResourceName; }
        }

        public override CaseFileSpecification Convert(IBaseObjectValue objectValue, Uri baseUri)
        {
            CaseFileSpecification result = Convert(objectValue.Text, Encoding.UTF8);
            result.BaseObjectValue = objectValue;
            result.SelfUri = result.GetUri(baseUri, UriType.Version);

            if (!objectValue.ReferenceId.Equals(Guid.Empty))
            {
                IBaseObjectValue objectModelValue = DataService.GetBaseObjectValue(objectValue.ReferenceId);
                result.ObjectModel = _objectModelService.Convert(objectModelValue, baseUri);
                result.ObjectModelUri = result.ObjectModel.SelfUri;
            }

            return result;
        }

        public override void WriteDetailedSummaryInfo(CaseFileSpecification item, Uri baseUri, XmlWriter xmlWriter)
        {
            IBaseObjectType casefileType = DataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile));
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", casefileType.Name);
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, casefileType.RelativeUri, item.ExtId, "/"));
            xmlWriter.WriteEndElement(); // Link

            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, BaseObjectType.RelativeUri, item.ExtId, ".xsd"));
            xmlWriter.WriteEndElement(); // Link

            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "assembly");
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, BaseObjectType.RelativeUri, item.ExtId, ".dll"));
            xmlWriter.WriteEndElement(); // Link

            IBaseObjectType representationType = DataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation));
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", representationType.Name);
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, representationType.RelativeUri, item.ExtId, "/"));
            xmlWriter.WriteEndElement(); // Link

            IBaseObjectType ruleType = DataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet));
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", ruleType.Name);
            xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, ruleType.RelativeUri, item.ExtId, "/"));
            xmlWriter.WriteEndElement(); // Link
        }
        #endregion

        #region ICaseFileSpecificationService Members
        public Assembly GetAssembly(CaseFileSpecification specification)
        {
            try
            {
                Logger.DebugFormat("GetAssembly({0})", specification.Name);

                XmlSchema xmlSchema = CreateXmlSchema(specification);

                string specificationNamespace = string.Format("{0}.{1}", specification.ObjectModel.Name, specification.Name);
                CodeNamespace codeNamespace = CreateCodeNamespace(specificationNamespace, xmlSchema);

                CompilerResults compileResults = CompileAssembly(specification.Name, codeNamespace);

                Assembly result = compileResults.CompiledAssembly;

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public byte[] GetAssemblyBytes(CaseFileSpecification specification)
        {
            try
            {
                Logger.DebugFormat("GetAssemblyBytes({0})", specification.Name);

                XmlSchema xmlSchema = CreateXmlSchema(specification);

                string specificationNamespace = string.Format("{0}.{1}", specification.ObjectModel.Name, specification.Name);
                CodeNamespace codeNamespace = CreateCodeNamespace(specificationNamespace, xmlSchema);

                CompilerResults compileResults = CompileAssembly(specification.Name, codeNamespace);

                byte[] result = File.ReadAllBytes(compileResults.PathToAssembly);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public override IEnumerable<CaseFileSpecification> GetEnumerable(string objectmodelname, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("GetCaseFileSpecifications({0})", objectmodelname);

                string specificationQueryString = string.Format(_specificationsTemplate, objectmodelname);
                IEnumerable<CaseFileSpecification> result = base.GetEnumerable(specificationQueryString, baseUri);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public string GetXmlSchema(CaseFileSpecification specification, Encoding encoding)
        {
            try
            {
                Logger.DebugFormat("GetXmlSchema({0}, {1})", specification.Name, encoding.HeaderName);

                XmlSchema xmlSchema = CreateXmlSchema(specification);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace(_xmlSchemaNamespacePrefix, _xmlSchemaNamespace);

                StringBuilder result = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = encoding;
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                XmlWriter xmlWriter = XmlWriter.Create(result, settings);
                xmlSchema.Write(xmlWriter, nsmgr);
                xmlWriter.Close();

                Logger.DebugFormat("Schema=\r\n{0}", result);

                return result.ToString();
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public bool IsValidCaseFileId(CaseFileSpecification specification, string caseFileId)
        {
            try
            {
                Logger.DebugFormat("IsValidCaseFileId({0}, {1}, {2})", specification.ObjectModel.Name, specification.Name, caseFileId);
                UriTemplate uriTemplate = new UriTemplate(UnescapeUriTemplate(specification.UriTemplate));

                Logger.DebugFormat("UriTemplate={0}", uriTemplate.ToString());

                // We need to add 'http:/' to fool the Uri-class that we're creating a valid Uri.
                Uri caseFileBaseUri = new Uri(string.Format(@"http://localhost/{0}/", specification.ExtId));
                Uri caseFileIdUri = new Uri(string.Format(@"http://localhost/{0}", caseFileId));
                UriTemplateMatch match = uriTemplate.Match(caseFileBaseUri, caseFileIdUri);
                bool result = (match != null);

                if (Logger.IsDebugEnabled)
                {
                    if (result)
                    {
                        foreach (string boundVariable in match.BoundVariables.Keys)
                        {
                            Logger.DebugFormat("{0}={1}", boundVariable, match.BoundVariables[boundVariable]);
                        }
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public bool IsValidCaseFileId(CaseFileSpecification specification, string caseFileId, string xml)
        {
            try
            {
                Logger.DebugFormat("IsValidCaseFileId({0}, {1}, {2})), xml=\r\n{3}", specification.ObjectModel.Name, specification.Name, caseFileId, xml);
                bool result = IsValidCaseFileId(specification, caseFileId);
                if (result)
                {
                    UriTemplate uriTemplate = new UriTemplate(UnescapeUriTemplate(specification.UriTemplate));

                    // We need to add 'http:/' to fool the Uri-class that we're creating a valid Uri.
                    Uri caseFileBaseUri = new Uri(string.Format("http://localhost/{0}/", specification.ExtId));
                    Uri caseFileIdUri = new Uri(string.Format("http://localhost/{0}", caseFileId));
                    UriTemplateMatch match = uriTemplate.Match(caseFileBaseUri, caseFileIdUri);
                    result = (match != null);
                    if (result)
                    {
                        XPathDocument xmlDocument = new XPathDocument(new StringReader(xml));
                        XPathNavigator xpathNavigator = xmlDocument.CreateNavigator();

                        XmlNamespaceManager xmlNamespaceResolver = new XmlNamespaceManager(xpathNavigator.NameTable);
                        xmlNamespaceResolver.AddNamespace(specification.Name[0].ToString(), string.Format("{0}.xsd", specification.SelfUri));

                        string[] xpathQueries = specification.XPathQueries;
                        for (int index = 0; index < match.BoundVariables.Count; ++index)
                        {
                            string boundKey = match.BoundVariables.GetKey(index);
                            string boundVariable = match.BoundVariables[boundKey];
                            string xpathQuery = xpathQueries[index];

                            Logger.DebugFormat("{0}={1} (XPath={2})", boundKey, match.BoundVariables[boundKey], xpathQuery);

                            XPathNavigator xpathResult = xpathNavigator.SelectSingleNode(xpathQuery, xmlNamespaceResolver);
                            result = (xpathResult != null);
                            if (result)
                            {
                                Logger.DebugFormat("{0}={1}", xpathQuery, xpathResult.Value);
                                result = boundVariable.Equals(xpathResult.Value);
                                if (!result)
                                {
                                    Logger.DebugFormat("Cannot match caseFileId {0} to template {1} ({2} != {3})", caseFileId, specification.UriTemplate, xpathQuery, boundVariable);
                                    break;
                                }
                            }
                            else
                            {
                                Logger.DebugFormat("Cannot match caseFileId {0} to template {1} ({2} == null)", caseFileId, specification.UriTemplate, xpathQuery);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("Cannot match caseFileId {0} to template {1}", caseFileId, specification.UriTemplate);
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public override bool Store(string specificationname, CaseFileSpecification specification, Uri baseUri, WebHttpHeaderInfo info)
        {
            try
            {
                Logger.DebugFormat("StoreCaseFileSpecification({0})", specificationname);

                TimePoint now = TimePoint.Now;

                IBaseObjectValue objectModelObjectValue = DataService.GetBaseObjectValue(specification.ObjectModel.Id);
                if (objectModelObjectValue == null)
                {
                    throw new ArgumentException(string.Format("Cannot find objectmodel (objectvalueid={0})", specification.ObjectModel.Id));
                }

                ValidateCaseFileSpecification(specification);

                return base.Store(specificationname, specification, objectModelObjectValue, baseUri, info);
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public void ValidateCaseFile(CaseFileSpecification specification, string caseFileId, string xml)
        {
            try
            {
                Logger.DebugFormat("ValidateCaseFile({0}, {1}, {2}), xml=\r\n{3}", specification.ObjectModel.Name, specification.Name, caseFileId, xml);
                ValidateCaseFileId(specification, caseFileId, xml);

                string xmlSchema = GetXmlSchema(specification, Encoding.UTF8);

                XmlHelper.Validate(xml, xmlSchema, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public void ValidateCaseFileId(CaseFileSpecification specification, string caseFileId)
        {
            Logger.DebugFormat("ValidateCaseFileId({0}, {1}, {2})", specification.ObjectModel.Name, specification.Name, caseFileId);
            if (!IsValidCaseFileId(specification, caseFileId))
            {
                throw new ArgumentOutOfRangeException("caseFileId", string.Format("Cannot match caseFileId {0} to template {1}", caseFileId, specification.UriTemplate));
            }
        }

        public void ValidateCaseFileId(CaseFileSpecification specification, string caseFileId, string xml)
        {
            Logger.DebugFormat("ValidateCaseFileId({0}, {1}, {2})), xml=\r\n{3}", specification.ObjectModel.Name, specification.Name, caseFileId, xml);
            if (!IsValidCaseFileId(specification, caseFileId, xml))
            {
                throw new ArgumentOutOfRangeException("caseFileId", string.Format("Cannot match caseFileId {0} to template {1}", caseFileId, specification.UriTemplate));
            }
        }
        #endregion

        #region Private Methods
        private static void AddXmlSchemaComplexTypes(XmlSchema xmlSchema, CaseFileSpecificationEntity entity, ObjectModel objectModel)
        {
            ObjectDefinition entityObjectDefinition = (from objectDefinition in objectModel.ObjectDefinitions
                                                       where objectDefinition.Name.Equals(entity.Type)
                                                       && objectDefinition.ObjectType == ObjectType.entity
                                                       select objectDefinition).First();
            if (!ComplexTypeDefined(xmlSchema, entityObjectDefinition))
            {
                XmlSchemaComplexType complexType = CreateXmlSchemaComplexType(entityObjectDefinition);
                xmlSchema.Items.Add(complexType);


                if (entity.Relation != null)
                {
                    XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
                    if (sequence == null)
                    {
                        sequence = new XmlSchemaSequence();
                        complexType.Particle = sequence;
                    }
                    foreach (CaseFileSpecificationRelation relation in entity.Relation)
                    {
                        XmlSchemaElement relationElement = CreateXmlSchemaElement(entity, relation, objectModel);
                        sequence.Items.Add(relationElement);

                        AddXmlSchemaComplexTypes(xmlSchema, relation, objectModel);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the complex type defined by entityObjectDefinition is already defined in
        /// the xmlSchema. This little helper function is used to prevent adding complex types more 
        /// than once.
        /// </summary>
        /// <param name="xmlSchema">the xml schema to add the complex type to</param>
        /// <param name="entityObjectDefinition">the definition of the new complex type</param>
        /// <returns></returns>
        private static bool ComplexTypeDefined(XmlSchema xmlSchema, ObjectDefinition entityObjectDefinition)
        {
            bool found = false;
            foreach (object o in xmlSchema.Items)
            {
                if (o.GetType() == typeof(XmlSchemaComplexType) && ((XmlSchemaComplexType)o).Name == entityObjectDefinition.Name)
                    found = true;
            }
            return found;
        }

        private static void AddXmlSchemaComplexTypes(XmlSchema xmlSchema, CaseFileSpecificationRelation relation, ObjectModel objectModel)
        {
            ObjectDefinition relationObjectDefinition = (from objectDefinition in objectModel.ObjectDefinitions
                                                         where objectDefinition.Name.Equals(relation.Type)
                                                         && objectDefinition.ObjectType == ObjectType.relation
                                                         select objectDefinition).First();
            XmlSchemaComplexType complexType = CreateXmlSchemaComplexType(relationObjectDefinition);
            xmlSchema.Items.Add(complexType);

            XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
            if (sequence == null)
            {
                sequence = new XmlSchemaSequence();
                complexType.Particle = sequence;
            }
            XmlSchemaElement entityElement = CreateXmlSchemaElement(relation.Entity);
            sequence.Items.Add(entityElement);

            ObjectRelation targetRelationDefinition = (from objectRelation in objectModel.ObjectRelations
                                                         where objectRelation.Source.Equals(relation.Type)
                                                         && objectRelation.Target.Equals(relation.Entity.Type)
                                                         select objectRelation).First();
            entityElement.MinOccurs = targetRelationDefinition.MinOccurs;
            entityElement.MaxOccursString = targetRelationDefinition.MaxOccurs;

            AddXmlSchemaComplexTypes(xmlSchema, relation.Entity, objectModel);
        }

        private static CompilerResults CompileAssembly(string assemblyName, CodeNamespace codeNamespace)
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = false;
            compilerParameters.IncludeDebugInformation = false;
            compilerParameters.WarningLevel = 3;
            compilerParameters.TreatWarningsAsErrors = false;
            compilerParameters.CompilerOptions = "/optimize";
            compilerParameters.OutputAssembly = string.Format("{0}{1}.dll", Path.GetTempPath(), assemblyName);

            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Xml.dll");

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNamespace);
            
            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerResults result = provider.CompileAssemblyFromDom(compilerParameters, compileUnit);

            if (result.Errors.HasErrors || result.Errors.HasWarnings)
            {
                StringBuilder compileErrorText = new StringBuilder();
                foreach (CompilerError error in result.Errors)
                {
                    compileErrorText.AppendLine(error.ErrorText);
                }
                throw new Exception(compileErrorText.ToString());
            }
            else if (!File.Exists(result.PathToAssembly))
            {
                throw new FileNotFoundException(string.Format("Cannot create assembly for specification {0}", codeNamespace.Name));
            }

            return result;
        }

        private static XmlSchema CreateXmlSchema(CaseFileSpecification specification)
        {
            XmlSchema result = new XmlSchema();
            result.Id = specification.Name;
            result.AttributeFormDefault = XmlSchemaForm.Unqualified;
            result.ElementFormDefault = XmlSchemaForm.Qualified;
            result.Namespaces.Add(_xmlSchemaNamespacePrefix, _xmlSchemaNamespace);

            XmlSchemaElement caseFileElement = CreateXmlSchemaElement(specification.Structure.Entity);
            result.Items.Add(caseFileElement);

            AddXmlSchemaComplexTypes(result, specification.Structure.Entity, specification.ObjectModel);

            // Try to compile the create xml-schema to be sure it is created correctly.
            XmlSchemaSet xmlSchemas = new XmlSchemaSet();
            xmlSchemas.Add(result);
            xmlSchemas.Compile();

            return result;
        }

        private static CodeNamespace CreateCodeNamespace(string targetNamespace, XmlSchema xmlSchema)
        {
            // System.CodeDom namespace for the XmlCodeExporter to put classes in.
            CodeNamespace result = new CodeNamespace(targetNamespace);
            
            // Create the importer for these schemas.
            XmlSchemas schemas = new XmlSchemas();
            schemas.Add(xmlSchema);
            XmlSchemaImporter importer = new XmlSchemaImporter(schemas);

            XmlCodeExporter exporter = new XmlCodeExporter(result);

            // Iterate schema top-level elements and export code for each.
            foreach (XmlSchemaElement element in xmlSchema.Elements.Values)
            {
                // Import the mapping first.
                XmlTypeMapping mapping = importer.ImportTypeMapping(element.QualifiedName);
                // Export the code finally.
                exporter.ExportTypeMapping(mapping);
            }

            return result;
        }

        private static XmlSchemaComplexType CreateXmlSchemaComplexType(ObjectDefinition objectDefinition)
        {
            XmlSchemaComplexType result = new XmlSchemaComplexType();
            result.Name = objectDefinition.Name;

            if (objectDefinition.Properties != null || objectDefinition.ComplexProperties != null)
            {
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                result.Particle = sequence;

                if (objectDefinition.Properties != null)
                {
                    foreach (ObjectDefinitionProperty property in objectDefinition.Properties)
                    {
                        XmlSchemaElement propertyElement = CreateXmlSchemaElement(property);
                        sequence.Items.Add(propertyElement);
                    }
                }
                if (objectDefinition.ComplexProperties != null)
                {
                    foreach (ObjectDefinitionComplexProperty property in objectDefinition.ComplexProperties)
                    {
                        XmlSchemaElement complexPropertyElement = CreateXmlSchemaElement(property);
                        sequence.Items.Add(complexPropertyElement);
                    }
                }
            }

            XmlSchemaAttribute attribute = new XmlSchemaAttribute();
            attribute.Name = "RegistrationId";
            attribute.SchemaTypeName = new XmlQualifiedName("string", _xmlSchemaNamespace);
            attribute.Use = XmlSchemaUse.Required;
            result.Attributes.Add(attribute);

            attribute = new XmlSchemaAttribute();
            attribute.Name = "RegistrationStart";
            attribute.SchemaTypeName = new XmlQualifiedName("dateTime", _xmlSchemaNamespace);
            attribute.Use = XmlSchemaUse.Required;
            result.Attributes.Add(attribute);

            attribute = new XmlSchemaAttribute();
            attribute.Name = "RegistrationEnd";
            attribute.SchemaTypeName = new XmlQualifiedName("dateTime", _xmlSchemaNamespace);
            attribute.Use = XmlSchemaUse.Optional;
            result.Attributes.Add(attribute);
         
            return result;
        }

        private static XmlSchemaElement CreateXmlSchemaElement(CaseFileSpecificationEntity entity)
        {
            XmlSchemaElement result = new XmlSchemaElement();
            result.Name = entity.Name;
            result.SchemaTypeName = new XmlQualifiedName(entity.Type);

            return result;
        }

        private static XmlSchemaElement CreateXmlSchemaElement(CaseFileSpecificationEntity sourceEntity, CaseFileSpecificationRelation relation, ObjectModel objectModel)
        {
            XmlSchemaElement result = new XmlSchemaElement();
            result.Name = relation.Name;
            result.SchemaTypeName = new XmlQualifiedName(relation.Type);

            ObjectRelation relationObject = (from objectRelation in objectModel.ObjectRelations
                                             where objectRelation.Source.Equals(sourceEntity.Name)
                                             && objectRelation.Target.Equals(relation.Type)
                                             select objectRelation).First();

            result.MinOccurs = relationObject.MinOccurs;
            result.MaxOccursString = relationObject.MaxOccurs;

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

            if (complexProperty.Properties != null ||
                complexProperty.ComplexProperties != null)
            {
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                innerComplexType.Particle = sequence;

                if (complexProperty.Properties != null)
                {
                    foreach (ObjectDefinitionProperty property in complexProperty.Properties)
                    {
                        XmlSchemaElement propertyElement = CreateXmlSchemaElement(property);
                        sequence.Items.Add(propertyElement);
                    }
                }
                if (complexProperty.ComplexProperties != null)
                {
                    foreach (ObjectDefinitionComplexProperty property in complexProperty.ComplexProperties)
                    {
                        XmlSchemaElement complexPropertyElement = CreateXmlSchemaElement(property);
                        sequence.Items.Add(complexPropertyElement);
                    }
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

        private static string UnescapeUriTemplate(string uriTemplate)
        {
            string result = uriTemplate.Replace("/", "");

            return result;
        }

        private static void ValidateCaseFileSpecification(CaseFileSpecification specification)
        {
            ValidateEntity(specification.Structure.Entity, specification.ObjectModel);
        }

        private static void ValidateEntity(CaseFileSpecificationEntity entity, ObjectModel objectModel)
        {
            var entityObjectDefinition = (from objectDefinition in objectModel.ObjectDefinitions
                                          where objectDefinition.Name.Equals(entity.Type)
                                          && objectDefinition.ObjectType == ObjectType.entity
                                          select objectDefinition);

            if (entityObjectDefinition.Count() != 1)
            {
                throw new ArgumentException(string.Format("Entity {0} does not exist in objectmodel {1}", entity.Type, objectModel.Name));
            }

            if (entity.Relation != null)
            {
                foreach (CaseFileSpecificationRelation relation in entity.Relation)
                {
                    ValidateRelation(entity, relation, objectModel);
                }
            }
        }

        private static void ValidateRelation(CaseFileSpecificationEntity sourceEntity, CaseFileSpecificationRelation relation, ObjectModel objectModel)
        {
            var relationObject = (from relationDefinition in objectModel.ObjectRelations
                                  where relationDefinition.Source.Equals(sourceEntity.Type)
                                  && relationDefinition.Target.Equals(relation.Name)
                                  select relationDefinition);

            if (relationObject.Count() != 1)
            {
                throw new ArgumentException(string.Format("Relation between {0} and {1} does not exist in objectmodel {2}", sourceEntity.Type, relation.Type, objectModel.Name));
            }

            var relationObjectDefinition = (from objectDefinition in objectModel.ObjectDefinitions
                                            where objectDefinition.Name.Equals(relation.Type)
                                            && objectDefinition.ObjectType == ObjectType.relation
                                            select objectDefinition);

            if (relationObjectDefinition.Count() != 1)
            {
                throw new ArgumentException(string.Format("Relation {0} does not exist in objectmodel {1}", relation.Type, objectModel.Name));
            }

            ValidateEntity(relation.Entity, objectModel);
        }
        #endregion
    }
}
