using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace TimeTraveller.Services.Rest.Extensions
{
    public class CustomizedErrorHandlingWebHttpBehavior: WebHttpBehavior
    {
        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new CustomizedErrorHandler());
        }
    }
}
