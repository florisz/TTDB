using System.IO;

using Luminis.Its.Services.CaseFiles;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.CaseFiles
{
    public class CaseFileHistoryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileService CaseFileService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileHistoryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members
        public override Stream Format(CommandContext context, object item)
        {
            var caseFile = item as CaseFile;

            string xml = CaseFileService.GetHistory(caseFile, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }
        #endregion
    }
}
