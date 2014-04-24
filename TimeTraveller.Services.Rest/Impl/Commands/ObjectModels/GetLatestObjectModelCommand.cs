using TimeTraveller.Services.ObjectModels;

namespace TimeTraveller.Services.Rest.Impl.Commands.ObjectModels
{
    public class GetLatestObjectModelCommand : AbstractGetObjectModelCommand, ICommand
    {
        #region Constructors
        public GetLatestObjectModelCommand(IObjectModelService objectModelService) 
            : base(objectModelService)
        {
        }
        #endregion

        #region AbstractGetObjectModelCommand Members
        public override ObjectModel GetObjectModel(CommandContext context)
        {
            return _objectModelService.Get(context.RequestedId, context.BaseUri);
        }
        #endregion

    }
}
