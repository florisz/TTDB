using System.IO;
using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.Rest.Impl.Commands.ObjectModels
{
    public abstract class AbstractGetObjectModelCommand: AbstractObjectModelCommand, ICommand
    {
        #region Constructors
        public AbstractGetObjectModelCommand(IObjectModelService objectModelService) 
            : base(objectModelService)
        {
        }
        #endregion

        #region Abstract Members
        public abstract ObjectModel GetObjectModel(CommandContext context);
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            ObjectModel objectModel = GetObjectModel(context);
            Stream result = formatter.Format(context, objectModel);

            return result;
        }

        #endregion

    }
}
