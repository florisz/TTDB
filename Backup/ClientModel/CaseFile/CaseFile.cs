using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Luminis.Its.Client.Model
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefile.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://luminis.net/its/schemas/casefile.xsd", IsNullable = false)]
    public partial class CaseFile
    {
        [System.Xml.Serialization.XmlElementAttribute("Link")]
        public CaseFileLink[] Link { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefile.xsd")]
    public partial class CaseFileLink
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CaseFileLinkRel rel { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/casefile.xsd")]
    public enum CaseFileLinkRel
    {
        casefilespecification,
        self,
    }

}
