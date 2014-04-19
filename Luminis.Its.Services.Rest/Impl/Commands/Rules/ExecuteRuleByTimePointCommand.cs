using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Rules;

namespace Luminis.Its.Services.Rest.Impl.Commands.Rules
{
    public class ExecuteRuleByTimePointCommand : AbstractExecuteRuleCommand, ICommand
    {
        #region Constructors
        public ExecuteRuleByTimePointCommand(IRuleService ruleService, ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService)
            : base(ruleService, caseFileSpecificationService, caseFileService)
        {
        }
        #endregion

        public override Rule GetRule(CommandContext context)
        {
            return _ruleService.Get(context.RequestedId, context.TimePoint, context.BaseUri);
        }
    }
}
