using System.IO;
using Luminis.Its.Services.ObjectModels;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters
{
    public class ChainedXmlFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public ChainedXmlFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members
        public override Stream Format(CommandContext context, object item)
        {
            Stream result = null;
            if (Chain != null)
            {
                result = Chain.Format(context, item);
            }
            else
            {
                string xml = (string)item;
                byte[] resultBuffer = context.Encoding.GetBytes(xml);
                result = new MemoryStream(resultBuffer);
                context.ContentType = context.RequestedContentType;
            }

            return result;
        }
        #endregion
    }
}
