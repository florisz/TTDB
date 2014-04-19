using System.IO;

using Luminis.Its.Services.Resources;
using Luminis.Unity;
using Luminis.Xml.Xslt;
using Microsoft.Practices.Unity;

namespace Luminis.Its.Services.Rest.Impl.Formatters
{
    public class HtmlFormatter : AbstractChainedFormatter, IFormatter
    {
        #region Private Properties
        private readonly string _xsltResourceName;
        #endregion

        #region Dependencies
        [Dependency]
        public IUnity Container { get; set; }

        [Dependency("TextFormatter")]
        public override IFormatter Chain { get; set; }
        #endregion

        #region Constructors
        public HtmlFormatter(string xsltResourceName)
        {
            _xsltResourceName = xsltResourceName;
        }
        #endregion

        #region IFormatter Members
        public override Stream Format(CommandContext context, object item)
        {
            string xml = (string)item;
            Resource script = GetResource(Container, _xsltResourceName);
            string html = Transform(script, xml);

            return Chain.Format(context, html);
        }
        #endregion

        #region Protected Methods
        protected static Resource GetResource(IUnity container, string resourceId)
        {
            IResourceService resourceService = container.Resolve<IResourceService>();
            return resourceService.Get(resourceId, null);
        }

        protected static string Transform(Resource xsltResource, string xml)
        {
            MemoryStream xsltStream = new MemoryStream(xsltResource.Content);
            TextReader xsltReader = new StreamReader(xsltStream, true);

            string result = XsltHelper.Transform(xsltReader, xml);
            return result;
        }
        #endregion
    }
}
