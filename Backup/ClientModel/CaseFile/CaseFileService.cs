using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Luminis.Its.Client.Model
{
    public sealed class CaseFileService
    {
        /// <summary>
        /// Converts a casefile xml to a casefile object
        /// Because the Silverlight API does not support xs:any functions the
        /// actual xml of the casefile must be read using a separate Linq query
        /// </summary>
        /// <param name="caseFileXml"></param>
        /// <param name="encoding"></param>
        /// <returns>reference to the newly instantiated CaseFile object</returns>
        public static CaseFile Convert(string caseFileXml, Encoding encoding)
        {
            // fill all standard (meta) attributes through XML mapping
            CaseFile caseFile = XmlHelper.FromXml<CaseFile>(caseFileXml, Encoding.UTF8);

            // Fill the content by selecting the first root element with a registration id
            XElement root = XElement.Load(new System.IO.StringReader(caseFileXml));
            IEnumerable<XElement> elements =
                from element in root.Elements()
                where element.Attributes().Contains(new XAttribute("RegistrationId", "undefined"), new AttributeComparer())
                select element;

            if (elements.Count<XElement>() != 1)
            {
                throw new ArgumentException("Casefile does not have one root element");
            }

            caseFile.content = elements.ElementAt<XElement>(0);

           return caseFile;
        }

    }

}
