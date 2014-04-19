using System.IO;

using Luminis.Its.Services.Rules;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.Rules
{
    public class RuleSummaryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IRuleService RuleService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public RuleSummaryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            Rule rule = item as Rule;

            string xml = RuleService.GetSummary(rule, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }

        #endregion
    }
}
