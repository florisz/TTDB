using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Luminis.Its.Client.Model
{
    public partial class CaseFile
    {
        #region Private Properties
        private XElement _content = null;
        #endregion

        #region Public Properties

        [XmlIgnore]
        public XElement content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// searches in the casefile's content for the collection of elements with the 
        /// specified hierarchical key and returns the reference to the list of elements
        /// </summary>
        /// <param name="key">name of the element to look for within the specified context</param>
        /// <returns>if found: the reference to the element 
        /// and null otherwise</returns>
        public List<XElement> GetElements(XElement rootElement, string key)
        {
            IEnumerable<XElement> elements =
                from element in rootElement.Elements()
                where element.Name.LocalName == key
                select element;

            if (elements.Count<XElement>() > 0)
            {
                return elements.ToList<XElement>();
            }

            return null;
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
