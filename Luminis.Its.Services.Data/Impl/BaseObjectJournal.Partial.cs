using System;

using Luminis.Its.Services;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Data.Impl
{
    public partial class BaseObjectJournal : IBaseObjectJournal
    {
        #region IJournalEntry Members
        public IBaseObject Parent
        {
            get { return BaseObject; }
            set { BaseObject = value as BaseObject; }
        }

        public TimePoint When
        {
            get { return new TimePoint(Timestamp); }
            set
            {
                if (value != null)
                {
                    Timestamp = value.TimeValue;
                }
                else
                {
                    Timestamp = DateTime.MaxValue;
                }
            }
        }

        public object Before
        {
            get { return BaseObjectValueBefore; }
            set { BaseObjectValueBefore = value as BaseObjectValue; }
        }

        public object After
        {
            get { return BaseObjectValueAfter; }
            set { BaseObjectValueAfter = value as BaseObjectValue; }
        }
        #endregion
    }
}
