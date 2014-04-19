using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using Luminis.Unity;

namespace Luminis.Unity.Impl
{
    public class UnityContainerWrapper : IUnity
    {
        #region Private Properties
        private IUnityContainer _unityContainer;
        #endregion

        #region Constructors
        public UnityContainerWrapper(IUnityContainer untityContainer)
        {
            _unityContainer = untityContainer;
        }
        #endregion

        #region IUnity Members
        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return _unityContainer.Resolve<T>(name);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _unityContainer.ResolveAll<T>();
        }
        #endregion
    }
}
