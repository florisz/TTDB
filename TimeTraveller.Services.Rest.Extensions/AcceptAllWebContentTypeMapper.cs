using System.ServiceModel.Channels;

namespace TimeTraveller.Services.Rest.Extensions
{
    /// <see cref="http://blogs.msdn.com/carlosfigueira/archive/2008/04/17/wcf-raw-programming-model-receiving-arbitrary-data.aspx"/>
    public class AcceptAllWebContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            return WebContentFormat.Raw;
        }
    }
}
