using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TimeTraveller.Services.Impl;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Data;
using TimeTraveller.Services.Representations;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.General.Xml;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.CaseFiles.Impl
{
    public class CaseFileService : AbstractTimeLineService<CaseFile>, ICaseFileService
    {
        #region Private Properties
        private const string _caseFileXmlSchemaPrefix = "cs";
        private const string _caseFileXsd = "casefile.xsd";
        private static readonly string _caseFileXsdResourceName = string.Format("{0}.{1}", typeof(ICaseFileService).Namespace, _caseFileXsd);
        private static readonly string _caseFileXmlSchemaUri = string.Format("http://timetraveller.net/its/schemas/{0}", _caseFileXsd);
        private const string _registrationIdAttributeName = "RegistrationId";
        private const string _registrationStartAttributeName = "RegistrationStart";
        private const string _registrationEndAttributeName = "RegistrationEnd";

        private static readonly string _caseFileSchemaName = string.Format("{0}.{1}", typeof(ICaseFileService).Namespace, "casefile.xsd");

        private ICaseFileSpecificationService _caseFileSpecificationService;
        private IRepresentationService _representationService;
        #endregion

        #region Constructors
        public CaseFileService(ILogger logger, IUnity container, ICaseFileSpecificationService caseFileSpecificationService, IRepresentationService representationService, IDataService dataService)
			: base(TimeTraveller.Services.Interfaces.ItemType.CaseFile, logger, container, dataService)
        {
            _caseFileSpecificationService = caseFileSpecificationService;
            _representationService = representationService;
        }
        #endregion

        #region AbstractTimeLineService Members
        public override string XmlSchemaName
        {
            get { return _caseFileXsd; }
        }

        public override string XmlSchemaResourceName
        {
            get { return _caseFileXsdResourceName; }
        }

        public override CaseFile Convert(IBaseObjectValue objectValue, Uri baseUri)
        {
            CaseFile result = new CaseFile()
            {
                BaseObjectValue = objectValue
            };
            result.SelfUri = result.GetUri(baseUri, UriType.Version);

            if (!objectValue.ReferenceId.Equals(Guid.Empty))
            {
                IBaseObjectValue specificationValue = DataService.GetBaseObjectValue(objectValue.ReferenceId);
                result.CaseFileSpecification = _caseFileSpecificationService.Convert(specificationValue, baseUri);
            }

            return result;
        }

        public override void WriteDetailedSummaryInfo(CaseFile item, Uri baseUri, XmlWriter xmlWriter)
        {
            IEnumerable<Representation> representations = _representationService.GetEnumerable(item.CaseFileSpecification.ObjectModel.Name, item.CaseFileSpecification.Name, baseUri);
            foreach (Representation representation in representations)
            {
                xmlWriter.WriteStartElement("Link");
                xmlWriter.WriteAttributeString("rel", BaseObjectType.Name);
                string viewQueryParameter = string.Format("?view={0}", representation.Name);
                xmlWriter.WriteAttributeString("href", item.FormatUri(baseUri, BaseObjectType.RelativeUri, item.ExtId, viewQueryParameter));
                xmlWriter.WriteEndElement(); // Link
            }
        }
        #endregion

        #region ICaseFileService Members
        public override string GetXml(CaseFile caseFile, Encoding encoding)
        {
            try
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(_caseFileXmlSchemaPrefix, _caseFileXmlSchemaUri);
                string result = XmlHelper.ToXml<CaseFile>(caseFile, encoding, true, namespaces);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public override CaseFile Convert(string caseFileXml, Encoding encoding)
        {
            try
            {
                Stream caseFileSchemaStream = this.GetType().Assembly.GetManifestResourceStream(_caseFileSchemaName);
                IEnumerable<ValidationEventArgs> validationErrors;

                Logger.DebugFormat("XmlHelper.Validate()\r\n{0}", caseFileXml);

                bool isValidXml = XmlHelper.Validate(caseFileXml, encoding, caseFileSchemaStream, out validationErrors);
                if (!isValidXml)
                {
                    StringBuilder validationErrorText = new StringBuilder();
                    foreach (ValidationEventArgs validationError in validationErrors)
                    {
                        validationErrorText.AppendLine(string.Format("{0}", validationError.Message));
                    }
                    throw new ArgumentOutOfRangeException("caseFileXml", string.Format("Invalid objectmodel supplied\r\n{0}", validationErrorText));
                }

                CaseFile result = XmlHelper.FromXml<CaseFile>(caseFileXml, encoding);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public CaseFile Get(CaseFileSpecification specification, string caseFileId, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("GetCaseFile({0}, {1})", specification.Name, caseFileId);

                CaseFile result = Get(specification, caseFileId, TimePoint.Now, baseUri);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public CaseFile Get(CaseFileSpecification specification, string caseFileId, TimePoint timePoint, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("GetCaseFile({0}, {1}, {2})", specification.Name, caseFileId, timePoint.TimeValue.ToString("O"));
                
                IBaseObjectValue casefileSpecificationObjectValue = DataService.GetBaseObjectValue(specification.Id);

                IRelationObject caseFileObject = DataService.GetBaseObject(caseFileId, BaseObjectType, casefileSpecificationObjectValue.Parent) as IRelationObject;
                if (caseFileObject == null)
                {
                    throw new ArgumentOutOfRangeException("caseFileId", string.Format("No casefile available for id {0}", caseFileId));
                }

                IBaseObjectValue caseFileObjectValue = caseFileObject.GetValue(timePoint);
                if (caseFileObjectValue == null)
                {
                    throw new ArgumentOutOfRangeException("caseFileId", string.Format("No casefile available for id {0}/{1}", caseFileId, timePoint.TimeValue.ToString("O")));
                }

                CaseFile result = new CaseFile()
                {
                    BaseObjectValue = caseFileObjectValue,
                    CaseFileSpecification = specification
                };
                
                IBaseObject rootObject = DataService.GetBaseObject(caseFileObject.Relation1.Id);
                IBaseObjectValue rootObjectValue = rootObject.GetValue(timePoint);
                if (rootObjectValue != null)
                {
                    result.Text = ConstructCaseFileContent(specification.Structure.Entity, rootObjectValue, timePoint);
                }

                result.CaseFileSpecificationUri = specification.GetUri(baseUri, UriType.Version);
                result.SelfUri = result.GetUri(baseUri, UriType.Version);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public CaseFile Get(CaseFileSpecification specification, string caseFileId, int versionNumber, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("GetCaseFile({0}, {1}, {2})", specification.Name, caseFileId, versionNumber);

                IBaseObjectValue casefileSpecificationObjectValue = DataService.GetBaseObjectValue(specification.Id);

                IRelationObject caseFileObject = DataService.GetBaseObject(caseFileId, BaseObjectType, casefileSpecificationObjectValue.Parent) as IRelationObject;
                if (caseFileObject == null)
                {
                    throw new ArgumentOutOfRangeException("caseFileId", string.Format("No casefile available for id {0}", caseFileId));
                }

                IBaseObjectValue caseFileObjectValue = caseFileObject.GetValue(versionNumber);
                if (caseFileObjectValue == null)
                {
                    throw new ArgumentOutOfRangeException("caseFileId", string.Format("No casefile available for id {0}/{1}", caseFileId, versionNumber));
                }

                CaseFile result = new CaseFile()
                {
                    BaseObjectValue = caseFileObjectValue,
                    CaseFileSpecification = specification
                };

                IBaseObject rootObject = DataService.GetBaseObject(caseFileObject.Relation1Id);
                IBaseObjectValue rootObjectValue = rootObject.GetValue(caseFileObjectValue.Start);
                if (rootObjectValue != null)
                {
                    result.Text = ConstructCaseFileContent(specification.Structure.Entity, rootObjectValue, caseFileObjectValue.Start);
                }

                result.CaseFileSpecificationUri = specification.GetUri(baseUri, UriType.Version);
                result.SelfUri = result.GetUri(baseUri, UriType.Version);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public IEnumerable<CaseFile> GetEnumerable(CaseFileSpecification specification, Uri baseUri)
        {
            try
            {
                string caseFileIdFilter = string.Format("{0}/", specification.ExtId);
                return base.GetEnumerable(caseFileIdFilter, baseUri);
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public bool Store(CaseFileSpecification specification, string caseFileId, CaseFile caseFile, Uri baseUri, IHeaderInfo info)
        {
            try
            {
                Logger.DebugFormat("StoreCaseFile({0}, {1})", specification.Name, caseFileId);

                // Validate the casefile xml to the schema in the specification
                _caseFileSpecificationService.ValidateCaseFile(specification, caseFileId, caseFile.Text);

                // Split the case file into base object's and base object values.
                IEnumerable<CaseFileObject> caseFileObjectValues = ReadCaseFileContent(caseFile.Text, specification);

                // Store the base object's and base object values
                IBaseObjectValue objectModelObjectValue = DataService.GetBaseObjectValue(specification.ObjectModel.Id);
                IBaseObjectValue caseFileSpecificationObjectValue = DataService.GetBaseObjectValue(specification.Id);
                IBaseObjectValue newObjectValue = StoreCaseFileObjectValues(caseFileId, caseFileObjectValues, TimePoint.Now, objectModelObjectValue, caseFileSpecificationObjectValue, info);

                DataService.SaveChanges();

                caseFile.BaseObjectValue = newObjectValue;
                caseFile.CaseFileSpecificationUri = specification.GetUri(baseUri, UriType.Version);
                caseFile.SelfUri = caseFile.GetUri(baseUri, UriType.Version);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Construct the case file content xml according to the entity-relation definition in the specification and the timepoint.
        /// </summary>
        /// <param name="rootEntityDefinition"></param>
        /// <param name="rootObjectValue"></param>
        /// <param name="timePoint"></param>
        /// <returns></returns>
        private string ConstructCaseFileContent(CaseFileSpecificationEntity rootEntityDefinition, IBaseObjectValue rootObjectValue, TimePoint timePoint)
        {
            StringBuilder result = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XmlWriter xmlWriter = XmlWriter.Create(result, settings);

            WriteCaseFileContent(rootEntityDefinition, rootObjectValue, timePoint, xmlWriter);

            xmlWriter.Close();

            Logger.DebugFormat("GetCaseFileContent({0})\r\n{1}", rootObjectValue.Id, result);

            return result.ToString();
        }

        /// <summary>
        /// Find the object value for the root entity of the casefile which the content matches the caseFileId.
        /// </summary>
        /// <param name="specification">The CaseFileSpecification</param>
        /// <param name="caseFileId">The id to find</param>
        /// <param name="timePoint">The timepoint to determine the valid object values</param>
        /// <returns>The BaseObjectValue containing the information for the case file</returns>
        private IBaseObjectValue FindRootObjectValue(CaseFileSpecification specification, string caseFileId, TimePoint timePoint)
        {
            IBaseObjectValue result = null;

            IBaseObjectValue objectModelObjectValue = DataService.GetBaseObjectValue(specification.ObjectModel.Id);
            
            string caseFileRootElementName = specification.Structure.Entity.Name;
            // Get all object values for the given (value of the) objectmodel and the root element name of the case file specification
            IBaseObjectType entityType = DataService.GetBaseObjectType((int)ItemType.Entity);
            IEnumerable<IBaseObjectValue> rootObjectValues = DataService.GetValues(objectModelObjectValue, caseFileRootElementName, entityType);
            if (rootObjectValues != null)
            {
                // Filter the root object values for the ones that contain the given caseFileId.
                var validObjectValues = (from objectValue in rootObjectValues
                                         where IsValidCaseFileId(objectValue, specification, caseFileId)
                                           && (objectValue.Range.Includes(timePoint))
                                         select objectValue).ToArray();

                if (validObjectValues != null)
                {
                    if (validObjectValues.Count() == 1)
                    {
                        result = validObjectValues.First();
                    }
                    else if (validObjectValues.Count() > 1)
                    {
                        throw new ArgumentException("caseFileId", string.Format("More than one {0} found for {1}", caseFileRootElementName, caseFileId));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Validate the object value to see if the content contains the caseFileId according to the UriTemplate of the specification.
        /// </summary>
        /// <param name="objectValue"></param>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <returns></returns>
        private bool IsValidCaseFileId(IBaseObjectValue objectValue, CaseFileSpecification specification, string caseFileId)
        {
            try 
            {	        
                StringBuilder caseFileXml = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;

                XmlWriter xmlWriter = XmlWriter.Create(caseFileXml);

                xmlWriter.WriteStartElement(objectValue.Parent.ExtId);
                xmlWriter.WriteRaw(objectValue.Text);
                xmlWriter.WriteEndElement();
                
                xmlWriter.Close();

                bool result = _caseFileSpecificationService.IsValidCaseFileId(specification, caseFileId, caseFileXml.ToString());
                return result;
            }
	        catch (Exception exception)
	        {
		        Logger.Debug("Exception", exception);
		        throw;
	        }
        }

        /// <summary>
        /// Read and split the case file content xml and return the IBaseObjectValues containing the entity and relation object values.
        /// </summary>
        /// <param name="xml">The string of xml</param>
        /// <param name="specification">The CaseFileSpecification</param>
        /// <returns>the entity and relation object values</returns>
        private IEnumerable<CaseFileObject> ReadCaseFileContent(string xml, CaseFileSpecification specification)
        {
            Logger.Debug("ReadCaseFileContent()");

            List<CaseFileObject> result = new List<CaseFileObject>();

            XmlReader xmlReader = XmlReader.Create(new StringReader(xml));

            CaseFileObject caseFileRootObject = ReadEntity(xmlReader, specification.Structure.Entity, result);
            caseFileRootObject.IsRootEntity = true;

            if (Logger.IsDebugEnabled)
            {
                foreach (CaseFileObject objectValue in result)
                {
                    Logger.DebugFormat("CaseFileObject('{0}', {1}, '{2}', '{3}', '{4}')={5}", objectValue.Id, objectValue.ObjectName, objectValue.ItemType, objectValue.Start, objectValue.End, objectValue.Text);
                }
            }

            return result;
        }

        /// <summary>
        /// Read the xml content from the xmlreader of the current element.
        /// 
        /// The content is complete when the sub element is found or the current element is ended.
        /// </summary>
        /// <param name="xmlReader">The XmlReader to read from</param>
        /// <param name="elementName">The current element name to read</param>
        /// <param name="subElementName">The sub element that finishes the content</param>
        /// <returns>The string containing the xml content</returns>
        private string ReadElementContent(XmlReader xmlReader, string elementName, string subElementName)
        {
            string[] subElementNames = new string[] { subElementName };
            return ReadElementContent(xmlReader, elementName, subElementNames);
        }

        /// <summary>
        /// Read the xml content from the xmlreader of the current element.
        /// 
        /// The content is complete when one of the sub elements is found or the current element is ended.
        /// </summary>
        /// <param name="xmlReader">The XmlReader to read from</param>
        /// <param name="elementName">The current element name to read</param>
        /// <param name="subElementNames">The sub elements that finish the content</param>
        /// <returns>The string containing the xml content</returns>
        private string ReadElementContent(XmlReader xmlReader, string elementName, string[] subElementNames)
        {
            StringBuilder result = new StringBuilder();

            xmlReader.MoveToContent();
            while (!(xmlReader.Name.Equals(elementName) && xmlReader.NodeType.Equals(XmlNodeType.EndElement)))
            {
                if (subElementNames.Contains(xmlReader.Name))
                {
                    break;
                }
                result.Append(xmlReader.ReadOuterXml());
                xmlReader.MoveToContent();
            }

            return result.ToString();
        }

        /// <summary>
        /// Read the entity and sub-relations from the xmlreader and add the content as IBaseObjectValues to the resulting list.
        /// </summary>
        /// <param name="xmlReader">The XmlReader to read from</param>
        /// <param name="entityDefinition">The Entity to read</param>
        /// <param name="resultingList">The resulting list</param>
        /// <returns>The IEntity read from the XmlReader</returns>
        private CaseFileObject ReadEntity(XmlReader xmlReader, CaseFileSpecificationEntity entityDefinition, List<CaseFileObject> resultingList)
        {
            xmlReader.MoveToContent();

            // Create the objects that (later on) will contain the information read from the XmlReader
            CaseFileObject entityObject = new CaseFileObject();
            entityObject.ObjectName = entityDefinition.Name;
			entityObject.ItemType = TimeTraveller.Services.Interfaces.ItemType.Entity;

            // Read the meta-registration attributes from the XmlReader
            ReadRegistrationAttribures(xmlReader, entityObject);
            xmlReader.MoveToContent();

            // Read start tag of the entity
            xmlReader.ReadStartElement(entityDefinition.Name);
            xmlReader.MoveToContent();

            // When one of the relation-tags is found in the XmlReader the content for the current entity is complete.
            string[] relations = new string[] {};
            if (entityDefinition.Relation != null)
            {
                relations = (from relation in entityDefinition.Relation
                             select relation.Name).ToArray();
            }

            entityObject.Text = ReadElementContent(xmlReader, entityDefinition.Name, relations);
            xmlReader.MoveToContent();

            // Read all sub-relations from the XmlReader for the current entity
            while (!(xmlReader.Name.Equals(entityDefinition.Name) && xmlReader.NodeType.Equals(XmlNodeType.EndElement)))
            {
                if (relations.Contains(xmlReader.Name))
                {
                    // Determine the correct relation definition from the entity definition
                    CaseFileSpecificationRelation relationDefinition = (from relation in entityDefinition.Relation
                                                                        where relation.Name.Equals(xmlReader.Name)
                                                                        select relation).First();
                    ReadRelation(xmlReader, entityObject, relationDefinition, resultingList);
                }
                else
                {
                    throw new ArgumentException(string.Format("{0} not valid as relation of entity {1}", xmlReader.Name, entityDefinition.Name));
                }
                xmlReader.MoveToContent();
            }

            // Add the entity to the resulting list.
            resultingList.Add(entityObject);

            // Read end tag of the entity
            xmlReader.ReadEndElement();

            return entityObject;
        }

        /// <summary>
        /// Read the meta-registration attributes from the xmlreader.
        /// </summary>
        /// <param name="xmlReader">The XmlReader</param>
        /// <param name="resultingObjectValue">The BaseObjectValue to add the registation information to</param>
        private void ReadRegistrationAttribures(XmlReader xmlReader, CaseFileObject resultingObjectValue)
        {
            // Read the registration meta information in the xml-attributes
            if (xmlReader.HasAttributes)
            {
                int index = 0;
                xmlReader.MoveToFirstAttribute();
                // Read all xml-attributes
                while (xmlReader.NodeType.Equals(XmlNodeType.Attribute) && index < xmlReader.AttributeCount)
                {
                    if (!string.IsNullOrEmpty(xmlReader.Value))
                    {
                        if (xmlReader.Name.Equals(_registrationIdAttributeName))
                        {
                            resultingObjectValue.Id = new Guid(xmlReader.Value);
                        }
                        else if (xmlReader.Name.Equals(_registrationStartAttributeName))
                        {
                            resultingObjectValue.Start = new TimePoint(DateTime.Parse(xmlReader.Value, CultureInfo.InvariantCulture));
                        }
                        else if (xmlReader.Name.Equals(_registrationEndAttributeName))
                        {
                            resultingObjectValue.End = new TimePoint(DateTime.Parse(xmlReader.Value, CultureInfo.InvariantCulture));
                        }
                    }
                    xmlReader.MoveToNextAttribute();
                    ++index;
                }
            }
        }

        /// <summary>
        /// Read the relation and sub-entity (and so on) from the xmlreader and add the content as IBaseObjectValues to the resulting list.
        /// </summary>
        /// <param name="xmlReader">The XmlReader to read from</param>
        /// <param name="relation1Object">The entity object for Relation1</param>
        /// <param name="relationDefinition">The Relation to read</param>
        /// <param name="resultingList">The resulting list</param>
        /// <returns>The IRelation read from the XmlReader</returns>
        private CaseFileObject ReadRelation(XmlReader xmlReader, CaseFileObject relation1Object, CaseFileSpecificationRelation relationDefinition, List<CaseFileObject> resultingList)
        {
            xmlReader.MoveToContent();

            // Create the objects that (later on) will contain the information read from the XmlReader
            CaseFileObject relationObject = new CaseFileObject();
            relationObject.ObjectName = xmlReader.Name;
            relationObject.ItemType = ItemType.Relation;
            relationObject.Relation1 = relation1Object;

            // Read the meta-registration attributes from the XmlReader
            ReadRegistrationAttribures(xmlReader, relationObject);
            xmlReader.MoveToContent();

            // Read start tag of the relation
            xmlReader.ReadStartElement(relationDefinition.Name);
            xmlReader.MoveToContent();

            relationObject.Text = ReadElementContent(xmlReader, relationDefinition.Name, relationDefinition.Entity.Name);
            xmlReader.MoveToContent();

            // Read the sub-entity from the XmlReader for the current relation
            if (xmlReader.Name.Equals(relationDefinition.Entity.Name) && xmlReader.IsStartElement())
            {
                CaseFileObject childEntityObject = ReadEntity(xmlReader, relationDefinition.Entity, resultingList);
                relationObject.Relation2 = childEntityObject;
            }
            else
            {
                throw new ArgumentException(string.Format("{0} not valid as entity of relation {1}", xmlReader.Name, relationDefinition.Name));
            }
            xmlReader.MoveToContent();

            resultingList.Add(relationObject);

            // Read end tag of the relation
            xmlReader.ReadEndElement();

            return relationObject;
        }

        /// <summary>
        /// Store the object values.
        /// </summary>
        /// <param name="caseFileId"></param>
        /// <param name="caseFileObjectValues">The object values to store</param>
        /// <param name="timePoint">The timestamp for the object values</param>
        /// <param name="objectModelObjectValue">The objectmodel that the object values are based on</param>
        /// <param name="casefileSpecificationObjectValue"></param>
        /// <param name="journalInfo">The journalInfo</param>
        /// <returns>The CaseFile object value</returns>
        private IBaseObjectValue StoreCaseFileObjectValues(string caseFileId, IEnumerable<CaseFileObject> caseFileObjectValues, TimePoint timePoint, IBaseObjectValue objectModelObjectValue, IBaseObjectValue casefileSpecificationObjectValue, IHeaderInfo journalInfo)
        {
            List<IBaseObjectValue> objectValues = new List<IBaseObjectValue>();
            // Store the entities
            IBaseObjectValue rootObjectValue = StoreEntities(caseFileObjectValues, timePoint, objectModelObjectValue);
            // Store the relations
            StoreRelations(caseFileObjectValues, timePoint, objectModelObjectValue);

            IBaseObject caseFileBaseObject = DataService.GetBaseObject(caseFileId, BaseObjectType, casefileSpecificationObjectValue.Parent);

            IBaseObjectValue result = null;
            if (caseFileBaseObject != null)
            {
                result = DataService.InsertValue(string.Empty, timePoint, caseFileBaseObject, casefileSpecificationObjectValue, journalInfo);
            }
            else
            {
                result = DataService.InsertValue(string.Empty, timePoint, Guid.NewGuid(), caseFileId, BaseObjectType, casefileSpecificationObjectValue, journalInfo);
            }
            IRelationObject resultAsRelation = result.Parent as IRelationObject;
            resultAsRelation.Relation1 = rootObjectValue.Parent as IEntityObject;
                    
            return result;
        }

        /// <summary>
        /// Store the entity objects and object values
        /// </summary>
        /// <param name="caseFileObjectValues">The object values to store</param>
        /// <param name="timeStamp">The timestamp for the object values</param>
        /// <param name="objectModelObjectValue">The objectmodel that the object values are based on</param>
        /// <returns>The object value of the root entity</returns>
        private IBaseObjectValue StoreEntities(IEnumerable<CaseFileObject> caseFileObjectValues, TimePoint timePoint, IBaseObjectValue objectModelObjectValue)
        {
            IBaseObjectValue result = null;
            var entities = (from caseFileObjectValue in caseFileObjectValues
                            where caseFileObjectValue.ItemType.Equals(ItemType.Entity)
                            select caseFileObjectValue);
            foreach (CaseFileObject objectValue in entities)
            {
                objectValue.BaseObjectValue = StoreObjectValue(objectValue, timePoint, objectModelObjectValue);
                if (objectValue.IsRootEntity)
                {
                    result = objectValue.BaseObjectValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Store one object and object value.
        /// </summary>
        /// <param name="caseFileObjectValue">The object value to store</param>
        /// <param name="timeStamp">The timestamp for the object value</param>
        /// <param name="referenceObjectValue">The reference object for the object value</param>
        /// <returns>The (new of existing) object value containing the information</returns>
        private IBaseObjectValue StoreObjectValue(CaseFileObject objectValue, TimePoint timePoint, IBaseObjectValue referenceObjectValue)
        {
            IBaseObjectValue result = null;

            IBaseObject existingBaseObject = DataService.GetBaseObject(objectValue.Id);
            // When the object and object value is new
            if (existingBaseObject == null)
            {
                // Insert the new object and object value
                IBaseObjectType baseObjectType = DataService.GetBaseObjectType((int)objectValue.ItemType);
                result = DataService.InsertValue(objectValue.Text, timePoint, objectValue.Id, objectValue.ObjectName, baseObjectType, referenceObjectValue);
            }
            else
            {
                // When the object is not new, retrieve the existing base object.
                IBaseObjectValue lastExistingObjectValue = existingBaseObject.Values.Last();

                // Verify that we're updating the correct object and object value
                // We only need to create a new objectvalue when the content has changed
                if (!lastExistingObjectValue.Text.Equals(objectValue.Text))
                {
                    result = DataService.InsertValue(objectValue.Text, timePoint, existingBaseObject, referenceObjectValue);
                }
                else
                {
                    Logger.DebugFormat("Object {0}/{1}/{2} is unchanged", existingBaseObject.ExtId, existingBaseObject.Id, lastExistingObjectValue.Id);
                    result = lastExistingObjectValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Store the relation objects and object values.
        /// </summary>
        /// <param name="caseFileObjectValues">The object values to store</param>
        /// <param name="entities">The entities to use in the Relation1 and Relation2</param>
        /// <param name="timeStamp">The timestamp for the object values</param>
        /// <param name="objectModelObjectValue">The objectmodel that the object values are based on</param>
        private void StoreRelations(IEnumerable<CaseFileObject> caseFileObjectValues, TimePoint timePoint, IBaseObjectValue objectModelObjectValue)
        {
            var relations = (from caseFileObjectValue in caseFileObjectValues
                             where caseFileObjectValue.ItemType.Equals(ItemType.Relation)
                             select caseFileObjectValue);

            foreach (CaseFileObject objectValue in relations)
            {
                objectValue.BaseObjectValue = StoreObjectValue(objectValue, timePoint, objectModelObjectValue);
                IRelationObject relationObject = objectValue.BaseObjectValue.Parent as IRelationObject;
                if (relationObject.Relation1 == null)
                {
                    relationObject.Relation1 = objectValue.Relation1.BaseObjectValue.Parent as IEntityObject;
                }
                if (relationObject.Relation2 == null)
                {
                    relationObject.Relation2 = objectValue.Relation2.BaseObjectValue.Parent as IEntityObject;
                }
            }
        }

        /// <summary>
        /// Write the xml content of the entity object value and it's relations to the xmlwriter.
        /// </summary>
        /// <param name="entityDefinition">The entity to write</param>
        /// <param name="entityObjectValue">The object value to write</param>
        /// <param name="xmlWriter">The XmlWriter to write to</param>
        private void WriteCaseFileContent(CaseFileSpecificationEntity entityDefinition, IBaseObjectValue entityObjectValue, TimePoint timePoint, XmlWriter xmlWriter)
        {
            string entityName = entityObjectValue.Parent.ExtId;
            xmlWriter.WriteStartElement(entityName);
            WriteRegistrationAttributes(entityObjectValue, xmlWriter);
            xmlWriter.WriteRaw(entityObjectValue.Text);

            if (entityDefinition.Relation != null)
            {
                foreach (CaseFileSpecificationRelation relationDefinition in entityDefinition.Relation)
                {
                    IEnumerable<IRelationObject> relations = DataService.GetRelations(entityObjectValue.Parent, relationDefinition.Name);
                    if (relations != null)
                    {
                        foreach (IRelationObject relation in relations)
                        {
                            IBaseObjectValue relationObjectValue = DataService.GetValue(relation.Id, timePoint);
                            if (relationObjectValue != null)
                            {
                                xmlWriter.WriteStartElement(relationDefinition.Name);

                                WriteRegistrationAttributes(relationObjectValue, xmlWriter);
                                xmlWriter.WriteRaw(relationObjectValue.Text);

                                IBaseObjectValue childObjectValue = DataService.GetValue(relation.Relation2.Id, timePoint);
                                WriteCaseFileContent(relationDefinition.Entity, childObjectValue, timePoint, xmlWriter);

                                xmlWriter.WriteEndElement();
                            }
                        }
                    }
                }
            }
            xmlWriter.WriteEndElement();
        }

        private void WriteRegistrationAttributes(IBaseObjectValue baseObjectValue, XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString(_registrationIdAttributeName, baseObjectValue.Parent.Id.ToString());
            xmlWriter.WriteAttributeString(_registrationStartAttributeName, baseObjectValue.Start.TimeValue.ToString("O"));
            if (!baseObjectValue.End.Equals(TimePoint.Future))
            {
                xmlWriter.WriteAttributeString(_registrationEndAttributeName, baseObjectValue.End.TimeValue.ToString("O"));
            }

        }
        #endregion
    
    }
}
