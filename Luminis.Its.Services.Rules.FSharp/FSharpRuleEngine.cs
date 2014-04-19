using System;
using System.IO;
using System.Xml;

using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Rules;
using Luminis.Logging;
using Luminis.Xml;

namespace Luminis.Its.Services.Rules.FSharp
{
    public class FSharpRuleEngine: IRuleEngine
    {
        #region private properties
        private ILogger _logger;
        private ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public FSharpRuleEngine(ILogger logger, ICaseFileSpecificationService caseFileSpecificationService)
        {
            _logger = logger;
            _caseFileSpecificationService = caseFileSpecificationService;
        }
        #endregion

        #region IRuleEngine Members
        public CaseFile Execute(Rule rule, CaseFile caseFile)
        {
            AppDomain tempAppDomain = null;
            try
            {
                CaseFileSpecification specification = caseFile.CaseFileSpecification;
                byte[] caseFileSpecificationAssemblyBuffer = _caseFileSpecificationService.GetAssemblyBytes(specification);
                string caseFileSpecificationAssemblyPath = string.Format("{0}.dll", specification.Name);
                File.WriteAllBytes(caseFileSpecificationAssemblyPath, caseFileSpecificationAssemblyBuffer);

                string rootEntityClassName = string.Format("{0}.{1}.{2}", specification.ObjectModel.Name, specification.Name, specification.Structure.Entity.Name);

                AppDomainSetup info = new AppDomainSetup()
                {
                    ApplicationBase = Environment.CurrentDirectory
                };
                tempAppDomain = AppDomain.CreateDomain("MyTempAppDomain", null, info);

                IFSharpRuleExecutor executor = CreateFSharpExecutor(tempAppDomain);
                string caseFileResult = executor.Execute(specification.Name, rootEntityClassName, rule.Name, rule.Script.Value, rule.Script.Method, caseFile.Text);

                CaseFile result = new CaseFile(caseFile);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(caseFileResult);
                result.Any = xmlDocument.DocumentElement;

                return result;
            }
            catch (Exception exception)
            {
                _logger.Debug(exception.Message, exception);
                throw;
            }
            finally
            {
                if (tempAppDomain != null)
                {
                    AppDomain.Unload(tempAppDomain);
                }
            }
        }
        #endregion

        #region Private Methods
        private IFSharpRuleExecutor CreateFSharpExecutor(AppDomain appDomain)
        {
            string assemblyName = typeof(FSharpRuleExecutor).Assembly.GetName().Name;
            _logger.DebugFormat("CreateFSharpExecutor(): assembly={0}, FSharpRuleExecutor={1}", assemblyName, typeof(FSharpRuleExecutor).FullName);

            object anObject = appDomain.CreateInstanceAndUnwrap(assemblyName, typeof(FSharpRuleExecutor).FullName);
            IFSharpRuleExecutor result = anObject as IFSharpRuleExecutor;

            return result;
        }
        #endregion
    }
}
