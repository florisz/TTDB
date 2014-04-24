using System.IO;
using TimeTraveller.Services.Representations;

namespace TimeTraveller.Services.Rest.Impl.Commands.Representations
{
    public abstract class AbstractRepresentationCommand : ICommand
    {
        #region Private Properties
        protected IRepresentationService _representationService;
        #endregion

        #region Constructors
        public AbstractRepresentationCommand(IRepresentationService representationService)
        {
            _representationService = representationService;
        }
        #endregion

        #region ICommand Members
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion
    }
}
