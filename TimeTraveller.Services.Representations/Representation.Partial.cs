using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Items;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Representations
{
    public partial class Representation : AbstractItem, IItem
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
                    RepresentationLink casefileSpecificationLink = FindLink(RepresentationLinkRel.casefilespecification);
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
                    RepresentationLink casefileSpecificationLink = FindLink(RepresentationLinkRel.casefilespecification);
                    if (casefileSpecificationLink == null)
                    {
                        casefileSpecificationLink = new RepresentationLink();
                        casefileSpecificationLink.rel = RepresentationLinkRel.casefilespecification;

                        List<RepresentationLink> links = new List<RepresentationLink>();
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
                    RepresentationLink casefileSpecificationLink = FindLink(RepresentationLinkRel.casefilespecification);
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
                    RepresentationLink selfLink = FindLink(RepresentationLinkRel.self);
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
                    RepresentationLink selfLink = FindLink(RepresentationLinkRel.self);
                    if (selfLink == null)
                    {
                        selfLink = new RepresentationLink();
                        selfLink.rel = RepresentationLinkRel.self;

                        List<RepresentationLink> links = new List<RepresentationLink>();
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
                    RepresentationLink selfLink = FindLink(RepresentationLinkRel.self);
                    if (selfLink != null)
                    {
                        selfLink.href = value;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private RepresentationLink FindLink(RepresentationLinkRel relFilter)
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

    public partial class RepresentationScript
    {
        #region Public Properties
        [XmlIgnore()]
        public XmlElement Element
        {
            get
            {
                return this.Any;
            }
        }

        [XmlIgnore()]
        public string Text
        {
            get
            {
                if (this.Element != null)
                {
                    return this.Element.OuterXml;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (value != null)
                {
                    XmlDocument scriptDocument = new XmlDocument();
                    scriptDocument.LoadXml(value);
                    this.Any = scriptDocument.DocumentElement;
                }
                else
                {
                    this.Any = null;
                }
            }
        }
        #endregion
    }
}
