using System;
using System.Collections.Generic;

namespace Luminis.Unity
{
    public interface IUnity
    {
        T Resolve<T>();
        T Resolve<T>(string name);
        IEnumerable<T> ResolveAll<T>();
    }
}
