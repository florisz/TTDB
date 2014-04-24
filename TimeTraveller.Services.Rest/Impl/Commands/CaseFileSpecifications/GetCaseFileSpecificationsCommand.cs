using System.Collections.Generic;
using System.IO;
using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public class GetCaseFileSpecificationsCommand : AbstractCaseFileSpecificationCommand, ICommand
    {
        #region Constructors
        public GetCaseFileSpecificationsCommand(ICaseFileSpecificationService caseFileSpecificationService)
            : base(caseFileSpecificationService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            IEnumerable<CaseFileSpecification> caseFileSpecifications = _specificationService.GetEnumerable((string)context.Arguments[0], context.BaseUri);

            Stream result = formatter.Format(context, caseFileSpecifications);

            return result;
        }
        #endregion
    }
}
