using System.IO;

using Luminis.Its.Services.ObjectModels;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.ObjectModels
{
    public class ObjectModelSummaryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IObjectModelService ObjectModelService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public ObjectModelSummaryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            ObjectModel objectModel = item as ObjectModel;

            string xml = ObjectModelService.GetSummary(objectModel, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }

        #endregion
    }
}
