using System.IO;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.ObjectModels;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public class StoreCaseFileSpecificationCommand : AbstractCaseFileSpecificationCommand, ICommand
    {
        #region Private Properties
        private IObjectModelService _objectModelService;
        #endregion

        #region Constructors
        public StoreCaseFileSpecificationCommand(ICaseFileSpecificationService caseFileSpecificationService, IObjectModelService objectModelService)
            : base(caseFileSpecificationService)
        {
            _objectModelService = objectModelService;
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string caseFileSpecificationXml = context.RequestBody;
            CaseFileSpecification caseFileSpecification = _specificationService.Convert(caseFileSpecificationXml, context.Encoding);

            caseFileSpecification.ObjectModel = _objectModelService.Get((string)context.Arguments[0], context.BaseUri);

            _specificationService.Store(context.RequestedId, caseFileSpecification, context.BaseUri, context.JournalInfo); 

            Stream result = formatter.Format(context, caseFileSpecification);

            return result;
        }
        #endregion
    }
}
