using System.IO;

using TimeTraveller.Services.CaseFiles;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles
{
    public class CaseFileXmlFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileService CaseFileService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileXmlFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            CaseFile caseFile = item as CaseFile;

            string xml = CaseFileService.GetXml(caseFile, context.Encoding);

            return Chain.Format(context, xml);
        }

        #endregion
    }
}
