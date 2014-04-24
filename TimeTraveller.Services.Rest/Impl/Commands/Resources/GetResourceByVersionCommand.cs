using TimeTraveller.Services.Resources;

namespace TimeTraveller.Services.Rest.Impl.Commands.Resources
{
    public class GetResourceByVersionCommand : AbstractGetResourceCommand, ICommand
    {
        #region Constructors
        public GetResourceByVersionCommand(IResourceService resourceService)
            : base(resourceService)
        {
        }
        #endregion

        #region AbstractGetResourceCommand Members
        public override Resource GetResource(string resourceId, CommandContext context)
        {
            Resource result = _resourceService.Get(resourceId, context.VersionNumber, context.BaseUri);

            return result;
        }
        #endregion
    }
}
