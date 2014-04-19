using System.IO;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Representations;

namespace Luminis.Its.Services.Rest.Impl.Commands.Representations
{
    public class StoreRepresentationCommand : AbstractRepresentationCommand, ICommand
    {
        #region Private Properties
        private const string _caseFileSpecificationTemplate = "{0}/{1}";

        private ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public StoreRepresentationCommand(IRepresentationService representationService, ICaseFileSpecificationService caseFileSpecificationService)
            : base(representationService)
        {
            _caseFileSpecificationService = caseFileSpecificationService;
        }
        #endregion

        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string representationXml = context.RequestBody;
            Representation representation = _representationService.Convert(representationXml, context.Encoding);
            string caseFileSpecificationId = string.Format(_caseFileSpecificationTemplate, context.Arguments[0], context.Arguments[1]);
            representation.CaseFileSpecification = _caseFileSpecificationService.Get(caseFileSpecificationId, context.BaseUri);

            _representationService.Store(context.RequestedId, representation, context.BaseUri, context.JournalInfo);

            Stream result = formatter.Format(context, representation);

            return result;
        }
        #endregion
    }
}
