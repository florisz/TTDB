using Luminis.Its.Services.Representations;

namespace Luminis.Its.Services.Rest.Impl.Commands.Representations
{
    public class GetLatestRepresentationCommand : AbstractGetRepresentationCommand, ICommand
    {
        #region Constructors
        public GetLatestRepresentationCommand(IRepresentationService representationService)
            : base(representationService)
        {
        }
        #endregion

        #region ICommand Members
        public override Representation GetRepresentation(CommandContext context)
        {
            Representation result = _representationService.Get(context.RequestedId, context.BaseUri);

            return result;
        }
        #endregion
    }
}
