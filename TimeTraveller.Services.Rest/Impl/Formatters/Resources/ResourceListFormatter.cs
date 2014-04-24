using System.Collections.Generic;
using System.IO;

using TimeTraveller.Services.Resources;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.Resources
{
    public class ResourceListFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IResourceService ResourceService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public ResourceListFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            IEnumerable<Resource> resources = item as IEnumerable<Resource>;

            string xml = ResourceService.GetList(resources, context.BaseUri, context.Encoding);
            
            return Chain.Format(context, xml);
        }

        #endregion
    }
}
