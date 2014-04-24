using System.Collections.Generic;
using System.IO;
using TimeTraveller.Services.Representations;

namespace TimeTraveller.Services.Rest.Impl.Commands.Representations
{
    public class GetRepresentationsCommand : AbstractRepresentationCommand, ICommand
    {
        #region Constructors
        public GetRepresentationsCommand(IRepresentationService representationService)
            : base(representationService)
        {
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            IEnumerable<Representation> representations = _representationService.GetEnumerable((string)context.Arguments[0], (string)context.Arguments[1], context.BaseUri);

            Stream result = formatter.Format(context, representations);

            return result;
        }
        #endregion
    }
}
