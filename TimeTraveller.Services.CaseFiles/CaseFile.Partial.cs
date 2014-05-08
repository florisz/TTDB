using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Items;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.CaseFiles
{
    public partial class CaseFile : AbstractItem, IItem
    {
        #region Constructors
        public CaseFile()
        {
        }

        public CaseFile(CaseFile objectToClone)
        {
            this.CaseFileSpecification = objectToClone.CaseFileSpecification;
            this.CaseFileSpecificationUri = objectToClone.CaseFileSpecificationUri;
            this.BaseObjectValue = objectToClone.BaseObjectValue;
            this.Link = objectToClone.Link;
            this.SelfUri = objectToClone.SelfUri;
            this.Text = objectToClone.Text;
        }
        #endregion

        #region IItem/AbstractItem Members
        [XmlIgnore()]
        public override string SelfUri
        {
            get
            {
                string result = string.Empty;
                if (this.Link != null)
                {
                    CaseFileLink selfLink = FindLink(CaseFileLinkRel.self);
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
                    CaseFileLink selfLink = FindLink(CaseFileLinkRel.self);
                    if (selfLink == null)
                    {
                        selfLink = new CaseFileLink();
                        selfLink.rel = CaseFileLinkRel.self;

                        List<CaseFileLink> links = new List<CaseFileLink>();
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
                    CaseFileLink selfLink = FindLink(CaseFileLinkRel.self);
                    if (selfLink != null)
                    {
                        selfLink.href = value;
                    }
                }
            }
        }
        #endregion

        #region Public Properties
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
                    CaseFileLink specificationLink = FindLink(CaseFileLinkRel.casefilespecification);
                    if (specificationLink != null)
                    {
                        result = specificationLink.href;
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    CaseFileLink specificationLink = FindLink(CaseFileLinkRel.casefilespecification);
                    if (specificationLink == null)
                    {
                        specificationLink = new CaseFileLink();
                        specificationLink.rel = CaseFileLinkRel.casefilespecification;

                        List<CaseFileLink> links = new List<CaseFileLink>();
                        if (this.Link != null)
                        {
                            links.AddRange(this.Link);
                        }
                        links.Add(specificationLink);
                        this.Link = links.ToArray();
                    }
                    specificationLink.href = value;
                }
                else
                {
                    CaseFileLink specificationLink = FindLink(CaseFileLinkRel.casefilespecification);
                    if (specificationLink != null)
                    {
                        specificationLink.href = value;
                    }
                }
            }
        }

        [XmlIgnore()]
        public XmlElement ContentElement
        {
            get
            {
                return this.Any;
            }
            set
            {
                this.Any = value;
            }
        }

        [XmlIgnore()]
        public string Text
        {
            get
            {
                if (this.ContentElement != null)
                {
                    return this.ContentElement.OuterXml;
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
                    XmlDocument contentDocument = new XmlDocument();
                    contentDocument.LoadXml(value);
                    this.ContentElement = contentDocument.DocumentElement;
                }
            }
        } 
        #endregion

        #region Private Methods
        private CaseFileLink FindLink(CaseFileLinkRel relFilter)
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
