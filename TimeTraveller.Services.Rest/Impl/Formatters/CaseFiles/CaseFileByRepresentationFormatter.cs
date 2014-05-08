using System;
using System.IO;

using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.Representations;
using TimeTraveller.Services.Interface;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles
{
    public class CaseFileByRepresentationFormatter : IFormatter
    {
        #region Private Properties
        private const string _representationTemplate = "{0}/{1}/{2}";

        private ICaseFileService _caseFileService;
        private IRepresentationService _representationService;
        #endregion

        #region Constructors
        public CaseFileByRepresentationFormatter(ICaseFileService caseFileService, IRepresentationService representationService)
        {
            _caseFileService = caseFileService;
            _representationService = representationService;
        }
        #endregion

        #region IFormatter Members

        public Stream Format(CommandContext context, object item)
        {
            CaseFile caseFile = item as CaseFile;

            string representationId = string.Format(_representationTemplate, context.Arguments[0], context.Arguments[1], context.Representation);

            Representation representation = _representationService.Get(representationId, context.BaseUri);
            if (representation == null)
            {
                throw new ArgumentOutOfRangeException("representation", string.Format("Unknown representation {0} in uri {1}", representationId, context.RequestUri));
            }

            string resultText = _representationService.Transform(caseFile.Text, representation);
            byte[] resultBuffer = context.Encoding.GetBytes(resultText);
            Stream result = new MemoryStream(resultBuffer);

            switch (representation.Script.ContentType)
            {
                case RepresentationScriptContentType.html:
                    context.ContentType = WebOperationContentType.Html;
                    break;
                case RepresentationScriptContentType.text:
                    context.ContentType = WebOperationContentType.Text;
                    break;
                default:
                    context.ContentType = WebOperationContentType.Xml;
                    break;
            }

            return result;
        }

        #endregion
    }
}
