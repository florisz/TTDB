using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Text;
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
using Luminis.Its.Services.Representations;
using Luminis.Logging;
using Luminis.Logging.Log4Net;
using Luminis.Patterns.Range;
using Luminis.Unity;
using Luminis.Xml.Xslt;

namespace Luminis.Its.Services.CaseFiles.Test
{
    /// <summary>
    /// Summary description for TestCaseFileService
    /// </summary>
    [TestFixture]
    public class TestCaseFileService
    {
        private MockRepository _mocks;
        private ICaseFileSpecificationService _caseFileSpecificationServiceImpl;

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

        [SetUp]
        public void TestInitialize()
        {
            _mocks = new MockRepository();

            _mocks.ReplayAll();
        }

        [TearDown]
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
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);

            Assert.IsNotNull(caseFileService);
        }

        [Test]
        public void TestConvertXml2CaseFileAndBack()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);

            string xml = @"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification""/>
                                <cs:Link rel=""self"" href=""http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers""/>
                                <Person>
                                    <FirstName>Alex</FirstName>
                                    <LastName>Harbers</LastName>
                                </Person>
                           </cs:CaseFile>";

            CaseFile caseFile = caseFileService.Convert(xml, Encoding.Unicode);
            caseFile.BaseObjectValue = new BaseObjectValue()
            {
                ParentBaseObject = new BaseObject()
                {
                    ExtId = "myobjectmodel/myspecification/Alex.Harbers"
                },
                Version = ++index
            };
            caseFile.CaseFileSpecification = new CaseFileSpecification()
            {
                Name = "myspecification"
            };
            logger.DebugFormat("CaseFile: {0}", caseFile.ExtId);
            logger.DebugFormat(".Text=\r\n{0}", caseFile.Text.Replace("\n", "\r\n"));

            string result = caseFileService.GetXml(caseFile, Encoding.Unicode);
            logger.DebugFormat("Result={0}", result.Replace("\n", "\r\n"));
        }

        [Test]
        public void TestGetCaseFile()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;
            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                BaseObjectType = objectModelType
            };
            BaseObjectValue objectModelObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Version = ++index
            };

            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = objectModelObjectValue
            };

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification",
                        Type = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    },
                    Version = ++index
                },
                Name = "myspecification",
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                }
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(myspecification.BaseObjectValue.Id)).IgnoreArguments().Return(myspecification.BaseObjectValue);

            TimePoint now = TimePoint.Now;

            BaseObjectType entityType = new BaseObjectType()
            {
                Id = (int)ItemType.Entity
            };

            BaseObject personObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "Person",
                BaseObjectType = entityType,
                Reference = objectModelObject
            };
            BaseObjectValue personObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = personObject,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<FirstName>Alex</FirstName>
                            <LastName>Harbers</LastName>"
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(personObject.Id)).IgnoreArguments().Return(personObject);

            BaseObject caseFileObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification/Alex.Harbers",
                BaseObjectType = caseFileType,
                Reference = myspecification.BaseObjectValue.Parent,
                Relation1 = personObject
            };
            BaseObjectValue caseFileObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = caseFileObject,
                Reference = myspecification.BaseObjectValue,
                Start = now,
                Text = string.Empty,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(caseFileObject.ExtId, caseFileType, myspecification.BaseObjectValue.Parent)).Return(caseFileObject);

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            CaseFile result = caseFileService.Get(myspecification, "myobjectmodel/myspecification/Alex.Harbers", new Uri("http://localhost:8080/its"));
            Assert.IsNotNull(result);

            logger.DebugFormat("Result={0}", result.Text);

            Assert.AreEqual("myobjectmodel/myspecification/Alex.Harbers", result.ExtId);
            Assert.AreEqual("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers?version=3", result.SelfUri);
            Assert.AreEqual("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification?version=2", result.CaseFileSpecificationUri);
            Assert.AreEqual("myobjectmodel/myspecification", result.CaseFileSpecification.ExtId);
            Assert.IsTrue(result.Text.Contains("<Person"));
            Assert.IsTrue(result.Text.Contains("<FirstName>Alex</FirstName>"));
            Assert.IsTrue(result.Text.Contains("<LastName>Harbers</LastName>"));
            Assert.IsTrue(result.Text.Contains("</Person>"));
        }

        private delegate IBaseObjectValue GetValueDelegate(Guid id, TimePoint timePoint);

        [Test]
        public void TestGetCaseFileWithMultipleLevels()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;
            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                BaseObjectType = objectModelType
            };
            BaseObjectValue objectModelObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Version = ++index
            };
            ObjectModel objectModel = new ObjectModel()
            {
                BaseObjectValue = objectModelObjectValue,
                Name = "myobjectmodel"
            };

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            BaseObject specificationObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = specificationType,
                Reference = objectModelObject
            };
            BaseObjectValue specificationObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = specificationObject,
                Reference = objectModelObjectValue,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationObjectValue.Id)).Return(specificationObjectValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = specificationObjectValue,
                Name = "myspecification",
                ObjectModel = objectModel,
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}",
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[] 
                        {
                            new CaseFileSpecificationRelation()
                            {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity()
                                {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                }
            };

            TimePoint now = TimePoint.Now;

            BaseObjectType entityType = new BaseObjectType()
            {
                Id = (int)ItemType.Entity
            };

            BaseObject personObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "Person",
                BaseObjectType = entityType,
                Reference = objectModelObject

            };
            BaseObjectValue personObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = personObject,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<FirstName>Alex</FirstName>
                            <LastName>Harbers</LastName>",
                Version = ++index
            };

            BaseObject house1Object = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "House",
                BaseObjectType = entityType,
                Reference = objectModelObject
            };
            BaseObjectValue house1ObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = house1Object,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<Address>Lindelaan 93A</Address>
                            <ZipCode>1234 AB</ZipCode>
                            <City>Arnhem</City>
                            <Country>Nederland</Country>",
                Version = ++index
            };

            BaseObjectType relationType = new BaseObjectType()
            {
                Id = (int)ItemType.Relation
            };

            BaseObject ownsHouse1Object = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "OwnsHouse",
                BaseObjectType = relationType,
                Reference = objectModelObject,
                Relation1 = personObject,
                Relation2 = house1Object
            };
            BaseObjectValue ownsHouse1ObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = ownsHouse1Object,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<From>2001-09-01</From>
                            <To>2009-06-15</To>",
                Version = ++index
            };

            BaseObject house2Object = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "House",
                BaseObjectType = entityType,
                Reference = objectModelObject
            };
            BaseObjectValue house2ObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = house2Object,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<Address>Patersstraat 13A</Address>
                            <ZipCode>6828 AB</ZipCode>
                            <City>Arnhem</City>
                            <Country>Nederland</Country>",
                Version = ++index
            };

            BaseObject ownsHouse2Object = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "OwnsHouse",
                BaseObjectType = relationType,
                Reference = objectModelObject,
                Relation1 = personObject,
                Relation2 = house2Object
            };
            BaseObjectValue ownsHouse2ObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = ownsHouse2Object,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<From>2009-06-15</From>",
                Version = ++index
            };
            
            List<IRelationObject> person2OwnsHouseRelations = new List<IRelationObject>() {ownsHouse1Object, ownsHouse2Object};

            Expect.On(dataService).Call(dataService.GetBaseObject(personObject.Id)).Return(personObject);

            Expect.On(dataService).Call(dataService.GetRelations(personObject, "OwnsHouse")).Return(person2OwnsHouseRelations);

            Expect.On(dataService).Call(dataService.GetValue(ownsHouse1Object.Id, TimePoint.Past)).IgnoreArguments().Do(
                new GetValueDelegate(
                    delegate(Guid id, TimePoint timePoint)
                    {
                        if (id.Equals(ownsHouse1Object.Id))
                        {

                            return ownsHouse1ObjectValue;
                        }
                        else if (id.Equals(ownsHouse2Object.Id))
                        {
                            return ownsHouse2ObjectValue;
                        }
                        else if (id.Equals(house1Object.Id))
                        {
                            return house1ObjectValue;
                        }
                        else if (id.Equals(house2Object.Id))
                        {
                            return house2ObjectValue;
                        }
                        else
                        {
                            throw new ArgumentException(string.Format("Invalid id {0} specified", id));
                        }
                    }
                )
            ).Repeat.Times(4);

            BaseObject caseFileObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification/Alex.Harbers",
                BaseObjectType = caseFileType,
                Reference = myspecification.BaseObjectValue.Parent,
                Relation1 = personObject
            };
            BaseObjectValue caseFileObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = caseFileObject,
                Reference = myspecification.BaseObjectValue,
                Start = now,
                Text = string.Empty,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(caseFileObject.ExtId, caseFileType, myspecification.BaseObjectValue.Parent)).Return(caseFileObject);

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            CaseFile result = caseFileService.Get(myspecification, "myobjectmodel/myspecification/Alex.Harbers", new Uri("http://localhost:8080/its"));

            Assert.IsNotNull(result);
            Assert.AreEqual("myobjectmodel/myspecification/Alex.Harbers", result.ExtId);
            Assert.AreEqual("myobjectmodel/myspecification", result.CaseFileSpecification.ExtId);

            string debugResult = result.Text;
            debugResult = debugResult.Replace("><", ">\r\n<");

            logger.DebugFormat("Result=\r\n{0}", debugResult);

            Assert.AreEqual("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification?version=2", result.CaseFileSpecificationUri);
            Assert.AreEqual("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers?version=8", result.SelfUri);
            Assert.IsTrue(result.Text.Contains("<Person"));
            Assert.IsTrue(result.Text.Contains("<FirstName>Alex</FirstName>"));
            Assert.IsTrue(result.Text.Contains("<LastName>Harbers</LastName>"));
            Assert.IsTrue(result.Text.Contains("<OwnsHouse"));
            Assert.IsTrue(result.Text.Contains("<House"));
            Assert.IsTrue(result.Text.Contains("<Address>Lindelaan 93A</Address>"));
            Assert.IsTrue(result.Text.Contains("<ZipCode>1234 AB</ZipCode>"));
            Assert.IsTrue(result.Text.Contains("<City>Arnhem</City>"));
            Assert.IsTrue(result.Text.Contains("<Country>Nederland</Country>"));
            Assert.IsTrue(result.Text.Contains("</House>"));
            Assert.IsTrue(result.Text.Contains("</OwnsHouse>"));
            Assert.IsTrue(result.Text.Contains("<House"));
            Assert.IsTrue(result.Text.Contains("<Address>Patersstraat 13A</Address>"));
            Assert.IsTrue(result.Text.Contains("<ZipCode>6828 AB</ZipCode>"));
            Assert.IsTrue(result.Text.Contains("<City>Arnhem</City>"));
            Assert.IsTrue(result.Text.Contains("<Country>Nederland</Country>"));
            Assert.IsTrue(result.Text.Contains("</House>"));
            Assert.IsTrue(result.Text.Contains("</Person>"));
        }

        private delegate IBaseObjectValue InsertValue1Delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue);
        private delegate IBaseObjectValue InsertValue2Delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo);
        private delegate void SaveChangesDelegate();

        [Test]
        public void TestStoreNewCaseFile()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            TimePoint now = TimePoint.Now;

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                BaseObjectType = objectModelType
            };

            BaseObjectValue objectModelObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = objectModelObjectValue,
                Name = "myobjectmodel"
            };

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            BaseObject specificationObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = specificationType,
                Reference = objectModelObject
            };
            BaseObjectValue specificationObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = specificationObject,
                Reference = objectModelObjectValue,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationObjectValue.Id)).Return(specificationObjectValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = specificationObjectValue,
                Name = "myspecification",
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            BaseObjectType entityType = new BaseObjectType()
            {
                Id = (int)ItemType.Entity
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Entity))).Return(entityType);

            Expect.On(dataService).Call(dataService.GetBaseObject(Guid.Empty)).IgnoreArguments().Return(null);

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel/myspecification/Alex.Harbers", caseFileType, specificationObject)).Return(null);

            List<BaseObject> insertedObjects = new List<BaseObject>();
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, null, null as IBaseObjectValue)).IgnoreArguments().Do(
                new InsertValue1Delegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
                    {
                        logger.DebugFormat("InsertValue({0}, {1}, {2}), content=\r\n{3}", timeStamp, extId, referenceObjectValue.Id, content);
                        BaseObject insertedObject = new BaseObject()
                        {
                            Id = id,
                            ExtId = extId,
                            Type = type,
                            Reference = referenceObjectValue.Parent
                        };

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = insertedObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(insertedObject);

                        return newValue;
                    }
                )
            );

            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, null, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValue2Delegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
                    {
                        logger.DebugFormat("InsertValue({0}, {1}, {2}, {3}), content=\r\n{4}", timeStamp, extId, referenceObjectValue.Id, journalInfo.Username, content);
                        BaseObject insertedObject = new BaseObject()
                        {
                            Id = id,
                            ExtId = extId,
                            Type = type,
                            Reference = referenceObjectValue.Parent
                        };

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = insertedObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(insertedObject);

                        return newValue;
                    }
                )
            );

            dataService.SaveChanges();

            caseFileSpecificationService.ValidateCaseFile(null, string.Empty, string.Empty);
            LastCall.On(caseFileSpecificationService).IgnoreArguments();

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                        <cs:Link rel=""casefilespecification"" href=""""/>
                                        <cs:Link rel=""self"" href=""""/>
                                        <Person RegistrationId=""{0}"" RegistrationStart=""{1}"" RegistrationEnd="""">
                                            <FirstName>Alex</FirstName>
                                            <LastName>Harbers</LastName>
                                        </Person>
                                    </cs:CaseFile>", Guid.NewGuid(), DateTime.Now.ToString("O"));
            CaseFile caseFile = caseFileService.Convert(caseFileXml, Encoding.Unicode);
            caseFile.CaseFileSpecification = myspecification;

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            caseFileService.Store(myspecification, "myobjectmodel/myspecification/Alex.Harbers", caseFile, new Uri("http://localhost:8080/its"), info);

            Assert.AreEqual("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification?version=2", caseFile.CaseFileSpecificationUri);
            Assert.AreEqual("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers?version=4", caseFile.SelfUri);
            Assert.AreEqual(2, insertedObjects.Count);
            IBaseObject personObject = insertedObjects[0];
            Assert.AreEqual("Person", personObject.ExtId);
            Assert.AreEqual("myobjectmodel", personObject.Reference.ExtId);
            Assert.AreEqual(1, personObject.Values.Count());

            IBaseObjectValue personValue = personObject.Values.Last();
            Assert.IsNotNull(personValue);
            Assert.IsNotNull(personValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, personValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, personValue.Start);
            Assert.AreEqual(TimePoint.Future, personValue.End);
            Assert.IsFalse(personValue.Text.Contains("<Person>"));
            Assert.IsTrue(personValue.Text.Contains("<FirstName>Alex</FirstName>"));
            Assert.IsTrue(personValue.Text.Contains("<LastName>Harbers</LastName>"));
            Assert.IsFalse(personValue.Text.Contains("</Person>"));

            IBaseObject caseFileObject = insertedObjects[1];
            Assert.AreEqual("myobjectmodel/myspecification/Alex.Harbers", caseFileObject.ExtId);
            Assert.AreEqual(specificationObject.ExtId, caseFileObject.Reference.ExtId);
            Assert.AreEqual(1, caseFileObject.Values.Count());

            IBaseObjectValue caseFileValue = insertedObjects[1].Values.Last();
            Assert.IsNotNull(caseFileValue);
            Assert.IsNotNull(caseFileValue.Reference);
            Assert.AreEqual(specificationObjectValue.Id, caseFileValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, caseFileValue.Start);
            Assert.AreEqual(TimePoint.Future, caseFileValue.End);
            Assert.AreEqual(string.Empty, caseFileValue.Text);

            IRelationObject caseFileRelationObject = caseFileValue.Parent as IRelationObject;
            Assert.AreEqual(personObject.Id, caseFileRelationObject.Relation1.Id);
        }

        private delegate IBaseObjectValue InsertValue3Delegate(string content, TimePoint timeStamp, IBaseObject baseObject, IBaseObjectValue referenceBaseObjectValue);
        private delegate IBaseObjectValue InsertValue4Delegate(string content, TimePoint timeStamp, IBaseObject baseObject, IBaseObjectValue referenceBaseObjectValue, WebHttpHeaderInfo journalInfo);

        [Test]
        public void TestStoreNewValueInExistingObject()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            TimePoint now = TimePoint.Now;

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };


            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                BaseObjectType = objectModelType
            };
            BaseObjectValue objectModelObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = objectModelObjectValue,
                Name = "myobjectmodel"
            };

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            BaseObject specificationObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = specificationType,
                Reference = objectModelObject
            };
            BaseObjectValue specificationObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = specificationObject,
                Reference = objectModelObjectValue,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationObjectValue.Id)).Return(specificationObjectValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = specificationObjectValue,
                Name = "myspecification",
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            BaseObject existingPersonObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "Person",
                Reference = objectModelObject
            };
            BaseObjectValue existingPersonObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingPersonObject,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<FirstName>Alex</FirstName>
                            <LastName>Harbers</LastName>",
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(existingPersonObject.Id)).Return(existingPersonObject);

            BaseObject existingCaseFileObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification/Alex.Harbers",
                Reference = specificationObject,
                Relation1 = existingPersonObject,
                Type = caseFileType
            };
            BaseObjectValue existingCaseFileObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingCaseFileObject,
                Reference = specificationObjectValue,
                Start = now,
                Text = string.Empty,
                Version = ++index
            };
            Expect.On(dataService).Call(dataService.GetBaseObject(existingCaseFileObject.ExtId, caseFileType, specificationObject)).Return(existingCaseFileObject);

            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, null as IBaseObject, null as IBaseObjectValue)).IgnoreArguments().Do(
                new InsertValue3Delegate(
                    delegate(string content, TimePoint timeStamp, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
                    {
                        logger.DebugFormat("InsertValue({0}, {1}, {2}), content=\r\n{3}", timeStamp, baseObject.Id, referenceObjectValue.Id, content);

                        baseObject.Values.Last().End = timeStamp.AddSeconds(-1);

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            Parent = baseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        return newValue;
                    }
                )
            );

            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, null as IBaseObject, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValue4Delegate(
                    delegate(string content, TimePoint timeStamp, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
                    {
                        logger.DebugFormat("InsertValue({0}, {1}, {2}, {3}), content=\r\n{4}", timeStamp, baseObject.Id, referenceObjectValue.Id, journalInfo.Username, content);

                        baseObject.Values.Last().End = timeStamp.AddSeconds(-1);

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            Parent = baseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        return newValue;
                    }
                )
            );

            dataService.SaveChanges();

            caseFileSpecificationService.ValidateCaseFile(null, string.Empty, string.Empty);
            LastCall.On(caseFileSpecificationService).IgnoreArguments();

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                                    <cs:Link rel=""casefilespecification"" href=""""/>
                                                    <cs:Link rel=""self"" href=""""/>
                                                    <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                        <FirstName>Second Alex</FirstName>
                                                        <LastName>Second Harbers</LastName>
                                                    </Person>
                                                </cs:CaseFile>", existingPersonObject.Id, existingPersonObjectValue.StartDate.ToString("O"));
            CaseFile caseFile = caseFileService.Convert(caseFileXml, Encoding.Unicode);
            caseFile.CaseFileSpecification = myspecification;
            caseFile.BaseObjectValue = existingCaseFileObjectValue;

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            caseFileService.Store(myspecification, "myobjectmodel/myspecification/Alex.Harbers", caseFile, new Uri("http://localhost:8080/its"), info);

            Assert.AreEqual("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification?version=2", caseFile.CaseFileSpecificationUri);
            Assert.AreEqual("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers?version=6", caseFile.SelfUri);
            Assert.IsNotNull(existingPersonObject);
            Assert.AreEqual("Person", existingPersonObject.ExtId);
            Assert.AreEqual(2, existingPersonObject.Values.Count());
            IBaseObjectValue firstValue = existingPersonObject.Values.First();
            Assert.IsNotNull(firstValue);
            Assert.IsNotNull(firstValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, firstValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Future, firstValue.End);
            Assert.IsTrue(firstValue.Text.Contains("<FirstName>Alex</FirstName>"));
            Assert.IsTrue(firstValue.Text.Contains("<LastName>Harbers</LastName>"));

            IBaseObjectValue lastValue = existingPersonObject.Values.Last();
            Assert.IsNotNull(lastValue); 
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(lastValue.Start, firstValue.End.AddSeconds(1));
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<FirstName>Second Alex</FirstName>"));
            Assert.IsTrue(lastValue.Text.Contains("<LastName>Second Harbers</LastName>"));

            Assert.IsNotNull(existingCaseFileObject);
            Assert.AreEqual("myobjectmodel/myspecification/Alex.Harbers", existingCaseFileObject.ExtId);
            Assert.AreEqual(2, existingCaseFileObject.Values.Count());
            firstValue = existingCaseFileObject.Values.First();
            Assert.IsNotNull(firstValue);
            Assert.IsNotNull(firstValue.Reference);
            Assert.AreEqual(specificationObjectValue.Id, firstValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Future, firstValue.End);
            Assert.AreEqual(string.Empty, firstValue.Text);

            lastValue = existingCaseFileObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(specificationObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(lastValue.Start, firstValue.End.AddSeconds(1));
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.AreEqual(string.Empty, lastValue.Text);
        }


        private delegate void ValidateCaseFileDelegate(CaseFileSpecification specification, string caseFileId, string xml);

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestStoreCaseFileWithInvalidXml()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            caseFileSpecificationService.ValidateCaseFile(null, string.Empty, string.Empty);
            LastCall.On(caseFileSpecificationService).IgnoreArguments().Do(
                new ValidateCaseFileDelegate(
                    delegate(CaseFileSpecification specification, string caseFileId, string xml)
                    {
                        throw new ArgumentOutOfRangeException("xml", "Invalid case file xml according to the specification schema.");
                    }
                )
            );
            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            string caseFileXml = @"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                        <cs:Link rel=""casefilespecification"" href=""""/>
                                        <cs:Link rel=""self"" href=""""/>
                                        <Person>
                                            <FirstName>Alex</FirstName>
                                            <ThisIsAnInvalidTag>Harbers</ThisIsAnInvalidTag>
                                        </Person>
                                    </cs:CaseFile>";
            CaseFile caseFile = caseFileService.Convert(caseFileXml, Encoding.Unicode);
            caseFile.CaseFileSpecification = new CaseFileSpecification()
            {
                Name = "myspecification",
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            WebHttpHeaderInfo journalInfo = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            caseFileService.Store(caseFile.CaseFileSpecification, "myobjectmodel/myspecification/Alex.Harbers", caseFile, new Uri("http://localhost:8080/its"), journalInfo);
        }

        private delegate IBaseObject CreateBaseObjectDelegate(IBaseObjectType type);
        private delegate IBaseObjectValue CreateBaseObjectValueDelegate();

        [Test]
        public void TestStoreNewCaseFileWithMultipleLevels()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            TimePoint now = TimePoint.Now;

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                BaseObjectType = objectModelType
            };
            BaseObjectValue objectModelObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);
            
            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = objectModelObjectValue,
                Name = "myobjectmodel"
            };

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            BaseObject specificationObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = specificationType,
                Reference = objectModelObject
            };
            BaseObjectValue specificationObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = specificationObject,
                Reference = objectModelObjectValue,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationObjectValue.Id)).Return(specificationObjectValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = specificationObjectValue,
                Name = "myspecification",
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[]
                        {
                            new CaseFileSpecificationRelation()
                            {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity()
                                {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                },
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(Guid.Empty)).IgnoreArguments().Return(null).Repeat.Times(5);

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel/myspecification/Alex.Harbers", caseFileType, specificationObject)).Return(null);

            BaseObjectType entityType = new BaseObjectType()
            {
                Id = (int)ItemType.Entity
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Entity))).Repeat.Times(3).Return(entityType);

            BaseObjectType relationType = new BaseObjectType()
            {
                Id = (int)ItemType.Relation
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Relation))).Repeat.Twice().Return(relationType);

            List<BaseObject> insertedObjects = new List<BaseObject>();
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, null, null as IBaseObjectValue)).IgnoreArguments().Do(
                new InsertValue1Delegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
                    {
                        logger.DebugFormat("InsertValue({0}, {1}, {2}), content=\r\n{3}", timeStamp, extId, referenceObjectValue.Id, content);
                        BaseObject baseObject = new BaseObject()
                        {
                            Id = id,
                            ExtId = extId,
                            Type = type,
                            Reference = referenceObjectValue.Parent
                        };

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = baseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(baseObject);

                        return newValue;
                    }
                )
            ).Repeat.Times(5);
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, null, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValue2Delegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
                    {
                        logger.DebugFormat("InsertValue({0}, {1}, {2}, {3}), content=\r\n{4}", timeStamp, extId, referenceObjectValue.Id, journalInfo.Username, content);
                        BaseObject baseObject = new BaseObject()
                        {
                            Id = id,
                            ExtId = extId,
                            Type = type,
                            Reference = referenceObjectValue.Parent
                        };

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = baseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(baseObject);

                        return newValue;
                    }
                )
            );

            dataService.SaveChanges();

            caseFileSpecificationService.ValidateCaseFile(null, string.Empty, string.Empty);
            LastCall.On(caseFileSpecificationService).IgnoreArguments();

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                        <cs:Link rel=""casefilespecification"" href=""""/>
                                        <cs:Link rel=""self"" href=""""/>
                                        <Person RegistrationId=""{0}"" RegistrationStart=""{1}"" RegistrationEnd="""">
                                            <FirstName>Alex</FirstName>
                                            <LastName>Harbers</LastName>
                                            <OwnsHouse RegistrationId=""{2}"" RegistrationStart=""{3}"" RegistrationEnd="""">
                                                <From>2001-01-01</From>
                                                <To>2004-08-30</To>
                                                <House RegistrationId=""{4}"" RegistrationStart=""{5}"" RegistrationEnd="""">
                                                    <Address>Lindelaan 13</Address>
                                                    <ZipCode>1234 AB</ZipCode>
                                                    <City>Amsterdam</City>
                                                </House>
                                            </OwnsHouse>
                                            <OwnsHouse RegistrationId=""{6}"" RegistrationStart=""{7}"" RegistrationEnd="""">
                                                <From>2004-09-01</From>
                                                <To></To>
                                                <House RegistrationId=""{8}"" RegistrationStart=""{9}"" RegistrationEnd="""">
                                                    <Address>Patersstraat 13A</Address>
                                                    <ZipCode>6828 AB</ZipCode>
                                                    <City>Arnhem</City>
                                                </House>
                                            </OwnsHouse>
                                        </Person>
                                    </cs:CaseFile>", Guid.NewGuid(), DateTime.Now.ToString("O")
                                                   , Guid.NewGuid(), DateTime.Now.ToString("O")
                                                   , Guid.NewGuid(), DateTime.Now.ToString("O")
                                                   , Guid.NewGuid(), DateTime.Now.ToString("O")
                                                   , Guid.NewGuid(), DateTime.Now.ToString("O"));
            CaseFile caseFile = caseFileService.Convert(caseFileXml, Encoding.Unicode);
            caseFile.CaseFileSpecification = myspecification;

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            caseFileService.Store(myspecification, "myobjectmodel/myspecification/Alex.Harbers", caseFile, new Uri("http://localhost:8080/its"), info);

            Assert.AreEqual("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification?version=2", caseFile.CaseFileSpecificationUri);
            Assert.AreEqual("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers?version=8", caseFile.SelfUri);
            Assert.AreEqual(6, insertedObjects.Count);

            BaseObject lindeLaanObject = insertedObjects[0];
            Assert.AreEqual("House", lindeLaanObject.ExtId);
            Assert.AreEqual("myobjectmodel", lindeLaanObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Entity, lindeLaanObject.Type.Id);
            Assert.AreEqual(1, lindeLaanObject.Values.Count());

            IBaseObjectValue lastValue = lindeLaanObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<Address>Lindelaan 13</Address>"));
            Assert.IsTrue(lastValue.Text.Contains("<ZipCode>1234 AB</ZipCode>"));
            Assert.IsTrue(lastValue.Text.Contains("<City>Amsterdam</City>"));

            BaseObject patersStraatObject = insertedObjects[1];
            Assert.AreEqual("House", patersStraatObject.ExtId);
            Assert.AreEqual("myobjectmodel", patersStraatObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Entity, patersStraatObject.Type.Id);
            Assert.AreEqual(1, patersStraatObject.Values.Count());

            lastValue = patersStraatObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<Address>Patersstraat 13A</Address>"));
            Assert.IsTrue(lastValue.Text.Contains("<ZipCode>6828 AB</ZipCode>"));
            Assert.IsTrue(lastValue.Text.Contains("<City>Arnhem</City>"));

            BaseObject alexharbersObject = insertedObjects[2];
            Assert.AreEqual("Person", alexharbersObject.ExtId);
            Assert.AreEqual("myobjectmodel", alexharbersObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Entity, alexharbersObject.Type.Id);
            Assert.AreEqual(1, alexharbersObject.Values.Count());

            lastValue = alexharbersObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<FirstName>Alex</FirstName>"));
            Assert.IsTrue(lastValue.Text.Contains("<LastName>Harbers</LastName>"));

            BaseObject ownsHouse1Object = insertedObjects[3];
            Assert.AreEqual("OwnsHouse", ownsHouse1Object.ExtId);
            Assert.AreEqual("myobjectmodel", ownsHouse1Object.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Relation, ownsHouse1Object.Type.Id);
            Assert.AreEqual(alexharbersObject, ownsHouse1Object.Relation1);
            Assert.AreEqual(lindeLaanObject, ownsHouse1Object.Relation2);
            Assert.AreEqual(1, ownsHouse1Object.Values.Count());

            lastValue = ownsHouse1Object.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<From>2001-01-01</From>"));
            Assert.IsTrue(lastValue.Text.Contains("<To>2004-08-30</To>"));

            BaseObject ownsHouse2Object = insertedObjects[4];
            Assert.AreEqual("OwnsHouse", ownsHouse2Object.ExtId);
            Assert.AreEqual("myobjectmodel", ownsHouse2Object.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Relation, ownsHouse2Object.Type.Id);
            Assert.AreEqual(alexharbersObject, ownsHouse2Object.Relation1);
            Assert.AreEqual(patersStraatObject, ownsHouse2Object.Relation2);
            Assert.AreEqual(1, ownsHouse2Object.Values.Count());

            lastValue = ownsHouse2Object.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<From>2004-09-01</From>"));
            Assert.IsTrue(lastValue.Text.Contains("<To></To>"));

            BaseObject caseFileObject = insertedObjects[5];
            Assert.AreEqual("myobjectmodel/myspecification/Alex.Harbers", caseFileObject.ExtId);
            Assert.AreEqual("myobjectmodel/myspecification", caseFileObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.CaseFile, caseFileObject.Type.Id);
            Assert.AreEqual(specificationObject.Id, caseFileObject.Reference.Id);
            Assert.AreEqual(alexharbersObject.Id, caseFileObject.Relation1.Id);
            Assert.AreEqual(1, caseFileObject.Values.Count());

            lastValue = caseFileObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(specificationObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.AreEqual(string.Empty, lastValue.Text);
        }

        [Test]
        public void TestStoreExistingCaseFileWithMultipleLevelsInNewAndExistingObjects()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            int index = 0;

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            TimePoint now = TimePoint.Now;

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                BaseObjectType = objectModelType
            };
            BaseObjectValue objectModelObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            ObjectModel objectmodel = new ObjectModel()
            {
                BaseObjectValue = objectModelObjectValue,
                Name = "myobjectmodel"
            };

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            BaseObject specificationObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = specificationType,
                Reference = objectModelObject
            };
            BaseObjectValue specificationObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = specificationObject,
                Reference = objectModelObjectValue,
                Start = now,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(specificationObjectValue.Id)).Return(specificationObjectValue);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                BaseObjectValue = specificationObjectValue,
                Name = "myspecification",
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[]
                        {
                            new CaseFileSpecificationRelation()
                            {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity()
                                {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                },
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            BaseObjectType entityType = new BaseObjectType()
            {
                Id = (int)ItemType.Entity
            };

            BaseObject existingPersonObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "Person",
                BaseObjectType = entityType,
                Reference = objectModelObject
            };
            BaseObjectValue existingPersonObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingPersonObject,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<FirstName>Alex</FirstName><LastName>Harbers</LastName>",
                Version = ++index
            };

            BaseObject existingHouseObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "House",
                BaseObjectType = entityType,
                Reference = objectModelObject
            };
            BaseObjectValue existingHouseObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingHouseObject,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<Address>Typefout 13</Address><ZipCode>1234 AB</ZipCode><City>Amsterdam</City>",
                Version = ++index
            };

            BaseObjectType relationType = new BaseObjectType()
            {
                Id = (int)ItemType.Relation
            };

            BaseObject existingOwnsHouseObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "OwnsHouse",
                BaseObjectType = relationType,
                Reference = objectModelObject,
                Relation1 = existingPersonObject,
                Relation2 = existingHouseObject
            };
            BaseObjectValue existingOwnsHouseObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingOwnsHouseObject,
                Reference = objectModelObjectValue,
                Start = now,
                Text = @"<From>2001-01-01</From><To>2004-08-30</To>",
                Version = ++index
            };

            BaseObject existingCaseFileObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification/Alex.Harbers",
                BaseObjectType = caseFileType,
                Reference = specificationObject,
                Relation1 = existingPersonObject
            };
            BaseObjectValue existingCaseFileObjectvalue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingCaseFileObject,
                Reference = specificationObjectValue,
                Start = now,
                Text = string.Empty,
                Version = ++index
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(existingPersonObject.Id)).Return(existingPersonObject);

            Expect.On(dataService).Call(dataService.GetBaseObject(existingHouseObject.Id)).Return(existingHouseObject);

            Expect.On(dataService).Call(dataService.GetBaseObject(existingOwnsHouseObject.Id)).Return(existingOwnsHouseObject);

            Expect.On(dataService).Call(dataService.GetBaseObject(Guid.Empty)).IgnoreArguments().Repeat.Twice().Return(null);

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Entity))).Return(entityType);

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Relation))).Return(relationType);

            Expect.On(dataService).Call(dataService.GetBaseObject(existingCaseFileObject.ExtId, caseFileType, specificationObject)).Return(existingCaseFileObject);

            List<BaseObject> insertedObjects = new List<BaseObject>();
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, null, null as IBaseObjectValue)).IgnoreArguments().Repeat.Times(2).Do(
                new InsertValue1Delegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
                    {
                        logger.DebugFormat("InsertValue1({0}, {1}, {2}), content=\r\n{3}", timeStamp, extId, referenceObjectValue.Id, content);
                        BaseObject baseObject = new BaseObject()
                        {
                            Id = id,
                            ExtId = extId,
                            Type = type,
                            Reference = referenceObjectValue.Parent
                        };

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = baseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(baseObject);

                        return newValue;
                    }
                )
            );
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, null as IBaseObject , null as IBaseObjectValue)).IgnoreArguments().Do(
                new InsertValue3Delegate(
                    delegate(string content, TimePoint timeStamp, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
                    {
                        logger.DebugFormat("InsertValue2({0}, {1}, {2}), content=\r\n{3}", timeStamp, baseObject.ExtId, referenceObjectValue.Id, content);

                        baseObject.Values.Last().End = timeStamp.AddSeconds(-1);

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = baseObject as BaseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(baseObject as BaseObject);

                        return newValue;
                    }
                )
            );
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, null as IBaseObject, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValue4Delegate(
                    delegate(string content, TimePoint timeStamp, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
                    {
                        logger.DebugFormat("InsertValue2({0}, {1}, {2}, {3}), content=\r\n{4}", timeStamp, baseObject.ExtId, referenceObjectValue.Id, journalInfo.Username, content);

                        baseObject.Values.Last().End = timeStamp.AddSeconds(-1);

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            ParentBaseObject = baseObject as BaseObject,
                            Reference = referenceObjectValue,
                            Start = timeStamp,
                            Text = content,
                            Version = ++index
                        };

                        insertedObjects.Add(baseObject as BaseObject);

                        return newValue;
                    }
                )
            );

            dataService.SaveChanges();

            caseFileSpecificationService.ValidateCaseFile(null, string.Empty, string.Empty);
            LastCall.On(caseFileSpecificationService).IgnoreArguments();

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);
            string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://luminis.net/its/schemas/casefile.xsd"">
                                        <cs:Link rel=""casefilespecification"" href=""""/>
                                        <cs:Link rel=""self"" href=""""/>
                                        <Person RegistrationId=""{0}"" RegistrationStart=""{1}"" RegistrationEnd="""">
                                            <FirstName>Alex</FirstName>
                                            <LastName>Harbers</LastName>
                                            <OwnsHouse RegistrationId=""{2}"" RegistrationStart=""{3}"" RegistrationEnd="""">
                                                <From>2001-01-01</From>
                                                <To>2004-08-30</To>
                                                <House RegistrationId=""{4}"" RegistrationStart=""{5}"" RegistrationEnd="""">
                                                    <Address>Lindelaan 13</Address>
                                                    <ZipCode>1234 AB</ZipCode>
                                                    <City>Amsterdam</City>
                                                </House>
                                            </OwnsHouse>
                                            <OwnsHouse RegistrationId=""{6}"" RegistrationStart=""{7}"" RegistrationEnd="""">
                                                <From>2004-09-01</From>
                                                <To></To>
                                                <House RegistrationId=""{8}"" RegistrationStart=""{9}"" RegistrationEnd="""">
                                                    <Address>Patersstraat 13A</Address>
                                                    <ZipCode>6828 AB</ZipCode>
                                                    <City>Arnhem</City>
                                                </House>
                                            </OwnsHouse>
                                        </Person>
                                    </cs:CaseFile>", existingPersonObject.Id, existingPersonObjectValue.StartDate.ToString("O"), 
                                                     existingOwnsHouseObject.Id, existingOwnsHouseObjectValue.StartDate.ToString("O"),
                                                     existingHouseObject.Id, existingHouseObjectValue.StartDate.ToString("O"),
                                                     Guid.NewGuid(), DateTime.Now.ToString("O"),
                                                     Guid.NewGuid(), DateTime.Now.ToString("O"));
            CaseFile caseFile = caseFileService.Convert(caseFileXml, Encoding.Unicode);
            caseFile.CaseFileSpecification = myspecification;

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            caseFileService.Store(myspecification, "myobjectmodel/myspecification/Alex.Harbers", caseFile, new Uri("http://localhost:8080/its"), info);

            Assert.AreEqual("http://localhost:8080/its/specifications/casefiles/myobjectmodel/myspecification?version=2", caseFile.CaseFileSpecificationUri);
            Assert.AreEqual("http://localhost:8080/its/casefiles/myobjectmodel/myspecification/Alex.Harbers?version=10", caseFile.SelfUri);
            Assert.AreEqual(4, insertedObjects.Count);

            BaseObject lindeLaanObject = insertedObjects[0];
            Assert.AreEqual("House", lindeLaanObject.ExtId);
            Assert.AreEqual("myobjectmodel", lindeLaanObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Entity, lindeLaanObject.Type.Id);
            Assert.AreEqual(2, lindeLaanObject.Values.Count());

            IBaseObjectValue firstValue = lindeLaanObject.Values.First();
            Assert.IsNotNull(firstValue);
            Assert.IsNotNull(firstValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, firstValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, firstValue.Start);
            Assert.AreNotEqual(TimePoint.Future, firstValue.End);
            Assert.IsTrue(firstValue.Text.Contains("<Address>Typefout 13</Address>"));
            Assert.IsTrue(firstValue.Text.Contains("<ZipCode>1234 AB</ZipCode>"));
            Assert.IsTrue(firstValue.Text.Contains("<City>Amsterdam</City>"));

            IBaseObjectValue lastValue = lindeLaanObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<Address>Lindelaan 13</Address>"));
            Assert.IsTrue(lastValue.Text.Contains("<ZipCode>1234 AB</ZipCode>"));
            Assert.IsTrue(lastValue.Text.Contains("<City>Amsterdam</City>"));

            BaseObject patersStraatObject = insertedObjects[1];
            Assert.AreEqual("House", patersStraatObject.ExtId);
            Assert.AreEqual("myobjectmodel", patersStraatObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Entity, patersStraatObject.Type.Id);
            Assert.AreEqual(1, patersStraatObject.Values.Count());

            lastValue = patersStraatObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<Address>Patersstraat 13A</Address>"));
            Assert.IsTrue(lastValue.Text.Contains("<ZipCode>6828 AB</ZipCode>"));
            Assert.IsTrue(lastValue.Text.Contains("<City>Arnhem</City>"));

            BaseObject ownsHouse2Object = insertedObjects[2];
            Assert.AreEqual("OwnsHouse", ownsHouse2Object.ExtId);
            Assert.AreEqual("myobjectmodel", ownsHouse2Object.Reference.ExtId);
            Assert.AreEqual((int)ItemType.Relation, ownsHouse2Object.Type.Id);
            Assert.AreEqual(patersStraatObject, ownsHouse2Object.Relation2);
            Assert.AreEqual(1, ownsHouse2Object.Values.Count());

            lastValue = ownsHouse2Object.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.IsTrue(lastValue.Text.Contains("<From>2004-09-01</From>"));
            Assert.IsTrue(lastValue.Text.Contains("<To></To>"));

            BaseObject caseFileObject = insertedObjects[3];
            Assert.AreEqual("myobjectmodel/myspecification/Alex.Harbers", caseFileObject.ExtId);
            Assert.AreEqual("myobjectmodel/myspecification", caseFileObject.Reference.ExtId);
            Assert.AreEqual((int)ItemType.CaseFile, caseFileObject.Type.Id);
            Assert.AreEqual(specificationObject.Id, caseFileObject.Reference.Id);
            Assert.AreEqual(existingPersonObject.Id, caseFileObject.Relation1.Id);
            Assert.AreEqual(2, caseFileObject.Values.Count());

            lastValue = caseFileObject.Values.Last();
            Assert.IsNotNull(lastValue);
            Assert.IsNotNull(lastValue.Reference);
            Assert.AreEqual(specificationObjectValue.Id, lastValue.Reference.Id);
            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
            Assert.AreEqual(TimePoint.Future, lastValue.End);
            Assert.AreEqual(string.Empty, lastValue.Text);
        }

        [Test]
        public void TestGetXmlSchema()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);

            string result = caseFileService.GetXmlSchemaName();
            Assert.AreEqual("casefile.xsd", result);
        }

        [Test]
        public void TestGetXmlSchemaText()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            _mocks.ReplayAll();

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);

            string result = caseFileService.GetXmlSchemaText();
            Assert.IsTrue(result.Contains("<xs:schema"));
            Assert.IsTrue(result.Contains(@"http://luminis.net/its/schemas/casefile.xsd"));
        }

        [Test]
        public void TestGetCaseFiles()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();
            IObjectModelService objectModelService = _mocks.StrictMock<IObjectModelService>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();
            IRepresentationService representationService = _mocks.StrictMock<IRepresentationService>();

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            BaseObjectType caseFileType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                RelativeUri = "/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFile))).Return(caseFileType);

            CaseFileSpecification myspecification = new CaseFileSpecification()
            {
                Name = "myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        Id = Guid.NewGuid(),
                        ExtId = "myobjectmodel/myspecification",
                        Type = new BaseObjectType()
                        {
                            Id = (int)ItemType.CaseFileSpecification,
                            RelativeUri = "/specifications/casefiles/"
                        }
                    }
                }
            };
            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.Convert(myspecification.BaseObjectValue, new Uri("http://localhost:8080/its"))).Return(myspecification).Repeat.Times(3);

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(myspecification.BaseObjectValue.Id)).Return(myspecification.BaseObjectValue).Repeat.Times(3);

            int index = 0;
            List<BaseObject> caseFileValues = new List<BaseObject>()
            { 
                new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification/Alex.Harbers",
                    ReferenceId = myspecification.BaseObjectValue.Parent.Id,
                    Type = caseFileType,
                    BaseObjectValues = new EntityCollection<BaseObjectValue>()
                    {
                        new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ReferenceId = myspecification.BaseObjectValue.Id,
                            Version = ++index
                        }
                    }
                },
                new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification/Floris.Zwarteveen",
                    ReferenceId = myspecification.BaseObjectValue.Parent.Id,
                    Type = caseFileType,
                    BaseObjectValues = new EntityCollection<BaseObjectValue>()
                    {
                        new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ReferenceId = myspecification.BaseObjectValue.Id,
                            Version = ++index
                        }
                    }
                },
                new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myspecification/Rob.VanRooijen",
                    ReferenceId = myspecification.BaseObjectValue.Parent.Id,
                    Type = caseFileType,
                    BaseObjectValues = new EntityCollection<BaseObjectValue>()
                    {
                        new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ReferenceId = myspecification.BaseObjectValue.Id,
                            Version = ++index
                        }
                    }
                }
            };

            Expect.On(dataService).Call(dataService.GetBaseObjects("myobjectmodel/myspecification/", caseFileType)).Return(caseFileValues.Cast<IBaseObject>());

            _mocks.ReplayAll();

            _caseFileSpecificationServiceImpl = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ICaseFileService caseFileService = new CaseFileService(logger, container, caseFileSpecificationService, representationService, dataService);

            IEnumerable<CaseFile> casefiles = caseFileService.GetEnumerable(myspecification, new Uri("http://localhost:8080/its"));

            Assert.AreEqual(3, casefiles.Count());

            foreach (CaseFile casefile in casefiles)
            {
                logger.DebugFormat("CaseFile: {0}", casefile.SelfUri);
            }
        }
    }
}
