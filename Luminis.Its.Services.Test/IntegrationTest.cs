using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Xml;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFiles.Impl;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.CaseFileSpecifications.Impl;
using Luminis.Its.Services.Data;
using Luminis.Its.Services.Data.Impl;
using Luminis.Its.Services.ObjectModels;
using Luminis.Its.Services.ObjectModels.Impl;
using Luminis.Its.Services.Rest;
using Luminis.Its.Services.Rest.Impl;
using Luminis.Its.Services.Rest.Impl.Commands.CaseFiles;
using Luminis.Its.Services.Rest.Impl.Commands.CaseFileSpecifications;
using Luminis.Its.Services.Rest.Impl.Commands.ObjectModels;
using Luminis.Its.Services.Rest.Impl.Commands.Representations;
using Luminis.Its.Services.Rest.Impl.Commands.Rules;
using Luminis.Its.Services.Rest.Impl.Formatters;
using Luminis.Its.Services.Rest.Impl.Formatters.CaseFiles;
using Luminis.Its.Services.Rest.Impl.Formatters.CaseFileSpecifications;
using Luminis.Its.Services.Rest.Impl.Formatters.ObjectModels;
using Luminis.Its.Services.Rest.Impl.Formatters.Representations;
using Luminis.Its.Services.Rest.Impl.Formatters.Rules;
using Luminis.Its.Services.Representations;
using Luminis.Its.Services.Representations.Impl;
using Luminis.Its.Services.Rules;
using Luminis.Its.Services.Rules.FSharp;
using Luminis.Its.Services.Rules.Impl;
using Luminis.Logging;
using Luminis.Logging.Log4Net;
using Luminis.Unity;

namespace Luminis.Its.Services.Test
{
    /// <summary>
    /// Summary description for IntegrationTest
    /// </summary>
    [TestFixture]
    public class IntegrationTest
    {
        private MockRepository _mocks;
        private ServiceHost _serviceHost;
        private ILogger _logger;
        private IUnity _unity;

        #region Additional test attributes before running the first test in the class
        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            BasicConfigurator.Configure();
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            LogManager.Shutdown();
        }

        [SetUp()]
        public void SetUp()
        {
            _mocks = new MockRepository();

            _mocks.ReplayAll();
        }

        [TearDown]
        public void TearDown()
        {
            _mocks.VerifyAll();
        }

        private void InitializeRestServiceHost()
        {
            IRestService service = new RestService(_logger, _unity);
            _serviceHost = new WebServiceHost(service, new Uri("http://localhost:8080/its"));
        }
        #endregion


