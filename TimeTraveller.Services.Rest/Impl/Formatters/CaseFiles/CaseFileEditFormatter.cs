using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.ObjectModels;
using TimeTraveller.General.Xml;
using Microsoft.Practices.Unity;

namespace TimeTraveller.Services.Rest.Impl.Formatters.CaseFiles
{
    public class CaseFileEditFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Dependencies
        [Dependency]
        public ICaseFileService CaseFileService { get; set; }

        public override sealed IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public CaseFileEditFormatter(IFormatter chain)
        {
            Chain = chain;
        }
        #endregion

        #region IFormatter Members
        public override Stream Format(CommandContext context, object item)
        {
            CaseFile caseFile = item as CaseFile;
            CaseFileSpecification caseFileSpec = caseFile.CaseFileSpecification;
            ObjectModel objectModel = caseFile.CaseFileSpecification.ObjectModel;

            string caseFileSpecXml = XmlHelper.ToXml(caseFileSpec, true);
            string objectModelXml = XmlHelper.ToXml(objectModel, true);
            string caseFileXml = CaseFileService.GetXml(caseFile, context.Encoding);

            StringBuilder resultXml = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = context.Encoding;
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);
            xmlWriter.WriteStartElement("ExtendedCaseFile");
            xmlWriter.WriteNode(GetXPathNavigator(objectModelXml, "ObjectModel"), true);
            xmlWriter.WriteNode(GetXPathNavigator(caseFileSpecXml, "CaseFileSpecification"), true);
            xmlWriter.WriteNode(GetXPathNavigator(caseFileXml, "CaseFile"), true);
            xmlWriter.WriteEndElement(); // ExtendedCaseFile
            xmlWriter.Close();

            string result = resultXml.ToString();

            // this is a temporary solution; finally a Silverlight form must be created
            string externalId = String.Format("{0}/{1}/{2}", objectModel.Name, caseFileSpec.Name, caseFile.ExtId);

            return Chain.Format(context, result);
        }

        private static XPathNavigator GetXPathNavigator(string xml, string rootElement)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.DocumentElement.Attributes.RemoveAll();

            XPathNavigator xpathNavigator = doc.CreateNavigator();
            xpathNavigator.MoveToRoot();
            return xpathNavigator;
        }
        #endregion
    }


}
