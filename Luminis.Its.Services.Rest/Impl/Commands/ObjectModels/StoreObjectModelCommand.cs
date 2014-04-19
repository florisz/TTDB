using System.IO;
using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.Rest.Impl.Commands.ObjectModels
{
    public class StoreObjectModelCommand : AbstractObjectModelCommand, ICommand
    {
        #region Constructors
        public StoreObjectModelCommand(IObjectModelService objectModelService)
            : base(objectModelService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string objectModelXml = context.RequestBody;
            ObjectModel objectModel = _objectModelService.Convert(objectModelXml, context.Encoding);

            _objectModelService.Store(context.RequestedId, objectModel, context.BaseUri, context.JournalInfo);

            Stream result = formatter.Format(context, objectModel);

            return result;
        }
        #endregion
    }
}
