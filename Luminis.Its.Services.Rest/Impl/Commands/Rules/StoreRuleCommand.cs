using System.IO;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Rules;

namespace Luminis.Its.Services.Rest.Impl.Commands.Rules
{
    public class StoreRuleCommand : AbstractRuleCommand, ICommand
    {
        #region Private Properties
        private const string _caseFileSpecificationTemplate = "{0}/{1}";

        private ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public StoreRuleCommand(IRuleService ruleService, ICaseFileSpecificationService caseFileSpecificationService)
            : base(ruleService)
        {
            _caseFileSpecificationService = caseFileSpecificationService;
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string ruleXml = context.RequestBody;
            Rule rule = _ruleService.Convert(ruleXml, context.Encoding);
            string caseFileSpecificationId = string.Format(_caseFileSpecificationTemplate, context.Arguments[0], context.Arguments[1]);
            rule.CaseFileSpecification = _caseFileSpecificationService.Get(caseFileSpecificationId, context.BaseUri);

            _ruleService.Store(context.RequestedId, rule, context.BaseUri, context.JournalInfo);

            Stream result = formatter.Format(context, rule);

            return result;
        }
        #endregion
    }
}
