using System;
using System.Text;
using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Representations.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Logging.Log4Net;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Representations.Test
{
    /// <summary>
    /// Summary description for TestRepresentationService
    /// </summary>
    [TestFixture]
    public class TestRepresentationService
    {
        private MockRepository _mocks;

        #region Additional test attributes before running the first test in the class
        [TestFixtureSetUp()]
        public static void ClassInitialize()
        {
            BasicConfigurator.Configure();
        }

        [TestFixtureTearDown()]
        public static void ClassCleanup()
        {
            LogManager.Shutdown();
        }

        [SetUp()]
        public void TestInitialize()
        {
            _mocks = new MockRepository();

            _mocks.ReplayAll();
        }

        [TearDown()]
        public void TestCleanup()
        {
            _mocks.VerifyAll();
        }
        #endregion
        [Test]
        public void TestConstructor()
        {
            _mocks.Record();

            ILogger logger = _mocks.StrictMock<ILogger>();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, unity, caseFileSpecificationService, dataService);

            Assert.IsNotNull(representationService);
        }

        [Test]
        public void TestConvertRepresentationToXmlAndBack()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, unity, caseFileSpecificationService, dataService);

            Representation myrepresentation = new Representation()
            {
                Name = "myrepresentation",
                CaseFileSpecification = new CaseFileSpecification()
                {
                    Name = "myspecification"
                },
                CaseFileSpecificationUri = "http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification",
                SelfUri = "http://localhost:8080/storage/representations/myobjectmodel/myspecification/mypresentation",
                Script = new RepresentationScript()
                {
                    Type = "xslt",
                    Text = @"<?xml version=""1.0"" encoding=""utf-8""?> 
                            <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""> 
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

            string xml = representationService.GetXml(myrepresentation, Encoding.Unicode);

            Assert.IsTrue(!string.IsNullOrEmpty(xml));

            logger.DebugFormat("Xml={0}", xml);

            myrepresentation = representationService.Convert(xml, Encoding.Unicode);

            Assert.AreEqual("myrepresentation", myrepresentation.Name);
            Assert.AreEqual("xslt", myrepresentation.Script.Type);
            Assert.IsTrue(!string.IsNullOrEmpty(myrepresentation.Script.Text));
        }

        [Test]
        public void TestGetRepresentation()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            BaseObjectValue specificationValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid()
                }
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationValue.Id)).Return(specificationValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = specificationValue
            };

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Convert(specificationValue, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            BaseObjectValue representationValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification/myrepresentation",
                    BaseObjectType = representationType,
                    ReferenceId = specificationValue.Parent.Id
                },
                ReferenceId = specificationValue.Id,
                Text = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <Representation xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://timetraveller.net/storage/schemas/representation.xsd"">
                              <Link rel=""casefilespecification"" href="""" />
                              <Link rel=""self"" href="""" />
                              <Name>myrepresentation</Name>
                              <Script Type=""xslt"" ContentType=""html"">
                                <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:vs=""http://microsoft.com/schemas/VisualStudio/TeamTest/2006"">
                                  <xsl:template match=""/"">
                                    <html xmlns="""">
                                      <body>This is myrepresentation.</body>
                                    </html>
                                  </xsl:template>
                                </xsl:stylesheet>
                              </Script>
                            </Representation>"
            };

            Expect.On(dataService).Call(dataService.GetValue(representationValue.Parent.ExtId, null, TimePoint.Past)).IgnoreArguments().Return(representationValue);

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, unity, caseFileSpecificationService, dataService);

            Representation myrepresentation = representationService.Get("myobjectmodel/myspecification/myrepresentation", new Uri("http://localhost:8080/storage"));

            Assert.IsNotNull(myrepresentation);
            Assert.AreEqual("myrepresentation", myrepresentation.Name);
            Assert.IsNotNull(myrepresentation.CaseFileSpecification);
            Assert.IsTrue(string.IsNullOrEmpty(myrepresentation.CaseFileSpecificationUri));
            Assert.AreEqual("http://localhost:8080/itsmyobjectmodel/myspecification/myrepresentation?version=0", myrepresentation.SelfUri);
            Assert.IsTrue(!string.IsNullOrEmpty(myrepresentation.Script.Text));
        }

        private delegate IBaseObjectValue InsertValueDelegate(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, IUserInfo journalInfo);

        [Test]
        public void TestStoreRepresentation()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            BaseObjectValue specificationValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification"
                }
            };

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myspecification",
                BaseObjectValue = specificationValue
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(myspecification.Id)).Return(specificationValue);

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel/myspecification/myrepresentation", representationType)).Return(null);

            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, null, Guid.Empty, "myobjectmodel/myspecification/myrepresentation", representationType, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValueDelegate(
                    delegate(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
                    {
                        BaseObjectValue value = new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ParentBaseObject = new BaseObject()
                            {
                                Id = id,
                                ExtId = extId,
                                Reference = referenceObjectValue.Parent,
                                Type = type
                            },
                            Reference = referenceObjectValue,
                            Start = timePoint,
                            End = TimePoint.Future,
                            Text = content
                        };

                        return value;
                    }
                )
            );

            dataService.SaveChanges();

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, unity, caseFileSpecificationService, dataService);

            Representation myrepresentation = new Representation()
            {
                Name = "myrepresentation",
                CaseFileSpecification = myspecification,
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

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            representationService.Store("myobjectmodel/myspecification/myrepresentation", myrepresentation, new Uri("http://localhost:8080/storage"), info);

            Assert.IsNotNull(myrepresentation);
            Assert.AreEqual("myrepresentation", myrepresentation.Name);
            Assert.IsTrue(string.IsNullOrEmpty(myrepresentation.SelfUri));
            Assert.IsTrue(!string.IsNullOrEmpty(myrepresentation.Script.Text));
        }

        [Test]
        public void TestTransformUsingXslt()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            IRepresentationTransformer xsltTransformer = new XsltTransformer();
            Expect.On(unity).Call(unity.Resolve<IRepresentationTransformer>("xslt")).Return(xsltTransformer);

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, unity, caseFileSpecificationService, dataService);

            string xml = @"<bla/>";

            Representation myrepresentation = new Representation()
            {
                Name = "myrepresentation",
                SelfUri = "",
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

            string html = representationService.Transform(xml, myrepresentation);

            logger.DebugFormat("HTML=\r\n{0}", html);

            Assert.IsTrue(!string.IsNullOrEmpty(html));
            Assert.IsTrue(html.StartsWith("<html"));
            Assert.IsTrue(html.EndsWith("</html>"));
        }

        [Test]
        public void TestGetXmlSchemas()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, container, caseFileSpecificationService, dataService);

            string result = representationService.GetXmlSchemaName();
            Assert.AreEqual("representation.xsd", result);
        }

        [Test]
        public void TestGetXmlSchema()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType representationType = new BaseObjectType()
            {
                Id = (int)ItemType.Representation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Representation))).Return(representationType);

            _mocks.ReplayAll();

            IRepresentationService representationService = new RepresentationService(logger, container, caseFileSpecificationService, dataService);

            string result = representationService.GetXmlSchemaText();
            Assert.IsTrue(result.Contains("<xs:schema"));
            Assert.IsTrue(result.Contains(@"http://timetraveller.net/storage/schemas/representation.xsd"));
        }
    }
}
