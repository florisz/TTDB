using System.Collections.Generic;
using System.IO;

using Luminis.Its.Services.Representations;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.Representations
{
    public class RepresentationListFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IRepresentationService RepresentationService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public RepresentationListFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            IEnumerable<Representation> representations = item as IEnumerable<Representation>;

            string xml = RepresentationService.GetList(representations, context.BaseUri, context.Encoding);
            
            return Chain.Format(context, xml);
        }

        #endregion
    }
}
