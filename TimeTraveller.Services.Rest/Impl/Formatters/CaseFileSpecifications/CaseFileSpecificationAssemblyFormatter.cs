using System.IO;

using TimeTraveller.Services.CaseFileSpecifications;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFileSpecifications
{
    public class CaseFileSpecificationAssemblyFormatter : IFormatter
    {
        #region Private Properties
        private ICaseFileSpecificationService _specificationService;
        #endregion

        #region Constructors
        public CaseFileSpecificationAssemblyFormatter(ICaseFileSpecificationService specificationService)
        {
            _specificationService = specificationService;
        }
        #endregion

        #region IFormatter Members

        public Stream Format(CommandContext context, object item)
        {
            CaseFileSpecification specification = item as CaseFileSpecification;

            byte[] resultBuffer = _specificationService.GetAssemblyBytes(specification);
            Stream result = new MemoryStream(resultBuffer);
            context.ContentType = WebOperationContentType.Assembly;

            return result;
        }

        #endregion
    }
}
