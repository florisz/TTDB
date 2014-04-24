using TimeTraveller.Services.Representations;

namespace TimeTraveller.Services.Rest.Impl.Commands.Representations
{
    public class GetRepresentationByVersionCommand : AbstractGetRepresentationCommand, ICommand
    {
        #region Constructors
        public GetRepresentationByVersionCommand(IRepresentationService representationService)
            : base(representationService)
        {
        }
        #endregion

        #region AbstractGetRepresentationCommand Members
        public override Representation GetRepresentation(CommandContext context)
        {
            Representation result = _representationService.Get(context.RequestedId, context.VersionNumber, context.BaseUri);

            return result;
        }
        #endregion
    }
}
