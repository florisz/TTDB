using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TimeTraveller.General.Xml
{
    /// <summary>
    /// The XmlHelper can serialize objects to xml. deserialize object from xml and can validate xml against an xml schema.
    /// </summary>
    /// <remarks>
    /// Encoding.Unicode is the default encoding for the methods that do not have an Encoding parameter.
    /// </remarks>
    public sealed class XmlHelper
    {
        #region Private Properties
        private static readonly Encoding _defaultEncoding = Encoding.Unicode;
        private const bool _defaultOmitEncodingPreamble = false;
        private static List<ValidationEventArgs> _validationErrors;
        #endregion

        #region Constructors
        private XmlHelper()
        {
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Deserialize xml string into an object. The default encoding is used.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlObjectType"></param>
        /// <returns></returns>
        public static object FromXml(string xml, Type xmlObjectType)
        {
            return FromXml(xml, xmlObjectType, _defaultEncoding);
        }

        /// <summary>
        /// Deserialize xml string into an object.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static object FromXml(string xml, Type xmlObjectType, Encoding encoding)
        {
            try
            {
                MemoryStream xmlStream = new MemoryStream(encoding.GetBytes(xml));
                StreamReader reader = new StreamReader(xmlStream, encoding, false);
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.CloseInput = true;
                XmlReader xmlReader = XmlReader.Create(reader, readerSettings);
                XmlSerializer xmlSerializer = new XmlSerializer(xmlObjectType);
                return xmlSerializer.Deserialize(xmlReader);
            }
            catch (InvalidOperationException exception)
            {
                throw new ArgumentException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Deserialize xml string into an object. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T FromXml<T>(string xml)
        {
            return (T)FromXml(xml, typeof(T), _defaultEncoding);
        }

        /// <summary>
        /// Deserialize xml string into an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T FromXml<T>(string xml, Encoding encoding)
        {
            return (T)FromXml(xml, typeof(T), encoding);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject)
        {
            return ToXml(xmlObject, xmlObject.GetType(), _defaultEncoding, _defaultOmitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, bool omitEncodingPreamble)
        {
            return ToXml(xmlObject, xmlObject.GetType(), _defaultEncoding, omitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Encoding encoding)
        {
            return ToXml(xmlObject, xmlObject.GetType(), encoding, _defaultOmitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Encoding encoding, bool omitEncodingPreamble)
        {
            return ToXml(xmlObject, xmlObject.GetType(), encoding, omitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject)
        {
            return ToXml(xmlObject, typeof(T), _defaultEncoding, _defaultOmitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, bool omitEncodingPreamble)
        {
            return ToXml(xmlObject, typeof(T), _defaultEncoding, omitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, XmlSerializerNamespaces namespaces)
        {
            return ToXml(xmlObject, typeof(T), _defaultEncoding, _defaultOmitEncodingPreamble, namespaces);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, bool omitEncodingPreamble, XmlSerializerNamespaces namespaces)
        {
            return ToXml(xmlObject, typeof(T), _defaultEncoding, omitEncodingPreamble, namespaces);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, Encoding encoding, XmlSerializerNamespaces namespaces)
        {
            return ToXml(xmlObject, typeof(T), encoding, _defaultOmitEncodingPreamble, namespaces);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, Encoding encoding, bool omitEncodingPreamble, XmlSerializerNamespaces namespaces)
        {
            return ToXml(xmlObject, typeof(T), encoding, omitEncodingPreamble, namespaces);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Type xmlObjectType)
        {
            return ToXml(xmlObject, xmlObjectType, _defaultEncoding, _defaultOmitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Type xmlObjectType, bool omitEncodingPreamble)
        {
            return ToXml(xmlObject, xmlObjectType, _defaultEncoding, omitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, Encoding encoding)
        {
            return ToXml(xmlObject, typeof(T), encoding, _defaultOmitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <returns></returns>
        public static string ToXml<T>(object xmlObject, Encoding encoding, bool omitEncodingPreamble)
        {
            return ToXml(xmlObject, typeof(T), encoding, omitEncodingPreamble);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Type xmlObjectType, Encoding encoding)
        {
            return ToXml(xmlObject, xmlObjectType, encoding, _defaultOmitEncodingPreamble, null);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="encoding"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Type xmlObjectType, Encoding encoding, bool omitEncodingPreamble)
        {
            return ToXml(xmlObject, xmlObjectType, encoding, omitEncodingPreamble, null);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="encoding"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Type xmlObjectType, Encoding encoding, XmlSerializerNamespaces namespaces)
        {
            return ToXml(xmlObject, xmlObjectType, encoding, _defaultOmitEncodingPreamble, null);
        }

        /// <summary>
        /// Serialize the object into an xml string.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="encoding"></param>
        /// <param name="omitEncodingPreamble"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static string ToXml(object xmlObject, Type xmlObjectType, Encoding encoding, bool omitEncodingPreamble, XmlSerializerNamespaces namespaces)
        {
            MemoryStream xmlMemoryStream = new MemoryStream();
            ToXml(xmlObject, xmlObjectType, xmlMemoryStream, encoding, namespaces);
            xmlMemoryStream.Close();
            byte[] xmlBuffer = xmlMemoryStream.ToArray();
            string result = encoding.GetString(xmlBuffer);
            if (omitEncodingPreamble)
            {
                byte[] encodingPreamble = encoding.GetPreamble();
                if (encodingPreamble[0].Equals(xmlBuffer[0]) &&
                    encodingPreamble[1].Equals(xmlBuffer[1]))
                {
                    result = result.Substring(1);
                }
            }
            return result;
        }

        /// <summary>
        /// Serialize the object into the output stream. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="outputStream"></param>
        public static void ToXml(object xmlObject, Stream outputStream)
        {
            ToXml(xmlObject, xmlObject.GetType(), outputStream, _defaultEncoding);
        }

        /// <summary>
        /// Serialize the object into the output stream.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="outputStream"></param>
        /// <param name="encoding"></param>
        public static void ToXml(object xmlObject, Stream outputStream, Encoding encoding)
        {
            ToXml(xmlObject, xmlObject.GetType(), outputStream, encoding);
        }

        /// <summary>
        /// Serialize the object into the output stream. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="outputStream"></param>
        public static void ToXml<T>(object xmlObject, Stream outputStream)
        {
            ToXml(xmlObject, typeof(T), outputStream, _defaultEncoding);
        }

        /// <summary>
        /// Serialize the object into the output stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="outputStream"></param>
        /// <param name="encoding"></param>
        public static void ToXml<T>(object xmlObject, Stream outputStream, Encoding encoding)
        {
            ToXml(xmlObject, typeof(T), outputStream, encoding);
        }

        /// <summary>
        /// Serialize the object into the xml writer. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="xmlWriter"></param>
        public static void ToXml<T>(object xmlObject, XmlWriter xmlWriter)
        {
            ToXml(xmlObject, typeof(T), xmlWriter);
        }

        /// <summary>
        /// Serialize the object into the output stream. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="outputStream"></param>
        public static void ToXml(object xmlObject, Type xmlObjectType, Stream outputStream)
        {
            ToXml(xmlObject, xmlObjectType, outputStream, _defaultEncoding);
        }

        /// <summary>
        /// Serialize the object into the output stream.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="outputStream"></param>
        /// <param name="encoding"></param>
        public static void ToXml(object xmlObject, Type xmlObjectType, Stream outputStream, Encoding encoding)
        {
            ToXml(xmlObject, xmlObjectType, outputStream, encoding, null);
        }

        /// <summary>
        /// Serialize the object into the output stream.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="outputStream"></param>
        /// <param name="encoding"></param>
        /// <param name="namespaces"></param>
        public static void ToXml(object xmlObject, Type xmlObjectType, Stream outputStream, Encoding encoding, XmlSerializerNamespaces namespaces)
        {
            XmlSerializer serializer = new XmlSerializer(xmlObjectType);
            
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.Encoding = encoding;
            settings.Indent = true;
            XmlWriter xmlWriter = XmlWriter.Create(outputStream, settings);
            
            serializer.Serialize(xmlWriter, xmlObject, namespaces);
        }

        /// <summary>
        /// Serialize the object into the xml writer. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="xmlWriter"></param>
        public static void ToXml(object xmlObject, Type xmlObjectType, XmlWriter xmlWriter)
        {
            XmlSerializer serializer = new XmlSerializer(xmlObjectType);
            serializer.Serialize(xmlWriter, xmlObject);
        }

        /// <summary>
        /// Serialize the object into the xml writer. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="xmlWriter"></param>
        /// <param name="namespaces"></param>
        public static void ToXml(object xmlObject, Type xmlObjectType, XmlWriter xmlWriter, XmlSerializerNamespaces namespaces)
        {
            XmlSerializer serializer = new XmlSerializer(xmlObjectType);
            serializer.Serialize(xmlWriter, xmlObject, namespaces);
        }

        /// <summary>
        /// Serialize the object and create an xml document. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(object xmlObject)
        {
            return ToXmlDocument(xmlObject, xmlObject.GetType(), _defaultEncoding);
        }

        /// <summary>
        /// Serialize the object and create an xml document. The default encoding is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument<T>(object xmlObject)
        {
            return ToXmlDocument(xmlObject, typeof(T), _defaultEncoding);
        }

        /// <summary>
        /// Serialize the object and create an xml document.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(object xmlObject, Encoding encoding)
        {
            return ToXmlDocument(xmlObject, xmlObject.GetType(), encoding);
        }

        /// <summary>
        /// Serialize the object and create an xml document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument<T>(object xmlObject, Encoding encoding)
        {
            return ToXmlDocument(xmlObject, typeof(T), encoding);
        }

        /// <summary>
        /// Serialize the object and create an xml document. The default encoding is used.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(object xmlObject, Type xmlObjectType)
        {
            return ToXmlDocument(xmlObject, xmlObjectType, _defaultEncoding);
        }

        /// <summary>
        /// Serialize the object and create an xml document.
        /// </summary>
        /// <param name="xmlObject"></param>
        /// <param name="xmlObjectType"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(object xmlObject, Type xmlObjectType, Encoding encoding)
        {
            MemoryStream xmlMemoryStream = new MemoryStream();
            ToXml(xmlObject, xmlObjectType, xmlMemoryStream, encoding);
            xmlMemoryStream.Position = 0;

            XmlDocument result = new XmlDocument();
            StreamReader reader = new StreamReader(xmlMemoryStream, encoding, false);
            result.Load(reader);

            xmlMemoryStream.Close();

            return result;
        }

        /// <summary>
        /// Validate the xml againt the xml schema. The default encoding is used to read the xml and xml schema strings.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchemaText"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, string xmlSchemaText)
        {
            Validate(xml, xmlSchemaText, _defaultEncoding);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchemaText"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, string xmlSchemaText, Encoding encoding)
        {
            try
            {
                _validationErrors = new List<ValidationEventArgs>();
                StringReader xmlSchemaReader = new StringReader(xmlSchemaText);
                XmlSchema xmlSchema = XmlSchema.Read(xmlSchemaReader, new ValidationEventHandler(validatingReader_ValidationEventHandler));

                if (_validationErrors.Count > 0)
                {
                    throw new ArgumentException("Invalid XmlSchema supplied", CreateValidationException(_validationErrors));
                }

                IEnumerable<ValidationEventArgs> validationErrors = null;
                bool isValid = Validate(xml, encoding, xmlSchema, out validationErrors);
                if (!isValid)
                {
                    Exception exception = CreateValidationException(validationErrors);
                    throw exception;
                }
            }
            finally
            {
                _validationErrors = null;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema. The default encoding is used to read the xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, Stream xmlSchemaStream)
        {
            IEnumerable<ValidationEventArgs> validationErrors = null;
            bool isValid = Validate(xml, _defaultEncoding, xmlSchemaStream, out validationErrors);
            if (!isValid)
            {
                Exception exception = CreateValidationException(validationErrors);
                throw exception;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema. The default encoding is used to read the xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, Stream xmlSchemaStream, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(xml, _defaultEncoding, xmlSchemaStream, null, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema. The default encoding is used to read the xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <param name="externalSchemas"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, Stream xmlSchemaStream, IDictionary<string, string> externalSchemas, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(xml, _defaultEncoding, xmlSchemaStream, externalSchemas, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, Encoding encoding, Stream xmlSchemaStream)
        {
            IEnumerable<ValidationEventArgs> validationErrors = null;
            bool isValid = Validate(xml, encoding, xmlSchemaStream, out validationErrors);
            if (!isValid)
            {
                Exception exception = CreateValidationException(validationErrors);
                throw exception;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, Encoding encoding, Stream xmlSchemaStream, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(xml, encoding, xmlSchemaStream, null, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <param name="externalSchemas"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, Encoding encoding, Stream xmlSchemaStream, IDictionary<string, string> externalSchemas)
        {
            IEnumerable<ValidationEventArgs> validationErrors = null;
            bool isValid = Validate(xml, encoding, xmlSchemaStream, externalSchemas, out validationErrors);
            if (!isValid)
            {
                Exception exception = CreateValidationException(validationErrors);
                throw exception;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchemaStream"></param>
        /// <param name="externalSchemas"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, Encoding encoding, Stream xmlSchemaStream, IDictionary<string, string> externalSchemas, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            _validationErrors = new List<ValidationEventArgs>();
            XmlSchema xmlSchema = XmlSchema.Read(xmlSchemaStream, new ValidationEventHandler(validatingReader_ValidationEventHandler));
            
            if (_validationErrors.Count > 0)
            {
                throw new ArgumentException("Invalid XmlSchema supplied");
            }

            return Validate(xml, encoding, xmlSchema, externalSchemas, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema. The default encoding is used to read the xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchema"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, XmlSchema xmlSchema)
        {
            Validate(xml, _defaultEncoding, xmlSchema);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchema"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, Encoding encoding, XmlSchema xmlSchema)
        {
            IEnumerable<ValidationEventArgs> validationErrors;
            bool isValid = Validate(xml, encoding, xmlSchema, out validationErrors);
            if (!isValid)
            {
                Exception exception = CreateValidationException(validationErrors);
                throw exception;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema.The default encoding is used to read the xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, XmlSchema xmlSchema, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(xml, _defaultEncoding, xmlSchema, null, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, Encoding encoding, XmlSchema xmlSchema, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(xml, encoding, xmlSchema, null, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="externalSchemas"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(string xml, Encoding encoding, XmlSchema xmlSchema, IDictionary<string, string> externalSchemas)
        {
            IEnumerable<ValidationEventArgs> validationErrors;
            bool isValid = Validate(xml, encoding, xmlSchema, externalSchemas, out validationErrors);
            if (!isValid)
            {
                Exception exception = CreateValidationException(validationErrors);
                throw exception;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema. The default encoding is used to read the xml string.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="externalSchemas"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, XmlSchema xmlSchema, IDictionary<string, string> externalSchemas, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(xml, _defaultEncoding, xmlSchema, externalSchemas, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="externalSchemas"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(string xml, Encoding encoding, XmlSchema xmlSchema, IDictionary<string, string> externalSchemas, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            MemoryStream xmlStream = new MemoryStream(encoding.GetBytes(xml));
            StreamReader streamReader = new StreamReader(xmlStream, encoding, false);
            return Validate(streamReader, xmlSchema, externalSchemas, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(StreamReader inputStream, XmlSchema xmlSchema, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            return Validate(inputStream, xmlSchema, null, out validationErrors);
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="externalSchemas"></param>
        /// <exception cref="System.ArgumentException">A System.ArgumentException is thrown when the xml is invalid.</exception>
        public static void Validate(StreamReader inputStream, XmlSchema xmlSchema, IDictionary<string, string> externalSchemas)
        {
            IEnumerable<ValidationEventArgs> validationErrors;
            bool isValid = Validate(inputStream, xmlSchema, externalSchemas, out validationErrors);
            if (!isValid)
            {
                Exception exception = CreateValidationException(validationErrors);
                throw exception;
            }
        }

        /// <summary>
        /// Validate the xml againt the xml schema.
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="xmlSchema"></param>
        /// <param name="externalSchemas"></param>
        /// <param name="validationErrors">When the xml is invalid this parameter contains the validation errors</param>
        /// <returns></returns>
        public static bool Validate(StreamReader xmlStream, XmlSchema xmlSchema, IDictionary<string, string> externalSchemas, out IEnumerable<ValidationEventArgs> validationErrors)
        {
            try
            {
                XmlParserContext parserContext = new XmlParserContext(null, null, string.Empty, XmlSpace.None);
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.ProhibitDtd = false;
                readerSettings.Schemas.Add(xmlSchema);
                if (externalSchemas != null)
                {
                    foreach (string schemaNamespace in externalSchemas.Keys)
                    {
                        readerSettings.Schemas.Add(schemaNamespace, externalSchemas[schemaNamespace]);
                    }
                }
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.ValidationEventHandler += new ValidationEventHandler(validatingReader_ValidationEventHandler);
                XmlReader validatingXmlReader = XmlReader.Create(xmlStream, readerSettings, parserContext);

                _validationErrors = new List<ValidationEventArgs>();

                while (validatingXmlReader.Read())
                {
                    // Just read the entire xml stream
                }

                // When there are validation errors return them.
                bool result = (_validationErrors.Count == 0);

                validationErrors = new List<ValidationEventArgs>(_validationErrors);

                return result;
            }
            finally
            {
                _validationErrors = null;
            }
        }

        private static void validatingReader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            // Add validation error events to the _validationErrors. These errors are handled when the xml-reader has read all xml text.
            _validationErrors.Add(e);
        }

        private static Exception CreateValidationException(IEnumerable<ValidationEventArgs> validationErrors)
        {
            StringBuilder validationErrorText = new StringBuilder();
            foreach (ValidationEventArgs validationError in validationErrors)
            {
                validationErrorText.AppendLine(validationError.Message);
            }
            return new ArgumentException(string.Format("Invalid xml supplied\r\n{0}", validationErrorText));
        }
        #endregion
    }
}
