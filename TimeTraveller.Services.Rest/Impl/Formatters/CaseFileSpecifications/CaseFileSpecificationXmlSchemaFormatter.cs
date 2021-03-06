﻿using System.IO;

using TimeTraveller.Services.CaseFileSpecifications;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFileSpecifications
{
    public class CaseFileSpecificationXmlSchemaFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileSpecificationService SpecificationService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileSpecificationXmlSchemaFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members

        public override Stream Format(CommandContext context, object item)
        {
            CaseFileSpecification specification = item as CaseFileSpecification;

            string xmlSchemaText = SpecificationService.GetXmlSchema(specification, context.Encoding);

            return Chain.Format(context, xmlSchemaText);
        }

        #endregion
    }
}
