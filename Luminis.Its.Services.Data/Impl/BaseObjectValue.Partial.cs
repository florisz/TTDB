using System;
using System.Data;
using System.Text;

using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Data.Impl
{
    partial class BaseObjectValue : IBaseObjectValue
    {
        #region Constructors
        public BaseObjectValue()
        {
            this.Id = Guid.NewGuid();
            this.StartDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            this.EndDate = TimePoint.Future.TimeValue;
        }
        #endregion

        #region IBaseObjectValue Members
        public string Text
        {
            get
            {
                if (ContentType.Equals("application/xml; charset=utf-8"))
                {
                    return Encoding.UTF8.GetString(Content);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    Content = Encoding.UTF8.GetBytes(value);
                    ContentType = "application/xml; charset=utf-8";
                }
                else
                {
                    Content = null;
                }
            }
        }

        public IBaseObject Parent
        {
            get
            {
               return this.ParentBaseObject as IBaseObject; 
            }
            set
            {
                this.ParentBaseObject = value as BaseObject;
            }
        }

        public Guid ParentId
        {
            get
            {
                Guid result = Guid.Empty;
                if (this.ParentBaseObjectReference != null && this.ParentBaseObjectReference.EntityKey != null)
                {
                    result = new Guid((string)this.BaseObjectValueReferenceReference.EntityKey.EntityKeyValues[0].Value);
                }

                return result;
            }
        }

        public TimePointRange Range
        {
            get
            {
                return new TimePointRange(this.StartDate, this.EndDate);
            }
            set
            {
                if (value != null)
                {
                    this.StartDate = value.Start.TimeValue;
                    this.EndDate = value.End.TimeValue;
                }
                else
                {
                    this.StartDate = TimePoint.Past.TimeValue;
                    this.EndDate = TimePoint.Future.TimeValue;
                }
            }
        }

        public IBaseObjectValue Reference
        {
            get
            {
                return this.BaseObjectValueReference as IBaseObjectValue;
            }
            set
            {
                this.BaseObjectValueReference = value as BaseObjectValue;
            }
        }

        private Guid _referenceId = Guid.Empty;
        public Guid ReferenceId
        {
            get
            {
                Guid result = _referenceId;
                if (result.Equals(Guid.Empty) && this.BaseObjectValueReferenceReference != null && this.BaseObjectValueReferenceReference.EntityKey != null)
                {
                    result = (Guid)this.BaseObjectValueReferenceReference.EntityKey.EntityKeyValues[0].Value;
                }

                return result;
            }
            set
            {
                _referenceId = value;
            }
        }


        public TimePoint Start
        {
            get
            {
                return new TimePoint(this.StartDate);
            }
            set
            {
                if (value != null)
                {
                    this.StartDate = value.TimeValue;
                }
                else
                {
                    this.StartDate = TimePoint.Past.TimeValue;
                }
            }
        }

        public TimePoint End
        {
            get
            {
                return new TimePoint(this.EndDate);
            }
            set
            {
                if (value != null)
                {
                    this.EndDate = value.TimeValue;
                }
                else
                {
                    this.EndDate = TimePoint.Future.TimeValue;
                }
            }
        }

        #endregion
    }
}
