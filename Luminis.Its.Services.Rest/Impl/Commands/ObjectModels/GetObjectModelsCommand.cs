using System.Collections.Generic;
using System.IO;
using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.Rest.Impl.Commands.ObjectModels
{
    public class GetObjectModelsCommand : AbstractObjectModelCommand, ICommand
    {
        #region Constructors
        public GetObjectModelsCommand(IObjectModelService objectModelService)
            : base(objectModelService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            IEnumerable<ObjectModel> objectModels = _objectModelService.GetEnumerable(context.BaseUri);

            Stream result = formatter.Format(context, objectModels);

            return result;
        }
        #endregion
    }
}
