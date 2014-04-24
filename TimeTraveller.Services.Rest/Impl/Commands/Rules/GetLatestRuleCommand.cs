using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public class GetLatestRuleCommand : AbstractGetRuleCommand, ICommand
    {
        #region Constructors
        public GetLatestRuleCommand(IRuleService ruleService)
            : base(ruleService)
        {
        }
        #endregion

        #region AbstractGetRuleCommand Members
        public override Rule GetRule(CommandContext context)
        {
            Rule result = _ruleService.Get(context.RequestedId, context.BaseUri);

            return result;
        }
        #endregion
    }
}
