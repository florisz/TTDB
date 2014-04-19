using System.IO;
using Luminis.Its.Services.Representations;

namespace Luminis.Its.Services.Rest.Impl.Commands.Representations
{
    public abstract class AbstractGetRepresentationCommand : AbstractRepresentationCommand, ICommand
    {
        #region Constructors
        public AbstractGetRepresentationCommand(IRepresentationService representationService)
            : base(representationService)
        {
        }
        #endregion

        #region Abstract Members
        public abstract Representation GetRepresentation(CommandContext context);

        #endregion
        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            Representation representation = GetRepresentation(context);

            Stream result = formatter.Format(context, representation);

            return result;
        }
        #endregion
    }
}
