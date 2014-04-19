using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Luminis.Xml;
using Microsoft.FSharp.Compiler.CodeDom;

namespace Luminis.Its.Services.Rules.FSharp
{
    public class FSharpRuleExecutor : MarshalByRefObject, IFSharpRuleExecutor
    {
        #region IFSharpRuleExecutor Members

        public string Execute(string caseFileSpecificationAssemblyPath, string rootEntityClassname, string ruleName, string ruleScript, string methodName, string caseFileContentXml)
        {
            Assembly caseFileSpecificationAssembly = AppDomain.CurrentDomain.Load(caseFileSpecificationAssemblyPath);

            Assembly fsharpAssembly = CreateFSharpAssembly(ruleName, ruleScript, caseFileSpecificationAssembly);

            string result = CallFSharpRule(fsharpAssembly, methodName, caseFileSpecificationAssembly, rootEntityClassname, caseFileContentXml);

            return result;
        }

        #endregion

        #region Private Methods
        private string CallFSharpRule(Assembly fsharpAssembly, string methodName, Assembly caseFileSpecificationAssembly, string caseFileRootEntityName, string caseFileContentXml)
        {
            MethodInfo fsharpMethod = GetRuleMethod(fsharpAssembly, methodName);

            Type rootEntityType = GetCaseFileEntityType(caseFileSpecificationAssembly, caseFileRootEntityName);

            object caseFileEntity = XmlHelper.FromXml(caseFileContentXml, rootEntityType);

            fsharpMethod.Invoke(null, new object[] { caseFileEntity });

            string result = XmlHelper.ToXml(caseFileEntity, rootEntityType, true);

            return result;
        }

        private Assembly CreateFSharpAssembly(string ruleName, string ruleScript, Assembly caseFileSpecificationAssembly)
        {
            if (!File.Exists(caseFileSpecificationAssembly.Location))
            {
                throw new FileNotFoundException(string.Format("Cannot find assembly {0}", caseFileSpecificationAssembly.Location), caseFileSpecificationAssembly.Location);
            }

            FSharpCodeProvider provider = new FSharpCodeProvider();
            
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add(caseFileSpecificationAssembly.Location);

            CompilerResults compilerResults = provider.CompileAssemblyFromSource(parameters, ruleScript);
            if (compilerResults.Errors.HasErrors || compilerResults.Errors.HasWarnings)
            {
                StringBuilder errors = new StringBuilder();
                errors.AppendLine(string.Format("Compile errors in rule {0}.", ruleName));

                foreach (CompilerError e in compilerResults.Errors)
                {
                    errors.AppendLine(string.Format("{0}:{1}: {2} - {3}", e.Line, e.Column, e.ErrorNumber, e.ErrorText));
                }

                throw new ArgumentException(errors.ToString());
            }

            Assembly  result = compilerResults.CompiledAssembly;

            return result;
        }

        private Type GetCaseFileEntityType(Assembly caseFileSpecificationAssembly, string rootEntityClassname)
        {
            Type result = caseFileSpecificationAssembly.GetType(rootEntityClassname);

            return result;
        }

        private MethodInfo GetRuleMethod(Assembly fsharpAssembly, string methodName)
        {
            string fsharpModuleName = string.Empty;
            string fsharpMethodName = string.Empty;
            int indexOfDot = methodName.LastIndexOf('.');
            if (indexOfDot > -1)
            {
                fsharpModuleName = methodName.Substring(0, indexOfDot);
                fsharpMethodName = methodName.Substring(indexOfDot + 1);
            }
            else
            {
                throw new ArgumentException(string.Format("The method name {0} does not contain '.'", methodName));
            }

            Type fsharpModule = fsharpAssembly.GetType(fsharpModuleName);
            if (fsharpModule == null)
            {
                throw new ArgumentNullException("fsharpModule", string.Format("Cannot find module {0}", fsharpModuleName));
            }

            MethodInfo result = fsharpModule.GetMethod(fsharpMethodName);
            if (result == null)
            {
                throw new ArgumentNullException("fsharpMethod", string.Format("Cannot find method {0} in module {1}", fsharpMethodName, fsharpModuleName));
            }

            return result;
        }
        #endregion
    }
}
