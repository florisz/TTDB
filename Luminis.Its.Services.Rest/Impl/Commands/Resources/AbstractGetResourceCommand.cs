using System.IO;
using Luminis.Its.Services.Resources;

namespace Luminis.Its.Services.Rest.Impl.Commands.Resources
{
    public abstract class AbstractGetResourceCommand : AbstractResourceCommand, ICommand
    {
        #region Constructors
        public AbstractGetResourceCommand(IResourceService resourceService)
            : base(resourceService)
        {
        }
        #endregion

        #region Abstract Members
        public abstract Resource GetResource(string resourceId, CommandContext context);
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string resourceId = GetResourceId(context);
            Resource resource = GetResource(resourceId, context);

            Stream result = formatter.Format(context, resource);

            return result;
        }
        #endregion
    }
}
