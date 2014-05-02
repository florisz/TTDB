using System;
using System.Collections.Generic;
using System.Text;

using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;

namespace TimeTraveller.Services.Interfaces
{
    public interface ITimeLineService<T> where T: IItem
    {
        ILogger Logger { get; }
        IUnity Container { get; }

        T Convert(string xml, Encoding encoding);
        
        T Get(string id, Uri baseUri);
        T Get(string id, TimePoint timePoint, Uri baseUri);
        T Get(string id, int versionNumber, Uri baseUri);
        IEnumerable<T> GetEnumerable(Uri baseUri);
        IEnumerable<T> GetEnumerable(string parentId, Uri baseUri);
        string GetXml(T item, Encoding encoding);
        string GetXmlSchemaAddress(Uri baseUri);
        string GetXmlSchemaName();
        string GetXmlSchemaText();
        bool Store(string id, T item, Uri baseUri, IUserInfo info);
    }
}
