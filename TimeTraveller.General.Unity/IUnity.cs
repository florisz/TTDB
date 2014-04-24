using System.Collections.Generic;

namespace TimeTraveller.General.Unity
{
    public interface IUnity
    {
        T Resolve<T>();
        T Resolve<T>(string name);
        IEnumerable<T> ResolveAll<T>();
    }
}
