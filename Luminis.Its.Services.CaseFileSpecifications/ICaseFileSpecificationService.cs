using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Luminis.Its.Services.Data;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.CaseFileSpecifications
{
    public interface ICaseFileSpecificationService
    {
        /// <summary>
        /// Convert a case file specification xml to a CaseFileSpecification.
        /// </summary>
        /// <param name="specificationXml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        CaseFileSpecification Convert(string specificationXml, Encoding encoding);

        /// <summary>
        /// Convert a case file specification value to a CaseFileSpecification.
        /// </summary>
        /// <param name="specificationObjectValue"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        CaseFileSpecification Convert(IBaseObjectValue specificationObjectValue, Uri baseUri);

        /// <summary>
        /// Get the compiled assembly for the casefile specification xml schema.
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        Assembly GetAssembly(CaseFileSpecification specification);

        /// <summary>
        /// Get the byte-array of the compiled assembly for the casefile specification xml schema.
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        byte[] GetAssemblyBytes(CaseFileSpecification specification);

        /// <summary>
        /// Get the latest version of the CaseFileSpecification.
        /// </summary>
        /// <param name="specificationname"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        CaseFileSpecification Get(string specificationname, Uri baseUri);

        /// <summary>
        /// Get the version of the CaseFileSpecification specified by the point in time.
        /// </summary>
        /// <param name="specificationname"></param>
        /// <param name="timePoint"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        CaseFileSpecification Get(string specificationname, TimePoint timePoint, Uri baseUri);

        /// <summary>
        /// Get the specified version of the CaseFileSpecification.
        /// </summary>
        /// <param name="specificationname"></param>
        /// <param name="versionNumber"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        CaseFileSpecification Get(string specificationname, int versionNumber, Uri baseUri);

        /// <summary>
        /// Get all the CaseFileSpecifications for the given objectmodel.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        IEnumerable<CaseFileSpecification> GetEnumerable(string objectmodelname, Uri baseUri);

        /// <summary>
        /// Get the history for the given specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(CaseFileSpecification specification, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the list formatted according to the given encoding and content-type.
        /// </summary>
        /// <param name="specifications"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetList(IEnumerable<CaseFileSpecification> specifications, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the summary for the given specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetSummary(CaseFileSpecification specification, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Convert a CaseFileSpecification to xml.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetXml(CaseFileSpecification specification, Encoding encoding);

        /// <summary>
        /// Get the complete XmlSchema for the CaseFileSpecification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetXmlSchema(CaseFileSpecification specification, Encoding encoding);

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
        /// Validate that the caseFileId is correctly formatted according to the UriTemplate in the specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <returns></returns>
        bool IsValidCaseFileId(CaseFileSpecification specification, string caseFileId);

        /// <summary>
        /// Validate that the caseFileId is correctly formatted according to the UriTemplate in the specification.
        /// Also validate that the caseFileId is correct according to the xpath queries in the UriTemplate.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        bool IsValidCaseFileId(CaseFileSpecification specification, string caseFileId, string xml);

        /// <summary>
        /// Store the CaseFileSpecification as a new casefilespecification or as a new version of the existing one.
        /// </summary>
        /// <param name="specificationname"></param>
        /// <param name="specification"></param>
        /// <param name="baseUri"></param>
        /// <param name="info"></param>
        /// <returns>true when the CaseFileSpecification is created, false when the CaseFileSpecification is updated</returns>
        bool Store(string specificationname, CaseFileSpecification specification, Uri baseUri, WebHttpHeaderInfo info);

        /// <summary>
        /// Validate that the caseFileId is correctly formatted according to the UriTemplate in the specification.
        /// Also validate that the caseFileId is correct according to the xpath queries in the UriTemplate.
        /// And validate that the xml is correct according to the XMLSchema of the specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <param name="xml"></param>
        void ValidateCaseFile(CaseFileSpecification specification, string caseFileId, string xml);

        /// <summary>
        /// Validate that the caseFileId is correctly formatted according to the UriTemplate in the specification.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        void ValidateCaseFileId(CaseFileSpecification specification, string caseFileId);

        /// <summary>
        /// Validate that the caseFileId is correctly formatted according to the UriTemplate in the specification.
        /// Also validate that the caseFileId is correct according to the xpath queries in the UriTemplate.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="caseFileId"></param>
        /// <param name="xml"></param>
        void ValidateCaseFileId(CaseFileSpecification specification, string caseFileId, string xml);
    }
}
