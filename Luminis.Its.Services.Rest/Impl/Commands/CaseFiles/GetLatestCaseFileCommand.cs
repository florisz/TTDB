using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Rules;

namespace Luminis.Its.Services.Rest.Impl.Commands.CaseFiles
{
    public class GetLatestCaseFileCommand : AbstractGetCaseFileCommand, ICommand
    {
        #region Constructors
        public GetLatestCaseFileCommand(ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService, IRuleService ruleService)
            : base(caseFileSpecificationService, caseFileService, ruleService)
        {
        }
        #endregion

        #region AbstractGetCaseFileCommand Members
        public override CaseFile GetCaseFile(CaseFileSpecification caseFileSpecification, string caseFileId, CommandContext context)
        {
            CaseFile result = _caseFileService.Get(caseFileSpecification, caseFileId, context.BaseUri);
            return result;
        }
        
        public override Rule GetRule(CommandContext context)
        {
        	Rule result = this.RuleService.Get(context.RequestedId, context.BaseUri);
            return result;
        }
        #endregion
    }
}
