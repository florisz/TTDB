using System.IO;
using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.Rest.Impl.Commands.ObjectModels
{
    public abstract class AbstractObjectModelCommand : ICommand
    {
        #region Private Properties
        protected IObjectModelService _objectModelService;
        #endregion

        #region Constructors
        public AbstractObjectModelCommand(IObjectModelService objectModelService)
        {
            _objectModelService = objectModelService;
        }
        #endregion

        #region ICommand
        public abstract Stream Execute(CommandContext context, IFormatter formatter);
        #endregion

    }
}
