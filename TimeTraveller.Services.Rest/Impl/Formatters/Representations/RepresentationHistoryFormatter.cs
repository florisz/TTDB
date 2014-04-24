using System.IO;

using TimeTraveller.Services.Representations;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.Representations
{
    public class RepresentationHistoryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IRepresentationService RepresentationService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public RepresentationHistoryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            Representation representation = item as Representation;

            string xml = RepresentationService.GetHistory(representation, context.BaseUri, context.Encoding);
            
            return Chain.Format(context, xml);
        }

        #endregion
    }
}
