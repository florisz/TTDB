using System;
using System.IO;
using System.Net;
using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFiles
{
    public class StoreCaseFileCommand : AbstractCaseFileCommand, ICommand
    {
        #region Constructors
        public StoreCaseFileCommand(ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService)
            : base(caseFileSpecificationService, caseFileService)
        {
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

            string caseFileId = GetCaseFileId(caseFileSpecification, context);

            string caseFileXml = context.RequestBody;
            CaseFile caseFile = _caseFileService.Convert(caseFileXml, context.Encoding);
            caseFile.CaseFileSpecification = caseFileSpecification;

            bool isCreated = _caseFileService.Store(caseFileSpecification, caseFileId, caseFile, context.BaseUri, context.JournalInfo);

            Stream result = formatter.Format(context, caseFile);
            if (isCreated)
            {
                context.Response.StatusCode = HttpStatusCode.Created;
            }

            return result;
        }
        #endregion
    }
}
