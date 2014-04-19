using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;

namespace Luminis.Its.Client.Model
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
            string result = encoding.GetString(xmlBuffer,0, xmlBuffer.Length);
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

        #endregion
    }
}
