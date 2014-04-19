using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using log4net;
using log4net.Config;

using Rhino.Mocks;

using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.ObjectModels;
using Luminis.Its.Services.Rules;
using Luminis.Its.Services.Rules.FSharp;
using Luminis.Logging;
using Luminis.Logging.Log4Net;

namespace Luminis.Its.Services.Rules.FSharp.Manual.Test
{
    /// <summary>
    /// Summary description for TestFSharpRuleEngine
    /// </summary>
    [TestFixture]
    public class TestFSharpRuleEngine
    {
        private MockRepository _mocks;

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
        #endregion

        [Test]
        public void TestConstructor()
        {
            _mocks.Record();

            ILogger logger = _mocks.StrictMock<ILogger>();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            _mocks.ReplayAll();

            FSharpRuleEngine fsharpRuleEngine = new FSharpRuleEngine(logger, caseFileSpecificationService);

            Assert.IsNotNull(fsharpRuleEngine);
        }

        private delegate byte[] GetAssemblyBytesDelegate(CaseFileSpecification specification);

        [Test]
        public void TestExecuteRuleInSeparateAppDomain()
        {
            _mocks.Record();

            ILogger logger = new Log4NetLogger();
            ICaseFileSpecificationService caseFileSpecificationService = _mocks.StrictMock<ICaseFileSpecificationService>();

            Expect.On(caseFileSpecificationService).Call(caseFileSpecificationService.GetAssemblyBytes(null)).IgnoreArguments().IgnoreArguments().Do(
                new GetAssemblyBytesDelegate(
                    delegate(CaseFileSpecification specification)
                    {
                        MemoryStream assemblyBytes = new MemoryStream();
                        Assembly thisAssembly = this.GetType().Assembly;
                        string specificationDll = "Luminis.Its.Services.Rules.FSharp.Manual.Test.myspecification.dll";
                        Stream specificationDllStream = thisAssembly.GetManifestResourceStream(specificationDll);
                        int byteValue = specificationDllStream.ReadByte();
                        while (byteValue != -1)
                        {
                            assemblyBytes.WriteByte((byte)byteValue);
                            byteValue = specificationDllStream.ReadByte();
                        }
                        assemblyBytes.Close();
                        return assemblyBytes.ToArray();
                    }
                )
            ).Repeat.Twice();

            _mocks.ReplayAll();

            IRuleEngine fsharpRuleEngine = new FSharpRuleEngine(logger, caseFileSpecificationService);

            Rule rule = new Rule()
            {
                Name = "myrule",
                Script = new RuleScript()
                {
                    Method = "MyObjectmodel.MySpecification.myrule.Execute",
                    Type = "fsharp",
                    Value = @"#light
module MyObjectmodel.MySpecification.myrule

open System
open MyObjectmodel.MySpecification

let Execute(p: Person) = 
    p.NumberOfHouses <- Seq.length(p.OwnsHouse)
    p.NumberOfHousesSpecified <- true"
                }
            };
            CaseFile input = new CaseFile()
            {
                CaseFileSpecification = new CaseFileSpecification()
                {
                    Name = "MySpecification",
                    ObjectModel = new ObjectModel()
                    {
                        Name = "MyObjectmodel"
                    },
                    Structure = new CaseFileSpecificationStructure()
                    {
                        Entity = new CaseFileSpecificationEntity()
                        {
                            Name = "Person",
                            Type = "Person"
                        }
                    }
                },
                Text = @"<Person>
                            <OwnsHouse/>
                            <OwnsHouse/>
                            <OwnsHouse/>
                         </Person>"
            };

            CaseFile result = fsharpRuleEngine.Execute(rule, input);

            Assert.IsNotNull(result);

            logger.Debug(result.Text);

            Assert.IsTrue(result.Text.Contains("<NumberOfHouses>3</NumberOfHouses>"));

            LogLoadedAssemblies(logger);

            input.Text = @"<Person>
                            <OwnsHouse/>
                            <OwnsHouse/>
                            <OwnsHouse/>
                            <OwnsHouse/>
                            <OwnsHouse/>
                         </Person>";

            result = fsharpRuleEngine.Execute(rule, input);

            Assert.IsNotNull(result);

            logger.Debug(result.Text);

            Assert.IsTrue(result.Text.Contains("<NumberOfHouses>5</NumberOfHouses>"));

            LogLoadedAssemblies(logger);
        }

        private static void LogLoadedAssemblies(ILogger logger)
        {
            LogLoadedAssemblies(logger, AppDomain.CurrentDomain);
        }

        private static void LogLoadedAssemblies(ILogger logger, AppDomain appDomain)
        {
            logger.DebugFormat("Loaded assemblies in appdomain: {0}", appDomain.FriendlyName);
            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                logger.DebugFormat("- {0}", loadedAssembly.GetName().Name);
            }
        }
    }
}
