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
namespace Luminis.Its.Services.CaseFiles {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://luminis.net/its/schemas/casefile.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://luminis.net/its/schemas/casefile.xsd", IsNullable=false)]
    public partial class CaseFile {
        
        private CaseFileLink[] linkField;
        
        private System.Xml.XmlElement anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Link")]
        public CaseFileLink[] Link {
            get {
                return this.linkField;
            }
            set {
                this.linkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://luminis.net/its/schemas/casefile.xsd")]
    public partial class CaseFileLink {
        
        private CaseFileLinkRel relField;
        
        private string hrefField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CaseFileLinkRel rel {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://luminis.net/its/schemas/casefile.xsd")]
    public enum CaseFileLinkRel {
        
        /// <remarks/>
        casefilespecification,
        
        /// <remarks/>
        self,
    }
}