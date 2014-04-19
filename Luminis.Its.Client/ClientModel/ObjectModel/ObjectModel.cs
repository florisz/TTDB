using System;
using System.Net;

using System.Linq;

namespace Luminis.Its.Client.Model
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd", IsNullable = false)]
    public partial class ObjectModel
    {
        public ObjectModelLink Link { get; set; }

        public string Name { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public ObjectDefinition[] ObjectDefinitions { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public ObjectRelation[] ObjectRelations { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public partial class ObjectModelLink
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ObjectModelLinkRel rel { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public enum ObjectModelLinkRel
    {
        objectmodel,
        self,
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public partial class ObjectRelation
    {
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string Source { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "IDREF")]
        public string Target { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MinOccurs { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MinOccursSpecified { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MaxOccurs { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public partial class ObjectDefinitionComplexProperty
    {
        [System.Xml.Serialization.XmlArrayItemAttribute("Property", IsNullable = false)]
        public ObjectDefinitionProperty[] Properties { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("ComplexProperty", IsNullable = false)]
        public ObjectDefinitionComplexProperty[] ComplexProperties { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MinOccurs { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MaxOccurs { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public partial class ObjectDefinitionProperty
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Required { get; set; } 

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RequiredSpecified { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public partial class ObjectDefinition
    {
        [System.Xml.Serialization.XmlArrayItemAttribute("Property", IsNullable = false)]
        public ObjectDefinitionProperty[] Properties { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("ComplexProperty", IsNullable = false)]
        public ObjectDefinitionComplexProperty[] ComplexProperties { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ObjectType ObjectType { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://luminis.net/its/schemas/objectmodel.xsd")]
    public enum ObjectType
    {
        entity,
        relation,
    }
}
