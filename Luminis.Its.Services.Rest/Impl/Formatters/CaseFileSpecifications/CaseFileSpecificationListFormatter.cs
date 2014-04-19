using System.Collections.Generic;
using System.IO;

using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.CaseFileSpecifications.Impl;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters.CaseFileSpecifications
{
    public class CaseFileSpecificationListFormatter: AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileSpecificationService SpecificationService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileSpecificationListFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            IEnumerable<CaseFileSpecification> caseFileSpecifications = item as IEnumerable<CaseFileSpecification>;

            string xml = SpecificationService.GetList(caseFileSpecifications, context.BaseUri, context.Encoding);
            
            return Chain.Format(context, xml);
        }

        #endregion
    }
}
