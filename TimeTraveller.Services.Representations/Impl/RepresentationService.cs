using System;
using System.Collections.Generic;
using System.Text;

using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Representations.Impl
{
    public class RepresentationService : AbstractTimeLineService<Representation>, IRepresentationService
    {
        #region Private Properties
        private static readonly string _representationXsd = "representation.xsd";
        private static readonly string _representationXsdResourceName = string.Format("{0}.{1}", typeof(IRepresentationService).Namespace, _representationXsd);
        private const string _representationsTemplate = "{0}/{1}/";

        private ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public RepresentationService(ILogger logger, IUnity container, ICaseFileSpecificationService caseFileSpecificationService, IDataService dataService)
            : base(ItemType.Representation, logger, container, dataService)
        {
            _caseFileSpecificationService = caseFileSpecificationService;
        }
        #endregion

        #region AbstractTimeLineService Members
        public override string XmlSchemaName
        {
            get { return _representationXsd; }
        }

        public override string XmlSchemaResourceName
        {
            get { return _representationXsdResourceName; }
        }

        public override Representation Convert(IBaseObjectValue objectValue, Uri baseUri)
        {
            Representation result = Convert(objectValue.Text, Encoding.UTF8);
            result.BaseObjectValue = objectValue;
            result.SelfUri = result.GetUri(baseUri, UriType.Version);

            if (!objectValue.ReferenceId.Equals(Guid.Empty))
            {
                IBaseObjectValue caseFileSpecificationValue = DataService.GetBaseObjectValue(objectValue.ReferenceId);
                result.CaseFileSpecification = _caseFileSpecificationService.Convert(caseFileSpecificationValue, baseUri);
                result.CaseFileSpecificationUri = result.CaseFileSpecification.SelfUri;
            }

            return result;
        }
        #endregion

        #region IRepresentationService Members
        public IEnumerable<Representation> GetEnumerable(string objectmodelname, string specificationname, Uri baseUri)
        {
            string representationQueryString = string.Format(_representationsTemplate, objectmodelname, specificationname);
            return GetEnumerable(representationQueryString, baseUri);
        }

        public override bool Store(string representationname, Representation representation, Uri baseUri, IHeaderInfo info)
        {
            try
            {
                Logger.DebugFormat("StoreRepresentation({0})", representationname);

                TimePoint now = TimePoint.Now;

                IBaseObjectValue caseFileSpecificationObjectValue = DataService.GetBaseObjectValue(representation.CaseFileSpecification.Id);
                if (caseFileSpecificationObjectValue == null)
                {
                    throw new ArgumentException(string.Format("Cannot find casefilespecification (casefilespecificationid={0})", representation.CaseFileSpecification.Id));
                }

                return base.Store(representationname, representation, caseFileSpecificationObjectValue, baseUri, info);
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public string Transform(string xml, Representation representation)
        {
            try
            {
                this.Logger.DebugFormat("Transform({0})", representation.Name);

                IRepresentationTransformer transformer = this.Container.Resolve<IRepresentationTransformer>(representation.Script.Type);
                string result = transformer.Transform(representation.Script.Text, xml);

                return result;
            }
            catch (Exception exception)
            {
                this.Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        #endregion
    }
}
