using System;
using System.Collections.Generic;
using System.Text;

using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.CaseFiles
{
    public interface ICaseFileService
    {
        /// <summary>
        /// Convert xml to a CaseFile.
        /// </summary>
        /// <param name="caseFileXml">the xml string containing the CaseFile information</param>
        /// <returns>the CaseFile</returns>
        CaseFile Convert(string caseFileXml, Encoding encoding);

        /// <summary>
        /// Retrieve the CaseFile for the specification and caseFileId. 
        /// The current timepoint is taken to determine the case file content.
        /// </summary>
        /// <param name="specification">the CaseFileSpecification</param>
        /// <param name="caseFileId">the id for the CaseFile</param>
        /// <param name="baseUri"></param>
        /// <returns>the CaseFile</returns>
        CaseFile Get(CaseFileSpecification specification, string caseFileId, Uri baseUri);

        /// <summary>
        /// Retrieve the CaseFile for the specification and caseFileId and the given timepoint.
        /// </summary>
        /// <param name="specification">the CaseFileSpecification</param>
        /// <param name="caseFileId">the id for the CaseFile</param>
        /// <param name="timePoint">the timepoint for the case file content</param>
        /// <param name="baseUri"></param>
        /// <returns>the CaseFile</returns>
        CaseFile Get(CaseFileSpecification specification, string caseFileId, TimePoint timePoint, Uri baseUri);

        /// <summary>
        /// Retrieve the CaseFile for the specification and caseFileId and the given version number.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <param name="versionNumber">the version number to retrieve</param>
        /// <param name="baseUri"></param>
        /// <returns>the CaseFile</returns>
        CaseFile Get(CaseFileSpecification specification, string caseFileId, int versionNumber, Uri baseUri);

        /// <summary>
        /// Retrieve the CaseFiles for the specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="baseUri"></param>
        /// <returns>the CaseFile</returns>
        IEnumerable<CaseFile> GetEnumerable(CaseFileSpecification specification, Uri baseUri);

        /// <summary>
        /// Get the history for the given casefile.
        /// </summary>
        /// <param name="caseFile"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(CaseFile caseFile, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the history for the given casefile.
        /// </summary>
        /// <param name="caseFile"></param>
        /// <param name="timePointRange"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(CaseFile caseFile, TimePointRange timePointRange, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the list formatted according to the given encoding and content-type.
        /// </summary>
        /// <param name="specifications"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetList(IEnumerable<CaseFile> caseFiles, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the summary for the given case file.
        /// </summary>
        /// <param name="caseFile"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetSummary(CaseFile caseFile, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Convert the CaseFile to xml.
        /// </summary>
        /// <param name="caseFile">the casefile to convert</param>
        /// <returns>the string containing the xml</returns>
        string GetXml(CaseFile caseFile, Encoding encoding);

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
        /// Store the CaseFile.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <param name="caseFile"></param>
        /// <param name="baseUri"></param>
        /// <param name="info"></param>
        /// <returns>true when the CaseFile is created, false when the CaseFile is updated</returns>
        bool Store(CaseFileSpecification specification, string caseFileId, CaseFile caseFile, Uri baseUri, IHeaderInfo info);
    }
}
