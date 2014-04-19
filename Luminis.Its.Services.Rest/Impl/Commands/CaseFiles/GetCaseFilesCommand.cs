using System;
using System.Collections.Generic;
using System.IO;
using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Rules;

namespace Luminis.Its.Services.Rest.Impl.Commands.CaseFiles
{
    public class GetCaseFilesCommand : AbstractGetCaseFileCommand, ICommand
    {
        #region Constructors
        public GetCaseFilesCommand(ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService, IRuleService ruleService)
            : base(caseFileSpecificationService, caseFileService, ruleService)
        {
        }
        #endregion

        #region overrides
        public override Rule GetRule(CommandContext context)
        {
            Rule result = this.RuleService.Get(context.RequestedId, context.BaseUri);
            return result;
        }
                
        public override CaseFile GetCaseFile(CaseFileSpecification caseFileSpecification, string caseFileId, CommandContext context)
        {
            CaseFile result = _caseFileService.Get(caseFileSpecification, caseFileId, context.BaseUri);
            return result;
        }
        #endregion
        
        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string specificationId = string.Format(_caseFileSpecificationTemplate, context.Arguments[0], context.Arguments[1]);
            CaseFileSpecification caseFileSpecification = _caseFileSpecificationService.Get(specificationId, context.BaseUri);
            if (caseFileSpecification == null)
            {
                throw new ArgumentOutOfRangeException("specificationname", string.Format("Unknown specification {0}/{1} in uri {1}", context.Arguments[0], context.Arguments[1], context.RequestUri));
            }

            IEnumerable<CaseFile> caseFiles = _caseFileService.GetEnumerable(caseFileSpecification, context.BaseUri);

            Stream result = formatter.Format(context, caseFiles);

            return result;
        }
        #endregion
    }
}
