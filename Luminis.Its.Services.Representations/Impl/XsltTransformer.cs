using System;
using System.IO;

using Luminis.Xml.Xslt;

namespace Luminis.Its.Services.Representations.Impl
{
    public class XsltTransformer : IRepresentationTransformer
    {
        #region Constructors
        public XsltTransformer()
        {
        }
        #endregion

        #region IRepresentationTransformer Members

        public string Transform(string script, string xml)
        {
            StringReader xsltReader = new StringReader(script);
            string result = XsltHelper.Transform(xsltReader, xml);

            return result;
        }

        #endregion
    }
}
