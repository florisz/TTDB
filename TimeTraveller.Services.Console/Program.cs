using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using log4net.Config;
using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFiles.Impl;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.CaseFileSpecifications.Impl;
<<<<<<< HEAD
using TimeTraveller.Services.Console;
using TimeTraveller.Services.Data.Impl;
=======
>>>>>>> 553a58cd090c78769c01e1111d448a0f14171233
using TimeTraveller.Services.ObjectModels;
using TimeTraveller.Services.ObjectModels.Impl;
using TimeTraveller.Services.Repository;
using TimeTraveller.Services.Repository.Impl;
using TimeTraveller.Services.Representations;
using TimeTraveller.Services.Representations.Impl;
using TimeTraveller.Services.Resources;
using TimeTraveller.Services.Resources.Impl;
using TimeTraveller.Services.Rest.Impl.Commands.CaseFiles;
using TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications;
using TimeTraveller.Services.Rest.Impl.Commands.ObjectModels;
using TimeTraveller.Services.Rest.Impl.Commands.Repository;
using TimeTraveller.Services.Rest.Impl.Commands.Representations;
using TimeTraveller.Services.Rest.Impl.Commands.Resources;
using TimeTraveller.Services.Rest.Impl.Commands.Rules;
using TimeTraveller.Services.Rest.Impl.Formatters;
using TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles;
using TimeTraveller.Services.Rest.Impl.Formatters.CaseFileSpecifications;
using TimeTraveller.Services.Rest.Impl.Formatters.ObjectModels;
using TimeTraveller.Services.Rest.Impl.Formatters.Representations;
using TimeTraveller.Services.Rest.Impl.Formatters.Resources;
using TimeTraveller.Services.Rest.Impl.Formatters.Rules;
using TimeTraveller.Services.Rules;
using TimeTraveller.Services.Rules.FSharp;
using TimeTraveller.Services.Rules.Impl;
using TimeTraveller.General.Logging.Log4Net;
using TimeTraveller.General.Unity;
using TimeTraveller.General.Unity.Impl;
using Microsoft.Practices.Unity;
using TimeTraveller.Services.Rest;
using TimeTraveller.Services.Rest.Impl;
using TimeTraveller.General.Logging;
using ILogger = TimeTraveller.General.Logging.ILogger;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Data.CouchDB;

namespace TimeTraveller.Services.Console
{
    class Program
    {
        public static IUnityContainer Container;

        private static ILogger _logger = ConsoleLogger.Instance;

