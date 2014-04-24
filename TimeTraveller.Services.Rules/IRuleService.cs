using System;
using System.Collections.Generic;
using System.Text;

using TimeTraveller.Services.CaseFiles;
using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.Rules
{
    public interface IRuleService
    {
        /// <summary>
        /// Convert the xml into a Rule
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        Rule Convert(string xml, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="caseFile"></param>
        /// <returns></returns>
        CaseFile Execute(Rule rule, CaseFile caseFile);

        /// <summary>
        /// Get the rule for the given rulename.
        /// </summary>
        /// <param name="rulename"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Rule Get(string rulename, Uri baseUri);

        /// <summary>
        /// Get the rule for the given rulename.
        /// </summary>
        /// <param name="rulename"></param>
        /// <param name="timePoint">the timepoint for the case file content</param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Rule Get(string rulename, TimePoint timePoint, Uri baseUri);

        /// <summary>
        /// Get the rule for the given rulename.
        /// </summary>
        /// <param name="rulename"></param>
        /// <param name="versionNumber">the version number to retrieve</param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Rule Get(string rulename, int versionNumber, Uri baseUri);

        /// <summary>
        /// Get the rules for the given objectmodel and specificationname.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="specificationname"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        IEnumerable<Rule> GetEnumerable(string objectmodelname, string specificationname, Uri baseUri);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(Rule rule, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="timePointRange"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(Rule rule, TimePointRange timePointRange, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the list formatted according to the given encoding and content-type.
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetList(IEnumerable<Rule> rules, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the summary for the given rule.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetSummary(Rule rule, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Convert the Rule into xml
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        string GetXml(Rule rule, Encoding encoding);

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
        /// Store the rule.
        /// </summary>
        /// <param name="rulename"></param>
        /// <param name="rule"></param>
        /// <param name="baseUri"></param>
        /// <param name="info"></param>
        /// <returns>true when the Rule is created, false when the Rule is updated</returns>
        bool Store(string rulename, Rule rule, Uri baseUri, WebHttpHeaderInfo info);
    }
}
