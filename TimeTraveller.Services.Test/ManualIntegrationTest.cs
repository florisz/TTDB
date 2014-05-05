using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Xml;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFiles.Impl;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.CaseFileSpecifications.Impl;
using TimeTraveller.Services.Console;
using TimeTraveller.Services.Data;
using TimeTraveller.Services.Data.Impl;
using TimeTraveller.Services.ObjectModels;
using TimeTraveller.Services.ObjectModels.Impl;
using TimeTraveller.Services.Resources;
using TimeTraveller.Services.Resources.Impl;
using TimeTraveller.Services.Rest;
using TimeTraveller.Services.Rest.Impl;
using TimeTraveller.Services.Representations;
using TimeTraveller.Services.Representations.Impl;
using TimeTraveller.Services.Rules;
using TimeTraveller.Services.Rules.Impl;
using TimeTraveller.Services.Rules.FSharp;
//using TimeTraveller.Tools.Sparx.ObjectModelGen;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Logging.Log4Net;
using TimeTraveller.General.Unity;

namespace TimeTraveller.Services.Manual.Test
{
    /// <summary>
    /// Summary description for ManualIntegrationTest
    /// </summary>
    [TestFixture]
    public class ManualIntegrationTest
    {
        private static ILogger _logger;
        private static Process _itsServerProcess;

        #region Additional test attributes before running the first test in the class
        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            BasicConfigurator.Configure();
            _logger = new Log4NetLogger();
            StartITSServer();
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            LogManager.Shutdown();
            _itsServerProcess.Kill();
        }

        [SetUp()]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        private static void StartITSServer()
        {
            // the configuration file is not copied as part of setting up a test environment
            // this test project contains the app.config of the service app as an embedded resource
            // to ensure the tests are run properly the embedded resource is saved in the cuurent directory
            // as the configuration file for the service application.
            string itsServerFileName = "TimeTraveller.Services.Console.exe";
            CreateConfigFiles(Environment.CurrentDirectory, itsServerFileName, "Log4Net.config");

            // start the ITSServer in a separate process
            _itsServerProcess = new Process();
            _itsServerProcess.StartInfo.FileName = String.Format(@"{0}\{1}", Environment.CurrentDirectory, itsServerFileName);
            _itsServerProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            _itsServerProcess.Start();
            Thread.Sleep(2000);
        }

        private static void CreateConfigFiles(string directoryPath, string exeName, string logName)
        {
            // create the app config file
            string appConfigResourceName = string.Format(@"{0}.config", exeName);
            string configFilePath = string.Format(@"{0}\{1}", directoryPath, appConfigResourceName);
            string @namespace = typeof(ManualIntegrationTest).Namespace;
            CopyEmbeddedResourceToFile(String.Format(@"{0}.{1}", @namespace, appConfigResourceName), configFilePath);

            // create the Log4Net config file
            CopyEmbeddedResourceToFile(String.Format(@"{0}.{1}", @namespace, logName), String.Format(@"{0}\{1}", directoryPath, logName));
        }

        private static void CopyEmbeddedResourceToFile(string resourceFullName, string configFilePath)
        {
            Stream stream = typeof(ManualIntegrationTest).Assembly.GetManifestResourceStream(resourceFullName);
            FileStream buffer = File.Create(configFilePath);
            int byteValue = stream.ReadByte();
            while (byteValue != -1)
            {
                buffer.WriteByte((byte)byteValue);
                byteValue = stream.ReadByte();
            }
            buffer.Close();
        }


        #endregion

        [Test]
        public void TestRestService2DataServiceAndBack()
        {
            TestStoreObjectModel();

            TestStoreCaseFileSpecification();

            TestStoreRepresentation();

            TestStoreSimpleCaseFile();

            TestStoreCaseFile();

            DateTime caseFileFirstVersion = DateTime.Now;

            Thread.Sleep(3000);

            TestStoreModifiedCaseFile();

            TestGetCaseFileByTimePoint(caseFileFirstVersion);

            TestStoreRule();

            TestExecuteRule();

            TestGetObjectModelSummary();

            TestStoreModifiedObjectModel();

            TestStoreCaseFileWithModifiedObjectModel();

            TestStoreResource();

            Assert.IsTrue(true, "end of integrated test");
        }