        static void Main(string[] args)
        {
            ServiceHost serviceHost = null;
            try
            {
                XmlConfigurator.Configure(new FileInfo("Log4Net.config"));

                Container = BuildTTDBContainer();

                _logger = Container.Resolve<ILogger>();

                serviceHost = new ServiceHost(typeof(RestServiceWrapper));
                serviceHost.Open();

                StringBuilder endPoints = new StringBuilder();
                foreach (ServiceEndpoint serviceEndpoint in serviceHost.Description.Endpoints)
                {
                    if (endPoints.Length > 0)
                    {
                        endPoints.Append(", ");
                    }
                    endPoints.Append(serviceEndpoint.Address);
                }

                _logger.DebugFormat("Service is listening on {0}", endPoints);

                bool shouldContinue = true;
                while (shouldContinue)
                {
                    System.Console.WriteLine("Press c + ENTER to cleanup the database...");
                    System.Console.WriteLine("Press ENTER to exit.");
                    string line = System.Console.ReadLine();

                    shouldContinue = HandleCommand(line);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected exception", exception);
                System.Console.WriteLine("Press ENTER to exit.");
                System.Console.ReadLine();
            }
            finally
            {
                if (serviceHost != null && serviceHost.State == CommunicationState.Opened)
                {
                    _logger.DebugFormat("Service has stopped", serviceHost);
                    serviceHost.Close();
                }
            }
        }

        private static bool HandleCommand(string line)
        {
            if (string.IsNullOrEmpty(line))
            	return false;
            
            if (line.Equals("c", StringComparison.InvariantCultureIgnoreCase))
            {
                IDataService dataService = Container.Resolve<IDataService>();
                dataService.Clean();
                return true;
            }
            
            _logger.DebugFormat("Invalid command {0}", line);
            return true;
        }

        private static IUnityContainer BuildTTDBContainer()
        {
            var container = new UnityContainer()
                .RegisterType<ICaseFileService, CaseFileService>()
                .RegisterType<ICaseFileSpecificationService, CaseFileSpecificationService>()
                .RegisterType<IDataService, CouchDB>()
                .RegisterType<IObjectModelService, ObjectModelService>()
                .RegisterType<IRepositoryService, RepositoryService>()
                .RegisterType<IRepresentationService, RepresentationService>()
                .RegisterType<IResourceService, ResourceService>()
                .RegisterType<IRestService, RestService>()
                .RegisterType<IRuleService, RuleService>()
                .RegisterType<IRepresentationTransformer, XsltTransformer>("xslt")
                .RegisterType<IRuleEngine, FSharpRuleEngine>("fsharp")
                .RegisterType<IFormatter, HtmlFormatter>("HistoryHtmlFormatter", new InjectionConstructor("scripts/itsbrowserhistory.xslt"))
                .RegisterType<IFormatter, HtmlFormatter>("ListHtmlFormatter", new InjectionConstructor("scripts/itsbrowserlist.xslt"))
                .RegisterType<IFormatter, HtmlFormatter>("SummaryHtmlFormatter", new InjectionConstructor("scripts/itsbrowsersummary.xslt"))
                .RegisterType<IFormatter, TextFormatter>("TextFormatter")
                .RegisterType<IFormatter, XmlFormatter>("XmlFormatter")
                .RegisterType<ICommand, GetCaseFileByTimePointCommand>("GETCaseFileTimePointCommand")
                .RegisterType<ICommand, GetCaseFileByVersionCommand>("GETCaseFileVersionCommand")
                .RegisterType<ICommand, GetLatestCaseFileCommand>("GETCaseFileCommand")
                .RegisterType<ICommand, GetCaseFilesCommand>("GETCaseFilesCommand")
                .RegisterType<ICommand, StoreCaseFileCommand>("PUTCaseFileCommand")
                .RegisterType<IFormatter, CaseFileHistoryFormatter>("CaseFileHistoryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("HistoryHtmlFormatter")))
                .RegisterType<IFormatter, CaseFileHistoryFormatter>("CaseFileHistoryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileListFormatter>("CaseFilesHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, CaseFileListFormatter>("CaseFilesXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileSummaryFormatter>("CaseFileSummaryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("SummaryHtmlFormatter")))
                .RegisterType<IFormatter, CaseFileSummaryFormatter>("CaseFileSummaryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileByRepresentationFormatter>("CaseFileRepresentationFormatter")
                .RegisterType<IFormatter, CaseFileXmlFormatter>("CaseFileFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileEditFormatter>("CaseFileEditXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileEditFormatter>("CaseFileEditHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<ICommand, GetCaseFileSpecificationByTimePointCommand>("GETCaseFileSpecificationTimePointCommand")
                .RegisterType<ICommand, GetCaseFileSpecificationByVersionCommand>("GETCaseFileSpecificationVersionCommand")
                .RegisterType<ICommand, GetLatestCaseFileSpecificationCommand>("GETCaseFileSpecificationCommand")
                .RegisterType<ICommand, GetCaseFileSpecificationsCommand>("GETCaseFileSpecificationsCommand")
                .RegisterType<ICommand, StoreCaseFileSpecificationCommand>("PUTCaseFileSpecificationCommand")
                .RegisterType<IFormatter, CaseFileSpecificationAssemblyFormatter>("CaseFileSpecification.dllFormatter")
                .RegisterType<IFormatter, CaseFileSpecificationHistoryFormatter>("CaseFileSpecificationHistoryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("HistoryHtmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationHistoryFormatter>("CaseFileSpecificationHistoryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationListFormatter>("CaseFileSpecificationsHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationListFormatter>("CaseFileSpecificationsXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationSummaryFormatter>("CaseFileSpecificationSummaryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("SummaryHtmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationSummaryFormatter>("CaseFileSpecificationSummaryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationXmlFormatter>("CaseFileSpecificationFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, CaseFileSpecificationXmlSchemaFormatter>("CaseFileSpecification.xsdFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<ICommand, GetObjectModelByTimePointCommand>("GETObjectModelTimePointCommand")
                .RegisterType<ICommand, GetObjectModelByVersionCommand>("GETObjectModelVersionCommand")
                .RegisterType<ICommand, GetLatestObjectModelCommand>("GETObjectModelCommand")
                .RegisterType<ICommand, GetObjectModelsCommand>("GETObjectModelsCommand")
                .RegisterType<ICommand, StoreObjectModelCommand>("PUTObjectModelCommand")
                .RegisterType<IFormatter, ObjectModelHistoryFormatter>("ObjectModelHistoryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("HistoryHtmlFormatter")))
                .RegisterType<IFormatter, ObjectModelHistoryFormatter>("ObjectModelHistoryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ObjectModelListFormatter>("ObjectModelsHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, ObjectModelListFormatter>("ObjectModelsXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ObjectModelSummaryFormatter>("ObjectModelSummaryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("SummaryHtmlFormatter")))
                .RegisterType<IFormatter, ObjectModelSummaryFormatter>("ObjectModelSummaryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ObjectModelXmlFormatter>("ObjectModelFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ObjectModelXmlSchemaFormatter>("ObjectModel.xsdFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<ICommand, GetRepositoryListCommand>("GETRepositorysCommand")
                .RegisterType<ICommand, GetRepositoryXmlSchemaCommand>("GETRepositoryXmlSchemaCommand")
                .RegisterType<ICommand, GetRepositoryXmlSchemasCommand>("GETRepositoryXmlSchemasCommand")
                .RegisterType<IFormatter, ChainedXmlFormatter>("RepositorysHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, XmlFormatter>("RepositorysXmlFormatter")
                .RegisterType<IFormatter, XmlFormatter>("RepositoryXmlSchema.xsdFormatter")
                .RegisterType<IFormatter, ChainedXmlFormatter>("RepositoryXmlSchemasHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, XmlFormatter>("RepositoryXmlSchemasXmlFormatter")
                .RegisterType<ICommand, GetRepresentationByTimePointCommand>("GETRepresentationTimePointCommand")
                .RegisterType<ICommand, GetRepresentationByVersionCommand>("GETRepresentationVersionCommand")
                .RegisterType<ICommand, GetLatestRepresentationCommand>("GETRepresentationCommand")
                .RegisterType<ICommand, GetRepresentationsCommand>("GETRepresentationsCommand")
                .RegisterType<ICommand, StoreRepresentationCommand>("PUTRepresentationCommand")
                .RegisterType<IFormatter, RepresentationHistoryFormatter>("RepresentationHistoryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("HistoryHtmlFormatter")))
                .RegisterType<IFormatter, RepresentationHistoryFormatter>("RepresentationHistoryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, RepresentationListFormatter>("RepresentationsHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, RepresentationListFormatter>("RepresentationsXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, RepresentationSummaryFormatter>("RepresentationSummaryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("SummaryHtmlFormatter")))
                .RegisterType<IFormatter, RepresentationSummaryFormatter>("RepresentationSummaryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, RepresentationXmlFormatter>("RepresentationFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<ICommand, GetResourceByTimePointCommand>("GETResourceTimePointCommand")
                .RegisterType<ICommand, GetResourceByVersionCommand>("GETResourceVersionCommand")
                .RegisterType<ICommand, GetLatestResourceCommand>("GETResourceCommand")
                .RegisterType<ICommand, GetResourcesCommand>("GETResourcesCommand")
                .RegisterType<ICommand, StoreResourceCommand>("PUTResourceCommand")
                .RegisterType<IFormatter, ResourceHistoryFormatter>("ResourceHistoryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("HistoryHtmlFormatter")))
                .RegisterType<IFormatter, ResourceHistoryFormatter>("ResourceHistoryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ResourceListFormatter>("ResourcesHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, ResourceListFormatter>("ResourcesXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ResourceSummaryFormatter>("ResourceSummaryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("SummaryHtmlFormatter")))
                .RegisterType<IFormatter, ResourceSummaryFormatter>("ResourceSummaryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, ResourceFormatter>("ResourceFormatter")
                .RegisterType<ICommand, GetRuleByTimePointCommand>("GETRuleTimePointCommand")
                .RegisterType<ICommand, GetRuleByVersionCommand>("GETRuleVersionCommand")
                .RegisterType<ICommand, GetLatestRuleCommand>("GETRuleCommand")
                .RegisterType<ICommand, GetRulesCommand>("GETRulesCommand")
                .RegisterType<ICommand, ExecuteRuleByTimePointCommand>("POSTRuleTimePointCommand")
                .RegisterType<ICommand, ExecuteRuleByVersionCommand>("POSTRuleVersionCommand")
                .RegisterType<ICommand, ExecuteLatestRuleCommand>("POSTRuleCommand")
                .RegisterType<ICommand, StoreRuleCommand>("PUTRuleCommand")
                .RegisterType<IFormatter, RuleHistoryFormatter>("RuleHistoryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("HistoryHtmlFormatter")))
                .RegisterType<IFormatter, RuleHistoryFormatter>("RuleHistoryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, RuleListFormatter>("RulesHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("ListHtmlFormatter")))
                .RegisterType<IFormatter, RuleListFormatter>("RulesXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, RuleSummaryFormatter>("RuleSummaryHtmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("SummaryHtmlFormatter")))
                .RegisterType<IFormatter, RuleSummaryFormatter>("RuleSummaryXmlFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<IFormatter, RuleXmlFormatter>("RuleFormatter", new InjectionConstructor(new ResolvedParameter<IFormatter>("XmlFormatter")))
                .RegisterType<ILogger, Log4NetLogger>()
                .RegisterType<IUnity, UnityContainerWrapper>(new ContainerControlledLifetimeManager())
                ;

            // a variable is used for debugging purposes
            return container;
        }
    }
}
