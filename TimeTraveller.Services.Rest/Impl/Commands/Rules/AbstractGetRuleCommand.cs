using System.IO;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public abstract class AbstractGetRuleCommand : AbstractRuleCommand, ICommand
    {
        #region Constructors
        public AbstractGetRuleCommand(IRuleService ruleService)
            : base(ruleService)
        {
        }
        #endregion

        #region Abstract Members
        public abstract Rule GetRule(CommandContext context);
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            Rule rule = GetRule(context);

            Stream result = formatter.Format(context, rule);

            return result;
        }
        #endregion
    }
}
