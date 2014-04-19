using System.IO;
using System.Net;
using Luminis.Its.Services.Resources;

namespace Luminis.Its.Services.Rest.Impl.Commands.Resources
{
    public class StoreResourceCommand : AbstractResourceCommand, ICommand
    {
        #region Constructors
        public StoreResourceCommand(IResourceService resourceService)
            : base(resourceService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string resourceId = GetResourceId(context);
            Resource resource = new Resource()
            {
                Content = context.RequestBodyBuffer,
                ContentType = context.Request.Headers[HttpRequestHeader.ContentType]
            };

            _resourceService.Store(resourceId, resource, context.BaseUri, context.JournalInfo);

            Stream result = formatter.Format(context, resource);

            return result;
        }
        #endregion
    }
}
