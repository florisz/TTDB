using Luminis.Its.Services.Representations;

namespace Luminis.Its.Services.Rest.Impl.Commands.Representations
{
    public class GetRepresentationByTimePointCommand : AbstractGetRepresentationCommand, ICommand
    {
        #region Constructors
        public GetRepresentationByTimePointCommand(IRepresentationService representationService)
            : base(representationService)
        {
        }
        #endregion

        #region AbstractGetRepresentationCommand Members
        public override Representation GetRepresentation(CommandContext context)
        {
            Representation result  = _representationService.Get(context.RequestedId, context.TimePoint, context.BaseUri);

            return result;
        }
        #endregion
    }
}
