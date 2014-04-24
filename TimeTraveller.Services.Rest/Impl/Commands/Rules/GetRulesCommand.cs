using System.Collections.Generic;
using System.IO;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public class GetRulesCommand : AbstractRuleCommand, ICommand
    {
        #region Constructors
        public GetRulesCommand(IRuleService ruleService)
            : base(ruleService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            IEnumerable<Rule> rules = _ruleService.GetEnumerable((string)context.Arguments[0], (string)context.Arguments[1], context.BaseUri);

            Stream result = formatter.Format(context, rules);

            return result;
        }
        #endregion
    }
}
