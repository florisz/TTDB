using System.Text;

using Luminis.Unity;

namespace Luminis.Its.Services.Rest.Impl
{
    /// <summary>
    /// The Formatter factory can create IFormatter instances based on the request context and using the Unity-container.
    /// 
    /// The formatter to lookup is constructed from the context, i.e.:
    /// 
    ///  CaseFilesHtml: 
    ///    CaseFile = the requested type
    ///    s = the list is requested
    ///    Html = the Accept-header contains 'text/html'
    ///    
    /// </summary>
    public sealed class FormatterFactory
    {
        #region Constructors
        private FormatterFactory()
        {
        }
        #endregion

        #region Factory Method
        public static IFormatter Create(IUnity container, CommandContext context)
        {
            IFormatter result = null;
            string formatterId = GetFormatterId(context);
            try
            {
                result = container.Resolve<IFormatter>(formatterId);
            }
            catch
            {
                // When the formatter including the requested content-type is not found
                // let's try it without this content type.
                formatterId = GetFormatterId(context, false);
                result = container.Resolve<IFormatter>(formatterId);
            }
            return result;
        }
        #endregion

        #region Private Methods
        private static string GetFormatterId(CommandContext context)
        {
            return GetFormatterId(context, true);
        }

        private static string GetFormatterId(CommandContext context, bool includingRequestedContentType)
        {
            StringBuilder result = new StringBuilder();
            result.Append(context.Type);
            if (context.FetchMultiple)
            {
                result.Append("s");
            }

            if (context.HistoryRequested)
            {
                result.Append("History");
            }
            else if (context.SummaryRequested)
            {
                result.Append("Summary");
            }
            else if (context.RepresentationRequested)
            {
                result.Append("Representation");
            }
            else if (context.EditRequested)
            {
                result.Append("Edit");
            }

            if (!string.IsNullOrEmpty(context.Extension))
            {
                result.Append(context.Extension);
            }
            else if (includingRequestedContentType)
            {
                result.Append(context.RequestedContentType);
            }

            result.Append("Formatter");

            return result.ToString();
        }
        #endregion
    }
}
