using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.Rest.Impl.Commands.ObjectModels
{
    public class GetObjectModelByVersionCommand : AbstractGetObjectModelCommand, ICommand
    {
        #region Constructors
        public GetObjectModelByVersionCommand(IObjectModelService objectModelService)
            : base(objectModelService)
        {
        }
        #endregion

        #region AbstractGetObjectModelCommand Members
        public override ObjectModel GetObjectModel(CommandContext context)
        {
            return _objectModelService.Get(context.RequestedId, context.VersionNumber, context.BaseUri);
        }
        #endregion
    }
}
