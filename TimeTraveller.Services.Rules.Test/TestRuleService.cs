using System;
using System.Text;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Rules.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Logging.Log4Net;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Rules.Test
{
    /// <summary>
    /// Summary description for TestRuleService
    /// </summary>
    [TestFixture]
    public class TestRuleService
    {
        private MockRepository _mocks;

        public TestRuleService()
        {
        }

        #region Additional test attributes before running the first test in the class
        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            BasicConfigurator.Configure();
        }

        [TestFixtureTearDown]
        public static void TestFixtureTearDown()
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
        #endregion

        [Test]
        public void TestConstructor()
        {
            _mocks.Record();

            ILogger logger = _mocks.StrictMock<ILogger>();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            _mocks.ReplayAll();

            IRuleService ruleService = new RuleService(logger, unity, caseFileSpecificationService, dataService);

            Assert.IsNotNull(ruleService);
        }

        [Test]
        public void TestConvertRuleToXmlAndBack()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

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
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = specificationValue
            };

            _mocks.ReplayAll();

            IRuleService ruleService = new RuleService(logger, unity, caseFileSpecificationService, dataService);

            Rule myrule = new Rule()
            {
                Name = "myrule",
                CaseFileSpecification = myspecification,
                CaseFileSpecificationUri = "http://localhost:8080/storage/specifications/casefiles/myobjectmodel/myspecification",
                SelfUri = "http://localhost:8080/storage/rules/myobjectmodel/myspecification/myrule",
                Script = new RuleScript()
                {
                    Method = "myobjectmodel.myspecification.myrule.Execute",
                    Type = "fsharp",
                    Value = @"#light
module myobjectmodel.myspecification.myrule

open System
open myobjectmodel.myspecification

let Execute(p: Person) =
    p.NumberOfHouses <- Seq.length(p.OwnsHouse)
    p.NumberOfHousesSpecified <- true
"
                }
            };

            string xml = ruleService.GetXml(myrule, Encoding.Unicode);

            Assert.IsTrue(!string.IsNullOrEmpty(xml));

            logger.DebugFormat("Xml={0}", xml);

            myrule = ruleService.Convert(xml, Encoding.Unicode);

            Assert.AreEqual("myrule", myrule.Name);
            Assert.AreEqual("fsharp", myrule.Script.Type);
            Assert.IsTrue(!string.IsNullOrEmpty(myrule.Script.Value));
        }

        [Test]
        public void TestGetRule()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            BaseObjectValue specificationValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification"
                }
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationValue.Id)).Return(specificationValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = specificationValue
            };
            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Convert(specificationValue, new Uri("http://localhost:8080/storage"))).Return(myspecification);

            BaseObjectValue ruleValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification/myrule",
                    BaseObjectType = ruleSetType,
                    ReferenceId = specificationValue.ParentBaseObject.Id
                },
                ReferenceId = specificationValue.Id,
                Text = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <Rule xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://timetraveller.net/storage/schemas/rule.xsd"">
                              <Link rel=""casefilespecification"" href="""" />
                              <Link rel=""self"" href="""" />
                              <Name>myrule</Name>
                              <Script Type=""fsharp"" Method=""myobjectmodel.myspecification.myrule.Execute"">#light
module myobjectmodel.myspecification.myrule

open System
open myobjectmodel.myspecification

let Execute(p: Person) =
    p.NumberOfHouses &lt;- Seq.length(p.OwnsHouse)
    p.NumberOfHousesSpecified &lt;- true
                              </Script>
                            </Rule>"
            };

            Expect.On(dataService).Call(dataService.GetValue(ruleValue.Parent.ExtId, null, TimePoint.Past)).IgnoreArguments().Return(ruleValue);

            _mocks.ReplayAll();

            IRuleService ruleService = new RuleService(logger, unity, caseFileSpecificationService, dataService);

            Rule myrule = ruleService.Get("myobjectmodel/myspecification/myrule", new Uri("http://localhost:8080/storage"));

            logger.DebugFormat("CaseFileSpecificationUri={0}", myrule.CaseFileSpecificationUri);
            logger.DebugFormat("SelfUri={0}", myrule.SelfUri);
            logger.DebugFormat("Script.Value={0}", myrule.Script.Value);

            Assert.IsNotNull(myrule);
            Assert.AreEqual("myrule", myrule.Name);
            Assert.IsNotNull(myrule.CaseFileSpecification);
            Assert.IsTrue(string.IsNullOrEmpty(myrule.CaseFileSpecificationUri));
            Assert.AreEqual("http://localhost:8080/itsmyobjectmodel/myspecification/myrule?version=0",myrule.SelfUri);
            Assert.IsTrue(!string.IsNullOrEmpty(myrule.Script.Value));

            logger.Debug(myrule.Script.Value);
        }

        private delegate IBaseObjectValue InsertValueDelegate(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, IUserInfo journalInfo);

        [Test]
        public void TestStoreRule()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel/myspecification/myrule", null)).IgnoreArguments().Return(null);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        Id = Guid.NewGuid(),
                        ExtId = "myobjectmodel/myspecification"
                    }
                }
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(myspecification.BaseObjectValue.Id)).Return(myspecification.BaseObjectValue);

            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, null, Guid.Empty, "myobjectmodel/myspecification/myrule", null as IBaseObjectType, null as IBaseObjectValue,  null as WebHttpHeaderInfo)).IgnoreArguments().Do(
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

            IRuleService ruleService = new RuleService(logger, unity, caseFileSpecificationService, dataService);

            Rule myrule = new Rule()
            {
                Name = "myrule",
                CaseFileSpecification = myspecification,
                Script = new RuleScript()
                {
                    Method = "myobjectmodel.myspecification.myrule.Execute",
                    Type = "fsharp",
                    Value = @"#light
module myobjectmodel.myspecification.myrule

open System
open myobjectmodel.myspecification

let Execute(p: Person) =
    p.NumberOfHouses <- Seq.length(p.OwnsHouse)
    p.NumberOfHousesSpecified <- true"
                }
            };

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            ruleService.Store("myobjectmodel/myspecification/myrule", myrule, new Uri("http://localhost:8080/storage"), info);

            Assert.IsNotNull(myrule);
            Assert.AreEqual("myrule", myrule.Name);
            Assert.IsTrue(string.IsNullOrEmpty(myrule.SelfUri));
            Assert.IsTrue(!string.IsNullOrEmpty(myrule.Script.Value));
        }

        [Test]
        public void TestGetXmlSchemas()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            _mocks.ReplayAll();

            IRuleService ruleService = new RuleService(logger, unity, caseFileSpecificationService, dataService);

            string result = ruleService.GetXmlSchemaName();
            Assert.AreEqual("rule.xsd", result);
        }

        [Test]
        public void TestGetXmlSchema()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            BaseObjectType ruleSetType = new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.RuleSet))).Return(ruleSetType);

            _mocks.ReplayAll();

            IRuleService ruleService = new RuleService(logger, unity, caseFileSpecificationService, dataService);

            string result = ruleService.GetXmlSchemaText();
            Assert.IsTrue(result.Contains("<xs:schema"));
            Assert.IsTrue(result.Contains(@"http://timetraveller.net/storage/schemas/rule.xsd"));
        }
    }
}
