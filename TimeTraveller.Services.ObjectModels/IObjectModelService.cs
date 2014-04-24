using System;
using System.Collections.Generic;
using System.Text;

using TimeTraveller.Services.Data;
using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.ObjectModels
{
    public interface IObjectModelService
    {
        /// <summary>
        /// 
        /// </summary>
        IBaseObjectType BaseObjectType { get; }

        /// <summary>
        /// Convert an object model xml to an ObjectModel.
        /// </summary>
        /// <param name="objectModelXml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        ObjectModel Convert(string objectModelXml, Encoding encoding);

        /// <summary>
        /// Convert an object model value to an ObjectModel.
        /// </summary>
        /// <param name="objectModelObjectValue"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        ObjectModel Convert(IBaseObjectValue objectModelObjectValue, Uri baseUri);

        /// <summary>
        /// Get the latest version of the ObjectModel.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        ObjectModel Get(string objectmodelname, Uri baseUri);

        /// <summary>
        /// Get the version of the ObjectModel specified by the point in time.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="timePoint"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        ObjectModel Get(string objectmodelname, TimePoint timePoint, Uri baseUri);

        /// <summary>
        /// Get the specified version of the ObjectModel.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="versionNumber"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        ObjectModel Get(string objectmodelname, int versionNumber, Uri baseUri);

        /// <summary>
        /// Get all the ObjectModels.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        IEnumerable<ObjectModel> GetEnumerable(Uri baseUri);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="objectModel"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(ObjectModel objectModel, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="objectModel"></param>
        /// <param name="timePointRange"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(ObjectModel objectModel, TimePointRange timePointRange, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the list formatted according to the given encoding and content-type.
        /// </summary>
        /// <param name="objectmodels"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetList(IEnumerable<ObjectModel> objectmodels, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the summary for the given objectmodel.
        /// </summary>
        /// <param name="objectModel"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetSummary(ObjectModel objectModel, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Convert an ObjectModel to xml.
        /// </summary>
        /// <param name="objectModel"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetXml(ObjectModel objectModel, Encoding encoding);

        /// <summary>
        /// Get the XmlSchema for the ObjectModel.
        /// </summary>
        /// <param name="objectmodel"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetXmlSchema(ObjectModel objectmodel, Encoding encoding);

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
        /// Store the objectmodel as a new objectmodel or as a new version of the existing one.
        /// </summary>
        /// <param name="objectmodelname"></param>
        /// <param name="objectModel"></param>
        /// <param name="baseUri"></param>
        /// <param name="info"></param>
        /// <returns>true when the objectModel is created, false when the objectModel is updated</returns>
        bool Store(string objectmodelname, ObjectModel objectModel, Uri baseUri, WebHttpHeaderInfo info);
    }
}
