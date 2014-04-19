using System.IO;

using Luminis.Its.Services.Repository;

namespace Luminis.Its.Services.Rest.Impl.Commands.Repository
{
    public class GetRepositoryListCommand : AbstractRepositoryCommand, ICommand
    {
        #region Constructors
        public GetRepositoryListCommand(IRepositoryService repositoryService)
            : base(repositoryService)
        {
        }
        #endregion

        #region AbstractRepositoryCommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string xml = _repositoryService.GetList(context.BaseUri, context.Encoding);

            Stream result = formatter.Format(context, xml);

            return result;
        } 
        #endregion
    }
}
