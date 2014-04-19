using System;

namespace Luminis.Its.Services
{
    public interface IBaseObjectType
    {
        int Id { get; }
        string Name { get; }
        string RelativeUri { get; }
    }
}
