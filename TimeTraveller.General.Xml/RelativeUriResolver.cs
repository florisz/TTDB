using System;
using System.Net;
using System.Xml;

namespace TimeTraveller.General.Xml
{
    public class RelativeUriResolver : XmlResolver
    {
        //
        // Fixme: Floris, what an example of beautiful documented code!!
        //
        public override ICredentials Credentials
        {
            set { ; }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return new XmlUrlResolver().GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            string result = null;
            if (baseUri != null)
            {
                result = string.Format("{0}://{1}:{2}{3}", "/resources/scripts/", baseUri.Scheme, baseUri.Host, baseUri.Port, "/its", relativeUri);
            }
            else
            {
                result = string.Format("http://localhost:8080/its/resources/scripts/{0}", relativeUri);
            }
            return new Uri(result);
        }
    }
}
