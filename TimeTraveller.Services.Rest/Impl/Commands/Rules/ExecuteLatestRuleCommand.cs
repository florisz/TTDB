using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public class ExecuteLatestRuleCommand : AbstractExecuteRuleCommand, ICommand
    {
        #region Constructors
        public ExecuteLatestRuleCommand(IRuleService ruleService, ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService)
            : base(ruleService, caseFileSpecificationService, caseFileService)
        {
        }
        #endregion

        public override Rule GetRule(CommandContext context)
        {
            return _ruleService.Get(context.RequestedId, context.BaseUri);
        }
    }
}
