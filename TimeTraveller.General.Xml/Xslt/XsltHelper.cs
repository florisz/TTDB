using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace TimeTraveller.General.Xml.Xslt
{
    /// <summary>
    /// This Class 
    /// </summary>
    public sealed class XsltHelper
    {
        #region Private Properties
        private const string _includePath = "INCLUDE-PATH";
        private const string _stylesheetPath = "STYLESHEET-PATH";

        private static readonly Encoding _defaultEncoding = Encoding.Unicode;
        #endregion

        #region Constructors
        private XsltHelper()
        {
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path. The default encoding is used.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlStream"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, Stream xmlStream)
        {
            return Transform(xsltPath, xmlStream, string.Empty, string.Empty, _defaultEncoding);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlStream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, Stream xmlStream, Encoding encoding)
        {
            return Transform(xsltPath, xmlStream, string.Empty, string.Empty, encoding);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path. The default encoding is used.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlStream"></param>
        /// <param name="includePath"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, Stream xmlStream, string includePath)
        {
            return Transform(xsltPath, xmlStream, includePath, string.Empty, _defaultEncoding);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlStream"></param>
        /// <param name="includePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, Stream xmlStream, string includePath, Encoding encoding)
        {
            return Transform(xsltPath, xmlStream, includePath, string.Empty, encoding);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path. The default encoding is used.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlStream"></param>
        /// <param name="includePath"></param>
        /// <param name="stylesheetPath"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, Stream xmlStream, string includePath, string stylesheetPath)
        {
            return Transform(xsltPath, xmlStream, includePath, stylesheetPath, _defaultEncoding);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlStream"></param>
        /// <param name="includePath"></param>
        /// <param name="stylesheetPath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, Stream xmlStream, string includePath, string stylesheetPath, Encoding encoding)
        {
            StreamReader xmlReader = new StreamReader(xmlStream, encoding);
            string xmlText = xmlReader.ReadToEnd();
            xmlStream.Close();
            xmlReader.Close();
            return Transform(xsltPath, xmlText, includePath, stylesheetPath);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path. The default encoding is used.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, string xmlText)
        {
            return Transform(xsltPath, xmlText, string.Empty, string.Empty);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt path.
        /// </summary>
        /// <param name="xsltPath"></param>
        /// <param name="xmlText"></param>
        /// <param name="includePath"></param>
        /// <param name="stylesheetPath"></param>
        /// <returns></returns>
        public static string Transform(string xsltPath, string xmlText, string includePath, string stylesheetPath)
        {
            Stream xsltStream;
            if (File.Exists(xsltPath))
            {
                xsltStream = File.OpenRead(xsltPath);
            }
            else
            {
                throw new ArgumentException(string.Format("Cannot find file {0}", xsltPath));
            }

            TextReader xsltReader = new StreamReader(xsltStream, true);
            return Transform(xsltReader, xmlText, includePath, stylesheetPath);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt reader.
        /// </summary>
        /// <param name="xsltReader"></param>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string Transform(TextReader xsltReader, string xmlText)
        {
            return Transform(xsltReader, xmlText, string.Empty, string.Empty);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt reader.
        /// </summary>
        /// <param name="xsltReader"></param>
        /// <param name="xmlText"></param>
        /// <param name="includePath"></param>
        /// <param name="stylesheetPath"></param>
        /// <returns></returns>
        public static string Transform(TextReader xsltReader, string xmlText, string includePath, string stylesheetPath)
        {
            TextReader xmlReader = new StringReader(xmlText);
            return Transform(xsltReader, xmlReader, includePath, stylesheetPath);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt reader.
        /// </summary>
        /// <param name="xsltReader"></param>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string Transform(TextReader xsltReader, TextReader xmlReader)
        {
            return Transform(xsltReader, xmlReader, string.Empty, string.Empty);
        }

        /// <summary>
        /// Transform the xml using the xslt specified in the xslt reader.
        /// </summary>
        /// <param name="xsltReader"></param>
        /// <param name="xmlReader"></param>
        /// <param name="includePath"></param>
        /// <param name="stylesheetPath"></param>
        /// <returns></returns>
        public static string Transform(TextReader xsltReader, TextReader xmlReader, string includePath, string stylesheetPath)
        {
            try
            {
                // create a new string writer for storing the output
                StringWriter output = new StringWriter();

                // Load the XSL into a transformer.
                // The xslt-text can include INCLUDE-PATH and/or STYLESHEET-PATH. Replace these with the given include- and stylesheetpath.
                StringBuilder xsltText = new StringBuilder(xsltReader.ReadToEnd());
                if (includePath != null)
                {
                    xsltText.Replace(_includePath, includePath.Replace(@"\", "/"));
                }
                if (stylesheetPath != null)
                {
                    xsltText.Replace(_stylesheetPath, stylesheetPath.Replace(@"\", "/"));
                }

                StringReader xsltTextReader = new StringReader(xsltText.ToString());

                RelativeUriResolver resolver = new RelativeUriResolver();  
                resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;  

                XslCompiledTransform xsltTransformer = new XslCompiledTransform();
                xsltTransformer.Load(new XmlTextReader(xsltTextReader),XsltSettings.TrustedXslt,resolver);

                // transform the XML file with the XSLT transformer into the desired result
                XmlReader xmlToTransForm = XmlTextReader.Create(xmlReader);
                xsltTransformer.Transform(xmlToTransForm, null, output);

                // clean up neatly
                output.Close();
                xsltTextReader.Close();

                // Done, return the tranformed result.
                return output.ToString();
            }
            catch
            {
                throw;
            }
            finally
            {
                xsltReader.Close();
            }
        }
        #endregion
    }
}
