using System.IO;
using Luminis.Its.Services.Resources;

namespace Luminis.Its.Services.Rest.Impl.Commands.Resources
{
    public abstract class AbstractResourceCommand : ICommand
    {
        #region Private Properties
        private const string _resourceBaseUriTemplate = "{0}/resources/";
        protected IResourceService _resourceService;
        #endregion

        #region Constructors
        public AbstractResourceCommand(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        #endregion

        #region ICommand Members
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion

        #region Protected Methods
        protected string GetResourceId(CommandContext context)
        {
            string resourceBaseUri = string.Format(_resourceBaseUriTemplate, context.BaseUri);
            string result = context.GetRelativeUri(resourceBaseUri);
            result = context.StripQueryParameters(result);

            return result;
        }
        #endregion
    }
}
