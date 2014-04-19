using System;

using Luminis.Its.Services.Data;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Resources.Impl
{
    public class InternalBaseObjectValue : IBaseObjectValue
    {
        #region Constructors
        public InternalBaseObjectValue()
        {
            Id = Guid.NewGuid();
        }
        #endregion

        #region IBaseObjectValue Members

        public Guid Id { get; set; }

        public IBaseObject Parent { get; set; }

        public Guid ParentId
        {
            get
            {
                return Parent.Id;
            }
            set
            {
                Parent.Id = value;
            }
        }

        public IBaseObjectValue Reference
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

        public TimePointRange Range
        {
            get
            {
                return new TimePointRange(Start, End);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public TimePoint Start { get; set; }

        public TimePoint End { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public string Text
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

        public int Version
        {
            get
            {
                return 1;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
