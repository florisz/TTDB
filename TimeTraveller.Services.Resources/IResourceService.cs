using System;
using System.Collections.Generic;
using System.Text;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Resources
{
    public interface IResourceService
    {
        /// <summary>
        /// 
        /// </summary>
        IBaseObjectType BaseObjectType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Resource Get(string id, Uri baseUri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timePoint"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Resource Get(string id, TimePoint timePoint, Uri baseUri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="versionNumber"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        Resource Get(string id, int versionNumber, Uri baseUri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        IEnumerable<Resource> GetEnumerable(Uri baseUri);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(Resource resource, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the history for the given objectmodel.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="timePointRange"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetHistory(Resource resource, TimePointRange timePointRange, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the list formatted according to the given encoding and content-type.
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetList(IEnumerable<Resource> resources, Uri baseUri, Encoding encoding);

        /// <summary>
        /// Get the summary for the given resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="baseUri"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string GetSummary(Resource resource, Uri baseUri, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <param name="baseUri"></param>
        /// <param name="info"></param>
        /// <returns>true when the Resource is created, false when updated</returns>
        bool Store(string id, Resource resource, Uri baseUri, IHeaderInfo info);
    }
}
