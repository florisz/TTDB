using System.IO;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public abstract class AbstractRuleCommand : ICommand
    {
        #region Private Properties
        protected IRuleService _ruleService;
        #endregion

        #region Constructors
        public AbstractRuleCommand(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }
        #endregion

        #region ICommand Members
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion
    }
}
