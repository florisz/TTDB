using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public class GetCaseFileSpecificationByVersionCommand : AbstractGetCaseFileSpecificationCommand, ICommand
    {
        #region Constructors
        public GetCaseFileSpecificationByVersionCommand(ICaseFileSpecificationService specificationService)
            : base(specificationService)
        {
        }
        #endregion

        #region AbstractGetCaseFileSpecificationCommand Members
        public override CaseFileSpecification GetCaseFileSpecification(CommandContext context)
        {
            CaseFileSpecification result = _specificationService.Get(context.RequestedId, context.VersionNumber, context.BaseUri);

            return result;
        }
        #endregion
    }
}
