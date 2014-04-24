using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using TimeTraveller.Services.Data;
using TimeTraveller.Services.Data.Impl;
using TimeTraveller.Services.Representations.Impl;
using TimeTraveller.Services.Resources.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;

namespace TimeTraveller.Services.Resources.Test
{
    /// <summary>
    /// Summary description for TestResourceService
    /// </summary>
    [TestFixture]
    public class TestResourceService
    {
        private MockRepository _mocks;

        #region Additional test attributes before running the first test in the class
        [TestFixtureSetUp]
        public static void TestFixtureSetUp()
        {
            BasicConfigurator.Configure();
        }

        [TestFixtureTearDown]
        public static void TestFixtureTearDown()
        {
            LogManager.Shutdown();
        }

        [SetUp]
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

            ILogger logger = NullLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            Assert.IsNotNull(resourceService);
        }

        [Test]
        public void TestGetResource()
        {
            _mocks.Record();

            ILogger logger = NullLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            BaseObjectValue resourceValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "images/myresource.bmp",
                    BaseObjectType = resourceType
                },
                ContentType = "image/bmp",
                Start = new TimePoint(DateTime.Now),
                End = TimePoint.Future
            };

            Expect.On(dataService).Call(dataService.GetValue(string.Empty, resourceType, TimePoint.Past)).IgnoreArguments().Return(resourceValue);
            
            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            Resource myresource = resourceService.Get("images/myresource.bmp", new Uri("http://localhost:8080"));

            Assert.AreEqual("images/myresource.bmp", myresource.ExtId);
            Assert.AreEqual("image/bmp", myresource.ContentType);
        }

        [Test]
        public void TestGetResources()
        {
            _mocks.Record();

            ILogger logger = ConsoleLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            List<IBaseObject> resourceValues = new List<IBaseObject>()
            {
                new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "images/myresource.bmp",
                    BaseObjectType = resourceType,
                    BaseObjectValues = new EntityCollection<BaseObjectValue>()
                    {
                        new BaseObjectValue()
                        {
                            Id = Guid.NewGuid(),
                            ContentType = "image/bmp",
                            Start = new TimePoint(DateTime.Now),
                            End = TimePoint.Future
                        }
                    }
                }
            };

            Expect.On(dataService).Call(dataService.GetBaseObjects(string.Empty, resourceType)).IgnoreArguments().Return(resourceValues);

            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            IEnumerable<Resource> resources = resourceService.GetEnumerable(new Uri("http://localhost:8080/its"));

            Assert.IsTrue(resources.Count() >= 3);

            foreach (Resource resource in resources)
            {
                logger.DebugFormat("Resource: {0} ({1})", resource.ExtId, resource.ContentType);
            }
        }

        [Test]
        public void TestStoreResource()
        {
            _mocks.Record();

            ILogger logger = NullLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            BaseObjectValue resourceValue = new BaseObjectValue()
            {
                Id = Guid.NewGuid(),
                ParentBaseObject = new BaseObject()
                {
                    Id = Guid.NewGuid(),
                    ExtId = "images/myresource.bmp",
                    BaseObjectType = resourceType
                },
                ContentType = "image/bmp",
                Start = new TimePoint(DateTime.Now),
                End = TimePoint.Future
            };

            Expect.On(dataService).Call(dataService.GetBaseObject(resourceValue.ParentBaseObject.ExtId, resourceType)).Return(null);

            Expect.On(dataService).Call(dataService.InsertValue(null as byte[], string.Empty, TimePoint.Past, Guid.Empty, string.Empty, resourceType, null as WebHttpHeaderInfo)).IgnoreArguments().Return(resourceValue);

            dataService.SaveChanges();

            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            Resource myresource = new Resource()
            {
                ContentType = "images/bmp",
                BaseObjectValue = new BaseObjectValue()
                {
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "images/myresource.bmp"
                    }
                }
            };

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            resourceService.Store("images/myresource.bmp", myresource, new Uri("http://localhost:8080/its"), info);
        }

        [Test]
        public void TestGetBuiltInResource()
        {
            _mocks.Record();

            ILogger logger = ConsoleLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            Resource myresource = resourceService.Get("images/timetraveller.png", new Uri("http://localhost:8080/its"));

            Assert.AreEqual("images/timetraveller.png", myresource.ExtId);
            Assert.AreEqual("image/png", myresource.ContentType);

            myresource = resourceService.Get("styles/itsbrowser.css", new Uri("http://localhost:8080/its"));

            Assert.AreEqual("styles/itsbrowser.css", myresource.ExtId);
            Assert.AreEqual("text/plain; charset=utf-8", myresource.ContentType);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCannotStoreBuiltInResource()
        {
            _mocks.Record();

            ILogger logger = NullLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            Resource myresource = new Resource()
            {
                ContentType = "images/bmp",
                BaseObjectValue = new BaseObjectValue()
                {
                    ParentBaseObject = new BaseObject()
                    {
                        ExtId = "images/timetraveller.png"
                    }
                }
            };

            WebHttpHeaderInfo info = new WebHttpHeaderInfo()
            {
                Username = "Alex Harbers"
            };

            resourceService.Store("images/timetraveller.png", myresource, new Uri("http://localhost:8080/its"), info);
        }

        //[Test]
        public void TestGetResourcesAsHtml()
        {
            _mocks.Record();

            ILogger logger = ConsoleLogger.Instance;
            IUnity container = _mocks.StrictMock<IUnity>();
            IDataService dataService = _mocks.StrictMock<IDataService>();

            BaseObjectType resourceType = new BaseObjectType()
            {
                Id = (int)ItemType.Resource
            };

            Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

            _mocks.ReplayAll();

            IResourceService resourceService = new ResourceService(logger, container, dataService);

            string xml = @"<Resources>
                                <Resource>
                                    <Link rel=""resource"" href=""http://localhost:8080/its/resources/images/timetraveller.png""/>
                                </Resource>
                                <Resource>
                                    <Link rel=""resource"" href=""http://localhost:8080/its/resources/styles/itsbrowser.css""/>
                                </Resource>
                            </Resources>";

            Resource resourcesXslt = resourceService.Get("scripts/itsbrowsersummary.xslt", new Uri("http://localhost:8080/its"));

            XsltTransformer transformer = new XsltTransformer();
            string script = Encoding.UTF8.GetString(resourcesXslt.Content);

            string html = transformer.Transform(script.Substring(1), xml);

            logger.Debug(html);
        }

    }
}
