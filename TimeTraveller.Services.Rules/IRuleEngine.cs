using TimeTraveller.Services.CaseFiles;

namespace TimeTraveller.Services.Rules
{
    public interface IRuleEngine
    {
        CaseFile Execute(Rule rule, CaseFile caseFile);
    }
}
