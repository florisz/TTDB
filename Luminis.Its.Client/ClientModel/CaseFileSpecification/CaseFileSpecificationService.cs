using System.Text;

namespace Luminis.Its.Client.Model
{
    public class CaseFileSpecificationService
    {
        /// <summary>
        /// Converts a CaseFileSpecification xml to a CaseFileSpecification object
        /// </summary>
        /// <param name="caseFileSpecificationXml"></param>
        /// <param name="encoding"></param>
        /// <returns>reference to the newly instantiated ObjectModel object</returns>
        public static CaseFileSpecification Convert(string caseFileSpecificationXml)
        {
            // fill all standard (meta) attributes through XML mapping
            CaseFileSpecification caseFileSpecification = XmlHelper.FromXml<CaseFileSpecification>(caseFileSpecificationXml, Encoding.UTF8);

            caseFileSpecification.InitialiseDictionary();

            return caseFileSpecification;
        }

        public static string GetXML(CaseFileSpecification model)
        {
            return XmlHelper.ToXml<CaseFileSpecification>(model, Encoding.UTF8);
        }

        public static string GetXML(CaseFileSpecification model, Encoding enc)
        {
            return XmlHelper.ToXml<CaseFileSpecification>(model, enc);
        }

        public static string GetXML(CaseFileSpecification model, Encoding enc, bool omitEncodingPreamble)
        {
            return XmlHelper.ToXml<CaseFileSpecification>(model, enc, omitEncodingPreamble);
        }


    }
}
