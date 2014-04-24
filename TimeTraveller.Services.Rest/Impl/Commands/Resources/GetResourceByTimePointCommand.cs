using TimeTraveller.Services.Resources;

namespace TimeTraveller.Services.Rest.Impl.Commands.Resources
{
    public class GetResourceByTimePointCommand : AbstractGetResourceCommand, ICommand
    {
        #region Constructors
        public GetResourceByTimePointCommand(IResourceService resourceService)
            : base(resourceService)
        {
        }
        #endregion

        #region AbstractGetResourceCommand Members
        public override Resource GetResource(string resourceId, CommandContext context)
        {
            Resource result = _resourceService.Get(resourceId, context.TimePoint, context.BaseUri);

            return result;
        }
        #endregion
    }
}
