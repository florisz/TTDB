using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Net.Sockets;
using Luminis.Its.Client.Model;


namespace Luminis.Its.Client
{
    public class ItsClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uriAdress"></param>
        /// <returns> list of strings containing the href links of the response on the uriadress</returns>
        static public List<string> GetObjectModelList(string serverUri)
        {
            // Sample Document is <list>
            // <List>
            //   <Link rel="objectmodel" href="http://localhost:8080/its/specifications/objectmodels/ExtendedModel?summary=true" />
            //   <Link rel="objectmodel" href="http://localhost:8080/its/specifications/objectmodels/SimpleModel?summary=true" />
            // </List>
            string uriOfModels = String.Format("{0}/specifications/objectmodels", serverUri);

            XDocument content = XDocument.Parse(Get(uriOfModels));
            Regex expr = new Regex(@"objectmodels/(\w*)");

            var om = from models in content.Root.Descendants()
                     select expr.Match(models.Attribute("href").Value).Groups[1].ToString();

            return om.ToList();
        }

        static public List<string> GetCaseFileSpecList(string serverUri, string objectModelSpec)
        {
            string uriOfCaseFileSpecs = String.Format("{0}/specifications/casefiles/{1}", serverUri, objectModelSpec);

            XDocument content = XDocument.Parse(Get(uriOfCaseFileSpecs));
            Regex expr = new Regex( objectModelSpec + @"/(\w*)");

            var om = from models in content.Root.Descendants()
                     select expr.Match(models.Attribute("href").Value).Groups[1].ToString();

            return om.ToList();
        }

        static public ObjectModel GetObjectModel(string serverUri, string objectModel)
        {
            string uriOfModel = String.Format("{0}/specifications/objectmodels/{1}", serverUri, objectModel);
            string result = Get(uriOfModel);

            return ObjectModelService.Convert(result);
        }

        static public CaseFileSpecification GetCaseFileSpec(string serverUri, string objectModel, string caseFileSpec)
        {
            string uriOfModel = String.Format("{0}/specifications/casefiles/{1}/{2}", serverUri, objectModel, caseFileSpec);
            string result = Get(uriOfModel);

            return CaseFileSpecificationService.Convert(result);
        }

        static public ObjectModel PutObjectModel(string serverUri, string objectModel, ObjectModel newModel)
        {
            string uriOfModel = String.Format("{0}/specifications/objectmodels/{1}", serverUri, objectModel);
            string modelXML = ObjectModelService.GetXML(newModel);

            string result = Put(uriOfModel, modelXML);

            return ObjectModelService.Convert(result);
        }

        static public CaseFileSpecification PutCaseFileSpecification(string serverUri, string objectModel, CaseFileSpecification newModel)
        {
            string uriOfModel = String.Format("{0}/specifications/casefiles/{1}/{2}", serverUri,objectModel, newModel.Name);
            
            string modelXML = CaseFileSpecificationService.GetXML(newModel);

            string result = Put(uriOfModel, modelXML);

            return CaseFileSpecificationService.Convert(result);
        }

        public static string Get(string uri)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
            webClient.Encoding = Encoding.Unicode;

            string result = webClient.DownloadString(uri);

            return result;
        }

        public static string Put(string uri, string content)
        {
            try
            {
                WebClient webClient = new WebClient();
                Encoding enc = Encoding.Unicode;  // Windows default Code Page

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/xml; charset=utf-16");
                webClient.Headers.Add(HttpRequestHeader.From, "workbench@luminis.nl");

                byte[] requestBuffer = Encoding.Unicode.GetBytes(content);
                byte[] resultBuffer = webClient.UploadData(uri, "PUT", requestBuffer);
                string result = Encoding.Unicode.GetString(resultBuffer);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
