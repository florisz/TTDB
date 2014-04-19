using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using Luminis.Its.Services;
using Luminis.Its.Services.ObjectModels;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Repository
{
    public interface IRepositoryService
    {
        string GetList(Uri baseUri, Encoding encoding);
        ObjectModel GetObjectModel(string objectmodelname, Uri baseUri, NameValueCollection queryParameters);
        IEnumerable<ObjectModel> GetObjectModels(Uri baseUri);
        string GetXmlSchema(string schemaName);
        string GetXmlSchemas(Uri baseUri, Encoding encoding);
    }
}
