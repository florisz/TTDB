using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public class GetRuleByVersionCommand : AbstractGetRuleCommand, ICommand
    {
        #region Constructors
        public GetRuleByVersionCommand(IRuleService ruleService)
            : base(ruleService)
        {
        }
        #endregion

        #region AbstractGetRuleCommand Members
        public override Rule GetRule(CommandContext context)
        {
            Rule result  = _ruleService.Get(context.RequestedId, context.VersionNumber, context.BaseUri);

            return result;
        }
        #endregion
    }
}
