using System.IO;
using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.Rules
{
    public abstract class AbstractExecuteRuleCommand : AbstractRuleCommand, ICommand
    {
        #region Private Properties
        private const string _caseFileSpecificationTemplate = "{0}/{1}";

        protected ICaseFileService _caseFileService;
        protected ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public AbstractExecuteRuleCommand(IRuleService ruleService, ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService)
            : base(ruleService)
        {
            _caseFileService = caseFileService;
            _caseFileSpecificationService = caseFileSpecificationService;
        }
        #endregion

        #region
        public abstract Rule GetRule(CommandContext context);
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            Rule rule = GetRule(context);

            string caseFileXml = context.RequestBody;
            CaseFile caseFile = _caseFileService.Convert(caseFileXml, context.Encoding);
            string caseFileSpecificationId = string.Format(_caseFileSpecificationTemplate, context.Arguments[0], context.Arguments[1]);
            caseFile.CaseFileSpecification = _caseFileSpecificationService.Get(caseFileSpecificationId, context.BaseUri);

            CaseFile resultingCaseFile = _ruleService.Execute(rule, caseFile);

            Stream result = formatter.Format(context, resultingCaseFile);

            return result;
        }
        #endregion
    }
}
