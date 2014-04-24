﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace TimeTraveller.Services.CaseFileSpecifications {
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd", IsNullable=false)]
    public partial class CaseFileSpecification {
        
        private CaseFileSpecificationLink[] linkField;
        
        private string nameField;
        
        private string uriTemplateField;
        
        private CaseFileSpecificationStructure structureField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Link")]
        public CaseFileSpecificationLink[] Link {
            get {
                return this.linkField;
            }
            set {
                this.linkField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string UriTemplate {
            get {
                return this.uriTemplateField;
            }
            set {
                this.uriTemplateField = value;
            }
        }
        
        /// <remarks/>
        public CaseFileSpecificationStructure Structure {
            get {
                return this.structureField;
            }
            set {
                this.structureField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationLink {
        
        private CaseFileSpecificationLinkRel relField;
        
        private string hrefField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CaseFileSpecificationLinkRel rel {
            get {
                return this.relField;
            }
            set {
                this.relField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href {
            get {
                return this.hrefField;
            }
            set {
                this.hrefField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd")]
    public enum CaseFileSpecificationLinkRel {
        
        /// <remarks/>
        casefilespecification,
        
        /// <remarks/>
        objectmodel,
        
        /// <remarks/>
        self,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationRelation {
        
        private CaseFileSpecificationEntity entityField;
        
        private string nameField;
        
        private string typeField;
        
        /// <remarks/>
        public CaseFileSpecificationEntity Entity {
            get {
                return this.entityField;
            }
            set {
                this.entityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationEntity {
        
        private CaseFileSpecificationRelation[] relationField;
        
        private string nameField;
        
        private string typeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Relation")]
        public CaseFileSpecificationRelation[] Relation {
            get {
                return this.relationField;
            }
            set {
                this.relationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://timetraveller.net/its/schemas/casefilespecification.xsd")]
    public partial class CaseFileSpecificationStructure {
        
        private CaseFileSpecificationEntity entityField;
        
        /// <remarks/>
        public CaseFileSpecificationEntity Entity {
            get {
                return this.entityField;
            }
            set {
                this.entityField = value;
            }
        }
    }
}
