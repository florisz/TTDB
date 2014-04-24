using System.IO;

using TimeTraveller.Services.CaseFiles;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles
{
    public class CaseFileSummaryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileService CaseFileService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileSummaryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members
        public override Stream Format(CommandContext context, object item)
        {
            CaseFile caseFile = item as CaseFile;

            string xml = CaseFileService.GetSummary(caseFile, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }
        #endregion
    }
}
