using System;
using System.Net;
using System.Text;

namespace Luminis.Its.Client.Model
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd", IsNullable = false)]
    public partial class CaseFileSpecification
    {
        [System.Xml.Serialization.XmlElementAttribute("Link")]
        public CaseFileSpecificationLink[] Link { get; set; }

        public string Name { get; set; }

        public string UriTemplate { get; set; }

        public CaseFileSpecificationStructure Structure { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationLink
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CaseFileSpecificationLinkRel rel { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd")]
    public enum CaseFileSpecificationLinkRel
    {
        casefilespecification,
        objectmodel,
        self,
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationRelation
    {
        public CaseFileSpecificationEntity Entity { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationEntity
    {
        [System.Xml.Serialization.XmlElementAttribute("Relation")]
        public CaseFileSpecificationRelation[] Relation { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationStructure
    {
        public CaseFileSpecificationEntity Entity { get; set; }
    }
}
