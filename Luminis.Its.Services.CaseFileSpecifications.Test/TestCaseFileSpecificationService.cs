using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using Luminis.Its.Services.Data;
using Luminis.Its.Services.Data.Impl;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.CaseFileSpecifications.Impl;
using Luminis.Its.Services.ObjectModels;
using Luminis.Its.Services.ObjectModels.Impl;
using Luminis.Logging;
using Luminis.Logging.Log4Net;
using Luminis.Patterns.Range;
using Luminis.Unity;
using Luminis.Xml;

namespace Luminis.Its.Services.CaseFileSpecifications.Test
{
    /// <summary>
    /// Summary description for TestCaseFileSpecificationService
    /// </summary>
    [TestFixture]
    public class TestCaseFileSpecificationService
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

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(specificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            Assert.IsNotNull(specificationService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConvertXml2CaseFileSpecificationWithMissingName()
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

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(specificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            Assert.IsNotNull(specificationService);

            string xml = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                                <Link rel=""objectmodel"" href=""""/>
                                <Link rel=""self"" href=""""/>
                                <UriTemplate>{/Person/Name}</UriTemplate>
                            </CaseFileSpecification>";

            CaseFileSpecification specification = specificationService.Convert(xml, Encoding.Unicode);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConvertXml2CaseFileSpecificationWithEmptyName()
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

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(specificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            Assert.IsNotNull(specificationService);

            string xml = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                                <Link rel=""objectmodel"" href=""""/>
                                <Link rel=""self"" href=""""/>
                                <Name></Name>
                                <UriTemplate>{/Person/Name}</UriTemplate>
                                <Structure>
                                    <Entity Name=""Person"" Type=""Person""/>
                                </Structure>
                            </CaseFileSpecification>";

            CaseFileSpecification specification = specificationService.Convert(xml, Encoding.Unicode);
        }

        [Test]
        public void TestConvertCaseFileSpecification2XmlAndBack()
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

            BaseObjectType specificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(specificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            Assert.IsNotNull(specificationService);

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModelUri = string.Empty,
                UriTemplate = "{/Person/Name}",
                SelfUri = string.Empty,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[] {
                            new CaseFileSpecificationRelation() {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity() {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                }
            };

            string xml = specificationService.GetXml(specification, Encoding.Unicode);
            logger.DebugFormat("xml={0}", xml);

            CaseFileSpecification result = specificationService.Convert(xml, Encoding.Unicode);

            Assert.IsNotNull(result);
            Assert.AreEqual("myspecification", result.Name);
            Assert.AreEqual(string.Empty, result.ObjectModelUri);
            Assert.AreEqual(string.Empty, result.SelfUri);
            Assert.AreEqual("{/Person/Name}", result.UriTemplate);
        }

        [Test]
        public void TestGetCaseFileSpecification()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

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
                Text = @"<ObjectModel xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
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
            BaseObject specificationObject = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = caseFileSpecificationType
            };
            BaseObjectValue specificationObjectValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = specificationObject,
                Reference = objectModelObjectValue,
                ReferenceId = objectModelObjectValue.Id,
                Text = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                               <Link rel=""objectmodel"" href=""""/>
                               <Link rel=""self"" href=""""/>
                               <Name>myspecification</Name>
                               <UriTemplate>{/Person/Name}</UriTemplate>
                               <Structure>
                                   <Entity Name=""Person"" Type=""Person""/>
                               </Structure>
                           </CaseFileSpecification>"
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            Expect.On(dataService).Call(dataService.GetValue(specificationObject.ExtId, caseFileSpecificationType, TimePoint.Past)).IgnoreArguments().Return(specificationObjectValue);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);
            CaseFileSpecification specification = specificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080"));

            Assert.IsNotNull(specification);
            Assert.AreEqual("myspecification", specification.Name);
            Assert.AreEqual("myobjectmodel/myspecification", specification.ExtId);
            Assert.AreEqual("{/Person/Name}", specification.UriTemplate);
            Assert.AreEqual("myobjectmodel", specification.ObjectModel.ExtId);
            Assert.AreEqual("myobjectmodel", specification.ObjectModel.Name);
        }

        private delegate IBaseObjectValue GetValueDelegate(string key);

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetUnknownCaseFileSpecification()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            Expect.On(dataService).Call(dataService.GetValue("myobjectmodel/myspecification", caseFileSpecificationType, TimePoint.Past)).IgnoreArguments().Return(null);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);
            CaseFileSpecification specification = specificationService.Get("myobjectmodel/myspecification", new Uri("http://localhost:8080/its"));
        }

        private static void validatingReader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            Assert.IsTrue(false, e.Message);
        }

        [Test]
        public void TestGetCaseFileSpecificationXmlSchema()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            Assert.IsNotNull(specificationService);
            string objectModelXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                    <ObjectModel xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
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

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModel = objectModel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[] {
                            new CaseFileSpecificationRelation() {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity() {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                },
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            string xmlSchemaText = specificationService.GetXmlSchema(specification, Encoding.Unicode);

            Assert.IsTrue(!string.IsNullOrEmpty(xmlSchemaText));

            logger.DebugFormat("Xml=\r\n{0}", xmlSchemaText.Replace("><", ">\r\n<"));

            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlSchemaText));
            XmlSchema xmlSchema = XmlSchema.Read(xmlReader, new ValidationEventHandler(validatingReader_ValidationEventHandler));

            Assert.IsNotNull(xmlSchema);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(xmlSchema);
            schemaSet.Compile();

            Assert.IsTrue(xmlSchema.IsCompiled);
        }

        [Test]
        public void TestGetCaseFileSpecifications()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            List<IBaseObject> specificationObjects = new List<IBaseObject>()
            {
                new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/myfirstspecification",
                    Type = caseFileSpecificationType,
                    BaseObjectValues = new EntityCollection<BaseObjectValue>()
                    {
                        new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            Text = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                                               <Link rel=""objectmodel"" href=""""/>
                                               <Link rel=""self"" href=""""/>
                                               <Name>myfirstspecification</Name>
                                               <UriTemplate>{/Person/Name}</UriTemplate>
                                               <Structure>
                                                   <Entity Name=""Person"" Type=""Person""/>
                                               </Structure>
                                           </CaseFileSpecification>"
                        }
                    }
                },
                new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "myobjectmodel/mysecondspecification",
                    Type = caseFileSpecificationType,
                    BaseObjectValues = new EntityCollection<BaseObjectValue>()
                    {
                        new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            Text = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                                               <Link rel=""objectmodel"" href=""""/>
                                               <Link rel=""self"" href=""""/>
                                               <Name>mysecondspecification</Name>
                                               <UriTemplate>{/Person/FirstName}.{/Person/LastName}</UriTemplate>
                                               <Structure>
                                                   <Entity Name=""Person"" Type=""Person""/>
                                               </Structure>
                                           </CaseFileSpecification>"
                        }
                    }
                }
            };
            Expect.On(dataService).Call(dataService.GetBaseObjects("myobjectmodel/", caseFileSpecificationType)).Return(specificationObjects);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);
            IEnumerable<CaseFileSpecification> specifications = specificationService.GetEnumerable("myobjectmodel", new Uri("http://localhost:8080/its"));

            Assert.AreEqual(2, specifications.Count());

            foreach (CaseFileSpecification specification in specifications)
            {
                logger.DebugFormat("CaseFileSpecification: {0}", specification.ExtId);
            }
        }

        private delegate IBaseObjectValue InsertValue1Delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo info);
        private delegate void SaveChangesDelegate();

        [Test]
        public void TestStoreNewCaseFileSpecification()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

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
                Text = @"<ObjectModel xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
                               <Link rel=""self"" href=""""/>
                               <Name>myobjectmodel</Name>
                               <ObjectDefinitions>
                                 <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""FirstName"" Type=""string"" Required=""true""/>
                                     <Property Name=""LastName"" Type=""string"" Required=""true""/>
                                   </Properties>
                                 </ObjectDefinition>
                               </ObjectDefinitions>
                           </ObjectModel>"
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel/myspecification", caseFileSpecificationType)).IgnoreArguments().Return(null);

            BaseObject insertedObject = null;
            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, Guid.Empty, string.Empty, caseFileSpecificationType, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValue1Delegate(
                    delegate(string content, TimePoint timeStamp, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo info)
                    {
                        insertedObject = new BaseObject()
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
                        Assert.AreEqual(objectModelObject.Id, insertedObject.Reference.Id);
                        Assert.AreEqual("myobjectmodel/myspecification", insertedObject.ExtId);
                        Assert.AreEqual(1, insertedObject.Values.Count());
                        IBaseObjectValue value = insertedObject.Values.Last();
                        Assert.IsNotNull(value);
                        Assert.AreEqual(objectModelObjectValue.Id, value.Reference.Id);
                        Assert.AreNotEqual(TimePoint.Past, value.Start);
                        Assert.AreEqual(TimePoint.Future, value.End);

                        logger.DebugFormat("Content:\r\n{0}", value.Text);
                        Assert.IsTrue(value.Text.Contains(@"xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"""));
                        Assert.IsTrue(value.Text.Contains(@"<Name>myspecification</Name>"));
                        Assert.IsTrue(value.Text.Contains(@"<UriTemplate>{/Person/Name}</UriTemplate>"));

                        value.Id = Guid.NewGuid();
                    }
                )
            );

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ObjectModel objectmodel = objectModelService.Convert(objectModelObjectValue.Text, Encoding.Unicode);
            objectmodel.BaseObjectValue = objectModelObjectValue;

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModel = objectmodel,
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
                UriTemplate = "{/Person/Name}"
            };

            WebHttpHeaderInfo headerInfo = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            specificationService.Store(string.Format("myobjectmodel/{0}", specification.Name), specification, new Uri("http://localhost:8080/its"), headerInfo);
        }

        private delegate IBaseObjectValue InsertValue2Delegate(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo info);

        [Test]
        public void TestStoreExistingCaseFileSpecification()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

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
                Text = @"<ObjectModel xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
                               <Link rel=""self"" href=""""/>
                               <Name>myobjectmodel</Name>
                               <ObjectDefinitions>
                                 <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""FirstName"" Type=""string"" Required=""true""/>
                                     <Property Name=""LastName"" Type=""string"" Required=""true""/>
                                   </Properties>
                                 </ObjectDefinition>
                               </ObjectDefinitions>
                           </ObjectModel>"
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            BaseObject existingSpecificiation = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = caseFileSpecificationType,
                Reference = objectModelObject
            };
            BaseObjectValue existingValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingSpecificiation,
                Reference = objectModelObjectValue,
                Text = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                               <Link rel=""objectmodel"" href=""""/>
                               <Link rel=""self"" href=""""/>
                               <Name>myspecification</Name>
                               <UriTemplate>{/Person/Name}</UriTemplate>
                               <Structure>
                                 <Entity Name=""Person"" Type=""Person""/>
                               </Structure>
                           </CaseFileSpecification>"
            };

            Expect.On(dataService).Call(dataService.GetBaseObject("myobjectmodel/myspecification", caseFileSpecificationType)).Return(existingSpecificiation);

            Expect.On(dataService).Call(dataService.InsertValue(string.Empty, TimePoint.Past, null as IBaseObject, null as IBaseObjectValue, null as WebHttpHeaderInfo)).IgnoreArguments().Do(
                new InsertValue2Delegate(
                    delegate(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo info)
                    {
                        baseObject.Values.Last().End = timePoint.AddSeconds(-1);

                        BaseObjectValue newValue = new BaseObjectValue()
                        {
                            Start = timePoint,
                            Parent = baseObject,
                            Reference = referenceObjectValue,
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
                        try
                        {
                            Assert.AreEqual("myobjectmodel/myspecification", existingSpecificiation.ExtId);
                            Assert.AreEqual(2, existingSpecificiation.Values.Count());

                            IBaseObjectValue firstValue = existingSpecificiation.Values.First();
                            Assert.IsNotNull(firstValue);
                            logger.DebugFormat("Content firstValue:\r\n{0}", firstValue.Text);
                            Assert.AreNotEqual(TimePoint.Future, firstValue.End);
                            Assert.IsTrue(firstValue.Text.Contains(@"xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"""));
                            Assert.IsTrue(firstValue.Text.Contains(@"<Name>myspecification</Name>"));
                            Assert.IsTrue(firstValue.Text.Contains(@"<UriTemplate>{/Person/Name}</UriTemplate>"));

                            IBaseObjectValue lastValue = existingSpecificiation.Values.Last();
                            Assert.IsNotNull(lastValue);
                            Assert.AreEqual(objectModelObjectValue.Id, lastValue.Reference.Id);
                            Assert.AreNotEqual(TimePoint.Past, lastValue.Start);
                            Assert.AreEqual(TimePoint.Future, lastValue.End);

                            logger.DebugFormat("Content lastValue:\r\n{0}", lastValue.Text);
                            Assert.IsTrue(lastValue.Text.Contains(@"xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"""));
                            Assert.IsTrue(lastValue.Text.Contains(@"<Name>myspecification</Name>"));
                            Assert.IsTrue(lastValue.Text.Contains(@"<UriTemplate>{/Person/FirstName}/{/Person/LastName}</UriTemplate>"));
                        }
                        catch (Exception exception)
                        {
                            logger.Error("Exception in SaveChangesDelegate()", exception);
                            throw;
                        }
                    }
                )
            );

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ObjectModel objectmodel = objectModelService.Convert(objectModelObjectValue.Text, Encoding.Unicode);
            objectmodel.BaseObjectValue = objectModelObjectValue;

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModel = objectmodel,
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
                UriTemplate = "{/Person/FirstName}/{/Person/LastName}"
            };

            WebHttpHeaderInfo headerInfo = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            specificationService.Store(string.Format("myobjectmodel/{0}", specification.Name), specification, new Uri("http://localhost:8080/its"), headerInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStoreCaseFileSpecificationWithUnknownEntity()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

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
                Text = @"<ObjectModel xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
                               <Link rel=""self"" href=""""/>
                               <Name>myobjectmodel</Name>
                               <ObjectDefinitions>
                                 <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""FirstName"" Type=""string"" Required=""true""/>
                                     <Property Name=""LastName"" Type=""string"" Required=""true""/>
                                   </Properties>
                                 </ObjectDefinition>
                                 <ObjectDefinition Name=""OwnsHouse"" ObjectType=""relation"">
                                   <Properties>
                                     <Property Name=""From"" Type=""date"" Required=""true""/>
                                     <Property Name=""To"" Type=""date""/>
                                   </Properties>
                                 </ObjectDefinition>
                                 <ObjectDefinition Name=""House"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""Address"" Type=""string"" Required=""true""/>
                                     <Property Name=""ZipCode"" Type=""string"" Required=""true""/>
                                     <Property Name=""City"" Type=""string"" Required=""true""/>
                                   </Properties>
                                 </ObjectDefinition>
                               </ObjectDefinitions>
                               <ObjectRelations>
                                 <ObjectRelation Source=""Person"" Target=""OwnsHouse"" MinOccurs=""0"" MaxOccurs=""unbounded"" />
                                 <ObjectRelation Source=""OwnsHouse"" Target=""House"" MinOccurs=""1"" MaxOccurs=""1""/>
                               </ObjectRelations>
                           </ObjectModel>"
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            BaseObject existingSpecificiation = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = caseFileSpecificationType,
                Reference = objectModelObject
            };
            BaseObjectValue existingValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingSpecificiation,
                Reference = objectModelObjectValue,
                Text = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                               <Link rel=""objectmodel"" href=""""/>
                               <Link rel=""self"" href=""""/>
                               <Name>myspecification</Name>
                               <UriTemplate>{/Person/Name}</UriTemplate>
                               <Structure>
                                 <Entity Name=""Person"" Type=""Person""/>
                               </Structure>
                           </CaseFileSpecification>"
            };

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ObjectModel objectmodel = objectModelService.Convert(objectModelObjectValue.Text, Encoding.Unicode);
            objectmodel.BaseObjectValue = objectModelObjectValue;

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModel = objectmodel,
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[] {
                            new CaseFileSpecificationRelation() {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity() {
                                    Name = "UnknownEntity",
                                    Type = "UnknownEntity"
                                }
                            }
                        }
                    }
                },
                UriTemplate = "{/Person/FirstName}/{/Person/LastName}"
            };

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            specificationService.Store(string.Format("myobjectmodel/{0}", specification.Name), specification, new Uri("http://localhost:8080/its"), info);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStoreCaseFileSpecificationWithInvalidRelation()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

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
                Text = @"<ObjectModel xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
                               <Link rel=""self"" href=""""/>
                               <Name>myobjectmodel</Name>
                               <ObjectDefinitions>
                                 <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""FirstName"" Type=""string"" Required=""true""/>
                                     <Property Name=""LastName"" Type=""string"" Required=""true""/>
                                   </Properties>
                                 </ObjectDefinition>
                                 <ObjectDefinition Name=""OwnsHouse"" ObjectType=""relation"">
                                   <Properties>
                                     <Property Name=""From"" Type=""date"" Required=""true""/>
                                     <Property Name=""To"" Type=""date""/>
                                   </Properties>
                                 </ObjectDefinition>
                                 <ObjectDefinition Name=""House"" ObjectType=""entity"">
                                   <Properties>
                                     <Property Name=""Address"" Type=""string"" Required=""true""/>
                                     <Property Name=""ZipCode"" Type=""string"" Required=""true""/>
                                     <Property Name=""City"" Type=""string"" Required=""true""/>
                                   </Properties>
                                 </ObjectDefinition>
                               </ObjectDefinitions>
                               <ObjectRelations>
                                 <!-- ObjectRelation Source=""Person"" Target=""OwnsHouse"" MinOccurs=""0"" MaxOccurs=""unbounded"" -->
                                 <ObjectRelation Source=""OwnsHouse"" Target=""House"" MinOccurs=""1"" MaxOccurs=""1""/>
                               </ObjectRelations>
                           </ObjectModel>"
            };
            Expect.On(dataService).Call(dataService.GetBaseObjectValue(objectModelObjectValue.Id)).Return(objectModelObjectValue);

            BaseObject existingSpecificiation = new BaseObject()
            {
                Id = Guid.NewGuid(),
                ExtId = "myobjectmodel/myspecification",
                BaseObjectType = caseFileSpecificationType,
                Reference = objectModelObject
            };
            BaseObjectValue existingValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = existingSpecificiation,
                Reference = objectModelObjectValue,
                Text = @"<CaseFileSpecification xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
                               <Link rel=""objectmodel"" href=""""/>
                               <Link rel=""self"" href=""""/>
                               <Name>myspecification</Name>
                               <UriTemplate>{/Person/Name}</UriTemplate>
                               <Structure>
                                 <Entity Name=""Person"" Type=""Person""/>
                               </Structure>
                           </CaseFileSpecification>"
            };

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ObjectModel objectmodel = objectModelService.Convert(objectModelObjectValue.Text, Encoding.Unicode);
            objectmodel.BaseObjectValue = objectModelObjectValue;

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModel = objectmodel,
                ObjectModelUri = string.Empty,
                SelfUri = string.Empty,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[] {
                            new CaseFileSpecificationRelation() {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity() {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                },
                UriTemplate = "{/Person/FirstName}/{/Person/LastName}"
            };

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            specificationService.Store(string.Format("myobjectmodel/{0}", specification.Name), specification, new Uri("http://localhost:8080/its"), info);
        }

        [Test]
        public void TestValidateCaseFileId()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification"
                    }
                },
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel",
                },
                UriTemplate = "{/Person/FirstName}.ThisIsFixedText.{/Person/LastName}"
            };

            specificationService.ValidateCaseFileId(specification, "myobjectmodel/myspecification/Alex.ThisIsFixedText.Harbers");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestValidateCaseFileIdWithInvalidFormat()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification"
                    }
                },
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel",
                },
                UriTemplate = "{/Person/FirstName}.ThisIsFixedText.{/Person/LastName}"
            };

            specificationService.ValidateCaseFileId(specification, "myobjectmodel/myspecification/Alex.ThisIsNotCorrect.Harbers");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestValidateCaseFileIdWithMissingLastName()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification"
                    }
                },
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel",
                },
                UriTemplate = "{/Person/FirstName}.ThisIsFixedText.{/Person/LastName}"
            };

            specificationService.ValidateCaseFileId(specification, "myobjectmodel/myspecification/Alex.ThisIsFixedText");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestValidateCaseFileIdWithMissingFirstName()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification"
                    }
                },
                ObjectModel = new ObjectModel()
                {
                    Name = "myobjectmodel",
                },
                UriTemplate = "{/Person/FirstName}.ThisIsFixedText.{/Person/LastName}"
            };

            specificationService.ValidateCaseFileId(specification, "myobjectmodel/myspecification/.ThisIsFixedText.Harbers");
        }

        [Test]
        public void TestValidateCaseFileIdWithCaseFileXml()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ObjectModel objectmodel = new ObjectModel()
            {
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
                }
            };

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification"
                    }
                },
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{/Person/FirstName}.ThisIsFixedText.{/Person/LastName}"
            };

            specificationService.ValidateCaseFile(specification,
                "myobjectmodel/myspecification/Alex.ThisIsFixedText.Harbers",
                @"<Person RegistrationId=""1234"" RegistrationStart=""2009-01-01T23:59:59"">
                      <FirstName>Alex</FirstName>
                      <LastName>Harbers</LastName>
                  </Person>");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestValidateCaseFileIdWithInvalidXml()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            ObjectModel objectmodel = new ObjectModel()
            {
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
                }
            };

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myobjectmodel/myspecification",
                BaseObjectValue = new BaseObjectValue()
                {
                    Id = Guid.NewGuid(),
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "myobjectmodel/myspecification"
                    }
                },
                ObjectModel = objectmodel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person"
                    }
                },
                UriTemplate = "{/Person/FirstName}.ThisIsFixedText.{/Person/LastName}"
            };

            specificationService.ValidateCaseFile(specification,
                "myobjectmodel/myspecification/Alex.ThisIsFixedText.Harbers",
                @"<Person RegistrationId=""1234"" RegistrationStart=""2009-01-01T23:59:59"">
                     <FirstName>Alex</FirstName><LastName>ThisIsNotRight</LastName>
                  </Person>");
        }

        [Test]
        public void TestGetAssembly()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            Assert.IsNotNull(specificationService);

            string objectModelXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                    <ObjectModel xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
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

            CaseFileSpecification specification = new CaseFileSpecification()
            {
                Name = "myspecification",
                ObjectModel = objectModel,
                Structure = new CaseFileSpecificationStructure()
                {
                    Entity = new CaseFileSpecificationEntity()
                    {
                        Name = "Person",
                        Type = "Person",
                        Relation = new CaseFileSpecificationRelation[] {
                            new CaseFileSpecificationRelation() {
                                Name = "OwnsHouse",
                                Type = "OwnsHouse",
                                Entity = new CaseFileSpecificationEntity() {
                                    Name = "House",
                                    Type = "House"
                                }
                            }
                        }
                    }
                },
                UriTemplate = "{/Person/FirstName}.{/Person/LastName}"
            };

            byte[] assemblyBytes = specificationService.GetAssemblyBytes(specification);

            Assert.AreEqual(6656, assemblyBytes.Length);

            Assembly myspecificationAssembly = Assembly.Load(assemblyBytes);

            Assert.IsNotNull(myspecificationAssembly);

            logger.DebugFormat("Assembly: {0} ({1} bytes)", myspecificationAssembly.GetName().Name, assemblyBytes.Length);

            string rootEntityClassname = string.Format("{0}.{1}.{2}", objectModel.Name, specification.Name, specification.Structure.Entity.Name);
            Type rootEntityType = myspecificationAssembly.GetType(rootEntityClassname);

            logger.DebugFormat("RootEntity: {0}", rootEntityType.FullName);

            Assert.AreEqual(rootEntityClassname, rootEntityType.FullName);

            string caseFileXml = string.Format(@"<Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                            <FirstName>Alex</FirstName>
                                            <LastName>Harbers</LastName>
                                        </Person>", Guid.NewGuid(), DateTime.Now.ToString("O"));

            object caseFileObject = XmlHelper.FromXml(caseFileXml, rootEntityType);
            logger.DebugFormat("CaseFileObject: {0}", caseFileObject);
        }

        [Test]
        public void TestGetXmlSchemas()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            string result = specificationService.GetXmlSchemaName();
            Assert.AreEqual("casefilespecification.xsd", result);
        }

        [Test]
        public void TestGetXmlSchema()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            string result = specificationService.GetXmlSchemaText();
            Assert.IsTrue(result.Contains("<xs:schema"));
            Assert.IsTrue(result.Contains(@"http://luminis.net/its/schemas/casefilespecification.xsd"));
        }

        [Test]
        public void TestGetXmlSchemaForAutohistorie()
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

            BaseObjectType caseFileSpecificationType = new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.CaseFileSpecification))).Return(caseFileSpecificationType);

            _mocks.ReplayAll();

            string objectModelXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<ObjectModel xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://luminis.net/its/schemas/objectmodel.xsd"">
  <Link rel=""self"" href="""" />
  <Name>Automobile</Name>
  <ObjectDefinitions>
    <ObjectDefinition Name=""Car"" ObjectType=""entity"">
      <Properties>
        <Property Name=""LicensePlate"" Type=""string"" Required=""true"" />
        <Property Name=""Mileage"" Type=""int"" Required=""false"" />
      </Properties>
    </ObjectDefinition>
    <ObjectDefinition Name=""CarHistory"" ObjectType=""relation"">
      <Properties />
    </ObjectDefinition>
    <ObjectDefinition Name=""CarIsOfType"" ObjectType=""relation"">
      <Properties />
    </ObjectDefinition>
    <ObjectDefinition Name=""CarModelType"" ObjectType=""entity"">
      <Properties>
        <Property Name=""CarBrand"" Type=""string"" Required=""true"" />
        <Property Name=""CarModel"" Type=""string"" Required=""true"" />
        <Property Name=""CarType"" Type=""string"" Required=""true"" />
        <Property Name=""EngineSize"" Type=""string"" Required=""false"" />
        <Property Name=""EngineType"" Type=""string"" Required=""false"" />
        <Property Name=""NumberOfDoors"" Type=""int"" Required=""false"" />
        <Property Name=""TypeOfFuel"" Type=""string"" Required=""false"" />
        <Property Name=""YearFrom"" Type=""string"" Required=""false"" />
        <Property Name=""YearTo"" Type=""string"" Required=""false"" />
        <Property Name=""EnergyLabel"" Type=""string"" Required=""false"" />
      </Properties>
    </ObjectDefinition>
    <ObjectDefinition Name=""CarOwner"" ObjectType=""entity"">
      <Properties>
        <Property Name=""Name"" Type=""string"" Required=""true"" />
        <Property Name=""Address"" Type=""string"" Required=""false"" />
      </Properties>
    </ObjectDefinition>
    <ObjectDefinition Name=""CarOwners"" ObjectType=""relation"">
      <Properties />
    </ObjectDefinition>
    <ObjectDefinition Name=""ShopVisit"" ObjectType=""entity"">
      <Properties>
        <Property Name=""Date"" Type=""date"" Required=""true"" />
        <Property Name=""Reason"" Type=""string"" Required=""false"" />
      </Properties>
    </ObjectDefinition>
  </ObjectDefinitions>
  <ObjectRelations>
    <ObjectRelation Source=""Car"" Target=""CarIsOfType"" MinOccurs=""1"" MaxOccurs=""1"" />
    <ObjectRelation Source=""CarIsOfType"" Target=""CarModelType"" MinOccurs=""1"" MaxOccurs=""1"" />
    <ObjectRelation Source=""Car"" Target=""CarHistory"" MaxOccurs=""unbounded"" />
    <ObjectRelation Source=""CarHistory"" Target=""ShopVisit"" MinOccurs=""1"" MaxOccurs=""1"" />
    <ObjectRelation Source=""Car"" Target=""CarOwners"" MaxOccurs=""unbounded"" />
    <ObjectRelation Source=""CarOwners"" Target=""CarOwner"" MinOccurs=""1"" MaxOccurs=""1"" />
  </ObjectRelations>
      </ObjectModel>";


            IObjectModelService objectModelService = new ObjectModelService(logger, container, dataService);
            ObjectModel automobileObjectModel = objectModelService.Convert(objectModelXml, Encoding.Unicode);

            ICaseFileSpecificationService specificationService = new CaseFileSpecificationService(logger, container, objectModelService, dataService);

            string caseFileSpecificationXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<CaseFileSpecification xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://luminis.net/its/schemas/casefilespecification.xsd"">
  <Link rel=""objectmodel"" href=""http://localhost:8080/its/specifications/objectmodels/Automobile"" />
  <Link rel=""self"" href="""" />
  <Name>Autohistorie</Name>
  <UriTemplate>{Car/LicensePlate}</UriTemplate>
  <Structure>
    <Entity Name=""Car"" Type=""Car"">
      <Relation Name=""CarOwners"" Type=""CarOwners"">
        <Entity Name=""CarOwner"" Type=""CarOwner"" />
      </Relation>
      <Relation Name=""CarIsOfType"" Type=""CarIsOfType"">
        <Entity Name=""CarModelType"" Type=""CarModelType"" />
      </Relation>
      <Relation Name=""CarHistory"" Type=""CarHistory"">
        <Entity Name=""ShopVisit"" Type=""ShopVisit"" />
      </Relation>
    </Entity>
  </Structure>
</CaseFileSpecification>";

            CaseFileSpecification autohistorie = specificationService.Convert(caseFileSpecificationXml, Encoding.Unicode);
            autohistorie.ObjectModel = automobileObjectModel;

            string xmlschemaText = specificationService.GetXmlSchema(autohistorie, Encoding.Unicode);

            logger.DebugFormat("XmlSchema:\r\n{0}", xmlschemaText.Replace("><", ">\r\n<"));
        }
    }
}
