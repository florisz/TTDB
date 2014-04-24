using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Luminis.Its.Client;
using System.Xml;
using System.Xml.Linq;
using Luminis.Its.Client.Model;


namespace Luminis.Its.Client.Test
{
    /// <summary>
    /// Summary description for ItsClientTest
    /// </summary>
    [TestClass]
    public class ItsClientTest
    {
        public ItsClientTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestGetObjectModels()
        {
            var l = ItsClient.GetObjectModelList("http://localhost:8080/its");
            Assert.IsTrue(l.Count == 2, String.Format(" Found {0}", l.Count));
            Assert.IsTrue(l[0] == "ExtendedModel", String.Format(" Model not Found {0}", "ExtendedModel"));
            Assert.IsTrue(l[1] == "SimpleModel", String.Format(" Model not Found {0}", "SimpleModel"));
        }

        [TestMethod]
        public void TestGetCaseFileSpecs()
        {
            var l = ItsClient.GetCaseFileSpecList("http://localhost:8080/its", "ExtendedModel");
            Assert.IsTrue(l.Count == 3, String.Format(" Found {0}", l.Count));
            Assert.IsTrue(l[0] == "Woning", String.Format(" Casefilespec not Found {0}", "Woning"));
            Assert.IsTrue(l[1] == "Persoon", String.Format(" Casefilespec not Found {0}", "Persoon"));
            Assert.IsTrue(l[2] == "HuisEigenaar", String.Format(" Casefilespec not Found {0}", "HuisEigenaar"));
        }

        [TestMethod]
        public void TestPutModelByXmlString()
        {
            string uriOfModel = String.Format("{0}/specifications/objectmodels/{1}", "http://localhost:8080/its", "ExtendedModel");

            string original = ItsClient.Get(uriOfModel);

            string result=ItsClient.Put(uriOfModel, original);

            XDocument originalDoc = XDocument.Parse(original);
            XDocument resultedDoc = XDocument.Parse(result);

            // parse: <Link rel="self" href="http://localhost:8080/its/specifications/objectmodels/ExtendedModel?version=17" /> 
            var originalVersion = (from link in originalDoc.Root.Descendants(XName.Get("Link", @"http://luminis.net/its/schemas/objectmodel.xsd"))
                                   where link.Attribute("rel").Value == "self"
                                   select Convert.ToInt32(link.Attribute("href").Value.Split(new char[] { '=' })[1])
                                  ).First();

            var resultedVersion = (from link in resultedDoc.Root.Descendants(XName.Get("Link", @"http://luminis.net/its/schemas/objectmodel.xsd"))
                                   where link.Attribute("rel").Value == "self"
                                   select Convert.ToInt32(link.Attribute("href").Value.Split(new char[] { '=' })[1])
                                  ).First();



            Assert.IsTrue(resultedVersion  == originalVersion+1, string.Format("Original {0} and new version {1} should differ 1", originalVersion,resultedVersion));
        }

        [TestMethod]
        public void TestObjectModelRead()
        {
            var om = ItsClient.GetObjectModel("http://localhost:8080/its", "ExtendedModel");

            Assert.IsTrue(om.ObjectDefinitions.Length == 8, String.Format("Expected 8 but got {0}", om.ObjectDefinitions.Length));
        }

        [TestMethod]
        public void TestObjectModelWrite()
        {
            // Get 
            var om = ItsClient.GetObjectModel("http://localhost:8080/its", "ExtendedModel");
            var oldProp = om.ObjectDefinitions.First(d => d.Name == "Auto").Properties.First(p => p.Name == "Kenteken");

            bool oldValue = oldProp.Required;

            // put change
            oldProp.Required = !oldValue;
            ObjectModel nm = ItsClient.PutObjectModel("http://localhost:8080/its", "ExtendedModel", om);
            var newProp = nm.ObjectDefinitions.First(d => d.Name == "Auto").Properties.First(p => p.Name == "Kenteken");

            // compare change
            Assert.IsTrue(oldValue != newProp.Required);
        }


        [TestMethod]
        public void TestObjectModelNewXmlParseCylce()
        {
            // create model
            ObjectModel om = new ObjectModel();
            om.Link = new ObjectModelLink() { rel = ObjectModelLinkRel.self, href = "" };
            om.Name = "MyName";

            // add definition
            List<ObjectDefinition> od = new List<ObjectDefinition>();
            od.Add(new ObjectDefinition() { Name = "e1", ObjectType = ObjectType.entity });
            od.Add(new ObjectDefinition() { Name = "r1", ObjectType = ObjectType.relation });
            om.ObjectDefinitions = od.ToArray();

            // add relation
            List<ObjectRelation> ol = new List<ObjectRelation>();
            ol.Add(new ObjectRelation() { Source = om.ObjectDefinitions[0].Name, Target = om.ObjectDefinitions[1].Name });
            om.ObjectRelations = ol.ToArray();

            string modelXML = ObjectModelService.GetXML(om, Encoding.Unicode, true);
            var doc = XDocument.Parse(modelXML);
            Assert.IsTrue(doc.Root.Descendants().Count() == 7);

            var omFromDoc = ObjectModelService.Convert(doc.ToString());

            Assert.IsTrue(om.Name == omFromDoc.Name);
            Assert.IsTrue(om.ObjectDefinitions.Length == omFromDoc.ObjectDefinitions.Length);
            Assert.IsTrue(om.ObjectRelations.Length == omFromDoc.ObjectRelations.Length);

        }

        [TestMethod]
        public void TestCaseFileSpecRead()
        {
            var caseFileSpec = ItsClient.GetCaseFileSpec("http://localhost:8080/its", "ExtendedModel", "Persoon");

            Assert.IsTrue(caseFileSpec.CaseFileSpecificationElements.Count == 7, String.Format("Expected 7 but got {0}", caseFileSpec.CaseFileSpecificationElements.Count));
        }

    }
}
