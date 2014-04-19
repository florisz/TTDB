using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Data.Impl
{
    partial class BaseObject : IBaseObject, IEntityObject, IRelationObject
    {
        #region IBaseObject/IEntity/IRelation Members
        public IBaseObject Reference
        {
            get
            {
                return this.BaseObjectReference as IBaseObject;
            }
            set
            {
                this.BaseObjectReference = value as BaseObject;
            }
        }

        private Guid _referenceId = Guid.Empty;
        public Guid ReferenceId
        {
            get
            {
                Guid result = _referenceId;
                if (_referenceId != Guid.Empty && this.BaseObjectReferenceReference != null && this.BaseObjectReferenceReference.EntityKey != null)
                {
                    result = new Guid((string)this.BaseObjectReferenceReference.EntityKey.EntityKeyValues[0].Value);
                }

                return result;
            }
            set
            {
                _referenceId = value;
            }
        }

        public IEntityObject Relation1
        {
            get
            {
                return this.BaseObjectRelation1 as IEntityObject;
            }
            set
            {
                this.BaseObjectRelation1 = value as BaseObject;
            }
        }

        public Guid Relation1Id
        {
            get
            {
                Guid result = Guid.Empty;
                if (this.BaseObjectRelation1Reference != null && this.BaseObjectRelation1Reference.EntityKey != null)
                {
                    result = (Guid)this.BaseObjectRelation1Reference.EntityKey.EntityKeyValues[0].Value;
                }

                return result;
            }
        }

        public IEntityObject Relation2
        {
            get
            {
                return this.BaseObjectRelation2 as IEntityObject;
            }
            set
            {
                this.BaseObjectRelation2 = value as BaseObject;
            }
        }

        public Guid Relation2Id
        {
            get
            {
                Guid result = Guid.Empty;
                if (this.BaseObjectRelation2Reference != null && this.BaseObjectRelation2Reference.EntityKey != null)
                {
                    result = (Guid)this.BaseObjectRelation2Reference.EntityKey.EntityKeyValues[0].Value;
                }

                return result;
            }
        }

        public IBaseObjectType Type
        {
            get
            {
                return this.BaseObjectType;
            }
            set
            {
                this.BaseObjectType = (BaseObjectType)value;
            }
        }

        public IEnumerable<IBaseObjectValue> Values
        {
            get
            {
                var result = (from b in this.BaseObjectValues
                              orderby b.Version
                              select b);
                return result.Cast<IBaseObjectValue>();
            }
        }

        public IBaseObjectValue GetValue()
        {
            var result = Values.LastOrDefault();
            return result;
        }

        public IBaseObjectValue GetValue(TimePoint timePoint)
        {
            var result = Values.Where(baseObjectValue => baseObjectValue.Range.Includes(timePoint)).FirstOrDefault();
            return result;
        }

        public IBaseObjectValue GetValue(int version)
        {
            var result = Values.Where(baseObjectValue => baseObjectValue.Version.Equals(version)).FirstOrDefault();
            return result;
        }
        #endregion
    }
}
