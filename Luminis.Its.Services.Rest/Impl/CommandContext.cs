using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using Luminis.Logging;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Rest.Impl
{
    public class CommandContext
    {
        #region Private Properties
        private const string _assemblyExtension = ".dll";
        private const string _historyParameter = "history";
        private const string _editParameter = "edit";
        private const string _representationParameter = "view";
        private const string _summaryParameter = "summary";
        private const string _timePointParameter = "timepoint";
        private const string _versionParameter = "version";
        private const string _xsdExtension = ".xsd";
        private const string _trueValue = "true";

        private static readonly Encoding _defaultEncoding = Encoding.UTF8;
        private static readonly WebOperationContentType _defaultContentType = WebOperationContentType.Xml;

        private ILogger _logger = NullLogger.Instance;
        #endregion

        #region Constructors
        public CommandContext(WebOperationContext context, Type type, params object[] arguments)
            : this(context, type.Name, false, arguments)
        {
        }

        public CommandContext(WebOperationContext context, Type type, bool fetchMultiple, params object[] arguments)
            : this(context, type.Name, fetchMultiple, arguments)
        {
        }

        public CommandContext(WebOperationContext context, string typeName, params object[] arguments)
            : this(context, typeName, false, arguments)
        {
        }

        public CommandContext(WebOperationContext context, string typeName, bool fetchMultiple, params object[] arguments)
        {
            Arguments = arguments;
            WebOperationContext = context;
            ContentType = _defaultContentType;
            FetchMultiple = fetchMultiple;
            RequestedId = GetRequestedId(arguments);
            Type = typeName;
        }
        #endregion

        #region Public Properties
        public string Accept
        {
            get
            {
                return Request.Headers[HttpRequestHeader.Accept];
            }
        }

        public object[] Arguments { get; private set; }

        public Uri BaseUri
        {
            get
            {
                return GetBaseUri();
            }
        }

        public WebOperationContentType ContentType { get; set; }

        public Encoding Encoding
        {
            get
            {
                return GetEncoding(Request);
            }
        }

        public string Extension { get; private set; }

        public bool HistoryRequested
        {
            get
            {
                string queryParameterValue = QueryParameters[_historyParameter];
                if (string.IsNullOrEmpty(queryParameterValue))
                    return false;
                return string.Equals(queryParameterValue.ToLower(), _trueValue);
            }
        }

        public bool EditRequested
        {
            get
            {
                string queryParameterValue = QueryParameters[_editParameter];
                if (string.IsNullOrEmpty(queryParameterValue))
                    return false;
                return string.Equals(queryParameterValue.ToLower(), _trueValue);
            }
        }

        public bool SummaryRequested
        {
            get
            {
                string queryParameterValue = QueryParameters[_summaryParameter];
                if (string.IsNullOrEmpty(queryParameterValue))
                    return false;
                return string.Equals(queryParameterValue.ToLower(), _trueValue);
            }
        }

        public bool IsTimePointSpecified
        {
            get
            {
                return (TimePoint != null);
            }
        }

        public bool VersionRequested
        {
            get
            {
                return !int.MinValue.Equals(VersionNumber);
            }
        }

        public bool RepresentationRequested
        {
            get
            {
                return !string.IsNullOrEmpty(Representation);
            }
        }

        public WebHttpHeaderInfo JournalInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Headers[HttpRequestHeader.From]))
                {
                    WebHttpHeaderInfo result = new WebHttpHeaderInfo()
                    {
                        Username = Request.Headers[HttpRequestHeader.From]
                    };
                    return result;
                }
                else
                {
                    throw new ArgumentNullException(string.Format("HTTP header does not contain {0}", HttpRequestHeader.From));
                }
            }
        }

        public bool FetchMultiple { get; private set; }

        public NameValueCollection QueryParameters
        {
            get
            {
                return UriTemplateMatch.QueryParameters;
            }
        }

        public string RelativeUri
        {
            get
            {
                return GetRelativeUri(BaseUri, RequestUri);
            }
        }

        public string Representation
        {
            get
            {
                return QueryParameters[_representationParameter];
            }
        }

        public IncomingWebRequestContext Request
        {
            get
            {
                return WebOperationContext.IncomingRequest;
            }
        }

        public string RequestBody
        {
            get
            {
                return GetRequestBody();
            }
        }

        public byte[] RequestBodyBuffer
        {
            get
            {
                return GetRequestBodyBuffer();
            }
        }

        public WebOperationContentType RequestedContentType
        {
            get
            {
                return GetRequestedContentType(Request);
            }
        }

        public string RequestedId { get; private set; }

        public Uri RequestUri
        {
            get
            {
                return UriTemplateMatch.RequestUri;
            }
        }

        public OutgoingWebResponseContext Response
        {
            get
            {
                return WebOperationContext.OutgoingResponse;
            }
        }

        public TimePoint TimePoint
        {
            get
            {
                TimePoint result = null;
                string queryParameterValue = QueryParameters[_timePointParameter];
                if (!string.IsNullOrEmpty(queryParameterValue))
                {
                    //Format: yyyy-MM-ddTHH:mm:ss.fffffffK, ie. 2008-04-10T06:30:00.0000000+01:00
                    result = new TimePoint(DateTime.Parse(queryParameterValue, CultureInfo.InvariantCulture));
                }
                return result;
            }
        }

        public string Type { get; private set; }

        public UriTemplateMatch UriTemplateMatch
        {
            get
            {
                return Request.UriTemplateMatch;
            }
        }

        public int VersionNumber
        {
            get
            {
                int result = int.MinValue;

                string queryParameterValue = QueryParameters[_versionParameter];
                if (!string.IsNullOrEmpty(queryParameterValue))
                {
                    result = int.Parse(queryParameterValue);
                }
                return result;
            }
        }

        public WebOperationContext WebOperationContext { get; private set; }
        #endregion

        #region Public Methods
        public T GetArgument<T>(int index)
        {
            return (T)Arguments[index];
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <returns></returns>
        public string GetRelativeUri()
        {
            return GetRelativeUri(BaseUri, RequestUri);
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        public string GetRelativeUri(Uri baseUri)
        {
            return GetRelativeUri(baseUri, RequestUri);
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        public string GetRelativeUri(string baseUri)
        {
            return GetRelativeUri(baseUri, RequestUri);
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public string GetRelativeUri(Uri baseUri, Uri requestUri)
        {
            return GetRelativeUri(baseUri.ToString(), requestUri.ToString());
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public string GetRelativeUri(Uri baseUri, string requestUri)
        {
            return GetRelativeUri(baseUri.ToString(), requestUri);
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public string GetRelativeUri(string baseUri, Uri requestUri)
        {
            return GetRelativeUri(baseUri, requestUri.ToString());
        }

        /// <summary>
        /// Strip the base uri from the request uri.
        /// 
        /// Example:
        ///   requesturi=http:///localhost/its/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid
        ///   baseuri=http:///localhost/its
        ///   
        ///   result => '/casefiles/myspec/mycasefileid/is.fixed.text/mycasefilesubid'
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public string GetRelativeUri(string baseUri, string requestUri)
        {
            string result = requestUri.Replace(baseUri, string.Empty);

            return result;
        }

        public string StripQueryParameters(string requestUri)
        {
            string result = requestUri;

            int indexOfQuestionMark = result.IndexOf("?");
            if (indexOfQuestionMark > -1)
            {
                result = result.Substring(0, indexOfQuestionMark);
            }

            return result;
        }

        #endregion

        #region Private Methods
        private Uri GetBaseUri()
        {
            string result = string.Format("{0}://{1}:{2}{3}", RequestUri.Scheme, RequestUri.Host, RequestUri.Port, UriTemplateMatch.BaseUri.PathAndQuery);
            return new Uri(result);
        }

        private Encoding GetEncoding(string headerText)
        {
            Encoding result = _defaultEncoding;
            try
            {
                if (!headerText.Contains("*"))
                {
                    string encodingName = headerText;
                    string[] parts = headerText.Split(';', ',', ' ');
                    string charsetPart = (from part in parts
                                          where part.StartsWith("charset")
                                          select part).FirstOrDefault();
                    if (!string.IsNullOrEmpty(charsetPart))
                    {
                        parts = charsetPart.Split('=');
                        if (parts.Length == 2)
                        {
                            encodingName = parts[1];
                        }
                    }
                    if (!string.IsNullOrEmpty(encodingName))
                    {
                        result = Encoding.GetEncoding(encodingName);
                    }
                }
            }
            catch (ArgumentException)
            {
                result = _defaultEncoding;
            }
            return result;
        }

        private Encoding GetEncoding(IncomingWebRequestContext request)
        {
            Encoding result = _defaultEncoding;

            if (!string.IsNullOrEmpty(request.Accept))
            {
                result = GetEncoding(request.Accept);
            }
            else if (!string.IsNullOrEmpty(request.ContentType))
            {
                result = GetEncoding(request.ContentType);
            }
            else if (!string.IsNullOrEmpty(request.Headers[HttpRequestHeader.AcceptCharset]))
            {
                result = GetEncoding(request.Headers[HttpRequestHeader.AcceptCharset]);
            }
            return result;
        }

        private string GetRequestBody()
        {
            byte[] result = GetRequestBodyBuffer();
            return Encoding.GetString(result);
        }

        private byte[] GetRequestBodyBuffer()
        {
            byte[] result = null;

            Stream bodyStream = Arguments.LastOrDefault() as Stream;
            if (bodyStream != null)
            {
                MemoryStream buffer = new MemoryStream();

                int byteRead = bodyStream.ReadByte();
                while (byteRead != -1)
                {
                    buffer.WriteByte((byte)byteRead);
                    byteRead = bodyStream.ReadByte();
                }
                buffer.Close();

                result = buffer.ToArray();
            }

            return result;
        }

        private WebOperationContentType GetRequestedContentType(IncomingWebRequestContext request)
        {
            WebOperationContentType result = WebOperationContentType.Xml;
            string acceptHeader = request.Headers[HttpRequestHeader.Accept];
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                string[] parts = acceptHeader.Split(',', ' ', ';');
                IDictionary<string, WebOperationContentType> acceptedContentTypes = new Dictionary<string, WebOperationContentType>();
                acceptedContentTypes.Add("text/html", WebOperationContentType.Html);
                acceptedContentTypes.Add("application/xml", WebOperationContentType.Xml);
                acceptedContentTypes.Add("*/*", WebOperationContentType.Html);
                acceptedContentTypes.Add("text/plain", WebOperationContentType.Text);
                foreach (string acceptedContentType in acceptedContentTypes.Keys)
                {
                    bool contains = (from p in parts
                                     where p.Equals(acceptedContentType)
                                     select true).FirstOrDefault();
                    if (contains)
                    {
                        result = acceptedContentTypes[acceptedContentType];
                        break;
                    }
                }
            }
            return result;
        }

        private string GetRequestedId(object[] arguments)
        {
            StringBuilder resultText = new StringBuilder();
            foreach (object argument in arguments)
            {
                if (typeof(string).IsAssignableFrom(argument.GetType()))
                {
                    if (resultText.Length > 0)
                    {
                        resultText.Append("/");
                    }
                    resultText.Append(argument);
                }
            }

            string result = resultText.ToString();

            if (result.EndsWith(_assemblyExtension))
            {
                result = result.Replace(_assemblyExtension, string.Empty);
                Extension = _assemblyExtension;
            }
            else if (result.EndsWith(_xsdExtension))
            {
                result = result.Replace(_xsdExtension, string.Empty);
                Extension = _xsdExtension;
            }

            return result;
        }

        #endregion
    }
}
