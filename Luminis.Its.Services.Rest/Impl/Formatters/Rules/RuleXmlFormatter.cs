using System.IO;

using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.Rules;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.Rules
{
    public class RuleXmlFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IRuleService RuleService { get; set; }

        [Dependency("CaseFileFormatter")]
        public IFormatter CaseFileFormatter { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public RuleXmlFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            Stream result = null;
            if (typeof(Rule).IsAssignableFrom(item.GetType()))
            {
                Rule rule = item as Rule;
                string xml = RuleService.GetXml(rule, context.Encoding);
                result = Chain.Format(context, xml);
            }
            else if (typeof(CaseFile).IsAssignableFrom(item.GetType()))
            {
                CaseFile caseFile = item as CaseFile;
                result = CaseFileFormatter.Format(context, caseFile);
            }
            return result;
        }

        #endregion
    }
}
