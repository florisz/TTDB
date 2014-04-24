using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public class GetLatestCaseFileSpecificationCommand : AbstractGetCaseFileSpecificationCommand,  ICommand
    {
        #region Constructors
        public GetLatestCaseFileSpecificationCommand(ICaseFileSpecificationService specificationService)
            : base(specificationService)
        {
        }
        #endregion

        #region AbstractGetCaseFileSpecificationCommand Members
        public override CaseFileSpecification GetCaseFileSpecification(CommandContext context)
        {
            CaseFileSpecification result = _specificationService.Get(context.RequestedId, context.BaseUri);

            return result;
        }
        #endregion
    }
}
