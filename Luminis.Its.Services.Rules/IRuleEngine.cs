using System;

using Luminis.Its.Services.CaseFiles;

namespace Luminis.Its.Services.Rules
{
    public interface IRuleEngine
    {
        CaseFile Execute(Rule rule, CaseFile caseFile);
    }
}
