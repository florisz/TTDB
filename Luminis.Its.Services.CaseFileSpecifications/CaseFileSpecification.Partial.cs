using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Luminis.Its.Services;
using Luminis.Its.Services.Data;
using Luminis.Its.Services.Items;
using Luminis.Its.Services.ObjectModels;

namespace Luminis.Its.Services.CaseFileSpecifications
{
    public partial class CaseFileSpecification: AbstractItem, IItem
    {
        #region IItem/AbstractItemMembers
        [XmlIgnore()]
        public override string SelfUri
        {
            get
            {
                string result = string.Empty;
                if (this.Link != null)
                {
                    CaseFileSpecificationLink selfLink = FindLink(CaseFileSpecificationLinkRel.self);
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
                    CaseFileSpecificationLink selfLink = FindLink(CaseFileSpecificationLinkRel.self);
                    if (selfLink == null)
                    {
                        selfLink = new CaseFileSpecificationLink();
                        selfLink.rel = CaseFileSpecificationLinkRel.self;

                        List<CaseFileSpecificationLink> links = new List<CaseFileSpecificationLink>();
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
                    CaseFileSpecificationLink selfLink = FindLink(CaseFileSpecificationLinkRel.self);
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
        public ObjectModel ObjectModel { get; set; }

        [XmlIgnore()]
        public string ObjectModelUri
        {
            get
            {
                string result = string.Empty;
                if (this.Link != null)
                {
                    CaseFileSpecificationLink objectModelLink = FindLink(CaseFileSpecificationLinkRel.objectmodel);
                    if (objectModelLink != null)
                    {
                        result = objectModelLink.href;
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    CaseFileSpecificationLink objectModelLink = FindLink(CaseFileSpecificationLinkRel.objectmodel);
                    if (objectModelLink == null)
                    {
                        objectModelLink = new CaseFileSpecificationLink();
                        objectModelLink.rel = CaseFileSpecificationLinkRel.objectmodel;

                        List<CaseFileSpecificationLink> links = new List<CaseFileSpecificationLink>();
                        if (this.Link != null)
                        {
                            links.AddRange(this.Link);
                        }
                        links.Add(objectModelLink);
                        this.Link = links.ToArray();
                    }
                    objectModelLink.href = value;
                }
                else
                {
                    CaseFileSpecificationLink objectModelLink = FindLink(CaseFileSpecificationLinkRel.objectmodel);
                    if (objectModelLink != null)
                    {
                        objectModelLink.href = value;
                    }
                }
            }
        }

        [XmlIgnore()]
        public string[] XPathQueries
        {
            get
            {
                List<string> result = new List<string>();
                string[] parts = this.UriTemplate.Split('{');
                foreach (string part in parts)
                {
                    int endOfVariable = part.IndexOf('}');
                    if (endOfVariable > -1)
                    {
                        result.Add(part.Substring(0, endOfVariable));
                    }
                }
                return result.ToArray();
            }
        }
        #endregion

        #region Private Methods
        private CaseFileSpecificationLink FindLink(CaseFileSpecificationLinkRel relFilter)
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
