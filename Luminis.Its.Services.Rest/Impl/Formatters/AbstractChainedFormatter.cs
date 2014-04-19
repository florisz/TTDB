using System.IO;

namespace Luminis.Its.Services.Rest.Impl.Formatters
{
    public abstract class AbstractChainedFormatter : IFormatter
    {
        #region Public Properties
        public abstract IFormatter Chain { get; set; }
        #endregion

        #region IFormatter Members
        public abstract Stream Format(CommandContext context, object item);
        #endregion
    }
}
