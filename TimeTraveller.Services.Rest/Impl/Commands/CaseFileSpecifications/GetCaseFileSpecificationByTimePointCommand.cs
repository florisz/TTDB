using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public class GetCaseFileSpecificationByTimePointCommand : AbstractGetCaseFileSpecificationCommand, ICommand
    {
        #region Constructors
        public GetCaseFileSpecificationByTimePointCommand(ICaseFileSpecificationService specificationService)
            : base(specificationService)
        {
        }
        #endregion

        #region AbstractGetCaseFileSpecificationCommand Members
        public override CaseFileSpecification GetCaseFileSpecification(CommandContext context)
        {
            CaseFileSpecification result = _specificationService.Get(context.RequestedId, context.TimePoint, context.BaseUri);

            return result;
        }
        #endregion
    }
}
