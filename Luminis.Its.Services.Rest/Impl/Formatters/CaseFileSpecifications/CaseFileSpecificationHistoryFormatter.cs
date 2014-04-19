using System.IO;
using Luminis.Its.Services.CaseFileSpecifications;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.CaseFileSpecifications
{
    public class CaseFileSpecificationHistoryFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileSpecificationService SpecificationService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileSpecificationHistoryFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            CaseFileSpecification specification = item as CaseFileSpecification;

            string xml = SpecificationService.GetHistory(specification, context.BaseUri, context.Encoding);

            return Chain.Format(context, xml);
        }

        #endregion
    }
}