        private void TestStoreResource()
        {
            string coffeeBeanPath = string.Format("{0}.{1}", this.GetType().Namespace, "Coffee Bean.bmp");
            Stream coffeeBeanStream = this.GetType().Assembly.GetManifestResourceStream(coffeeBeanPath);
            MemoryStream buffer = new MemoryStream();
            int byteValue = coffeeBeanStream.ReadByte();
            while (byteValue != -1)
            {
                buffer.WriteByte((byte)byteValue);
                byteValue = coffeeBeanStream.ReadByte();
            }
            buffer.Close();

            byte[] requestBuffer = buffer.ToArray();
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "image/bmp");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/resources/images/Coffee%20Bean.bmp", "PUT", requestBuffer);

            webClient.Headers.Add(HttpRequestHeader.ContentType, "image/bmp");
            resultBuffer = webClient.DownloadData("http://localhost:8080/its/resources/images/Coffee%20Bean.bmp");

            Assert.AreEqual(17062, resultBuffer.Length);
        }

        private void TestStoreCaseFileWithModifiedObjectModel()
        {
            string getCaseFileRequest = "http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers";
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.DownloadData(getCaseFileRequest);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("GetCaseFile() 4th time={0}", result.Replace("\n", "\r\n"));

            string newCaseFileXml = result;
            // TODO: zie ITS-10: het moet eigenlijk Gender zijn maar dat werkt niet, Het gewijzigde objectmodel wordt niet herkend.
            //newCaseFileXml = newCaseFileXml.Replace("</LastName>", "</LastName><Gender>Male</Gender>");
            newCaseFileXml = newCaseFileXml.Replace("</LastName>", "</LastName><NumberOfHouses>512</NumberOfHouses>");

            byte[] requestBuffer = Encoding.Unicode.GetBytes(newCaseFileXml);
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            resultBuffer = webClient.UploadData("http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers", "PUT", requestBuffer);
            result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("StoreCaseFile() 3rd time={0}", result.Replace("\n", "\r\n"));

        }

        private void TestStoreModifiedObjectModel()
        {
            string objectModelXml = @"<ObjectModel xmlns=""http://timetraveller.net/its/schemas/objectmodel.xsd"">
                                                   <Link rel=""self"" href=""http://localhost:8080/its/specifications/objectmodels/MyObjectmodel""/>
                                                   <Name>MyObjectmodel</Name>
                                                   <ObjectDefinitions>
                                                     <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                                       <Properties>
                                                         <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                                         <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                                         <Property Name=""NumberOfHouses"" Type=""int"" />
                                                         <Property Name=""Gender"" Type=""string"" />
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
                                                    <ObjectDefinition Name=""OwnsHouse"" ObjectType=""relation"">
                                                       <Properties>
                                                         <Property Name=""From"" Type=""date"" Required=""true"" />
                                                         <Property Name=""To"" Type=""date"" />
                                                       </Properties>
                                                    </ObjectDefinition>
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

            WebClient webClient = new WebClient();

            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] requestBuffer = Encoding.Unicode.GetBytes(objectModelXml);
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/specifications/objectmodels/MyObjectmodel", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);

            _logger.DebugFormat("StoreObjectModel()={0} 2nd time", result);
        }

        private void TestExecuteRule()
        {
            string getCaseFileRequest = "http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers";
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            byte[] resultBuffer = webClient.DownloadData(getCaseFileRequest);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("GetCaseFile() 3rd time={0}", result.Replace("\n", "\r\n"));

            byte[] requestBuffer = Encoding.Unicode.GetBytes(result);
            string executeRuleRequest = "http://localhost:8080/its/rules/MyObjectmodel/MySpecification/MyRule";
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            resultBuffer = webClient.UploadData(executeRuleRequest, "POST", requestBuffer);
            result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("ExecuteRule()={0}", result.Replace("\n", "\r\n"));
        }

        private void TestStoreRule()
        {
            string ruleXml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <Rule xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://timetraveller.net/its/schemas/rule.xsd"">
                              <Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/MyObjectmodel/MySpecification""/>
                              <Link rel=""self"" href=""http://localhost:8080/its/rules/MyObjectmodel/MySpecification/MyRule"" />
                              <Name>MyRule</Name>
                              <Script Type=""fsharp"" Method=""MyObjectmodel.MySpecification.MyRule.Execute"">#light
module MyObjectmodel.MySpecification.MyRule

open System
open MyObjectmodel.MySpecification

let Execute(p: Person) =
    p.NumberOfHouses &lt;- Seq.length(p.OwnsHouse)
    p.NumberOfHousesSpecified &lt;- true
                              </Script>
                            </Rule>";

            byte[] requestBuffer = Encoding.Unicode.GetBytes(ruleXml);
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/rules/MyObjectmodel/MySpecification/MyRule", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("StoreRule()={0}", result);
        }

        private void TestGetCaseFileByTimePoint(DateTime caseFileFirstVersion)
        {
            string getCaseFileRequest = string.Format("http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers?timepoint={0}&view=MyRepresentation", caseFileFirstVersion.ToString("O").Replace("+", "%2B"));
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            byte[] resultBuffer = webClient.DownloadData(getCaseFileRequest);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("GetCaseFile() 2nd time={0}", result.Replace("\n", "\r\n"));
        }

        private void TestStoreModifiedCaseFile()
        {
            string getCaseFileRequest = "http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers";
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            byte[] resultBuffer = webClient.DownloadData(getCaseFileRequest);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("GetCaseFile()={0}", result.Replace("\n", "\r\n"));

            string newCaseFileXml = result;
            newCaseFileXml = newCaseFileXml.Replace("026-12345678", "026-87654321");
            newCaseFileXml = newCaseFileXml.Replace("Patersstraat 13A", "PATERSSTRAAT 13A");

            byte[] requestBuffer = Encoding.Unicode.GetBytes(newCaseFileXml);
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            resultBuffer = webClient.UploadData("http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers", "PUT", requestBuffer);
            result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            DateTime caseFileSecondVersion = DateTime.Now;

            _logger.DebugFormat("StoreCaseFile() 2nd time={0}", caseFileSecondVersion.ToString("O"), result.Replace("\n", "\r\n"));
        }

        public void TestStoreCaseFile()
        {
            DateTime now = DateTime.Now;
            string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://timetraveller.net/its/schemas/casefile.xsd"">
                                                <cs:Link rel=""self"" href=""http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers""/>
                                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/MyObjectmodel/MySpecification""/>
                                                <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Alex</FirstName>
                                                    <LastName>Harbers</LastName>
                                                    <TelephoneNumbers>
                                                        <TelephoneNumber>
                                                            <NumberType>privat</NumberType>
                                                            <Number>026-12345678</Number>
                                                        </TelephoneNumber>
                                                        <TelephoneNumber>
                                                            <NumberType>mobile</NumberType>
                                                            <Number>06-876543210</Number>
                                                        </TelephoneNumber>
                                                    </TelephoneNumbers>
                                                    <OwnsHouse RegistrationId=""{2}"" RegistrationStart=""{3}"">
                                                        <From>2001-01-01</From>
                                                        <To>2004-08-30</To>
                                                        <House RegistrationId=""{4}"" RegistrationStart=""{5}"">
                                                            <Address>Lindelaan 13</Address>
                                                            <ZipCode>1234 AB</ZipCode>
                                                            <City>Amsterdam</City>
                                                        </House>
                                                    </OwnsHouse>
                                                    <OwnsHouse RegistrationId=""{6}"" RegistrationStart=""{7}"">
                                                        <From>2004-09-01</From>
                                                        <House RegistrationId=""{8}"" RegistrationStart=""{9}"">
                                                            <Address>Patersstraat 13A</Address>
                                                            <ZipCode>6828 AB</ZipCode>
                                                            <City>Arnhem</City>
                                                        </House>
                                                    </OwnsHouse>
                                                </Person>
                                           </cs:CaseFile>", Guid.NewGuid(), now.ToString("O"),
                                                    Guid.NewGuid(), now.ToString("O"),
                                                    Guid.NewGuid(), now.ToString("O"),
                                                    Guid.NewGuid(), now.ToString("O"),
                                                    Guid.NewGuid(), now.ToString("O"));


            byte[] requestBuffer = Encoding.Unicode.GetBytes(caseFileXml);
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            DateTime caseFileFirstVersion = DateTime.Now;

            _logger.DebugFormat("StoreCaseFile()={0}\r\n{1}", caseFileFirstVersion.ToString("O"), result.Replace("\n", "\r\n"));
        }

        public void TestStoreSimpleCaseFile()
        {
            DateTime now = DateTime.Now;
            string caseFileXml = string.Format(@"<cs:CaseFile xmlns:cs=""http://timetraveller.net/its/schemas/casefile.xsd"">
                                                <cs:Link rel=""self"" href=""http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Alex.MiddleName.Harbers""/>
                                                <cs:Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/MyObjectmodel/MySpecification""/>
                                                <Person RegistrationId=""{0}"" RegistrationStart=""{1}"">
                                                    <FirstName>Floris</FirstName>
                                                    <LastName>Zwarteveen</LastName>
                                                </Person>
                                           </cs:CaseFile>", Guid.NewGuid(), now.ToString("O"));


            byte[] requestBuffer = Encoding.Unicode.GetBytes(caseFileXml);
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/casefiles/MyObjectmodel/MySpecification/Floris.MiddleName.Zwarteveen", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            DateTime caseFileFirstVersion = DateTime.Now;

            _logger.DebugFormat("TestStoreSimpleCaseFile()={0}\r\n{1}", caseFileFirstVersion.ToString("O"), result.Replace("\n", "\r\n"));
        }

        public void TestStoreRepresentation()
        {
            string representationXml = @"<r:Representation xmlns:r=""http://timetraveller.net/its/schemas/representation.xsd"">
                                                    <r:Link rel=""casefilespecification"" href=""http://localhost:8080/its/specifications/casefiles/MyObjectmodel/MySpecification""/>
                                                    <r:Link rel=""self"" href=""http://localhost:8080/its/representations/MyObjectmodel/MySpecification/MyRepresentation""/>
                                                    <r:Name>MyRepresentation</r:Name>
                                                    <r:Script Type=""xslt"" ContentType=""html"">
                                                        <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""> 
                                                          <xsl:output method=""xml""/>
                                                          <xsl:template match=""/""> 
                                                            <html> 
                                                              <body> 
                                                                <xsl:text>Person: </xsl:text>
                                                                <xsl:value-of select=""Person/FirstName""/>
                                                                <xsl:text>.</xsl:text>
                                                                <xsl:value-of select=""Person/LastName""/>
                                                                <br/>
                                                                <xsl:text>Telephonenumbers:</xsl:text>
                                                                <br/>
                                                                <xsl:apply-templates select=""//TelephoneNumber""/>
                                                                <xsl:text>Houses:</xsl:text>
                                                                <br/>
                                                                <xsl:apply-templates select=""//OwnsHouse""/>
                                                              </body> 
                                                            </html> 
                                                          </xsl:template>
                                                          <xsl:template match=""TelephoneNumber""> 
                                                                <xsl:value-of select=""NumberType""/>
                                                                <xsl:text>: </xsl:text>
                                                                <xsl:value-of select=""Number""/>
                                                                <br/>
                                                          </xsl:template>
                                                          <xsl:template match=""OwnsHouse""> 
                                                                <xsl:value-of select=""House/Address""/>
                                                                <xsl:text> </xsl:text>
                                                                <xsl:value-of select=""House/ZipCode""/>
                                                                <xsl:text> </xsl:text>
                                                                <xsl:value-of select=""House/City""/>
                                                                <br/>
                                                                <xsl:text>Period: </xsl:text>
                                                                <xsl:value-of select=""From""/>
                                                                <xsl:text> - </xsl:text>
                                                                <xsl:value-of select=""To""/>
                                                                <br/>
                                                          </xsl:template>
                                                        </xsl:stylesheet>
                                                    </r:Script>
                                                </r:Representation>";
            byte[] requestBuffer = Encoding.Unicode.GetBytes(representationXml);
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/representations/MyObjectmodel/MySpecification/MyRepresentation", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("StoreRepresentation()={0}", result);
        }

        public void TestStoreCaseFileSpecification()
        {
            string specificationXml = @"<CaseFileSpecification xmlns=""http://timetraveller.net/its/schemas/casefilespecification.xsd"">
                                                  <Link rel=""objectmodel"" href=""http://localhost:8080/its/specifications/objectmodels/MyObjectmodel""/>
                                                  <Link rel=""self"" href=""http://localhost:8080/its/specifications/casefiles/myobjectmodel/MySpecification""/>
                                                  <Name>MySpecification</Name>
                                                  <UriTemplate>{/Person/FirstName}.MiddleName.{/Person/LastName}</UriTemplate>
                                                  <Structure>
                                                      <Entity Name=""Person"" Type=""Person"">
                                                        <Relation Name=""OwnsHouse"" Type=""OwnsHouse"">
                                                          <Entity Name=""House"" Type=""House"" />
                                                        </Relation>
                                                      </Entity>
                                                  </Structure>
                                                </CaseFileSpecification>";
            WebClient webClient = new WebClient();
            byte[] requestBuffer = Encoding.Unicode.GetBytes(specificationXml);
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/specifications/casefiles/MyObjectmodel/MySpecification", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);

            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("StoreCaseFileSpecification()={0}", result);
        }

        public void TestStoreObjectModel()
        {
            string objectModelXml = @"<ObjectModel xmlns=""http://timetraveller.net/its/schemas/objectmodel.xsd"">
                                       <Link rel=""self"" href=""http://localhost:8080/its/specifications/objectmodels/MyObjectmodel""/>
                                       <Name>MyObjectmodel</Name>
                                       <ObjectDefinitions>
                                         <ObjectDefinition Name=""Person"" ObjectType=""entity"">
                                           <Properties>
                                             <Property Name=""FirstName"" Type=""string"" Required=""true"" />
                                             <Property Name=""LastName"" Type=""string"" Required=""true"" />
                                             <Property Name=""NumberOfHouses"" Type=""int"" />
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
                                        <ObjectDefinition Name=""OwnsHouse"" ObjectType=""relation"">
                                           <Properties>
                                             <Property Name=""From"" Type=""date"" Required=""true"" />
                                             <Property Name=""To"" Type=""date"" />
                                           </Properties>
                                        </ObjectDefinition>
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

            WebClient webClient = new WebClient();

            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
            byte[] requestBuffer = Encoding.Unicode.GetBytes(objectModelXml);
            byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/specifications/objectmodels/MyObjectmodel", "PUT", requestBuffer);
            string result = Encoding.Unicode.GetString(resultBuffer);
            Assert.IsNotNull(!string.IsNullOrEmpty(result));

            _logger.DebugFormat("StoreObjectModel()={0}", result);
        }

        public void TestGetObjectModelSummary()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.Accept, "text/html; charset=utf-8");

                byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/its/specifications/objectmodels/MyObjectmodel?summary=true");
                string result = Encoding.UTF8.GetString(resultBuffer);

                _logger.DebugFormat("result={0}", result);

                Assert.IsTrue(!string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains(@"<html>"));
                Assert.IsTrue(result.Contains(@"</html>"));
            }
            catch (Exception exception)
            {
                _logger.Error("", exception);
                throw;
            }
        }

        //public void TestGenerateAndStoreObjectModelFromEA2DataService()
        //{
        //    try
        //    {
        //        StartITSServer();

        //        Stream manifestResourceStream = this.GetType().Assembly.GetManifestResourceStream("TimeTraveller.Services.Manual.Test.TestModel.eap");
        //        FileStream fileStream = new FileStream(@".\TestModel.eap", FileMode.Create);

        //        int byteValue = manifestResourceStream.ReadByte();
        //        while (byteValue != -1)
        //        {
        //            fileStream.WriteByte((byte)byteValue);
        //            byteValue = manifestResourceStream.ReadByte();
        //        }
        //        fileStream.Close();
        //        manifestResourceStream.Close();

        //        ObjectModelGen generator = new ObjectModelGen();
        //        generator.Progress += new EventHandler<ObjectModelGenEventArgs>(ObjectModelGen_OnProgress);
        //        generator.Generate(@".\TestModel.eap", @".");

        //        Assert.IsTrue(File.Exists(@".\ObjectModel.xml"));

        //        string objectModelXml = File.ReadAllText(@".\ObjectModel.xml");

        //        _logger.DebugFormat("ObjectModel.xml=\r\n{0}", objectModelXml);

        //        WebClient webClient = new WebClient();

        //        byte[] requestBuffer = Encoding.Unicode.GetBytes(objectModelXml);
        //        webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
        //        webClient.Headers.Add(HttpRequestHeader.From, "alex.harbers@gmail.com");
        //        byte[] resultBuffer = webClient.UploadData("http://localhost:8080/its/specifications/objectmodels/myobjectmodel", "PUT", requestBuffer);
        //        string result = Encoding.Unicode.GetString(resultBuffer);
        //        _logger.DebugFormat("StoreObjectModel()={0}", result);

        //        Assert.IsTrue(!string.IsNullOrEmpty(result));

        //        resultBuffer = webClient.DownloadData("http://localhost:8080/its/specifications/objectmodels/myobjectmodel.xsd");
        //        result = Encoding.Unicode.GetString(resultBuffer);
        //        _logger.DebugFormat("GetObjectModel()={0}", result);

        //        Assert.IsTrue(!string.IsNullOrEmpty(result));
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.Error("", exception);
        //        throw;
        //    }
        //}

        //private void ObjectModelGen_OnProgress(object sender, ObjectModelGenEventArgs e)
        //{
        //    _logger.Debug(e.Message);
        //}
        
        //public void TestGetResourcesAsHtml()
        //{
        //    _mocks.Record();

        //    _logger = new Log4NetLogger();
        //    _unity = _mocks.StrictMock<IUnity>();
        //    IDataService dataService = _mocks.StrictMock<IDataService>();
        //    IResourceService resourceService = _mocks.StrictMock<IResourceService>();
        //    Expect.On(_unity).Call(_unity.Resolve<IResourceService>()).Repeat.Twice().Return(resourceService);

        //    BaseObjectType resourceType = new BaseObjectType()
        //    {
        //        Id = (int)ItemType.Resource,
        //        RelativeUri = "/resources/"
        //    };

        //    Expect.On(dataService).Call(dataService.GetBaseObjectType(ItemTypeHelper.Convert(ItemType.Resource))).Return(resourceType);

        //    List<IBaseObject> resources = new List<IBaseObject>();
        //    Expect.On(dataService).Call(dataService.GetBaseObjects(string.Empty, resourceType)).Return(resources);

        //    Expect.On(resourceService).Call(resourceService.GetEnumerable(new Uri("http://localhost:8080/its"))).Do(
        //        new GetResourcesDelegate(
        //            delegate(Uri baseUri)
        //            {
        //                return _resourceServiceImpl.GetEnumerable(baseUri);
        //            }
        //        )
        //    );

        //    Expect.On(resourceService).Call(resourceService.Get(string.Empty, new Uri("http://localhost:8080/its"))).IgnoreArguments().Do(
        //        new GetResourceDelegate(
        //            delegate(string id, Uri baseUri)
        //            {
        //                return _resourceServiceImpl.Get(id, baseUri);
        //            }
        //        )
        //    );

        //    Expect.On(resourceService).Call(resourceService.GetList(null, new Uri("http://localhost:8080/its"), Encoding.UTF8)).IgnoreArguments().Do(
        //        new GetResourceListDelegate(
        //            delegate(IEnumerable<Resource> items, Uri baseUri, Encoding encoding)
        //            {
        //                return _resourceServiceImpl.GetList(items, baseUri, encoding);
        //            }
        //        )
        //    );

        //    _mocks.ReplayAll();

        //    try
        //    {
        //        _resourceServiceImpl = new ResourceService(_logger, _unity, dataService);

        //        InitializeRestServiceHost();

        //        _serviceHost.Open();

        //        WebClient webClient = new WebClient();
        //        webClient.Headers.Add(HttpRequestHeader.Accept, "text/html");

        //        byte[] resultBuffer = webClient.DownloadData("http://localhost:8080/its/resources/");
        //        string result = Encoding.UTF8.GetString(resultBuffer);

        //        _logger.DebugFormat("result={0}", result);

        //        Assert.IsTrue(!string.IsNullOrEmpty(result));
        //        Assert.IsTrue(result.Contains(@"<html>"));
        //        Assert.IsTrue(result.Contains(@"</html>"));
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.Error("", exception);
        //        throw;
        //    }
        //    finally
        //    {
        //        if (_serviceHost != null && _serviceHost.State == CommunicationState.Opened)
        //        {
        //            _serviceHost.Close();
        //        }
        //    }
        //}

    }
}
