using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.Rest.Impl.Commands.ObjectModels
{
    public class GetObjectModelByTimePointCommand : AbstractGetObjectModelCommand, ICommand
    {
        #region Constructors
        public GetObjectModelByTimePointCommand(IObjectModelService objectModelService)
            : base(objectModelService)
        {
        }
        #endregion

        #region AbstractGetObjectModelCommand Members
        public override ObjectModel GetObjectModel(CommandContext context)
        {
            return _objectModelService.Get(context.RequestedId, context.TimePoint, context.BaseUri);
        }
        #endregion
    }
}
