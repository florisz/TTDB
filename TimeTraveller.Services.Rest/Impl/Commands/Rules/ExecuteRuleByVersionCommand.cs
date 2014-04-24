using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public class ExecuteRuleByVersionCommand : AbstractExecuteRuleCommand, ICommand
    {
        #region Constructors
        public ExecuteRuleByVersionCommand(IRuleService ruleService, ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService)
            : base(ruleService, caseFileSpecificationService, caseFileService)
        {
        }
        #endregion

        public override Rule GetRule(CommandContext context)
        {
            return _ruleService.Get(context.RequestedId, context.VersionNumber, context.BaseUri); 
        }
    }
}
