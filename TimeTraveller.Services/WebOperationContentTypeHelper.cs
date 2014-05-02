using System;
using System.Text;
using TimeTraveller.Services.Interface;

namespace TimeTraveller.Services
{

    public sealed class WebOperationContentTypeHelper
    {
        private const string _assemblyContentType = "application/x-msdownload";
        private const string _htmlContentType = "text/html; charset={0}";
        private const string _textContentType = "text/plain; charset={0}";
        private const string _xmlContentType = "application/xml; charset={0}";

        private WebOperationContentTypeHelper()
        {
        }

        public static string Convert(WebOperationContentType type, Encoding encoding)
        {
            switch (type)
            {
                case WebOperationContentType.Assembly:
                    return _assemblyContentType;
                case WebOperationContentType.Html:
                    return string.Format(_htmlContentType, encoding.HeaderName);
                case WebOperationContentType.Other:
                    return string.Empty;
                case WebOperationContentType.Text:
                    return string.Format(_textContentType, encoding.HeaderName);
                case WebOperationContentType.Xml:
                    return string.Format(_xmlContentType, encoding.HeaderName);
                default:
                    throw new ArgumentException(string.Format("Invalid WebOperationContentType {0} specified", type));
            }
        }
    }
}
