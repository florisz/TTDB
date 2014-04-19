using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Luminis.Its.Services.Rest
{
    [ServiceContract(Namespace="http://luminis.net/its")]
    public interface IRestService
    {
        [OperationContract()]
        [WebGet(UriTemplate = "casefiles/{objectmodelname}/{specificationname}/*")]
        Stream GetCaseFile(string objectmodelname, string specificationname);

        [OperationContract()]
        [WebGet(UriTemplate = "casefiles/{objectmodelname}/{specificationname}/")]
        Stream GetCaseFiles(string objectmodelname, string specificationname);

        [OperationContract()]
        [WebGet(UriTemplate = "specifications/casefiles/{objectmodelname}/{specificationname}")]
        Stream GetCaseFileSpecification(string objectmodelname, string specificationname);

        [OperationContract()]
        [WebGet(UriTemplate = "specifications/casefiles/{objectmodelname}/")]
        Stream GetCaseFileSpecifications(string objectmodelname);

        [OperationContract()]
        [WebGet(UriTemplate = "specifications/objectmodels/{objectmodelname}")]
        Stream GetObjectModel(string objectmodelname);

        [OperationContract()]
        [WebGet(UriTemplate = "specifications/objectmodels/")]
        Stream GetObjectModels();

        [OperationContract()]
        [WebGet(UriTemplate = "representations/{objectmodelname}/{specificationname}/{representationname}")]
        Stream GetRepresentation(string objectmodelname, string specificationname, string representationname);

        [OperationContract()]
        [WebGet(UriTemplate = "representations/{objectmodelname}/{specificationname}/")]
        Stream GetRepresentations(string objectmodelname, string specificationname);

        [OperationContract()]
        [WebGet(UriTemplate = "resources/*")]
        Stream GetResource();

        [OperationContract()]
        [WebGet(UriTemplate = "resources/")]
        Stream GetResources();

        [OperationContract()]
        [WebGet(UriTemplate = "/")]
        Stream GetRepositoryInfo();

        [OperationContract()]
        [WebGet(UriTemplate = "rules/{objectmodelname}/{specificationname}/{rulename}")]
        Stream GetRule(string objectmodelname, string specificationname, string rulename);

        [OperationContract()]
        [WebGet(UriTemplate = "rules/{objectmodelname}/{specificationname}/")]
        Stream GetRules(string objectmodelname, string specificationname);

        [OperationContract()]
        [WebGet(UriTemplate = "schemas/{schemaname}")]
        Stream GetXmlSchema(string schemaname);

        [OperationContract()]
        [WebGet(UriTemplate = "schemas/")]
        Stream GetXmlSchemas();

        [OperationContract()]
        [WebInvoke(Method = "PUT", UriTemplate = "casefiles/{objectmodelname}/{specificationname}/*")]
        Stream StoreCaseFile(string objectmodelname, string specificationname, Stream caseFile);

        [OperationContract()]
        [WebInvoke(Method = "PUT", UriTemplate = "specifications/casefiles/{objectmodelname}/{specificationname}")]
        Stream StoreCaseFileSpecification(string objectmodelname, string specificationname, Stream specification);

        [OperationContract()]
        [WebInvoke(Method = "PUT", UriTemplate = "specifications/objectmodels/{objectmodelname}")]
        Stream StoreObjectModel(string objectmodelname, Stream objectmodel);

        [OperationContract()]
        [WebInvoke(Method = "PUT", UriTemplate = "representations/{objectmodelname}/{specificationname}/{representationname}")]
        Stream StoreRepresentation(string objectmodelname, string specificationname, string representationname, Stream representation);

        [OperationContract()]
        [WebInvoke(Method="PUT", UriTemplate = "resources/*")]
        Stream StoreResource(Stream resource);

        [OperationContract()]
        [WebInvoke(Method = "PUT", UriTemplate = "rules/{objectmodelname}/{specificationname}/{rulename}")]
        Stream StoreRule(string objectmodelname, string specificationname, string rulename, Stream rule);
    }
}
