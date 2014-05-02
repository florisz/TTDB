using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Xml;

using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.ObjectModels;
using TimeTraveller.Services.Representations;
using TimeTraveller.Services.Resources;
using TimeTraveller.Services.Rules;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Repository.Impl
{
    public class RepositoryService : IRepositoryService
    {
        #region Private Properties
        private const string _timePointParameter = "timepoint";
        private const string _versionParameter = "version";

        private ILogger _logger;
        private IUnity _container;
        #endregion

        #region Constructors
        public RepositoryService(ILogger logger, IUnity container)
        {
            _logger = logger;
            _container = container;
        }
        #endregion

        #region IRepositoryService Members
        public string GetList(Uri baseUri, Encoding encoding)
        {
            StringBuilder resultXml = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.Encoding = encoding;
            settings.OmitXmlDeclaration = true;
            XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);

            xmlWriter.WriteStartElement("Links");

            IObjectModelService objectModelService = _container.Resolve<IObjectModelService>();
            IBaseObjectType objectModelType = objectModelService.BaseObjectType;
            IResourceService resourceService = _container.Resolve<IResourceService>();
            IBaseObjectType resourceType = resourceService.BaseObjectType;
            
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", string.Format("{0}{1}", baseUri, "/schemas/"));
            xmlWriter.WriteEndElement(); // Link

            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", objectModelType.Name);
            xmlWriter.WriteAttributeString("href", string.Format("{0}{1}", baseUri, objectModelType.RelativeUri));
            xmlWriter.WriteEndElement(); // Link

            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", resourceType.Name);
            xmlWriter.WriteAttributeString("href", string.Format("{0}{1}", baseUri, resourceType.RelativeUri));
            xmlWriter.WriteEndElement(); // Link

            xmlWriter.WriteEndElement(); // Links
            xmlWriter.Close();

            string result = resultXml.ToString();
            return result;
        }

        public ObjectModel GetObjectModel(string objectmodelname, Uri baseUri, NameValueCollection queryParameters)
        {
            IObjectModelService objectModelService = _container.Resolve<IObjectModelService>();

            ObjectModel result = null;
            string queryParameterValue = queryParameters[_versionParameter];
            if (!string.IsNullOrEmpty(queryParameterValue))
            {
                _logger.DebugFormat("?{0}={1}", _versionParameter, queryParameterValue);
                int versionNumber = int.Parse(queryParameterValue);
                result = objectModelService.Get(objectmodelname, versionNumber, baseUri);
            }
            else
            {
                queryParameterValue = queryParameters[_timePointParameter];
                if (!string.IsNullOrEmpty(queryParameterValue))
                {
                    //Format: yyyy-MM-ddTHH:mm:ss.fffffffK, ie. 2008-04-10T06:30:00.0000000+01:00
                    TimePoint timePoint = new TimePoint(DateTime.Parse(queryParameterValue, CultureInfo.InvariantCulture));
                    _logger.DebugFormat("?{0}={1} ({2})", _timePointParameter, queryParameterValue, timePoint.ToString("O"));

                    result = objectModelService.Get(objectmodelname, timePoint, baseUri);
                }
                else
                {
                    result = objectModelService.Get(objectmodelname, baseUri);
                }
            }

            return result;
        }

        public IEnumerable<ObjectModel> GetObjectModels(Uri baseUri)
        {
            IObjectModelService objectModelService = _container.Resolve<IObjectModelService>();

            return objectModelService.GetEnumerable(baseUri);
        }

        public string GetXmlSchema(string schemaName)
        {
            string result = string.Empty;

            IObjectModelService objectModelService = _container.Resolve<IObjectModelService>();
            if (objectModelService.GetXmlSchemaName().Equals(schemaName))
            {
                result = objectModelService.GetXmlSchemaText();
            }
            if (string.IsNullOrEmpty(result))
            {
                ICaseFileSpecificationService caseFileSpecificationService = _container.Resolve<ICaseFileSpecificationService>();
                if (caseFileSpecificationService.GetXmlSchemaName().Equals(schemaName))
                {
                    result = caseFileSpecificationService.GetXmlSchemaText();
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                ICaseFileService caseFileService = _container.Resolve<ICaseFileService>();
                if (caseFileService.GetXmlSchemaName().Equals(schemaName))
                {
                    result = caseFileService.GetXmlSchemaText();
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                IRepresentationService representationService = _container.Resolve<IRepresentationService>();
                if (representationService.GetXmlSchemaName().Equals(schemaName))
                {
                    result = representationService.GetXmlSchemaText();
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                IRuleService ruleService = _container.Resolve<IRuleService>();
                if (ruleService.GetXmlSchemaName().Equals(schemaName))
                {
                    result = ruleService.GetXmlSchemaText();
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentException(string.Format("Invalid schema {0}", schemaName));
            }

            return result;
        }

        public string GetXmlSchemas(Uri baseUri, Encoding encoding)
        {
            StringBuilder resultXml = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.Encoding = encoding;
            settings.Indent = true;

            XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);
            xmlWriter.WriteStartElement("Schemas");

            IObjectModelService objectModelService = _container.Resolve<IObjectModelService>();
            string schemaAddress = objectModelService.GetXmlSchemaAddress(baseUri);
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", schemaAddress);
            xmlWriter.WriteEndElement();
            
            ICaseFileSpecificationService caseFileSpecificationService = _container.Resolve<ICaseFileSpecificationService>();
            schemaAddress = caseFileSpecificationService.GetXmlSchemaAddress(baseUri);
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", schemaAddress);
            xmlWriter.WriteEndElement();

            ICaseFileService caseFileService = _container.Resolve<ICaseFileService>();
            schemaAddress = caseFileService.GetXmlSchemaAddress(baseUri);
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", schemaAddress);
            xmlWriter.WriteEndElement();

            IRepresentationService representationService = _container.Resolve<IRepresentationService>();
            schemaAddress = representationService.GetXmlSchemaAddress(baseUri);
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", schemaAddress);
            xmlWriter.WriteEndElement();

            IRuleService ruleService = _container.Resolve<IRuleService>();
            schemaAddress = ruleService.GetXmlSchemaAddress(baseUri);
            xmlWriter.WriteStartElement("Link");
            xmlWriter.WriteAttributeString("rel", "schema");
            xmlWriter.WriteAttributeString("href", schemaAddress);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            string result = resultXml.ToString();
            return result;
        }
        #endregion
    }
}
