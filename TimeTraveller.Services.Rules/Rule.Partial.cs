using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Items;

namespace TimeTraveller.Services.Rules
{
    public partial class Rule: AbstractItem, IItem
    {
        #region IItem/AbstractItem Members
        [XmlIgnore()]
        public CaseFileSpecification CaseFileSpecification { get; set; }

        [XmlIgnore()]
        public string CaseFileSpecificationUri
        {
            get
            {
                string result = string.Empty;
                if (this.Link != null)
                {
                    RuleLink casefileSpecificationLink = FindLink(RuleLinkRel.casefilespecification);
                    if (casefileSpecificationLink != null)
                    {
                        result = casefileSpecificationLink.href;
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    RuleLink casefileSpecificationLink = FindLink(RuleLinkRel.casefilespecification);
                    if (casefileSpecificationLink == null)
                    {
                        casefileSpecificationLink = new RuleLink();
                        casefileSpecificationLink.rel = RuleLinkRel.casefilespecification;

                        List<RuleLink> links = new List<RuleLink>();
                        if (this.Link != null)
                        {
                            links.AddRange(this.Link);
                        }
                        links.Add(casefileSpecificationLink);
                        this.Link = links.ToArray();
                    }
                    casefileSpecificationLink.href = value;
                }
                else
                {
                    RuleLink casefileSpecificationLink = FindLink(RuleLinkRel.casefilespecification);
                    if (casefileSpecificationLink != null)
                    {
                        casefileSpecificationLink.href = value;
                    }
                }
            }
        }

        [XmlIgnore()]
        public override string SelfUri
        {
            get
            {
                string result = string.Empty;
                if (this.Link != null)
                {
                    RuleLink selfLink = FindLink(RuleLinkRel.self);
                    if (selfLink != null)
                    {
                        result = selfLink.href;
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    RuleLink selfLink = FindLink(RuleLinkRel.self);
                    if (selfLink == null)
                    {
                        selfLink = new RuleLink();
                        selfLink.rel = RuleLinkRel.self;

                        List<RuleLink> links = new List<RuleLink>();
                        if (this.Link != null)
                        {
                            links.AddRange(this.Link);
                        }
                        links.Add(selfLink);
                        this.Link = links.ToArray();
                    }
                    selfLink.href = value;
                }
                else
                {
                    RuleLink selfLink = FindLink(RuleLinkRel.self);
                    if (selfLink != null)
                    {
                        selfLink.href = value;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private RuleLink FindLink(RuleLinkRel relFilter)
        {
            if (this.Link != null)
            {
                return this.Link.FirstOrDefault(l => l.rel == relFilter);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
