using System;

using Luminis.Its.Services.Data;

namespace Luminis.Its.Services.Resources.Impl
{
    public class InternalBaseObjectType : IBaseObjectType
    {
        #region Constructors
        public InternalBaseObjectType(IBaseObjectType objectToClone)
        {
            Id = objectToClone.Id;
            Name = objectToClone.Name;
            RelativeUri = objectToClone.RelativeUri;
        }
        #endregion

        #region IBaseObjectType Members
        public int Id { get; set; }

        public string Name { get; set; }

        public string RelativeUri { get; set; }
        #endregion
    }
}
