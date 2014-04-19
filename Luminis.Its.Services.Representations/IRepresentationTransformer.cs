using System;

namespace Luminis.Its.Services.Representations
{
    public interface IRepresentationTransformer
    {
        string Transform(string script, string xml);
    }
}
