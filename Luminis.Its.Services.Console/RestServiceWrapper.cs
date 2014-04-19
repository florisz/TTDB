using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Linq;

using Luminis.Its.Services.Rest;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Console
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Prefix)]
    public class RestServiceWrapper : IRestService
    {
        #region Private Properties
        private IRestService _innerRestService;
        private IRestService RestService
        {
            get
            {
                return _innerRestService;
            }
        }
        #endregion

        #region Constructors
        public RestServiceWrapper()
        {
            _innerRestService = Program.Container.Resolve<IRestService>();
        }
        #endregion

        #region IRestService Members
        public Stream GetCaseFile(string objectmodelname, string specificationname)
        {
            return this.RestService.GetCaseFile(objectmodelname, specificationname);
        }

        public Stream GetCaseFiles(string objectmodelname, string specificationname)
        {
            return this.RestService.GetCaseFiles(objectmodelname, specificationname);
        }

        public Stream GetCaseFileSpecification(string objectmodelname, string specificationname)
        {
            return this.RestService.GetCaseFileSpecification(objectmodelname, specificationname);
        }

        public Stream GetCaseFileSpecifications(string objectmodelname)
        {
            return this.RestService.GetCaseFileSpecifications(objectmodelname);
        }

        public Stream GetObjectModel(string objectmodelname)
        {
            return this.RestService.GetObjectModel(objectmodelname);
        }

        public Stream GetObjectModels()
        {
            return this.RestService.GetObjectModels();
        }

        public Stream GetRepresentation(string objectmodelname, string specificationname, string representationname)
        {
            return this.RestService.GetRepresentation(objectmodelname, specificationname, representationname);
        }

        public Stream GetRepresentations(string objectmodelname, string specificationname)
        {
            return this.RestService.GetRepresentations(objectmodelname, specificationname);
        }

        public Stream GetResource()
        {
            return this.RestService.GetResource();
        }

        public Stream GetResources()
        {
            return this.RestService.GetResources();
        }

        public Stream GetRepositoryInfo()
        {
            return this.RestService.GetRepositoryInfo();
        }

        public Stream GetRule(string objectmodelname, string specificationname, string rulename)
        {
            return this.RestService.GetRule(objectmodelname, specificationname, rulename);
        }

        public Stream GetRules(string objectmodelname, string specificationname)
        {
            return this.RestService.GetRules(objectmodelname, specificationname);
        }

        public Stream GetXmlSchema(string schemaname)
        {
            return this.RestService.GetXmlSchema(schemaname);
        }

        public Stream GetXmlSchemas()
        {
            return this.RestService.GetXmlSchemas();
        }

        public Stream StoreCaseFile(string objectmodelname, string specificationname, Stream casefile)
        {
            return this.RestService.StoreCaseFile(objectmodelname, specificationname, casefile);
        }

        public Stream StoreCaseFileSpecification(string objectmodelname, string specificationname, Stream specification)
        {
            return this.RestService.StoreCaseFileSpecification(objectmodelname, specificationname, specification);
        }

        public Stream StoreObjectModel(string objectmodelname, Stream objectmodel)
        {
            return this.RestService.StoreObjectModel(objectmodelname, objectmodel);
        }

        public Stream StoreRepresentation(string objectmodelname, string specificationname, string representationname, Stream  representation)
        {
            return this.RestService.StoreRepresentation(objectmodelname, specificationname, representationname, representation);
        }

        public Stream StoreResource(Stream resource)
        {
            return this.RestService.StoreResource(resource);
        }

        public Stream StoreRule(string objectmodelname, string specificationname, string rulename, Stream rule)
        {
            return this.RestService.StoreRule(objectmodelname, specificationname, rulename, rule);
        }

        #endregion
    }
}
