﻿using System.IO;

namespace Luminis.Its.Services.Rest.Impl.Formatters
{
    public class XmlFormatter : IFormatter
    {
        #region Constructors
        public XmlFormatter()
        {
        }
        #endregion

        #region IFormatter Members
        public Stream Format(CommandContext context, object item)
        {
            string text = (string)item;
            byte[] resultBuffer = context.Encoding.GetBytes(text);
            Stream result = new MemoryStream(resultBuffer);
            context.ContentType = WebOperationContentType.Xml;

            return result;
        }
        #endregion
    }
}
