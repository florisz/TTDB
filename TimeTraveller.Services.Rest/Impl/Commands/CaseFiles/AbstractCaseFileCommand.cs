using System.IO;
using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFiles
{
    public abstract class AbstractCaseFileCommand : ICommand
    {
        #region Private Properties
        private const string _caseFileBaseUriTemplate = "{0}/casefiles/";
        protected const string _caseFileSpecificationTemplate = "{0}/{1}";

        protected ICaseFileService _caseFileService;
        protected ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public AbstractCaseFileCommand(ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService)
        {
            _caseFileSpecificationService = caseFileSpecificationService;
            _caseFileService = caseFileService;
        }
        #endregion

        #region ICommand Members
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion

        #region Protected Methods
        protected string GetCaseFileId(CaseFileSpecification specification, CommandContext context)
        {
            string caseFileBaseUri = string.Format(_caseFileBaseUriTemplate, context.BaseUri);
            string result = context.GetRelativeUri(caseFileBaseUri);
            result = context.StripQueryParameters(result);

            _caseFileSpecificationService.ValidateCaseFileId(specification, result);

            return result;
        }        
        #endregion
    }
}
