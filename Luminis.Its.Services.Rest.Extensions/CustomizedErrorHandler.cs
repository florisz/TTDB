using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Luminis.Its.Services.Rest.Extensions
{
    public class CustomizedErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members
        public bool HandleError(Exception error)
        {
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException<string> faultException = new FaultException<string>(FormatException(error), error.Message);
            fault = Message.CreateMessage(version, faultException.CreateMessageFault(), string.Empty);

            var httpResponseMessageProp = new HttpResponseMessageProperty();
            httpResponseMessageProp.Headers[HttpResponseHeader.ContentType] = "application/xml; charset=utf-8";
            httpResponseMessageProp.StatusCode = GetHttpStatusCode(error);

            fault.Properties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProp);
        }
        #endregion

        #region Private Properties
        private static HttpStatusCode GetHttpStatusCode(Exception exception)
        {
            HttpStatusCode result = HttpStatusCode.InternalServerError;
            if (typeof(ArgumentOutOfRangeException).IsAssignableFrom(exception.GetType()))
            {
                result = HttpStatusCode.NotFound;
            }
            else if (typeof(ArgumentException).IsAssignableFrom(exception.GetType()))
            {
                result = HttpStatusCode.BadRequest;
            }
            else if (typeof(InvalidOperationException).IsAssignableFrom(exception.GetType()))
            {
                result = HttpStatusCode.MethodNotAllowed;
            }
            else if (typeof(NotImplementedException).IsAssignableFrom(exception.GetType()))
            {
                result = HttpStatusCode.NotImplemented;
            }
            return result;
        }

        private static string FormatException(Exception exception)
        {
            StringBuilder result = new StringBuilder();
            FormatException(result, exception);
            return result.ToString();
        }

        private static void FormatException(StringBuilder result, Exception exception)
        {
            result.AppendLine(string.Format("{0}: {1}", exception.GetType().FullName, exception.Message));
            if (exception.InnerException != null)
            {
                result.AppendLine("--- Inner exception --- ");
                FormatException(result, exception.InnerException);
                result.AppendLine("--- End of inner exception stack trace ---");
            }
            result.AppendLine(exception.StackTrace);
        } 
        #endregion
    }
}
