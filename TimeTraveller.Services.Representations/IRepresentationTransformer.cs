namespace TimeTraveller.Services.Representations
{
    public interface IRepresentationTransformer
    {
        string Transform(string script, string xml);
    }
}
