using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using TimeTraveller.Services.ObjectModels.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Logging.Log4Net;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.ObjectModels.Test
{
    /// <summary>
    /// Summary description for TestObjectModelService
    /// </summary>
    [TestFixture]
    public class TestObjectModelService
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
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);
            
            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);

            Assert.IsNotNull(objectModelService);
        }

        [Test]
        public void TestConvertObjectModel2XmlAndBack()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);

            Assert.IsNotNull(objectModelService);

            ObjectModel objectModel = new ObjectModel()
            {
                Name = "myobjectmodel",
                SelfUri = string.Empty,
                ObjectDefinitions = new ObjectDefinition[] {
                    new ObjectDefinition() {
                        Name = "Person",
                        ObjectType = ObjectType.entity,
                        Properties = new ObjectDefinitionProperty[] {
                            new ObjectDefinitionProperty()
                            {
                                Name = "FirstName",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            },
                            new ObjectDefinitionProperty()
                            {
                                Name = "LastName",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            }
                        },
                        ComplexProperties = new ObjectDefinitionComplexProperty[] {
                            new ObjectDefinitionComplexProperty() {
                                Name = "TelephoneNumbers",
                                MinOccurs = 0,
                                MaxOccurs = "1",
                                ComplexProperties = new ObjectDefinitionComplexProperty[] {
                                    new ObjectDefinitionComplexProperty() {
                                        Name = "TelephoneNumber",
                                        MinOccurs = 1,
                                        MaxOccurs = "unbounded",
                                        Properties = new ObjectDefinitionProperty[] {
                                            new ObjectDefinitionProperty()
                                            {
                                                Name = "NumberType",
                                                Type = "string",
                                                Required = true,
                                                RequiredSpecified = true
                                            },
                                            new ObjectDefinitionProperty()
                                            {
                                                Name = "Number",
                                                Type = "string",
                                                Required = true,
                                                RequiredSpecified = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ObjectDefinition() {
                        Name = "OwnsHouse",
                        ObjectType = ObjectType.relation
                    },
                    new ObjectDefinition() {
                        Name = "House",
                        ObjectType = ObjectType.entity,
                        Properties = new ObjectDefinitionProperty[] {
                            new ObjectDefinitionProperty()
                            {
                                Name = "Address",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            },
                            new ObjectDefinitionProperty()
                            {
                                Name = "ZipCode",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            },
                            new ObjectDefinitionProperty()
                            {
                                Name = "City",
                                Type = "string",
                                Required = true,
                                RequiredSpecified = true
                            }
                        }
                    }
                },
                ObjectRelations = new ObjectRelation[]
                {
                    new ObjectRelation()
                    {
                        Source = "Person",
                        Target = "OwnsHouse"
                    },
                    new ObjectRelation()
                    {
                        Source = "OwnsHouse",
                        Target = "House",
                        MinOccurs = 1,
                        MaxOccurs = "1"
                    }
                }
            };

            string xml = objectModelService.GetXml(objectModel, Encoding.Unicode);

            logger.DebugFormat("Xml=\r\n{0}", xml);

            Assert.IsTrue(!string.IsNullOrEmpty(xml));
            Assert.IsTrue(xml.Contains("<ObjectModel"));
            Assert.IsTrue(xml.Contains(@"<Link rel=""self"" href="""" />"));
            Assert.IsTrue(xml.Contains("<Name>myobjectmodel</Name>"));

            objectModel = objectModelService.Convert(xml, Encoding.Unicode);

            Assert.IsNotNull(objectModel);

            logger.Debug("After convert xml2objectmodel");
            logger.DebugFormat("Name={0}", objectModel.Name);

            Assert.AreEqual("myobjectmodel", objectModel.Name);
            foreach (ObjectDefinition definition in objectModel.ObjectDefinitions)
            {
                logger.DebugFormat("ObjectDefinition: {0}", definition.Name);
                if (definition.Properties != null)
                {
                    foreach (ObjectDefinitionProperty property in definition.Properties)
                    {
                        logger.DebugFormat("  Property: {0}, type={1}", property.Name, property.Type);
                    }
                }
                if (definition.ComplexProperties != null)
                {
                    foreach (ObjectDefinitionComplexProperty complexProperty in definition.ComplexProperties)
                    {
                        logger.DebugFormat("  ComplexProperty: {0}", complexProperty.Name);
                    }
                }
                if (definition.EntityRelations != null)
                {
                    foreach (ObjectRelation relation in definition.EntityRelations)
                    {
                        logger.DebugFormat("  Relation: source={0}, target={1}", relation.SourceObjectDefinition.Name, relation.TargetObjectDefinition.Name);
                    }
                }
            }
            foreach (ObjectRelation relation in objectModel.ObjectRelations)
            {
                logger.DebugFormat("ObjectRelation: source={0}, target={1}", relation.SourceObjectDefinition.Name, relation.TargetObjectDefinition.Name);
            }
        }

        private static void validatingReader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            Assert.IsTrue(false, e.Message);
        }

        [Test]
        public void TestGetObjectModelXmlSchema()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);

            Assert.IsNotNull(objectModelService);

            string objectModelXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                    <ObjectModel xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://timetraveller.net/storage/schemas/objectmodel.xsd"">
                                      <Link rel=""self"" href="""" />
                                      <Name>myobjectmodel</Name>
                                      <ObjectDefinitions>
                                        <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                          <Properties>
                                            <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                            <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                          </Properties>
                                          <ComplexProperties>
                                            <ComplexProperty Name=""TelephoneNumbers"" MinOccurs=""0"" MaxOccurs=""1"">
                                              <ComplexProperties>
                                                <ComplexProperty Name=""TelephoneNumber"" MinOccurs=""1"" MaxOccurs=""unbounded"">
                                                  <Properties>
                                                    <Property Name=""NumberType"" Type=""string"" Required=""true"" />
                                                    <Property Name=""Number"" Type=""string"" Required=""true"" />
                                                  </Properties>
                                                </ComplexProperty>
                                              </ComplexProperties>
                                            </ComplexProperty>
                                          </ComplexProperties>
                                        </ObjectDefinition>
                                        <ObjectDefinition Name=""OwnsHouse"" ObjectType=""relation"" />
                                        <ObjectDefinition Name=""House"" ObjectType=""entity"">
                                          <Properties>
                                            <Property Name=""Address"" Type=""string"" Required=""true"" />
                                            <Property Name=""ZipCode"" Type=""string"" Required=""true"" />
                                            <Property Name=""City"" Type=""string"" Required=""true"" />
                                          </Properties>
                                        </ObjectDefinition>
                                      </ObjectDefinitions>
                                      <ObjectRelations>
                                        <ObjectRelation Source=""Person"" Target=""OwnsHouse"" MinOccurs=""0"" MaxOccurs=""unbounded"" />
                                        <ObjectRelation Source=""OwnsHouse"" Target=""House"" MinOccurs=""1"" MaxOccurs=""1"" />
                                      </ObjectRelations>
                                    </ObjectModel>";

            ObjectModel objectModel = objectModelService.Convert(objectModelXml, Encoding.Unicode);

            string xmlSchemaText = objectModelService.GetXmlSchema(objectModel, Encoding.Unicode);

            Assert.IsTrue(!string.IsNullOrEmpty(xmlSchemaText));

            logger.DebugFormat("Xml=\r\n{0}", xmlSchemaText.Replace("><", ">\r\n<"));

            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlSchemaText));
            XmlSchema xmlSchema = XmlSchema.Read(xmlReader, new ValidationEventHandler(validatingReader_ValidationEventHandler));

            Assert.IsNotNull(xmlSchema);
        }

        [Test]
        public void TestGetObjectModel()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            BaseObject objectModelObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel",
                Type = objectModelType
            };
            BaseObjectValue value = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = objectModelObject,
                Text = @"<ObjectModel xmlns=""http://timetraveller.net/storage/schemas/objectmodel.xsd"">
                               <Link rel=""self"" href=""""/>
                               <Name>myobjectmodel</Name>
                               <ObjectDefinitions>
                                 <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                     <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                   </Properties>
                                 </ObjectDefinition>
                               </ObjectDefinitions>
                            </ObjectModel>"
            };

            Expect.On(dataService).Call(dataService.GetValue("myobjectmodel", null, TimePoint.Past)).IgnoreArguments().Return(value);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ObjectModel objectModel = objectModelService.Get("/specifications/objectmodels/myobjectmodel", new Uri("http://localhost:8080"));

            Assert.IsNotNull(objectModel);
            Assert.AreEqual("myobjectmodel", objectModel.Name);
            Assert.AreEqual("http://localhost:8080//specifications/objectmodels/myobjectmodel?version=0", objectModel.SelfUri);
        }

        private delegate IBaseObjectValue InsertValueDelegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IUserInfo journalInfo);
        private delegate void SaveChangesDelegate();

        [Test]
        public void TestStoreNewObjectModel()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel", null)).IgnoreArguments().Return(null);

            BaseObject insertedObject = null;
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, null as IBaseObjectType, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValueDelegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo)
                    {
                        insertedObject = new BaseObject()
                        {
                            Id = id,
                            ExtId = extId,
                            Type = type
                        };

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            Parent = insertedObject,
                            Start = timeStamp,
                            Text = content
                        };

                        return newValue;
                    }
                )
            );

            dataService.SaveChanges();
            LastCall.On(dataService).Do(
                new SaveChangesDelegate(
                    delegate()
                    {
                        Assert.IsNotNull(insertedObject);
                        Assert.AreNotEqual(Guid.Empty, insertedObject.Id);
                        Assert.AreEqual("myobjectmodel", insertedObject.ExtId);
                        Assert.IsNull(insertedObject.Reference);
                        Assert.AreEqual(1, insertedObject.Values.Count());
                        IBaseObjectValue value = insertedObject.Values.Last();
                        Assert.IsNotNull(value);
                        Assert.IsNull(value.Reference);
                        Assert.AreEqual(TimePoint.Future, value.End);

                        logger.DebugFormat("Content:\r\n{0}", value.Text);
                        Assert.IsTrue(value.Text.Contains(@"xmlns=""http://timetraveller.net/storage/schemas/objectmodel.xsd"""));
                        Assert.IsTrue(value.Text.Contains(@"<Name>myobjectmodel</Name>"));
                    }
                )
            );

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ObjectModel objectModel = new ObjectModel()
            {
                BaseObjectValue = new BaseObjectValue()
                {
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel",
                        Type = new BaseObjectType()
                        {
                            Id = (int)ItemType.ObjectModel
                        }
                    },
                },
                Name = "myobjectmodel",
                SelfUri = string.Empty
            };

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            objectModelService.Store(objectModel.Name, objectModel, new Uri("http://localhost:8080"), info);
        }

        [Test]
        public void TestGetXmlSchemas()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, unity, dataService);

            string result = objectModelService.GetXmlSchemaName();
            Assert.AreEqual("objectmodel.xsd", result);
        }

        [Test]
        public void TestGetXmlSchema()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, unity, dataService);

            string result = objectModelService.GetXmlSchemaText();
            Assert.IsTrue(result.Contains("<xs:schema"));
            Assert.IsTrue(result.Contains(@"http://timetraveller.net/storage/schemas/objectmodel.xsd"));
        }

        [Test]
        public void TestGetSummary()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType objectModelType = new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                RelativeUri = "/specifications/objectmodels/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.ObjectModel))).Return(objectModelType);

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                Name = "casefilespecification",
                RelativeUri = "/specifications/casefiles/"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(specificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, unity, dataService);

            Guid baseObjectvalueGuid = Guid.NewGuid();
            ObjectModel objectModel = new ObjectModel()
            {
                Name = "myobjectmodel",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = baseObjectvalueGuid,
                    ParentBaseObject = new BaseObject()
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
                    },
                    Version = 3
                }
            };

            string result = objectModelService.GetSummary(objectModel, new Uri("http://localhost:8080/storage"), Encoding.Unicode);

            logger.Debug(result);

            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains(@"<Summary>"));
            Assert.IsTrue(result.Contains(@"<Name>myobjectmodel</Name>"));
            Assert.IsTrue(result.Contains(@"<Link rel=""latest"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel"" />"));
            Assert.IsTrue(result.Contains(@"<Link rel=""current"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=3"" />"));
            Assert.IsTrue(result.Contains(@"<Link rel=""version"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
            Assert.IsTrue(result.Contains(@"<Link rel=""version"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
            Assert.IsTrue(result.Contains(@"<Link rel=""summary"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?summary=true"" />"));
            Assert.IsTrue(result.Contains(@"<Link rel=""history"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?history=true"" />"));
            Assert.IsTrue(result.Contains(@"<Link rel=""casefilespecification"" href=""http://localhost:8080/storage/specifications/casefiles/myobjectmodel/"" />"));
            Assert.IsTrue(result.Contains(@"</Summary>"));
        }

        [Test]
        public void TestGetHistory()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            IUnity unity = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

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

            int index = 0;
            List<IJournalEntry> journalEntries = new List<IJournalEntry>()
            {
                new BaseObjectJournal()
                {
                    Id = ++index,
                    Timestamp = DateTime.Parse("2009-06-25T09:21:28.000"),
                    Username = "user1@gmail.com",
                    After = objectModelBaseObjectValues[0]
                },
                new BaseObjectJournal()
                {
                    Id = ++index,
                    Timestamp = DateTime.Parse("2009-06-25T09:21:42.000"),
                    Username = "user2@gmail.com",
                    Before = objectModelBaseObjectValues[0],
                    After = objectModelBaseObjectValues[1]
                },
                new BaseObjectJournal()
                {
                    Id = ++index,
                    Timestamp = DateTime.Parse("2009-06-25T09:22:00.000"),
                    Username = "user3@gmail.com",
                    Before = objectModelBaseObjectValues[1],
                    After = objectModelBaseObjectValues[2]
                }
            };

            Expect.On(dataService).Call(dataService.GetJournal(objectModelBaseObjectValue.Parent, new TimePointRange(TimePoint.Past, TimePoint.Future))).Return(journalEntries);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, unity, dataService);

            ObjectModel objectModel = new ObjectModel()
            {
                Name = "myobjectmodel",
                BaseObjectValue = objectModelBaseObjectValue
            };

            string result = objectModelService.GetHistory(objectModel, new Uri("http://localhost:8080/storage"), Encoding.Unicode);

            logger.Debug(result);

            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains(@"<History>"));
            Assert.IsTrue(result.Contains(@"<Name>myobjectmodel</Name>"));
            Assert.IsTrue(result.Contains(@"<Journal>"));
            Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=1"" />"));
            Assert.IsTrue(result.Contains(@"<Timestamp>06-25-2009 09:21 28</Timestamp>"));
            Assert.IsTrue(result.Contains(@"<Username>user1@gmail.com</Username>"));
            Assert.IsTrue(result.Contains(@"</Journal>"));
            Assert.IsTrue(result.Contains(@"<Journal>"));
            Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=2"" />"));
            Assert.IsTrue(result.Contains(@"<Timestamp>06-25-2009 09:21 42</Timestamp>"));
            Assert.IsTrue(result.Contains(@"<Username>user2@gmail.com</Username>"));
            Assert.IsTrue(result.Contains(@"</Journal>"));
            Assert.IsTrue(result.Contains(@"<Journal>"));
            Assert.IsTrue(result.Contains(@"<Link rel=""objectmodel"" href=""http://localhost:8080/storage/specifications/objectmodels/myobjectmodel?version=3"" />"));
            Assert.IsTrue(result.Contains(@"<Timestamp>06-25-2009 09:22 00</Timestamp>"));
            Assert.IsTrue(result.Contains(@"<Username>user3@gmail.com</Username>"));
            Assert.IsTrue(result.Contains(@"</Journal>"));
            Assert.IsTrue(result.Contains(@"</History>"));
        }
    }
}
