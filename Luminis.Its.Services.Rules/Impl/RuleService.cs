using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Luminis.Its.Services;
using Luminis.Its.Services.CaseFiles;
using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.Data;
using Luminis.Its.Services.Impl;
using Luminis.Its.Services.Rules;
using Luminis.Logging;
using Luminis.Patterns.Range;
using Luminis.Unity;
using Luminis.Xml;

namespace Luminis.Its.Services.Rules.Impl
{
    public class RuleService : AbstractTimeLineService<Rule>, IRuleService
    {
        #region Private Properties
        private static readonly string _ruleXsd = "rule.xsd";
        private static readonly string _ruleXsdResourceName = string.Format("{0}.{1}", typeof(IRuleService).Namespace, _ruleXsd);
        private const string _rulesTemplate = "{0}/{1}/";

        private ICaseFileSpecificationService _caseFileSpecificationService;
        #endregion

        #region Constructors
        public RuleService(ILogger logger, IUnity container, ICaseFileSpecificationService caseFileSpecificationService, IDataService dataService)
            : base(ItemType.RuleSet, logger, container, dataService) 
        {
            _caseFileSpecificationService = caseFileSpecificationService;
        }
        #endregion

        #region AbstractTimeLineService Members
        public override string XmlSchemaName
        {
            get { return _ruleXsd; }
        }

        public override string XmlSchemaResourceName
        {
            get { return _ruleXsdResourceName; }
        }

        public override Rule Convert(IBaseObjectValue objectValue, Uri baseUri)
        {
            Rule result = Convert(objectValue.Text, Encoding.Unicode);
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

        #region IRuleService Members
        public CaseFile Execute(Rule rule, CaseFile caseFile)
        {
            try
            {
                this.Logger.DebugFormat("Execute({0})", rule.Name);

                IRuleEngine ruleEngine = this.Container.Resolve<IRuleEngine>(rule.Script.Type);

                CaseFile result = ruleEngine.Execute(rule, caseFile);

                return result;
            }
            catch (Exception exception)
            {
                this.Logger.Debug("Unexpected exception", exception);
                throw;
            } 
        }

        public IEnumerable<Rule> GetEnumerable(string objectmodelname, string specificationname, Uri baseUri)
        {
            string ruleQueryString = string.Format(_rulesTemplate, objectmodelname, specificationname);
            return base.GetEnumerable(ruleQueryString, baseUri);
        }

        public override bool Store(string rulename, Rule rule, Uri baseUri, WebHttpHeaderInfo info)
        {
            try
            {
                Logger.DebugFormat("StoreRule({0})", rulename);

                TimePoint now = TimePoint.Now;

                IBaseObjectValue caseFileSpecificationObjectValue = DataService.GetBaseObjectValue(rule.CaseFileSpecification.Id);
                if (caseFileSpecificationObjectValue == null)
                {
                    throw new ArgumentException(string.Format("Cannot find casefilespecification (casefilespecificationid={0})", rule.CaseFileSpecification.Id));
                }

                return base.Store(rulename, rule, caseFileSpecificationObjectValue, baseUri, info);
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }
        #endregion
    }
}
