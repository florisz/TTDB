using Luminis.Its.Services.Resources;

namespace Luminis.Its.Services.Rest.Impl.Commands.Resources
{
    public class GetLatestResourceCommand : AbstractGetResourceCommand, ICommand
    {
        #region Constructors
        public GetLatestResourceCommand(IResourceService resourceService)
            : base(resourceService)
        {
        }
        #endregion

        #region AbstractGetResourceCommand Members
        public override Resource GetResource(string resourceId, CommandContext context)
        {
            Resource result = _resourceService.Get(resourceId, context.BaseUri);

            return result;
        }
        #endregion
    }
}
