using System;
using System.Net;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Luminis.Its.Client.Model
{
    public partial class CaseFileSpecification
    {
        #region Private Properties
        private Dictionary<string, CaseFileSpecificationElement> _caseFileSpecificationElements = 
                                                new Dictionary<string,CaseFileSpecificationElement>();
        #endregion

        #region Public Properties
        [XmlIgnore]
        public Dictionary<string, CaseFileSpecificationElement> CaseFileSpecificationElements
        {
            get
            {
                return _caseFileSpecificationElements;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Make a dictionary so separate elements can easily be found
        /// Dictionary key is the Name which is assumed to be unique
        /// </summary>
        public void InitialiseDictionary()
        {
            CaseFileSpecificationEntity rootEntity = this.Structure.Entity;

            AddSpecificationElements(rootEntity);
        }
        #endregion

        #region Private Methods
        private void AddSpecificationElements(CaseFileSpecificationEntity cfsEntity)
        {
            CaseFileSpecificationElement entityEelement = new CaseFileSpecificationElement(cfsEntity);
            _caseFileSpecificationElements.Add(entityEelement.Name, entityEelement);
            
            if (cfsEntity.Relation == null)
                return;

            // add all relations
            foreach(CaseFileSpecificationRelation cfsRelation in cfsEntity.Relation)
            {
                CaseFileSpecificationElement relationElement = new CaseFileSpecificationElement(cfsRelation);
                _caseFileSpecificationElements.Add(relationElement.Name, relationElement);

                // recursively add the entities within the relations
                AddSpecificationElements(cfsRelation.Entity);
            }
        }
        #endregion
    }

    public class CaseFileSpecificationElement
    {
        private object _element;

        public CaseFileSpecificationElement(object element)
        {
            _element = element; 
        }
        public object Element
        {
            get
            {
                return _element;
            }
        }
        public string Name
        {
            get
            {
                if (Element.GetType() == typeof(CaseFileSpecificationEntity))
                    return ((CaseFileSpecificationEntity)Element).Name;
                else
                    return ((CaseFileSpecificationRelation)Element).Name;
            }
        }
        public string Type
        {
            get
            {
                if (Element.GetType() == typeof(CaseFileSpecificationEntity))
                    return ((CaseFileSpecificationEntity)Element).Type;
                else
                    return ((CaseFileSpecificationRelation)Element).Type;
            }
        }

    }
}
