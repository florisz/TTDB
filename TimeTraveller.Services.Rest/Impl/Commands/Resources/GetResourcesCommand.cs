using System.Collections.Generic;
using System.IO;
using TimeTraveller.Services.Resources;

namespace TimeTraveller.Services.Rest.Impl.Commands.Resources
{
    public class GetResourcesCommand : AbstractResourceCommand, ICommand
    {
        #region Constructors
        public GetResourcesCommand(IResourceService resourceService)
            : base(resourceService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            IEnumerable<Resource> resources = _resourceService.GetEnumerable(context.BaseUri);

            Stream result = formatter.Format(context, resources);

            return result;
        }
        #endregion
    }
}
