namespace TimeTraveller.Services.Rules.FSharp
{
    public interface IFSharpRuleExecutor
    {
        string Execute(string caseFileSpecificationAssemblyPath, string rootEntityClassname, string ruleName, string ruleScript, string methodName, string caseFileContentXml);
    }
}
