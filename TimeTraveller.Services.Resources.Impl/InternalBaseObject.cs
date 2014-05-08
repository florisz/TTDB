using System;
using System.Collections.Generic;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Resources.Impl
{
    public class InternalBaseObject : IBaseObject
    {
        #region Constructors
        public InternalBaseObject()
        {
            Id = Guid.NewGuid();
        } 
        #endregion

        #region IBaseObject Members
        public Guid Id { get; set; }

        public string ExtId { get; set; }

        public string InternalResourceId { get; set; }

        public IBaseObject Reference
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid ReferenceId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IBaseObjectType Type { get; set; }

        public IEnumerable<IBaseObjectValue> Values { get; set; }

        public IBaseObjectValue GetValue()
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue GetValue(TimePoint timePoint)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue GetValue(int version)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
