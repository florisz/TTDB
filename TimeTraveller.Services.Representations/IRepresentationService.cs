using System;
using System.Collections.Generic;
using System.Text;

using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Representations
{
    public interface IRepresentationService
    {
        /// <summary>
        /// Convert the xml into a Representation
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        Representation Convert(string xml, Encoding encoding);

        /// <summary>
        /// Get the representation for the given representationname.
        /// </summary>
        /// <param name="representationname"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Representation Get(string representationname, Uri baseUri);

        /// <summary>
        /// Get the representation for the given representationname.
        /// </summary>
        /// <param name="representationname"></param>
        /// <param name="timePoint">the timepoint for the case file content</param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Representation Get(string representationname, TimePoint timePoint, Uri baseUri);

        /// <summary>
        /// Get the representation for the given representationname.
        /// </summary>
        /// <param name="representationname"></param>
        /// <param name="versionNumber">the version number to retrieve</param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Representation Get(string representationname, int versionNumber, Uri baseUri);

        /// <summary>
        /// Get the representations for the given objectmodel and specificationname.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="specificationname"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        IEnumerable<Representation> GetEnumerable(string objectmodelname, string specificationname, Uri baseUri);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="representation"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(Representation representation, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="representation"></param>
        /// <param name="timePointRange"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(Representation representation, TimePointRange timePointRange, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the list formatted according to the given encoding and content-type.
        /// </summary>
        /// <param name="representations"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetList(IEnumerable<Representation> representations, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the summary for the given representation.
        /// </summary>
        /// <param name="representation"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetSummary(Representation representation, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Convert the Representation into xml
        /// </summary>
        /// <param name="representation"></param>
        /// <returns></returns>
        string GetXml(Representation representation, Encoding encoding);

        /// <summary>
        /// Get the XmlSchema-address for the meta-XmlSchema.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        string GetXmlSchemaAddress(Uri baseUri);

        /// <summary>
        /// Get the XmlSchema-name for the meta-XmlSchema.
        /// </summary>
        /// <returns></returns>
        string GetXmlSchemaName();

        /// <summary>
        /// Get the XmlSchema-text for the meta-XmlSchema.
        /// </summary>
        /// <returns></returns>
        string GetXmlSchemaText();

        /// <summary>
        /// Store the representation.
        /// </summary>
        /// <param name="representationname"></param>
        /// <param name="representation"></param>
        /// <param name="baseUri"></param>
        /// <param name="info"></param>
        /// <returns>true when the Representation is created, false when the Representation is updated</returns>
        bool Store(string representationname, Representation representation, Uri baseUri, IHeaderInfo info);

        /// <summary>
        /// Transform the xml using the given representation.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="representation"></param>
        /// <returns></returns>
        string Transform(string xml, Representation representation);
    }
}