        [Test]
        public void TestStoreAndRetrieve()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            InMemoryDataService dataService = new InMemoryDataService(_logger);
            IRepresentationTransformer xsltTransformer = new XsltTransformer();
            IObjectModelService objectModelService = new ObjectModelService(_logger, _unity, dataService);
            ICaseFileSpecificationService caseFileSpecificationService = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);
            IRepresentationService representationService = new RepresentationService(_logger, _unity, caseFileSpecificationService, dataService);
            ICaseFileService caseFileService = new CaseFileService(_logger, _unity, caseFileSpecificationService, representationService, dataService);
            IRuleService ruleService = new RuleService(_logger, _unity, caseFileSpecificationService, dataService);
            IRuleEngine fsharpRuleEngine = new FSharpRuleEngine(_logger, caseFileSpecificationService);

            Expect.On(_unity).Call(_unity.Resolve<ICaseFileService>()).Repeat.Any().Return(caseFileService);
            Expect.On(_unity).Call(_unity.Resolve<ICaseFileSpecificationService>()).Repeat.Any().Return(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<IObjectModelService>()).Repeat.Any().Return(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<IRepresentationService>()).Repeat.Any().Return(representationService);
            Expect.On(_unity).Call(_unity.Resolve<IRepresentationTransformer>("xslt")).Repeat.Any().Return(xsltTransformer);
            Expect.On(_unity).Call(_unity.Resolve<IRuleService>()).Repeat.Any().Return(ruleService);
            Expect.On(_unity).Call(_unity.Resolve<IRuleEngine>("fsharp")).Repeat.Any().Return(fsharpRuleEngine);

            ICommand getCaseFileCommand = new GetLatestCaseFileCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileCommand")).Repeat.Any().Return(getCaseFileCommand);

            ICommand getCaseFileSpecificationCommand = new GetLatestCaseFileSpecificationCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationCommand")).Repeat.Any().Return(getCaseFileSpecificationCommand);

            ICommand getObjectModelCommand = new GetLatestObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelCommand")).Repeat.Any().Return(getObjectModelCommand);

            IFormatter caseFileRepresentationFormatter = new CaseFileByRepresentationFormatter(caseFileService, representationService);
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileRepresentationXmlFormatter")).Repeat.Any().Return(caseFileRepresentationFormatter);

            IFormatter caseFileXmlFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Repeat.Any().Return(caseFileXmlFormatter);

            IFormatter caseFileSpecificationXmlFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Repeat.Any().Return(caseFileSpecificationXmlFormatter);

            IFormatter objectModelXmlFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Repeat.Any().Return(objectModelXmlFormatter);

            IFormatter representationXmlFormatter = new RepresentationXmlFormatter(new TextFormatter()) { RepresentationService = representationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepresentationXmlFormatter")).Repeat.Any().Return(representationXmlFormatter);

            IFormatter ruleXmlFormatter = new RuleXmlFormatter(new TextFormatter()) { CaseFileFormatter = caseFileXmlFormatter, RuleService = ruleService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RuleXmlFormatter")).Repeat.Any().Return(ruleXmlFormatter);

            ICommand storeCaseFileCommand = new StoreCaseFileCommand(caseFileSpecificationService, caseFileService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTCaseFileCommand")).Repeat.Any().Return(storeCaseFileCommand);

            ICommand storeCaseFileSpecificationCommand = new StoreCaseFileSpecificationCommand(caseFileSpecificationService, objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTCaseFileSpecificationCommand")).Repeat.Any().Return(storeCaseFileSpecificationCommand);

            ICommand storeObjectModelCommand = new StoreObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTObjectModelCommand")).Repeat.Any().Return(storeObjectModelCommand);

            ICommand storeRepresentationCommand = new StoreRepresentationCommand(representationService, caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTRepresentationCommand")).Repeat.Any().Return(storeRepresentationCommand);

            ICommand storeRuleCommand = new StoreRuleCommand(ruleService, caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTRuleCommand")).Repeat.Any().Return(storeRuleCommand);


            _mocks.ReplayAll();

            try
            {
                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                string objectModelXml = @"<ObjectModel xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
                                                   <Link rel=""self"" href=""http://localhost:8080/its/specifications/objectmodels/myobjectmodel""/>
                                                   <Name>myobjectmodel</Name>
                                                   <ObjectDefinitions>
                                                     <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                                       <Properties>
                                                         <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                                         <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                                         <Property Name=""Rules"" Type=""string"" Required=""false"" />
                                                       </Properties>
                                                     </ObjectDefinition>
                                                   </ObjectDefinitions>
                                                </ObjectModel>";
                byte[] requestBuffer = Encoding.Unicode.GetBytes(objectModelXml);

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@luminis.nl");
                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/specifications/objectmodels/myobjectmodel", "PUT", requestBuffer);
                string result = Encoding.Unicode.GetString(resultBuffer);
                _logger.DebugFormat("StoreObjectModel()={0}", result);

                Assert.IsNotNull(result);

                string specificationXml = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                                                    <Link rel=""objectmodel"" href=""http://localhost:8080/its/specifications/objectmodels/myobjectmodel""/>
                                                    <Link rel=""self"" href=""http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification""/>
                                                    <Name>myspecification</Name>
                                                    <UriTemplate>{/Person/FirstName}.MiddleName.{/Person/LastName}</UriTemplate>
                                                    <Structure>
                                                        <Entity Name=""Person"" Type=""Person""/>
                                                    </Structure>
                                                </CaseFileSpecification>";
                requestBuffer = Encoding.Unicode.GetBytes(specificationXml);

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@luminis.nl");
                resultBuffer = webClient.UploadData("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification", "PUT", requestBuffer);
                result = Encoding.Unicode.GetString(resultBuffer);
                _logger.DebugFormat("StoreCaseFileSpecification()={0}", result);

                Assert.IsNotNull(result);

                string representationXml = @"<Representation xmlns=""http://luminis.net/its/schemas/representation.xsd"">
                                                    <Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification""/>
                                                    <Link rel=""self"" href=""http://localhost:8080/its/representations/myobjectmodel/myspecification/myrepresentation""/>
                                                    <Name>myrepresentation</Name>
                                                    <Script Type=""xslt"" ContentType=""html"">
                                                        <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""> 
                                                          <xsl:template match=""/""> 
                                                            <html> 
                                                              <body> 
                                                                <xsl:value-of select=""Person/FirstName""/>
                                                                <xsl:text>.</xsl:text>
                                                                <xsl:value-of select=""Person/LastName""/>
                                                              </body> 
                                                            </html> 
                                                          </xsl:template> 
                                                        </xsl:stylesheet>
                                                    </Script>
                                                </Representation>";
                requestBuffer = Encoding.Unicode.GetBytes(representationXml);

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@luminis.nl");
                resultBuffer = webClient.UploadData("http://localhost:8080/its/representations/myobjectmodel/myspecification/myrepresentation", "PUT", requestBuffer);
                result = Encoding.Unicode.GetString(resultBuffer);
                _logger.DebugFormat("StoreRepresentation()={0}", result);

                Assert.IsNotNull(result);

                string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                                <cs:Link rel=""self"" href=""http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers""/>
                                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification""/>
                                                <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                </Person>
                                           </cs:CaseFile>", Guid.NewGuid(), DateTime.Now.ToString("O"));
                requestBuffer = Encoding.Unicode.GetBytes(caseFileXml);

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@luminis.nl");
                resultBuffer = webClient.UploadData("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers", "PUT", requestBuffer);
                result = Encoding.Unicode.GetString(resultBuffer);

                _logger.DebugFormat("StoreCaseFile()={0}", result.Replace("\n", "\r\n"));

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                resultBuffer = webClient.DownloadData("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers");
                result = Encoding.Unicode.GetString(resultBuffer);

                Assert.IsNotNull(result);
                _logger.DebugFormat("GetCaseFile()={0}", result.Replace("\n", "\r\n"));

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                resultBuffer = webClient.DownloadData("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers?view=myrepresentation");
                result = Encoding.Unicode.GetString(resultBuffer);
                Assert.IsNotNull(result);

                _logger.DebugFormat("GetCaseFile(view=myrepresentation)={0}", result.Replace("\n", "\r\n"));

                string ruleXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <Rule xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://luminis.net/its/schemas/rule.xsd"">
                              <Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification""/>
                              <Link rel=""self"" href=""http://localhost:8080/its/rules/myobjectmodel/myspecification/myrules"" />
                              <Name>MyRule</Name>
                              <Script Type=""fsharp"" Method=""myobjectmodel.myspecification.myrule.Execute"">#light
module myobjectmodel.myspecification.myrule

open System
open myobjectmodel.myspecification

let Execute(p: Person) =
    p.Rules &lt;- ""done.""</Script>
                            </Rule>";

                requestBuffer = Encoding.Unicode.GetBytes(ruleXml);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@luminis.nl");
                resultBuffer = webClient.UploadData("http://localhost:8080/its/rules/myobjectmodel/myspecification/myrule", "PUT", requestBuffer);
                result = Encoding.Unicode.GetString(resultBuffer);

                Assert.IsNotNull(!string.IsNullOrEmpty(result));

                _logger.DebugFormat("StoreRule()={0}", result);

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                resultBuffer = webClient.DownloadData("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers");
                result = Encoding.Unicode.GetString(resultBuffer);

                Assert.IsNotNull(result);
                _logger.DebugFormat("GetCaseFile() 2nd time={0}", result.Replace("\n", "\r\n"));

                // AH: 3 june 2009
                // This cannot be executed (yet) in the automatic build. FSharp is not installed on the build server.
                //
                //requestBuffer = Encoding.Unicode.GetBytes(result);
                //string executeRuleRequest = "http://localhost:8080/its/rules/myobjectmodel/myspecification/myrule";
                //webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                //resultBuffer = webClient.UploadData(executeRuleRequest, "POST", requestBuffer);
                //result = Encoding.Unicode.GetString(resultBuffer);

                //Assert.IsNotNull(!string.IsNullOrEmpty(result));

                //_logger.DebugFormat("ExecuteRule()={0}", result.Replace("\n", "\r\n"));

                dataService.LogContent();
            }
            catch (Exception exception)
            {
                _logger.Error("", exception);
                throw;
            }
            finally
            {
                if (_serviceHost != null && _serviceHost.State == CommunicationState.Opened)
                {
                    _serviceHost.Close();
                }
            }
        }
    }
}
