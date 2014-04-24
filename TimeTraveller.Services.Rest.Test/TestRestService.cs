using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using log4net;
using log4net.Config;
using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFiles.Impl;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.CaseFileSpecifications.Impl;
using TimeTraveller.Services.Data;
using TimeTraveller.Services.Data.Impl;
using TimeTraveller.Services.ObjectModels;
using TimeTraveller.Services.ObjectModels.Impl;
using TimeTraveller.Services.Repository;
using TimeTraveller.Services.Repository.Impl;
using TimeTraveller.Services.Representations;
using TimeTraveller.Services.Representations.Impl;
using TimeTraveller.Services.Resources;
using TimeTraveller.Services.Rest.Impl;
using TimeTraveller.Services.Rest.Impl.Commands.CaseFiles;
using TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications;
using TimeTraveller.Services.Rest.Impl.Commands.ObjectModels;
using TimeTraveller.Services.Rest.Impl.Commands.Repository;
using TimeTraveller.Services.Rest.Impl.Commands.Representations;
using TimeTraveller.Services.Rest.Impl.Commands.Rules;
using TimeTraveller.Services.Rest.Impl.Formatters;
using TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles;
using TimeTraveller.Services.Rest.Impl.Formatters.CaseFileSpecifications;
using TimeTraveller.Services.Rest.Impl.Formatters.ObjectModels;
using TimeTraveller.Services.Rest.Impl.Formatters.Representations;
using TimeTraveller.Services.Rest.Impl.Formatters.Rules;
using TimeTraveller.Services.Rules;
using TimeTraveller.Services.Rules.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Logging.Log4Net;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace TimeTraveller.Services.Rest.Test
{
    /// <summary>
    /// Summary description for TestRestService
    /// </summary>
    [TestFixture]
    public class TestRestService
    {
        private MockRepository _mocks;
        private ServiceHost _serviceHost;
        private ILogger _logger;
        private IUnity _unity;
        private IObjectModelService _objectModelServiceImpl;
        private ICaseFileSpecificationService _caseFileSpecificationServiceImpl;
        private ICaseFileService _caseFileServiceImpl;
        private IRepositoryService _repositoryServiceImpl;
        private IRepresentationService _representationServiceImpl;
        private IRuleService _ruleServiceImpl;

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

        [SetUp]
        public void TestInitialize()
        {
            _mocks = new MockRepository();
        }


        [TearDown]
        public void TestCleanup()
        {
            _mocks.VerifyAll();
        }

        private void InitializeRestServiceHost()
        {
            IRestService service = new RestService(_logger, _unity);
            _serviceHost = new WebServiceHost(service, new Uri("http://localhost:8080/storage"));
        }

        [Test]
        public void TestConstructor()
        {
            _mocks.Record();

            _logger = _mocks.StrictMock<ILogger>();
            _unity = _mocks.StrictMock<IUnity>();

            _mocks.ReplayAll();

            IRestService service = new RestService(_logger, _unity);

            Assert.IsNotNull(service);
        }

        private delegate string GetCaseFileSpecificationXmlDelegate(CaseFileSpecification specification, Encoding encoding);

        [Test]
        public void TestGetLatestCaseFileSpecification()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand getCaseFileSpecificationCommand = new GetLatestCaseFileSpecificationCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationCommand")).Return(getCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationXmlFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Return(caseFileSpecificationXmlFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = caseFileSpecificationType
                    },
                    Version = ++index

                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.ObjectModel,
                                RelativeUri = "/specifications/objectmodels/"
                            }
                        },
                        Version = ++index
                    },
                    Name = "myobjectmodel",
                    SelfUri = string.Empty
                },
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{primaryidentfier}/{secundaryidentifier}/thirdidentifier/{thirdidentifier}"
            };
            myspecification.ObjectModelUri = myspecification.ObjectModel.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            myspecification.SelfUri = myspecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXml(null as CaseFileSpecification, Encoding.Unicode)).IgnoreArguments().Do(
                new GetCaseFileSpecificationXmlDelegate(
                    delegate(CaseFileSpecification specification, Encoding endcoding)
                    {
                        return _caseFileSpecificationServiceImpl.GetXml(specification, endcoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification");
                string result = Encoding.UTF8.GetString(resultBuffer);
                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myspecification</Name>"));
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


        private delegate string GetCaseFileSpecificationXmlSchemaDelegate(CaseFileSpecification specification, Encoding encoding);

        [Test]
        public void TestGetLatestCaseFileSpecificationXmlSchema()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand getCaseFileSpecificationCommand = new GetLatestCaseFileSpecificationCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationCommand")).Return(getCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationFormatter = new CaseFileSpecificationXmlSchemaFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecification.xsdFormatter")).Return(caseFileSpecificationFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = caseFileSpecificationType
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.ObjectModel,
                                RelativeUri = "/specifications/objectmodels/"
                            }
                        },
                        Version = ++index
                    },
                    Name = "myobjectmodel",
                    ObjectDefinitions = new ObjectDefinition[] {
                        new ObjectDefinition() {
                            Name = "Person",
                            ObjectType = ObjectType.entity,
                            Properties = new ObjectDefinitionProperty[] {
                                new ObjectDefinitionProperty() {
                                    Name = "FirstName",
                                    Type = "string",
                                    Required = true,
                                    RequiredSpecified = true
                                },
                                new ObjectDefinitionProperty() {
                                    Name = "LastName",
                                    Type = "string",
                                    Required = true,
                                    RequiredSpecified = true
                                }
                            }
                        }
                    },
                    SelfUri = string.Empty,
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{primaryidentfier}/{secundaryidentifier}/thirdidentifier/{thirdidentifier}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXmlSchema(myspecification, Encoding.UTF8)).Do(
                new GetCaseFileSpecificationXmlSchemaDelegate(
                    delegate(CaseFileSpecification specification, Encoding encoding)
                    {
                        return _caseFileSpecificationServiceImpl.GetXmlSchema(specification, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification.xsd");
                string result = Encoding.UTF8.GetString(resultBuffer);
                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<xs:schema"));
                Assert.IsTrue(result.Contains(@"id=""myspecification"""));
                Assert.IsTrue(result.Contains(@"<xs:element name=""Person"" type=""Person"""));
                Assert.IsTrue(result.Contains(@"<xs:complexType name=""Person"""));
                Assert.IsTrue(result.EndsWith(@"</xs:schema>"));
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


        [Test]
        public void TestGetCaseFileSpecificationByTimePoint()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand getCaseFileSpecificationCommand = new GetCaseFileSpecificationByTimePointCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationTimePointCommand")).Return(getCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationXmlFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Return(caseFileSpecificationXmlFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = caseFileSpecificationType
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.ObjectModel,
                                RelativeUri = "/specifications/objectmodels/"
                            }
                        },
                        Version = ++index
                    },
                    Name = "myobjectmodel",
                    SelfUri = string.Empty
                },
                UriTemplate = "{primaryidentfier}/{secundaryidentifier}/thirdidentifier/{thirdidentifier}"
            };
            myspecification.ObjectModelUri = myspecification.ObjectModel.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            myspecification.SelfUri = myspecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
 
            TimePoint timePoint = new TimePoint(DateTime.ParseExact("2009-01-01T13:50:10.0000000+01:00", "O", CultureInfo.InvariantCulture));
            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", timePoint, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXml(null as CaseFileSpecification, Encoding.Unicode)).IgnoreArguments().Do(
                new GetCaseFileSpecificationXmlDelegate(
                    delegate(CaseFileSpecification specification, Encoding encoding)
                    {
                        return _caseFileSpecificationServiceImpl.GetXml(specification, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?timepoint=2009-01-01T13:50:10");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myspecification</Name>"));
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

        [Test]
        public void TestGetCaseFileSpecificationByVersion()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand getCaseFileSpecificationCommand = new GetCaseFileSpecificationByVersionCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationVersionCommand")).Return(getCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationXmlFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) {SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Return(caseFileSpecificationXmlFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = caseFileSpecificationType
                    },
                    Version = ++index

                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.ObjectModel,
                                RelativeUri = "/specifications/objectmodels/"
                            }
                        },
                        Version = ++index
                    },
                    Name = "myobjectmodel",
                    SelfUri = string.Empty
                },
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{primaryidentfier}/{secundaryidentifier}/thirdidentifier/{thirdidentifier}"
            };
            myspecification.ObjectModelUri = myspecification.ObjectModel.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            myspecification.SelfUri = myspecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", myspecification.BaseObjectValue.Version, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXml(null as CaseFileSpecification, Encoding.Unicode)).IgnoreArguments().Do(
                new GetCaseFileSpecificationXmlDelegate(
                    delegate(CaseFileSpecification specification, Encoding endcoding)
                    {
                        return _caseFileSpecificationServiceImpl.GetXml(specification, endcoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1");
                string result = Encoding.UTF8.GetString(resultBuffer);
                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myspecification</Name>"));
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

        private delegate CaseFileSpecification GetCaseFileSpecificationDelegate(string specificationname, Uri baseUri);

        [Test]
        [ExpectedException(typeof(WebException))]
        public void TestGetUnknownCaseFileSpecification()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand getCaseFileSpecificationCommand = new GetLatestCaseFileSpecificationCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationCommand")).Return(getCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationXmlFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService};
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Return(caseFileSpecificationXmlFormatter);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/unknownspecification", new Uri("http://localhost:8080/storage"))).Do(
                new GetCaseFileSpecificationDelegate(
                    delegate(string specificationname, Uri baseUri)
                    {
                        throw new ArgumentOutOfRangeException("specificationname", string.Format("Unknown specification {0}", specificationname));
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/unknownspecification");
                string result = Encoding.UTF8.GetString(resultBuffer);

                Assert.IsFalse(true, "This code should not be executed");
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

        private delegate string GetCaseFileSpecificationListDelegate(IEnumerable<CaseFileSpecification> items, Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetCaseFileSpecifications()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand getCaseFileSpecificationsCommand = new GetCaseFileSpecificationsCommand(caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileSpecificationsCommand")).Return(getCaseFileSpecificationsCommand);

            IFormatter caseFileSpecificationFormatter = new CaseFileSpecificationListFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationsXmlFormatter")).Return(caseFileSpecificationFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                Name = "casefilespecification",
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            int index = 0;
            List<CaseFileSpecification> specifications = new List<CaseFileSpecification>();
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/firstspecification",
                        BaseObjectType = caseFileSpecificationType
                    },
                    Version = ++index
                },
                Name = "firstspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{primaryidentfier}/{secundaryidentifier}?thirdidentifier={thirdidentifier}"
            };
            specifications.Add(myspecification);

            myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/secondspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "secondspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{identfier}"
            };
            specifications.Add(myspecification);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetEnumerable("myobjectmodel", new Uri("http://localhost:8080/storage"))).Return(specifications);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetList(specifications, new Uri("http://localhost:8080/storage"), Encoding.Unicode)).Do(
                new GetCaseFileSpecificationListDelegate(
                    delegate(IEnumerable<CaseFileSpecification> items, Uri baseUri, Encoding encoding)
                    {
                        return _caseFileSpecificationServiceImpl.GetList(items, baseUri, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/");
                string result = Encoding.Unicode.GetString(resultBuffer);
                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/firstspecification?summary=true"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/secondspecification?summary=true"" />"));
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

        private delegate string ConvertCaseFileDelegate(CaseFile caseFile, Encoding encoding);

        [Test]
        public void TestGetLatestCaseFile()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();
            
            ICommand getCaseFileCommand = new GetLatestCaseFileCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileCommand")).Return(getCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Return(caseFileFormatter);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{/Person/FirstName}/ThisIsFixed/{/Person/MiddleName}/{/Person/LastName}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            caseFileSpecificationService.ValidateCaseFileId(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers");

            CaseFile caseFile = new CaseFile()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFile,
                            RelativeUri = "/casefiles/"
                        }
                    },
                    Version = ++index
                },
                CaseFileSpecification = myspecification,
                Text = @"<Person>
                            <FirstName>Alex</FirstName>
                            <MiddleName>MiddleName</MiddleName>
                            <LastName>Harbers</LastName>
                        </Person>"
            };
            caseFile.CaseFileSpecificationUri = caseFile.CaseFileSpecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            caseFile.SelfUri = caseFile.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            Expect.On(caseFileService).Call(caseFileService.Get(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers", new Uri("http://localhost:8080/storage"))).Return(caseFile);

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            Expect.On(caseFileService).Call(caseFileService.GetXml(null as CaseFile, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertCaseFileDelegate(
                    delegate(CaseFile c, Encoding encoding)
                    {
                        return _caseFileServiceImpl.GetXml(c, encoding);
                    }
                )
            );
            _mocks.ReplayAll();

            try
            {
                _caseFileServiceImpl = new CaseFileService(_logger, _unity, caseFileSpecificationService, representationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(result.Contains(@"<cs:CaseFile"));
                Assert.IsTrue(result.Contains(@"xmlns:cs=""http://timetraveller.net/storage/schemas/casefile.xsd"""));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"" />"));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Person>"));
                Assert.IsTrue(result.Contains(@"<FirstName>Alex</FirstName>"));
                Assert.IsTrue(result.Contains(@"<MiddleName>MiddleName</MiddleName>"));
                Assert.IsTrue(result.Contains(@"<LastName>Harbers</LastName>"));
                Assert.IsTrue(result.Contains(@"</Person>"));
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

        [Test]
        public void TestGetCaseFileByVersion()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getCaseFileCommand = new GetCaseFileByVersionCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileVersionCommand")).Return(getCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Return(caseFileFormatter);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{/Person/FirstName}/ThisIsFixed/{/Person/MiddleName}/{/Person/LastName}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            caseFileSpecificationService.ValidateCaseFileId(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers");

            CaseFile caseFile = new CaseFile()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFile,
                            RelativeUri = "/casefiles/"
                        }
                    },
                    Version = ++index
                },
                CaseFileSpecification = myspecification,
                Text = @"<Person>
                            <FirstName>Alex</FirstName>
                            <MiddleName>MiddleName</MiddleName>
                            <LastName>Harbers</LastName>
                        </Person>"
            };
            caseFile.CaseFileSpecificationUri = caseFile.CaseFileSpecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            caseFile.SelfUri = caseFile.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            Expect.On(caseFileService).Call(caseFileService.Get(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers", 2, new Uri("http://localhost:8080/storage"))).Return(caseFile);

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            Expect.On(caseFileService).Call(caseFileService.GetXml(null as CaseFile, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertCaseFileDelegate(
                    delegate(CaseFile c, Encoding encoding)
                    {
                        return _caseFileServiceImpl.GetXml(c, encoding);
                    }
                )
            );
            _mocks.ReplayAll();

            try
            {
                _caseFileServiceImpl = new CaseFileService(_logger, _unity, caseFileSpecificationService, representationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers?version=2");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(result.Contains(@"<cs:CaseFile"));
                Assert.IsTrue(result.Contains(@"xmlns:cs=""http://timetraveller.net/storage/schemas/casefile.xsd"""));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"" />"));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Person>"));
                Assert.IsTrue(result.Contains(@"<FirstName>Alex</FirstName>"));
                Assert.IsTrue(result.Contains(@"<MiddleName>MiddleName</MiddleName>"));
                Assert.IsTrue(result.Contains(@"<LastName>Harbers</LastName>"));
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

        [Test]
        public void TestGetCaseFileByTimePoint()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getCaseFileCommand = new GetCaseFileByTimePointCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileTimePointCommand")).Return(getCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Return(caseFileFormatter);
            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{/Person/FirstName}/ThisIsFixed/{/Person/MiddleName}/{/Person/LastName}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            caseFileSpecificationService.ValidateCaseFileId(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers");

            CaseFile caseFile = new CaseFile()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFile,
                            RelativeUri = "/casefiles/"
                        }
                    },
                    Version = ++index
                },
                CaseFileSpecification = myspecification,
                Text = @"<Person>
                            <FirstName>Alex</FirstName>
                            <MiddleName>MiddleName</MiddleName>
                            <LastName>Harbers</LastName>
                        </Person>"
            };
            caseFile.CaseFileSpecificationUri = caseFile.CaseFileSpecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            caseFile.SelfUri = caseFile.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            TimePoint timePoint = new TimePoint(DateTime.ParseExact("2009-01-01T14:10:35.9800106+01:00", "O", CultureInfo.InvariantCulture));
            Expect.On(caseFileService).Call(caseFileService.Get(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers", timePoint, new Uri("http://localhost:8080/storage"))).Return(caseFile);

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            Expect.On(caseFileService).Call(caseFileService.GetXml(null as CaseFile, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertCaseFileDelegate(
                    delegate(CaseFile c, Encoding encoding)
                    {
                        return _caseFileServiceImpl.GetXml(c, encoding);
                    }
                )
            );
            _mocks.ReplayAll();

            try
            {
                _caseFileServiceImpl = new CaseFileService(_logger, _unity, caseFileSpecificationService, representationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers?timepoint=2009-01-01T14:10:35.9800106%2B01:00");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(result.Contains(@"<cs:CaseFile"));
                Assert.IsTrue(result.Contains(@"xmlns:cs=""http://timetraveller.net/storage/schemas/casefile.xsd"""));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"" />"));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Person>"));
                Assert.IsTrue(result.Contains(@"<FirstName>Alex</FirstName>"));
                Assert.IsTrue(result.Contains(@"<MiddleName>MiddleName</MiddleName>"));
                Assert.IsTrue(result.Contains(@"<LastName>Harbers</LastName>"));
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

        [Test]
        [ExpectedException(typeof(WebException))]
        public void TestGetCaseFileWithUnknownCaseFileSpecification()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getCaseFileCommand = new GetLatestCaseFileCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileCommand")).Return(getCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Return(caseFileFormatter);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(null);

            _mocks.ReplayAll();

            try
            {
                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                Stream httpStream = webClient.OpenRead("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/mycasefileidentifier/ThisIsFixed/mycasefilesubidentifier?thirdidentifier=mythirdidentifier");

                Assert.IsFalse(true, "This code should not be executed");
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

        private delegate void ValidateCaseFileIdDelegate(CaseFileSpecification specification, string caseFileId);

        [Test]
        [ExpectedException(typeof(WebException))]
        public void TestGetUnmatchedCaseFile()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getCaseFileCommand = new GetLatestCaseFileCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileCommand")).Return(getCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Return(caseFileFormatter);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{firstname}/ThisIsFixed/{middlename}/thirdidentifier={lastname}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            caseFileSpecificationService.ValidateCaseFileId(myspecification, "myobjectmodel/myspecification/mycasefileidentifier/ThisIsUnmatched/mycasefilesubidentifier/thirdidentifier=mycasefilethirdidentifier");
            LastCall.Do(
                new ValidateCaseFileIdDelegate(
                    delegate(CaseFileSpecification spec, string caseFileId)
                    {
                        throw new ArgumentException(string.Format("Casefileid {0} does not match template {1}", caseFileId, spec.UriTemplate));
                    }
                )
            );
            _mocks.ReplayAll();

            try
            {
                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                Stream httpStream = webClient.OpenRead("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/mycasefileidentifier/ThisIsUnmatched/mycasefilesubidentifier/thirdidentifier=mycasefilethirdidentifier");

                Assert.IsFalse(true, "This code should not be executed");
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

        private delegate string GetCaseFileListDelegate(IEnumerable<CaseFile> items, Uri baseUri, Encoding encoding);
        
        [Test]
        public void TestGetCaseFiles()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getCaseFilesCommand = new GetCaseFilesCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFilesCommand")).Return(getCaseFilesCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFilesXmlFormatter")).Return(caseFileFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                Name = "casefilespecification",
                RelativeUri = "/specifications/casefiles/"
            };

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                Name = "casefile",
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = caseFileSpecificationType
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                UriTemplate = "{primaryidentfier}/{secundaryidentifier}?thirdidentifier={thirdidentifier}"
            };
            myspecification.SelfUri = myspecification.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get(myspecification.ExtId, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            List<CaseFile> casefiles = new List<CaseFile>()
            {
                new CaseFile()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            Id = Guid.NewGuid(),
                            ExtId = "myobjectmodel/myspecification/Alex.Harbers",
                            ReferenceId = myspecification.BaseObjectValue.Id,
                            Type = caseFileType
                        },
                        Version = ++index
                    },
                    CaseFileSpecification = myspecification,
                },
                new CaseFile()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            Id = Guid.NewGuid(),
                            ExtId = "myobjectmodel/myspecification/Floris.Zwarteveen",
                            ReferenceId = myspecification.BaseObjectValue.Id,
                            Type = caseFileType
                        },
                        Version = ++index
                    },
                    CaseFileSpecification = myspecification,
                }
            };
            foreach (CaseFile casefile in casefiles)
            {
                casefile.SelfUri = casefile.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);
            }
            Expect.On(caseFileService).Call(caseFileService.GetEnumerable(myspecification, new Uri("http://localhost:8080/storage"))).Return(casefiles);

            Expect.On(caseFileService).Call(caseFileService.GetList(casefiles, new Uri("http://localhost:8080/storage"), Encoding.Unicode)).Do(
                new GetCaseFileListDelegate(
                    delegate(IEnumerable<CaseFile> items, Uri baseUri, Encoding encoding)
                    {
                        return _caseFileServiceImpl.GetList(items, baseUri, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileServiceImpl = new CaseFileService(_logger, _unity, caseFileSpecificationService, representationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/");
                string result = Encoding.Unicode.GetString(resultBuffer);
                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""casefile"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex.Harbers?summary=true"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""casefile"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Floris.Zwarteveen?summary=true"" />"));
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

        private delegate CaseFileSpecification ConvertCaseFileSpecificationXmlDelegate(string xml, Encoding encoding);
        private delegate bool StoreCaseFileSpecificationDelegate(string specificationId, CaseFileSpecification specification, Uri baseUri, WebHttpHeaderInfo journalInfo);

        [Test]
        public void TestStoreCaseFileSpecification()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand storeCaseFileSpecificationCommand = new StoreCaseFileSpecificationCommand(caseFileSpecificationService, objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTCaseFileSpecificationCommand")).Return(storeCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Return(caseFileSpecificationFormatter);

            int index = 0;
            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.ObjectModel,
                            RelativeUri = "/specifications/objectmodels/"
                        }
                    },
                    Version = ++index
                },
                SelfUri = string.Empty
            };
            Expect.On(objectModelService).Call(objectModelService.Get("myobjectmodel", new Uri("http://localhost:8080/storage"))).Return(objectmodel);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            caseFileSpecificationService.Store(string.Empty, null, new Uri("http://localhost:8080/storage"), null);
            LastCall.On(caseFileSpecificationService).IgnoreArguments().Do(
                new StoreCaseFileSpecificationDelegate(
                    delegate(string specificationId, CaseFileSpecification specification, Uri baseUri, WebHttpHeaderInfo journalInfo)
                    {
                        specification.BaseObjectValue = new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ParentBaseObject = new BaseObject()
                            {
                                ExtId = specificationId,
                                BaseObjectType = caseFileSpecificationType
                            },
                            Version = ++index
                        };
                        specification.ObjectModelUri = specification.ObjectModel.GetUri(baseUri, UriType.Version);
                        specification.SelfUri = specification.GetUri(baseUri, UriType.Version);

                        return true;
                    }
                )
            );

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertCaseFileSpecificationXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        CaseFileSpecification specification = _caseFileSpecificationServiceImpl.Convert(xml, encoding);
                        return specification;
                    }
                )
            );

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXml(null as CaseFileSpecification, Encoding.Unicode)).IgnoreArguments().Do(
                new GetCaseFileSpecificationXmlDelegate(
                    delegate(CaseFileSpecification specification, Encoding encoding)
                    {
                        return _caseFileSpecificationServiceImpl.GetXml(specification, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                string specificationXml = @"<CaseFileSpecification xmlns=""http://timetraveller.net/storage/schemas/casefilespecification.xsd"">
                                                    <Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel""/>
                                                    <Link rel=""self"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                                    <Name>myspecification</Name>
                                                    <UriTemplate>{primaryidentfier}/{secundaryidentifier}?thirdidentifier={thirdidentifier}</UriTemplate>
                                                    <Structure>
                                                        <Entity Name=""Person"" Type=""Person""/>
                                                    </Structure>
                                                </CaseFileSpecification>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(specificationXml);
                
                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
                webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.nl");

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=2"" />"));
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


        [Test]
        [ExpectedException(typeof(WebException))]
        public void TestStoreInvalidCaseFileSpecificationWithMissingName()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            ICommand storeCaseFileSpecificationCommand = new StoreCaseFileSpecificationCommand(caseFileSpecificationService, objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTCaseFileSpecificationCommand")).Return(storeCaseFileSpecificationCommand);

            IFormatter caseFileSpecificationFormatter = new CaseFileSpecificationXmlFormatter(new TextFormatter()) { SpecificationService = caseFileSpecificationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileSpecificationXmlFormatter")).Return(caseFileSpecificationFormatter);

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertCaseFileSpecificationXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        return _caseFileSpecificationServiceImpl.Convert(xml, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(_logger, _unity, objectModelService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                string specificationXml = @"<CaseFileSpecification xmlns=""http://timetraveller.net/storage/schemas/specification.xsd"">
                                                    <Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel""/>
                                                    <Link rel=""self"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                                    <UriTemplate>{primaryidentfier}/{secundaryidentifier}?thirdidentifier={thirdidentifier}</UriTemplate>
                                                </CaseFileSpecification>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(specificationXml);

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                Assert.IsFalse(true, "This code should not be executed");
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

        private delegate string GetObjectModelXmlDelegate(ObjectModel objectModel, Encoding encoding);

        [Test]
        public void TestGetLatestObjectModel()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand getObjectModelCommand = new GetLatestObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelCommand")).Return(getObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            int index = 0;
            ObjectModel myobjectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel",
                        BaseObjectType = objectModelType
                    },
                    Version = ++index
                },
                Name = "myobjectmodel",
            };
            myobjectmodel.SelfUri = myobjectmodel.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(objectModelService).Call(objectModelService.Get("myobjectmodel", new Uri("http://localhost:8080/storage"))).Return(myobjectmodel);

            Expect.On(objectModelService).Call(objectModelService.GetXml(null as ObjectModel, Encoding.Unicode)).IgnoreArguments().Do(
                new GetObjectModelXmlDelegate(
                    delegate(ObjectModel objectModel, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetXml(objectModel, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);
                _repositoryServiceImpl = new RepositoryService(_logger, _unity);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myobjectmodel</Name>"));
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

        private delegate string GetObjectModelXmlSchemaDelegate(ObjectModel objectmodel, Encoding encoding);

        [Test]
        public void TestGetLatestObjectModelXmlSchema()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand getObjectModelCommand = new GetLatestObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelCommand")).Return(getObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlSchemaFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModel.xsdFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            int index = 0;
            ObjectModel myobjectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel",
                        BaseObjectType = objectModelType
                    },
                    Version = ++index
                },
                Name = "myobjectmodel",
                ObjectDefinitions = new ObjectDefinition[] {
                    new ObjectDefinition() {
                        Name = "Person",
                        ObjectType = ObjectType.entity,
                        Properties = new ObjectDefinitionProperty[] {
                            new ObjectDefinitionProperty() {
                                Name = "FirstName",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            },
                            new ObjectDefinitionProperty() {
                                Name = "LastName",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            }
                        }
                    }
                },
                SelfUri = string.Empty
            };

            Expect.On(objectModelService).Call(objectModelService.Get("myobjectmodel", new Uri("http://localhost:8080/storage"))).Return(myobjectmodel);

            Expect.On(objectModelService).Call(objectModelService.GetXmlSchema(myobjectmodel, Encoding.UTF8)).Do(
                new GetObjectModelXmlSchemaDelegate(
                    delegate(ObjectModel objectModel, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetXmlSchema(objectModel, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel.xsd");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<xs:schema"));
                Assert.IsTrue(result.Contains(@"id=""myobjectmodel"""));
                Assert.IsTrue(result.Contains(@"<xs:complexType name=""Person"""));
                Assert.IsTrue(result.EndsWith(@"</xs:schema>"));
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


        [Test]
        public void TestGetObjectModelByTimePoint()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand getObjectModelCommand = new GetObjectModelByTimePointCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelTimePointCommand")).Return(getObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            int index = 0;
            ObjectModel myobjectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel",
                        BaseObjectType = objectModelType
                    },
                    Version = ++index
                },
                Name = "myobjectmodel",
            };
            myobjectmodel.SelfUri = myobjectmodel.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            TimePoint timePoint = new TimePoint(DateTime.ParseExact("2009-01-01T00:00:00.0000000+01:00", "O", CultureInfo.InvariantCulture));
            Expect.On(objectModelService).Call(objectModelService.Get("myobjectmodel", timePoint, new Uri("http://localhost:8080/storage"))).Return(myobjectmodel);

            Expect.On(objectModelService).Call(objectModelService.GetXml(null as ObjectModel, Encoding.Unicode)).IgnoreArguments().Do(
                new GetObjectModelXmlDelegate(
                    delegate(ObjectModel objectModel, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetXml(objectModel, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?timepoint=2009-01-01");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myobjectmodel</Name>"));

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

        [Test]
        public void TestGetObjectModelByVersion()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand getObjectModelCommand = new GetObjectModelByVersionCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelVersionCommand")).Return(getObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            int index = 0;
            ObjectModel myobjectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.ObjectModel,
                            RelativeUri = "/specifications/objectmodels/"
                        }
                    },
                    Version = ++index
                },
                Name = "myobjectmodel",
            };
            myobjectmodel.SelfUri = myobjectmodel.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(objectModelService).Call(objectModelService.Get("myobjectmodel", 1, new Uri("http://localhost:8080/storage"))).Return(myobjectmodel);

            Expect.On(objectModelService).Call(objectModelService.GetXml(null as ObjectModel, Encoding.Unicode)).IgnoreArguments().Do(
                new GetObjectModelXmlDelegate(
                    delegate(ObjectModel objectModel, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetXml(objectModel, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myobjectmodel</Name>"));

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

        private delegate string GetObjectModelHistoryDelegate(ObjectModel objectmodel, Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetObjectModelHistory()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand getObjectModelCommand = new GetLatestObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelCommand")).Return(getObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelHistoryFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelHistoryXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                Name = "objectmodel",
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            Guid baseObjectvalueGuid = Guid.NewGuid();
            BaseObject objectModelBaseObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                Type = objectModelType,
                BaseObjectValues = new EntityCollection<BaseObjectValue>()
                {
                    new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        Version = 1
                    },
                    new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        Version = 2
                    },
                    new BaseObjectValue()
                    {
                        Id = baseObjectvalueGuid,
                        Version = 3
                    },
                }
            };

            BaseObjectValue[] objectModelBaseObjectValues = objectModelBaseObject.BaseObjectValues.ToArray();

            BaseObjectValue objectModelBaseObjectValue = new BaseObjectValue()
            {
                Id = baseObjectvalueGuid,
                ParentBaseObject = objectModelBaseObject,
                Version = objectModelBaseObjectValues[2].Version
            };

            ObjectModel myobjectmodel = new ObjectModel()
            {
                Name = "myobjectmodel",
                BaseObjectValue = objectModelBaseObjectValue
            };

            Expect.On(objectModelService).Call(objectModelService.Get("myobjectmodel", new Uri("http://localhost:8080/storage"))).Return(myobjectmodel);

            int index = 0;
            List<IJournalEntry> journalEntries = new List<IJournalEntry>()
            {
                new BaseObjectJournal()
                {
                    Id = ++index,
                    Timestamp = DateTime.Parse("2009-06-25T09:21:28.000"),
                    Username = "user1@gmail.nl",
                    After = objectModelBaseObjectValues[0]
                },
                new BaseObjectJournal()
                {
                    Id = ++index,
                    Timestamp = DateTime.Parse("2009-06-25T09:21:42.000"),
                    Username = "user2@gmail.nl",
                    Before = objectModelBaseObjectValues[0],
                    After = objectModelBaseObjectValues[1]
                },
                new BaseObjectJournal()
                {
                    Id = ++index,
                    Timestamp = DateTime.Parse("2009-06-25T09:22:00.000"),
                    Username = "user3@gmail.nl",
                    Before = objectModelBaseObjectValues[1],
                    After = objectModelBaseObjectValues[2]
                }
            };

            Expect.On(dataService).Call(dataService.GetJournal(objectModelBaseObjectValue.Parent, new TimePointRange(TimePoint.Past, TimePoint.Future))).Return(journalEntries);

            Expect.On(objectModelService).Call(objectModelService.GetHistory(null as ObjectModel, null, Encoding.Unicode)).IgnoreArguments().Do(
                new GetObjectModelHistoryDelegate(
                    delegate(ObjectModel objectModel, Uri baseUri, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetHistory(objectModel, baseUri, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?history=true");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<History>"));
                Assert.IsTrue(result.Contains(@"<Name>myobjectmodel</Name>"));
                Assert.IsTrue(result.Contains(@"<Journal>"));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
                Assert.IsTrue(result.Contains(@"<Timestamp>06-25-2009 09:21 28</Timestamp>"));
                Assert.IsTrue(result.Contains(@"<Username>user1@gmail.nl</Username>"));
                Assert.IsTrue(result.Contains(@"</Journal>"));
                Assert.IsTrue(result.Contains(@"<Journal>"));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
                Assert.IsTrue(result.Contains(@"<Timestamp>06-25-2009 09:21 42</Timestamp>"));
                Assert.IsTrue(result.Contains(@"<Username>user2@gmail.nl</Username>"));
                Assert.IsTrue(result.Contains(@"</Journal>"));
                Assert.IsTrue(result.Contains(@"<Journal>"));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=3"" />"));
                Assert.IsTrue(result.Contains(@"<Timestamp>06-25-2009 09:22 00</Timestamp>"));
                Assert.IsTrue(result.Contains(@"<Username>user3@gmail.nl</Username>"));
                Assert.IsTrue(result.Contains(@"</Journal>"));
                Assert.IsTrue(result.Contains(@"</History>"));
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

        private delegate string GetObjectModelListDelegate(IEnumerable<ObjectModel> items, Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetObjectModels()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand getObjectModelsCommand = new GetObjectModelsCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETObjectModelsCommand")).Return(getObjectModelsCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelsXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                Name = "objectmodel",
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            int index = 0;
            List<ObjectModel> objectmodels = new List<ObjectModel>();
            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "firstobjectmodel",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.ObjectModel,
                            RelativeUri = "/specifications/objectmodels/"
                        }
                    },
                    Version = ++index
                },
                Name = "firstobjectmodel",
                SelfUri = string.Empty
            };
            objectmodels.Add(objectmodel);

            objectmodel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "secondobjectmodel",
                        BaseObjectType = objectModelType
                    },
                    Version = ++index
                },
                Name = "secondobjectmodel",
                SelfUri = string.Empty
            };
            objectmodels.Add(objectmodel);

            Expect.On(objectModelService).Call(objectModelService.GetEnumerable(new Uri("http://localhost:8080/storage"))).Return(objectmodels);

            Expect.On(objectModelService).Call(objectModelService.GetList(objectmodels, new Uri("http://localhost:8080/storage"), Encoding.UTF8)).Do(
                new GetObjectModelListDelegate(
                    delegate(IEnumerable<ObjectModel> items, Uri baseUri, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetList(items, baseUri, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/specifications/objectmodels/");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/firstobjectmodel?summary=true"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/secondobjectmodel?summary=true"" />"));
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

        private delegate ObjectModel ConvertObjectModelXmlDelegate(string xml, Encoding encoding);
        private delegate bool StoreObjectModelDelegate(string objectModelId, ObjectModel objectModel, Uri baseUri, WebHttpHeaderInfo journalInfo);

        [Test]
        public void TestStoreObjectModel()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand storeObjectModelCommand = new StoreObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTObjectModelCommand")).Return(storeObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Return(objectModelFormatter);

            int index = 0;
            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            objectModelService.Store(string.Empty, null, new Uri("http://localhost:8080/storage"), null);
            LastCall.On(objectModelService).IgnoreArguments().Do(
                new StoreObjectModelDelegate(
                    delegate(string objectModelId, ObjectModel objectModel, Uri baseUri, WebHttpHeaderInfo journalInfo)
                    {
                        objectModel.BaseObjectValue = new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ParentBaseObject = new BaseObject()
                            {
                                ExtId = objectModelId,
                                BaseObjectType = objectModelType
                            },
                            Version = ++index
                        };
                        objectModel.SelfUri = objectModel.GetUri(baseUri, UriType.Version);

                        return true;
                    }
                )
            );

            Expect.On(objectModelService).Call(objectModelService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertObjectModelXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        ObjectModel objectmodel = _objectModelServiceImpl.Convert(xml, encoding);
                        return objectmodel;
                    }
                )
            );

            Expect.On(objectModelService).Call(objectModelService.GetXml(null as ObjectModel, Encoding.Unicode)).IgnoreArguments().Do(
                new GetObjectModelXmlDelegate(
                    delegate(ObjectModel objectmodel, Encoding encoding)
                    {
                        return _objectModelServiceImpl.GetXml(objectmodel, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                string objectmodelXml = @"<ObjectModel xmlns=""http://timetraveller.net/storage/schemas/objectmodel.xsd"">
                                                    <Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel""/>
                                                    <Name>myobjectmodel</Name>
                                                    <ObjectDefinitions>
                                                     <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                                       <Properties>
                                                         <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                                         <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                                       </Properties>
                                                     </ObjectDefinition>
                                                   </ObjectDefinitions>
                                                </ObjectModel>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(objectmodelXml);

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
                webClient.Headers.Add(HttpRequestHeader.From, "user1@gmail.com");

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
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

        [Test]
        [ExpectedException(typeof(WebException))]
        public void TestStoreObjectModelWithMissingFrominHttpHeader()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand storeObjectModelCommand = new StoreObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTObjectModelCommand")).Return(storeObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);


            Expect.On(objectModelService).Call(objectModelService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertObjectModelXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        ObjectModel objectmodel = _objectModelServiceImpl.Convert(xml, encoding);
                        return objectmodel;
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                string objectmodelXml = @"<ObjectModel xmlns=""http://timetraveller.net/storage/schemas/objectmodel.xsd"">
                                                    <Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel""/>
                                                    <Name>myobjectmodel</Name>
                                                    <ObjectDefinitions>
                                                     <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                                       <Properties>
                                                         <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                                         <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                                       </Properties>
                                                     </ObjectDefinition>
                                                   </ObjectDefinitions>
                                                </ObjectModel>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(objectmodelXml);

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
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

        [Test]
        [ExpectedException(typeof(WebException))]
        public void TestStoreInvalidObjectModel()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();

            ICommand storeObjectModelCommand = new StoreObjectModelCommand(objectModelService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTObjectModelCommand")).Return(storeObjectModelCommand);

            IFormatter objectModelFormatter = new ObjectModelXmlFormatter(new TextFormatter()) { ObjectModelService = objectModelService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("ObjectModelXmlFormatter")).Return(objectModelFormatter);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            Expect.On(objectModelService).Call(objectModelService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertObjectModelXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        ObjectModel objectmodel = _objectModelServiceImpl.Convert(xml, encoding);
                        return objectmodel;
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _objectModelServiceImpl = new ObjectModelService(_logger, _unity, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                string objectmodelXml = @"<bla/>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(objectmodelXml);

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/specifications/objectmodels/myobjectmodel", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
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

        private delegate CaseFile Convert2CaseFileDelegate(string xml, Encoding encoding);
        private delegate bool StoreCaseFileDelegate(CaseFileSpecification specification, string caseFileId, CaseFile caseFile, Uri baseUri, WebHttpHeaderInfo journalInfo);

        [Test]
        public void TestStoreCaseFile()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();

            ICommand storeCaseFileCommand = new StoreCaseFileCommand(caseFileSpecificationService, caseFileService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTCaseFileCommand")).Return(storeCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileXmlFormatter")).Return(caseFileFormatter);

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{/Person/FirstName}.MiddleName.{/Person/LastName}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            caseFileSpecificationService.ValidateCaseFileId(myspecification, "myobjectmodel/myspecification/Alex.MiddleName.Harbers");

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            caseFileService.Store(myspecification, "myobjectmodel/myspecification/Alex.MiddleName.Harbers", null, new Uri("http://localhost:8080/storage"), null);
            LastCall.IgnoreArguments().Do(
                new StoreCaseFileDelegate(
                    delegate(CaseFileSpecification specification, string caseFileId, CaseFile caseFile, Uri baseUri, WebHttpHeaderInfo journalInfo)
                    {
                        caseFile.BaseObjectValue = new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ParentBaseObject = new BaseObject()
                            {
                                ExtId = caseFileId,
                                BaseObjectType = caseFileType
                            },
                            Version = ++index
                        };
                        caseFile.CaseFileSpecificationUri = caseFile.CaseFileSpecification.GetUri(baseUri, UriType.Version);
                        caseFile.SelfUri = caseFile.GetUri(baseUri, UriType.Version);

                        return true;
                    }
                )
            );

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            Expect.On(caseFileService).Call(caseFileService.GetXml(null as CaseFile, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertCaseFileDelegate(
                    delegate(CaseFile c, Encoding encoding)
                    {
                        return _caseFileServiceImpl.GetXml(c, encoding);
                    }
                )
            );

            Expect.On(caseFileService).Call(caseFileService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new Convert2CaseFileDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        CaseFile caseFile = _caseFileServiceImpl.Convert(xml, encoding);
                        return caseFile;
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _caseFileServiceImpl = new CaseFileService(_logger, _unity, caseFileSpecificationService, representationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://timetraveller.net/storage/schemas/casefile.xsd"">
                                                <cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers""/>
                                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                                <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                </Person>
                                           </cs:CaseFile>", Guid.NewGuid(), DateTime.Now.ToString("O"));
                byte[] requestBuffer = Encoding.UTF8.GetBytes(caseFileXml);

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
                webClient.Headers.Add(HttpRequestHeader.From, "user1@gmail.com");

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result.Replace("\n", "\r\n"));

                Assert.IsTrue(result.Contains(@"<cs:CaseFile"));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification?version=1"""));
                Assert.IsTrue(result.Contains(@"<cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers?version=2"""));
                Assert.IsTrue(result.Contains(@"<Person"));
                Assert.IsTrue(result.Contains(@"<FirstName>Alex</FirstName>"));
                Assert.IsTrue(result.Contains(@"<LastName>Harbers</LastName>"));
                Assert.IsTrue(result.Contains(@"</cs:CaseFile>"));
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

        private delegate string ConvertRepresentationDelegate(Representation representation, Encoding encoding);

        [Test]
        public void TestLatestGetRepresentation()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getRuleCommand = new GetLatestRepresentationCommand(representationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRepresentationCommand")).Return(getRuleCommand);

            IFormatter representationFormatter = new RepresentationXmlFormatter(new TextFormatter()) { RepresentationService = representationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepresentationXmlFormatter")).Return(representationFormatter);

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation,
                RelativeUri = "/representations/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            int index = 0;
            Representation myrepresentation = new Representation()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/myrepresentation",
                        BaseObjectType = representationType
                    },
                    Version = ++index
                },
                CaseFileSpecification = new CaseFileSpecification()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel/myspecification",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.CaseFileSpecification,
                                RelativeUri = "/specifications/casefiles/"
                            }
                        },
                        Version = ++index
                    }
                },
                Name = "myrepresentation",
                Script = new RepresentationScript()
                {
                    Type = "xslt",
                    Text = @"<?xml version=""1.0"" encoding=""utf-8""?> 
                            <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" 
                                            xmlns:vs=""http://microsoft.com/schemas/VisualStudio/TeamTest/2006""> 

                              <xsl:template match=""/""> 
                                <html> 
                                  <body> 
                                    This is myrepresentation.
                                  </body> 
                                </html> 
                              </xsl:template> 
                            </xsl:stylesheet>"
                }
            };
            myrepresentation.SelfUri = myrepresentation.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(representationService).Call(representationService.Get(myrepresentation.ExtId, new Uri("http://localhost:8080/storage"))).Return(myrepresentation);

            Expect.On(representationService).Call(representationService.GetXml(null as Representation, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertRepresentationDelegate(
                    delegate(Representation representation, Encoding encoding)
                    {
                        return _representationServiceImpl.GetXml(representation, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _representationServiceImpl = new RepresentationService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/representations/myobjectmodel/myspecification/myrepresentation");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/representations/myobjectmodel/myspecification/myrepresentation?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myrepresentation</Name>"));
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

        private delegate string GetRepresentationListDelegate(IEnumerable<Representation> items, Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetRepresentations()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            ICommand getRepresentationsCommand = new GetRepresentationsCommand(representationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRepresentationsCommand")).Return(getRepresentationsCommand);

            IFormatter representationFormatter = new RepresentationListFormatter(new TextFormatter()) { RepresentationService = representationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepresentationsXmlFormatter")).Return(representationFormatter);

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation,
                Name = "representation",
                RelativeUri = "/representations/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            int index = 0;
            List<Representation> representations = new List<Representation>();
            Representation myrepresentation = new Representation()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/firstrepresentation",
                        BaseObjectType = representationType
                    },
                    Version = ++index
                },
                Name = "firstrepresentation",
                SelfUri = string.Empty
            };
            representations.Add(myrepresentation);

            myrepresentation = new Representation()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/secondrepresentation",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.Representation,
                            RelativeUri = "/representations/"
                        }
                    },
                    Version = ++index
                },
                Name = "secondrepresentation",
                SelfUri = string.Empty
            };
            representations.Add(myrepresentation);

            Expect.On(representationService).Call(representationService.GetEnumerable("myobjectmodel", "myspecification", new Uri("http://localhost:8080/storage"))).Return(representations);

            Expect.On(representationService).Call(representationService.GetList(representations, new Uri("http://localhost:8080/storage"), Encoding.UTF8)).Do(
                new GetRepresentationListDelegate(
                    delegate(IEnumerable<Representation> items, Uri baseUri, Encoding encoding)
                    {
                        return _representationServiceImpl.GetList(items, baseUri, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _representationServiceImpl = new RepresentationService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/representations/myobjectmodel/myspecification/");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""representation"" href=""http://localhost:8080/storage/representations/myobjectmodel/myspecification/firstrepresentation?summary=true"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""representation"" href=""http://localhost:8080/storage/representations/myobjectmodel/myspecification/secondrepresentation?summary=true"" />"));
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

        private delegate Representation GetRepresentationXmlDelegate(string xml, Encoding encoding);
        private delegate bool StoreRepresentationDelegate(string representationId, Representation representation, Uri baseUri, WebHttpHeaderInfo journalInfo);

        [Test]
        public void TestStoreRepresentation()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            ICommand storeRepresentationCommand = new StoreRepresentationCommand(representationService, caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTRepresentationCommand")).Return(storeRepresentationCommand);

            IFormatter representationFormatter = new RepresentationXmlFormatter(new TextFormatter()) { RepresentationService = representationService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepresentationXmlFormatter")).Return(representationFormatter);

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation,
                RelativeUri = "/representations/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            int index = 0;
            representationService.Store(string.Empty, null, new Uri("http://localhost:8080/storage"), null);
            LastCall.On(representationService).IgnoreArguments().Do(
                new StoreRepresentationDelegate(
                    delegate(string representationId, Representation representation, Uri baseUri, WebHttpHeaderInfo journalInfo)
                    {
                        representation.BaseObjectValue = new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ParentBaseObject = new BaseObject()
                            {
                                ExtId = representationId,
                                BaseObjectType = representationType
                            },
                            Version = ++index
                        };
                        representation.SelfUri = representation.GetUri(baseUri, UriType.Version);

                        return true;
                    }
                )
            );

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        Id = Guid.NewGuid(),
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles"
                        }
                    }
                }
            };
            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get(myspecification.Name, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            Expect.On(representationService).Call(representationService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new GetRepresentationXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        Representation representation = _representationServiceImpl.Convert(xml, encoding);
                        return representation;
                    }
                )
            );

            Expect.On(representationService).Call(representationService.GetXml(null as Representation, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertRepresentationDelegate(
                    delegate(Representation representation, Encoding encoding)
                    {
                        return _representationServiceImpl.GetXml(representation, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _representationServiceImpl = new RepresentationService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
                webClient.Headers.Add(HttpRequestHeader.From, "user1@gmail.com");

                string representationXml = @"<Representation xmlns=""http://timetraveller.net/storage/schemas/representation.xsd"">
                                                    <Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                                    <Link rel=""self"" href=""http://localhost:8080/storage/representations/myobjectmodel/myspecification/myrepresentation""/>
                                                    <Name>myrepresentation</Name>
                                                    <Script Type=""xslt"" ContentType=""html"">
                                                        <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
                                                          <xsl:template match=""/"">
                                                            <html>
                                                              <body>This is myrepresentation.</body>
                                                            </html>
                                                          </xsl:template>
                                                        </xsl:stylesheet>
                                                    </Script>
                                                </Representation>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(representationXml);

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/representations/myobjectmodel/myspecification/myrepresentation", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/representations/myobjectmodel/myspecification/myrepresentation?version=1"" />"));
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

        private delegate string TranformDelegate(string caseFileXml, Representation representation);

        [Test]
        public void TestGetLatestCaseFileUsingRepresentation()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getCaseFileCommand = new GetLatestCaseFileCommand(caseFileSpecificationService, caseFileService, ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETCaseFileCommand")).Return(getCaseFileCommand);

            IFormatter caseFileFormatter = new CaseFileByRepresentationFormatter(caseFileService, representationService);
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("CaseFileRepresentationXmlFormatter")).Return(caseFileFormatter);

            IRepresentationTransformer xsltTransformer = _mocks.StrictMock<IRepresentationTransformer>();
            Expect.On(_unity).Call(_unity.Resolve<IRepresentationTransformer>("xslt")).Return(xsltTransformer);

            Expect.On(xsltTransformer).Call(xsltTransformer.Transform(string.Empty, string.Empty)).IgnoreArguments().Return(@"<html><body>Alex.Harbers</body></html>");

            int index = 0;
            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel"
                },
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                UriTemplate = "{/Person/FirstName}/ThisIsFixed/{/Person/MiddleName}/{/Person/LastName}"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            caseFileSpecificationService.ValidateCaseFileId(myspecification, "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers");

            Representation myrepresentation = new Representation()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/myrepresentation",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.Representation,
                            RelativeUri = "/representations/"
                        }
                    },
                    Version = ++index
                },
                Name = "myrepresentation",
                SelfUri = string.Empty,
                Script = new RepresentationScript()
                {
                    Type = "xslt",
                    Text = @"<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""> 
                              <xsl:template match=""/""> 
                                <html> 
                                  <body> 
                                    <xsl:value-of select=""Person/FirstName""/>
                                    <xsl:text>.</xsl:text>
                                    <xsl:value-of select=""Person/LastName""/>
                                  </body> 
                                </html> 
                              </xsl:template> 
                            </xsl:stylesheet>"
                }
            };

            Expect.On(representationService).Call(representationService.Get(myrepresentation.ExtId, new Uri("http://localhost:8080/storage"))).Return(myrepresentation);

            CaseFile caseFile = new CaseFile()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFile,
                            RelativeUri = "/casefiles/"
                        }
                    },
                    Version = ++index
                },
                CaseFileSpecification = myspecification,
                CaseFileSpecificationUri = string.Empty,
                SelfUri = string.Empty,
                Text = @"<Person>
                            <FirstName>Alex</FirstName>
                            <MiddleName>MiddleName</MiddleName>
                            <LastName>Harbers</LastName>
                        </Person>"
            };

            Expect.On(caseFileService).Call(caseFileService.Get(myspecification, caseFile.ExtId, new Uri("http://localhost:8080/storage"))).Return(caseFile);

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation,
                RelativeUri = "/representations/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            Expect.On(representationService).Call(representationService.Transform(string.Empty, null)).IgnoreArguments().Do(
                new TranformDelegate(
                    delegate(string caseFileXml, Representation representation)
                    {
                        return _representationServiceImpl.Transform(caseFileXml, representation);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _representationServiceImpl = new RepresentationService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex/ThisIsFixed/MiddleName/Harbers?view=myrepresentation");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsTrue(result.StartsWith(@"<html"));
                Assert.IsTrue(result.Contains(@"<body>Alex.Harbers</body>"));
                Assert.IsTrue(result.EndsWith(@"</html>"));
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

        private delegate string GetRuleXmlDelegate(Rule rule, Encoding encoding);

        [Test]
        public void TestGetLatestRule()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getRuleCommand = new GetLatestRuleCommand(ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRuleCommand")).Return(getRuleCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService};
            IFormatter ruleFormatter = new RuleXmlFormatter(new TextFormatter()) { CaseFileFormatter = caseFileFormatter, RuleService = ruleService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RuleXmlFormatter")).Return(ruleFormatter);

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet,
                RelativeUri = "/rules/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            int index = 0;
            Rule myrule = new Rule()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/myrule",
                        BaseObjectType = ruleSetType
                    },
                    Version = ++index
                },
                CaseFileSpecification = new CaseFileSpecification()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel/myspecification",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.CaseFileSpecification,
                                RelativeUri = "/specifications/casefiles/"
                            }
                        },
                        Version = ++index
                    }
                },
                Name = "myrule",
                Script = new RuleScript()
                {
                    Method = "myobjectmodel.myspecification.myrule.Execute",
                    Type = "fsharp",
                    Value = @"#light
module myobjectmodel.myspecification.myrule

open System
open myobjectmodel.myspecification

let Execute(p: Person) =
    true"
                }
            };
            myrule.SelfUri = myrule.GetUri(new Uri("http://localhost:8080/storage"), UriType.Version);

            Expect.On(ruleService).Call(ruleService.Get(myrule.ExtId, new Uri("http://localhost:8080/storage"))).Return(myrule);

            Expect.On(ruleService).Call(ruleService.GetXml(null as Rule, Encoding.Unicode)).IgnoreArguments().Do(
                new GetRuleXmlDelegate(
                    delegate(Rule rule, Encoding encoding)
                    {
                        return _ruleServiceImpl.GetXml(rule, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _ruleServiceImpl = new RuleService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule?version=1"" />"));
                Assert.IsTrue(result.Contains("<Name>myrule</Name>"));
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

        private delegate string GetRuleListDelegate(IEnumerable<Rule> items, Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetRules()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand getRulesCommand = new GetRulesCommand(ruleService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRulesCommand")).Return(getRulesCommand);

            IFormatter ruleFormatter = new RuleListFormatter(new TextFormatter()) { RuleService = ruleService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RulesXmlFormatter")).Return(ruleFormatter);

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet,
                Name = "rule",
                RelativeUri = "/rules/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            int index = 0;
            List<Rule> rules = new List<Rule>();
            Rule myrule = new Rule()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/firstrule",
                        BaseObjectType = ruleSetType
                    },
                    Version = ++index
                },
                Name = "firstrule",
                SelfUri = string.Empty
            };
            rules.Add(myrule);

            myrule = new Rule()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/secondrule",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.RuleSet,
                            RelativeUri = "/rules/"
                        }
                    },
                    Version = ++index
                },
                Name = "secondrule",
                SelfUri = string.Empty
            };
            rules.Add(myrule);

            Expect.On(ruleService).Call(ruleService.GetEnumerable("myobjectmodel", "myspecification", new Uri("http://localhost:8080/storage"))).Return(rules);

            Expect.On(ruleService).Call(ruleService.GetList(rules, new Uri("http://localhost:8080/storage"), Encoding.UTF8)).Do(
                new GetRuleListDelegate(
                    delegate(IEnumerable<Rule> items, Uri baseUri, Encoding encoding)
                    {
                        return _ruleServiceImpl.GetList(items, baseUri, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _ruleServiceImpl = new RuleService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/rules/myobjectmodel/myspecification/");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""rule"" href=""http://localhost:8080/storage/rules/myobjectmodel/myspecification/firstrule?summary=true"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""rule"" href=""http://localhost:8080/storage/rules/myobjectmodel/myspecification/secondrule?summary=true"" />"));
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

        private delegate Rule ConvertRuleXmlDelegate(string xml, Encoding encoding);
        private delegate bool StoreRuleDelegate(string ruleId, Rule rule, Uri baseUri, WebHttpHeaderInfo journalInfo);

        [Test]
        public void TestStoreRule()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand storeRuleCommand = new StoreRuleCommand(ruleService, caseFileSpecificationService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("PUTRuleCommand")).Return(storeRuleCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService};
            IFormatter ruleFormatter = new RuleXmlFormatter(new TextFormatter()) { CaseFileFormatter = caseFileFormatter, RuleService = ruleService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RuleXmlFormatter")).Return(ruleFormatter);

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet,
                RelativeUri = "/rules/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            int index = 0;
            ruleService.Store(string.Empty, null, new Uri("http://localhost:8080/storage"), null);
            LastCall.On(ruleService).IgnoreArguments().Do(
                new StoreRuleDelegate(
                    delegate(string ruleId, Rule rule, Uri baseUri, WebHttpHeaderInfo journalInfo)
                    {
                        rule.BaseObjectValue = new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ParentBaseObject = new BaseObject()
                            {
                                ExtId = ruleId,
                                BaseObjectType = ruleSetType
                            },
                            Version = ++index
                        };
                        rule.SelfUri = rule.GetUri(baseUri, UriType.Version);

                        return true;
                    }
                )
            );

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        Id = Guid.NewGuid(),
                        ExtId = "myobjectmodel/myspecification",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles"
                        }
                    }
                }
            };
            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get(myspecification.Name, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            Expect.On(ruleService).Call(ruleService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Do(
                new ConvertRuleXmlDelegate(
                    delegate(string xml, Encoding encoding)
                    {
                        Rule rule = _ruleServiceImpl.Convert(xml, encoding);
                        return rule;
                    }
                )
            );

            Expect.On(ruleService).Call(ruleService.GetXml(null as Rule, Encoding.Unicode)).IgnoreArguments().Do(
                new GetRuleXmlDelegate(
                    delegate(Rule rule, Encoding encoding)
                    {
                        return _ruleServiceImpl.GetXml(rule, encoding);
                    }
                )
            );

            _mocks.ReplayAll();

            try
            {
                _ruleServiceImpl = new RuleService(_logger, _unity, caseFileSpecificationService, dataService);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");
                webClient.Headers.Add(HttpRequestHeader.From, "user1@gmail.com");

                string ruleXml = @"<Rule xmlns=""http://timetraveller.net/storage/schemas/rule.xsd"">
                                        <Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                        <Link rel=""self"" href=""http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule""/>
                                        <Name>myrule</Name>
                                        <Script Type=""fsharp"" Method=""myobjectmodel.myspecification.myrule.Execute"">#light
module myobjectmodel.myspecification.myrule

open System
open myobjectmodel.myspecification

let Execute(p: Person) =
    true
                                        </Script>
                                    </Rule>";
                byte[] requestBuffer = Encoding.UTF8.GetBytes(ruleXml);

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule", "PUT", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains(@"<Link rel=""self"" href=""http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule?version=1"" />"));
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

        [Test]
        public void TestExecuteRuleByVersion()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRuleService ruleService = _mocks.StrictMock<IRuleService>();

            ICommand executeRuleCommand = new ExecuteRuleByVersionCommand(ruleService, caseFileSpecificationService, caseFileService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("POSTRuleVersionCommand")).Return(executeRuleCommand);

            IFormatter caseFileFormatter = new CaseFileXmlFormatter(new TextFormatter()) { CaseFileService = caseFileService};
            IFormatter ruleFormatter = new RuleXmlFormatter(new TextFormatter()) { CaseFileFormatter = caseFileFormatter, RuleService = ruleService };
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RuleXmlFormatter")).Return(ruleFormatter);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myspecification"
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/storage"))).Return(myspecification);

            CaseFile inputCaseFile = new CaseFile()
            {
                Text = string.Format(@"<Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                </Person>", Guid.NewGuid(), DateTime.Now.ToString("O"))
            };
            CaseFile resultingCaseFile = new CaseFile()
            {
                Text = string.Format(@"<Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                    <FSharpResult>true</FSharpResult>
                                                </Person>", Guid.NewGuid(), DateTime.Now.ToString("O"))
            };

            Expect.On(caseFileService).Call(caseFileService.Convert(string.Empty, Encoding.Unicode)).IgnoreArguments().Return(inputCaseFile);

            Expect.On(caseFileService).Call(caseFileService.GetXml(resultingCaseFile, Encoding.Unicode)).IgnoreArguments().Return(string.Format(@"<cs:CaseFile xmlns:cs=""http://timetraveller.net/storage/schemas/casefile.xsd"">
                                                <cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers""/>
                                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                                <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                    <FSharpResult>true</FSharpResult>
                                                </Person>
                                           </cs:CaseFile>", Guid.NewGuid(), DateTime.Now.ToString("O")));

            int index = 0;
            Rule myrule = new Rule()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification/myrule",
                        BaseObjectType = new BaseObjectType()
                        {
                            Id = (int)ItemType.RuleSet,
                            RelativeUri = "/rules/"
                        }
                    },
                    Version = ++index
                },
                CaseFileSpecification = new CaseFileSpecification()
                {
                    BaseObjectValue = new BaseObjectValue()
                    {
                        Id = Guid.NewGuid(),
                        ParentBaseObject = new BaseObject()
                        {
                            ExtId = "myobjectmodel/myspecification",
                            BaseObjectType = new BaseObjectType()
                            {
                                Id = (int)ItemType.CaseFileSpecification,
                                RelativeUri = "/specifications/casefiles/"
                            }
                        },
                        Version = ++index
                    }
                }
            };

            Expect.On(ruleService).Call(ruleService.Get(myrule.ExtId, myrule.Version, new Uri("http://localhost:8080/storage"))).Return(myrule);

            Expect.On(ruleService).Call(ruleService.Execute(myrule, inputCaseFile)).Return(resultingCaseFile);

            _mocks.ReplayAll();

            try
            {
                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://timetraveller.net/storage/schemas/casefile.xsd"">
                                                <cs:Link rel=""self"" href=""http://localhost:8080/storage/casefiles/myobjectmodel/myspecification/Alex.MiddleName.Harbers""/>
                                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification""/>
                                                <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                </Person>
                                           </cs:CaseFile>", Guid.NewGuid(), DateTime.Now.ToString("O"));
                byte[] requestBuffer = Encoding.UTF8.GetBytes(caseFileXml);

                byte[] resultBuffer = webClient.UploadData("http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule?version=1", "POST", requestBuffer);
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("Result={0}", result);

                Assert.IsNotNull(result);
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

        private delegate string GetXmlSchemasDelegate(Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetXmlSchemas()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IRepositoryService repositoryService = _mocks.StrictMock<IRepositoryService>();

            ICommand getXmlSchemaCommand = new GetRepositoryXmlSchemasCommand(repositoryService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRepositoryXmlSchemasCommand")).Return(getXmlSchemaCommand);

            IFormatter repositoryFormatter = new TextFormatter();
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepositoryXmlSchemasXmlFormatter")).Return(repositoryFormatter);

            Expect.On(repositoryService).Call(repositoryService.GetXmlSchemas(null, Encoding.UTF8)).IgnoreArguments().Do(
                new GetXmlSchemasDelegate(
                    delegate(Uri baseUri, Encoding encoding)
                    {
                        return _repositoryServiceImpl.GetXmlSchemas(baseUri, encoding);
                    }
                )
            );

            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            Expect.On(_unity).Call(_unity.Resolve<IObjectModelService>()).Return(objectModelService);

            Expect.On(objectModelService).Call(objectModelService.GetXmlSchemaAddress(new Uri("http://localhost:8080/storage"))).Return("http://localhost:8080/storage/schemas/objectmodel.xsd");

            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            Expect.On(_unity).Call(_unity.Resolve<ICaseFileSpecificationService>()).Return(caseFileSpecificationService);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXmlSchemaAddress(new Uri("http://localhost:8080/storage"))).Return("http://localhost:8080/storage/schemas/casefilespecification.xsd");

            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            Expect.On(_unity).Call(_unity.Resolve<ICaseFileService>()).Return(caseFileService);

            Expect.On(caseFileService).Call(caseFileService.GetXmlSchemaAddress(new Uri("http://localhost:8080/storage"))).Return("http://localhost:8080/storage/schemas/casefile.xsd");

            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            Expect.On(_unity).Call(_unity.Resolve<IRepresentationService>()).Return(representationService);

            Expect.On(representationService).Call(representationService.GetXmlSchemaAddress(new Uri("http://localhost:8080/storage"))).Return("http://localhost:8080/storage/schemas/representation.xsd");

            IRuleService ruleService = _mocks.StrictMock<IRuleService>();
            Expect.On(_unity).Call(_unity.Resolve<IRuleService>()).Return(ruleService);

            Expect.On(ruleService).Call(ruleService.GetXmlSchemaAddress(new Uri("http://localhost:8080/storage"))).Return("http://localhost:8080/storage/schemas/rule.xsd");

            _mocks.ReplayAll();

            try
            {
                _repositoryServiceImpl = new RepositoryService(_logger, _unity);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/schemas/");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Link rel=""schema"" href=""http://localhost:8080/storage/schemas/objectmodel.xsd"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""schema"" href=""http://localhost:8080/storage/schemas/casefilespecification.xsd"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""schema"" href=""http://localhost:8080/storage/schemas/casefile.xsd"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""schema"" href=""http://localhost:8080/storage/schemas/representation.xsd"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""schema"" href=""http://localhost:8080/storage/schemas/rule.xsd"" />"));
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

        private delegate string GetXmlSchemaDelegate(string schemaName);

        [Test]
        public void TestGetXmlSchema()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IRepositoryService repositoryService = _mocks.StrictMock<IRepositoryService>();

            ICommand getXmlSchemaCommand = new GetRepositoryXmlSchemaCommand(repositoryService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRepositoryXmlSchemaCommand")).Return(getXmlSchemaCommand);

            IFormatter repositoryFormatter = new TextFormatter();
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepositoryXmlSchema.xsdFormatter")).Return(repositoryFormatter);

            Expect.On(repositoryService).Call(repositoryService.GetXmlSchema(string.Empty)).IgnoreArguments().Do(
                new GetXmlSchemaDelegate(
                    delegate(string schemaName)
                    {
                        return _repositoryServiceImpl.GetXmlSchema(schemaName);
                    }
                )
            );

            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            Expect.On(_unity).Call(_unity.Resolve<IObjectModelService>()).Return(objectModelService);

            Expect.On(objectModelService).Call(objectModelService.GetXmlSchemaName()).Return("objectmodel.xsd");

            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            Expect.On(_unity).Call(_unity.Resolve<ICaseFileSpecificationService>()).Return(caseFileSpecificationService);

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetXmlSchemaName()).Return("casefilespecification.xsd");

            ICaseFileService caseFileService = _mocks.StrictMock<ICaseFileService>();
            Expect.On(_unity).Call(_unity.Resolve<ICaseFileService>()).Return(caseFileService);

            Expect.On(caseFileService).Call(caseFileService.GetXmlSchemaName()).Return("casefile.xsd");

            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();
            Expect.On(_unity).Call(_unity.Resolve<IRepresentationService>()).Return(representationService);

            Expect.On(representationService).Call(representationService.GetXmlSchemaName()).Return("representation.xsd");

            IRuleService ruleService = _mocks.StrictMock<IRuleService>();
            Expect.On(_unity).Call(_unity.Resolve<IRuleService>()).Return(ruleService);

            Expect.On(ruleService).Call(ruleService.GetXmlSchemaName()).Return("rule.xsd");

            Expect.On(ruleService).Call(ruleService.GetXmlSchemaText()).Return("<xs:schema/>");

            _mocks.ReplayAll();

            try
            {
                _repositoryServiceImpl = new RepositoryService(_logger, _unity);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/schemas/rule.xsd");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<xs:schema/>"));
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

        private delegate IEnumerable<Resource> GetResourcesDelegate(Uri baseUri);
        private delegate Resource GetResourceDelegate(string id, Uri baseUri);
        private delegate string GetResourceListDelegate(IEnumerable<Resource> items, Uri baseUri, Encoding encoding);

        private delegate string GetRepositoryListDelegate(Uri baseUri, Encoding encoding);

        [Test]
        public void TestGetRepositoryInfo()
        {
            _mocks.Record();

            _logger = new Log4NetLogger();
            _unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            Expect.On(_unity).Call(_unity.Resolve<IObjectModelService>()).Return(objectModelService);
            IResourceService resourceService = _mocks.StrictMock<IResourceService>();
            Expect.On(_unity).Call(_unity.Resolve<IResourceService>()).Return(resourceService);
            IRepositoryService repositoryService = _mocks.StrictMock<IRepositoryService>();

            ICommand getXmlSchemaCommand = new GetRepositoryListCommand(repositoryService);
            Expect.On(_unity).Call(_unity.Resolve<ICommand>("GETRepositorysCommand")).Return(getXmlSchemaCommand);

            IFormatter repositoryFormatter = new TextFormatter();
            Expect.On(_unity).Call(_unity.Resolve<IFormatter>("RepositorysXmlFormatter")).Return(repositoryFormatter);

            Expect.On(repositoryService).Call(repositoryService.GetList(null, Encoding.UTF8)).IgnoreArguments().Do(
                new GetRepositoryListDelegate(
                    delegate(Uri baseUri, Encoding encoding)
                    {
                        return _repositoryServiceImpl.GetList(baseUri, encoding);
                    }
                )
            );
            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                Name = "objectmodel",
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(objectModelService).Call(objectModelService.BaseObjectType).Return(objectModelType);

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource,
                Name = "resource",
                RelativeUri = "/resources/"
            };

            Expect.On(resourceService).Call(resourceService.BaseObjectType).Return(resourceType);

            _mocks.ReplayAll();

            try
            {
                _repositoryServiceImpl = new RepositoryService(_logger, _unity);

                InitializeRestServiceHost();

                _serviceHost.Open();

                WebClient webClient = new WebClient();

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/storage/");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<Links>"));
                Assert.IsTrue(result.Contains(@"<Link rel=""schema"" href=""http://localhost:8080/storage/schemas/"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/"" />"));
                Assert.IsTrue(result.Contains(@"<Link rel=""resource"" href=""http://localhost:8080/storage/resources/"" />"));
                Assert.IsTrue(result.Contains(@"</Links>"));
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
