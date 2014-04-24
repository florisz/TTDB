using System.IO;
using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFileSpecifications
{
    public abstract class AbstractCaseFileSpecificationCommand : ICommand
    {
        #region Private Properties
        protected ICaseFileSpecificationService _specificationService;
        #endregion

        #region Constructors
        public AbstractCaseFileSpecificationCommand(ICaseFileSpecificationService specificationService)
        {
            _specificationService = specificationService;
        }
        #endregion

        #region ICommand Members
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion
    }
}
