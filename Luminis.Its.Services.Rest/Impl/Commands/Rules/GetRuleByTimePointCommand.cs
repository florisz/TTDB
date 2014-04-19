using Luminis.Its.Services.Rules;

namespace Luminis.Its.Services.Rest.Impl.Commands.Rules
{
    public class GetRuleByTimePointCommand : AbstractGetRuleCommand, ICommand
    {
        #region Constructors
        public GetRuleByTimePointCommand(IRuleService ruleService)
            : base(ruleService)
        {
        }
        #endregion

        #region AbstractGetRuleCommand Members
        public override Rule GetRule(CommandContext context)
        {
            Rule result = _ruleService.Get(context.RequestedId, context.TimePoint, context.BaseUri);

            return result;
        }
        #endregion
    }
}
