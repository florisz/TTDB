using System.IO;

using TimeTraveller.Services.Repository;

namespace TimeTraveller.Services.Rest.Impl.Commands.Repository
{
    public class GetRepositoryXmlSchemaCommand : AbstractRepositoryCommand, ICommand
    {
        #region Constructors
        public GetRepositoryXmlSchemaCommand(IRepositoryService repositoryService)
            : base(repositoryService)
        {
        }
        #endregion

        #region AbstractRepositoryCommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string text = _repositoryService.GetXmlSchema((string)context.Arguments[0]);

            Stream result = formatter.Format(context, text);

            return result;
        }
        #endregion
    }
}
