using System.IO;

using TimeTraveller.Services.Repository;

namespace TimeTraveller.Services.Rest.Impl.Commands.Repository
{
    public abstract class AbstractRepositoryCommand : ICommand
    {
        #region Private Properties
        protected IRepositoryService _repositoryService;
        #endregion

        #region Constructors
        public AbstractRepositoryCommand(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }
        #endregion

        #region ICommand Members
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion
    }
}
