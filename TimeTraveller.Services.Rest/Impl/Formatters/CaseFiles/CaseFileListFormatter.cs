using System.Collections.Generic;
using System.IO;

using TimeTraveller.Services.CaseFiles;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles
{
    public class CaseFileListFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileService CaseFileService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileListFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members
        public override Stream Format(CommandContext context, object item)
        {
            IEnumerable<CaseFile> caseFiles = item as IEnumerable<CaseFile>;

            string xml = CaseFileService.GetList(caseFiles, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }
        #endregion
    }
}
