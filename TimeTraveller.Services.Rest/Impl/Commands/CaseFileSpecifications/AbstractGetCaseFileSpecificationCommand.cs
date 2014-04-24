using System.IO;
using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public abstract class AbstractGetCaseFileSpecificationCommand : AbstractCaseFileSpecificationCommand, ICommand
    {
        #region Constructors
        public AbstractGetCaseFileSpecificationCommand(ICaseFileSpecificationService specificationService)
            : base(specificationService)
        {
        }
        #endregion

        #region Abstract Members
        public abstract CaseFileSpecification GetCaseFileSpecification(CommandContext context);
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            CaseFileSpecification specification = GetCaseFileSpecification(context);

            Stream result = formatter.Format(context, specification);

            return result;
        }
        #endregion
    }
}
