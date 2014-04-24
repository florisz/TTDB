using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFiles
{
    public class GetCaseFileByTimePointCommand : AbstractGetCaseFileCommand, ICommand
    {
        #region Constructors
        public GetCaseFileByTimePointCommand(ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService, IRuleService ruleService)
            : base(caseFileSpecificationService, caseFileService, ruleService)
        {
        }
        #endregion

        #region AbstractGetCaseFileCommand Members
        public override CaseFile GetCaseFile(CaseFileSpecification caseFileSpecification, string caseFileId, CommandContext context)
        {
            CaseFile result = _caseFileService.Get(caseFileSpecification, caseFileId, context.TimePoint, context.BaseUri);

            return result;
        }
        
        public override Rule GetRule(CommandContext context)
        {
            Rule result = this.RuleService.Get(context.RequestedId, context.TimePoint, context.BaseUri);
            return result;
        }

        #endregion
    }
}
