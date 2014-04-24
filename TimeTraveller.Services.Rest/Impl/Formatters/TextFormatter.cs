using System.IO;

namespace TimeTraveller.Services.Rest.Impl.Formatters
{
    public class TextFormatter : IFormatter
    {
        #region Constructors
        public TextFormatter()
        {
        }
        #endregion

        #region IFormatter Members
        public Stream Format(CommandContext context, object item)
        {
            string text = (string)item;
            byte[] resultBuffer = context.Encoding.GetBytes(text);
            Stream result = new MemoryStream(resultBuffer);
            context.ContentType = context.RequestedContentType;

            return result;
        }
        #endregion
    }
}
