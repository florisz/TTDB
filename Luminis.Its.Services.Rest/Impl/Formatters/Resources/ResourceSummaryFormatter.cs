using System.IO;

using Luminis.Its.Services.Resources;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.Resources
{
    public class ResourceSummaryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IResourceService ResourceService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public ResourceSummaryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            Resource resource = item as Resource;

            string xml = ResourceService.GetSummary(resource, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }

        #endregion
    }
}
