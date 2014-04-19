using System.IO;

using Luminis.Its.Services.Resources;

namespace Luminis.Its.Services.Rest.Impl.Formatters.Resources
{
    /// <summary>
    /// The ResourceFormatter will output the resource as a stream will 
    /// set the the content type of response according to the content-type of the resource.
    /// </summary>
    public class ResourceFormatter: IFormatter
    {
        #region Private Properties
        private IResourceService _resourceService;
        #endregion

        #region Constructors
        public ResourceFormatter(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        #endregion

        #region IFormatter Members

        public Stream Format(CommandContext context, object item)
        {
            Resource resource = item as Resource;

            // We do not use a chained formatter here.
            Stream result = new MemoryStream(resource.Content);
            context.ContentType = WebOperationContentType.Other;
            context.Response.ContentType = resource.ContentType;

            return result;
        }

        #endregion
    }
}
