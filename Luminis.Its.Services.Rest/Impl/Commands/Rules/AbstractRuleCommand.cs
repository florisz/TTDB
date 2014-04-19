using System.IO;
using Luminis.Its.Services.Rules;

namespace Luminis.Its.Services.Rest.Impl.Commands.Rules
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
