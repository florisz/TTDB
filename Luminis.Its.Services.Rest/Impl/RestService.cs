using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.ObjectModels;
using Luminis.Its.Services.Representations;
using Luminis.Its.Services.Resources;
using Luminis.Its.Services.Rules;
using Luminis.Logging;
using Luminis.Unity;

namespace Luminis.Its.Services.Rest.Impl
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RestService : IRestService
    {
        #region Private Properties
        private ILogger _logger = NullLogger.Instance;
        private IUnity _container;
        #endregion

        #region Constructors
        public RestService(ILogger logger, IUnity container)
        {
            _logger = logger;
            _container = container;
        }
        #endregion

        #region IRestService Members
        public Stream GetCaseFile(string objectmodelname, string specificationname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(CaseFile), objectmodelname, specificationname);

            return Execute(context);
        }

        public Stream GetCaseFiles(string objectmodelname, string specificationname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(CaseFile), true, objectmodelname, specificationname);

            return Execute(context);
        }

        public Stream GetCaseFileSpecification(string objectmodelname, string specificationname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(CaseFileSpecification), objectmodelname, specificationname);

            return Execute(context);
        }

        public Stream GetCaseFileSpecifications(string objectmodelname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(CaseFileSpecification), true, objectmodelname);

            return Execute(context);
        }

        public Stream GetObjectModel(string objectmodelname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(ObjectModel), objectmodelname);

            return Execute(context);
        }

        public Stream GetObjectModels()
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(ObjectModel), true);

            return Execute(context);
        }

        public Stream GetRepositoryInfo()
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, "Repository", true);

            return Execute(context);
        }

        public Stream GetRepresentation(string objectmodelname, string specificationname, string representationname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Representation), objectmodelname, specificationname, representationname);

            return Execute(context);
        }

        public Stream GetRepresentations(string objectmodelname, string specificationname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Representation), true, objectmodelname, specificationname);

            return Execute(context);
        }

        public Stream GetResource()
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Resource));

            return Execute(context);
        }

        public Stream GetResources()
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Resource), true);

            return Execute(context);
        }

        public Stream GetRule(string objectmodelname, string specificationname, string rulename)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Rule), objectmodelname, specificationname, rulename);

            return Execute(context);
        }

        public Stream GetRules(string objectmodelname, string specificationname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Rule), true, objectmodelname, specificationname);

            return Execute(context);
        }

        public Stream GetXmlSchema(string schemaname)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, "RepositoryXmlSchema", schemaname);

            return Execute(context);
        }

        public Stream GetXmlSchemas()
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, "RepositoryXmlSchema", true);

            return Execute(context);
        }

        public Stream StoreCaseFile(string objectmodelname, string specificationname, Stream caseFileStream)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(CaseFile), objectmodelname, specificationname, caseFileStream);

            return Execute(context);
        }

        public Stream StoreCaseFileSpecification(string objectmodelname, string specificationname, Stream specificationStream)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(CaseFileSpecification), objectmodelname, specificationname, specificationStream);

            return Execute(context);
        }

        public Stream StoreObjectModel(string objectmodelname, Stream objectmodelStream)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(ObjectModel), objectmodelname, objectmodelStream);

            return Execute(context);
        }

        public Stream StoreRepresentation(string objectmodelname, string specificationname, string representationname, Stream representationStream)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Representation), objectmodelname, specificationname, representationname, representationStream);

            return Execute(context);
        }

        public Stream StoreResource(Stream resourceStream)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Resource), resourceStream);

            return Execute(context);
        }

        public Stream StoreRule(string objectmodelname, string specificationname, string rulename, Stream ruleStream)
        {
            CommandContext context = new CommandContext(WebOperationContext.Current, typeof(Rule), objectmodelname, specificationname, rulename, ruleStream);

            return Execute(context);
        }
        #endregion

        #region Private Methods
        private Stream Execute(CommandContext context)
        {
            try
            {
                ICommand command = CommandFactory.Create(_container, context);
                IFormatter formatter = FormatterFactory.Create(_container, context);

                Stream result = command.Execute(context, formatter);

                if (context.ContentType != WebOperationContentType.Other)
                {
                    context.Response.ContentType = WebOperationContentTypeHelper.Convert(context.ContentType, context.Encoding);
                }
                return result;
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected exception", exception);
                throw;
            }
        }
        #endregion
    }
}

