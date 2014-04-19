using System.IO;

using Luminis.Its.Services.Repository;

namespace Luminis.Its.Services.Rest.Impl.Commands.Repository
{
    public class GetRepositoryXmlSchemasCommand : AbstractRepositoryCommand, ICommand
    {
        #region Constructors
        public GetRepositoryXmlSchemasCommand(IRepositoryService repositoryService)
            : base(repositoryService)
        {
        }
        #endregion

        #region AbstractRepositoryCommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string text = _repositoryService.GetXmlSchemas(context.BaseUri, context.Encoding);

            Stream result = formatter.Format(context, text);

            return result;
        }
        #endregion
    }
}
