using System.Collections.Generic;
using System.IO;

using Luminis.Its.Services.ObjectModels;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.ObjectModels
{
    public class ObjectModelListFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public IObjectModelService ObjectModelService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public ObjectModelListFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            IEnumerable<ObjectModel> objectModels = item as IEnumerable<ObjectModel>;

            string xml = ObjectModelService.GetList(objectModels, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }

        #endregion
    }
}
