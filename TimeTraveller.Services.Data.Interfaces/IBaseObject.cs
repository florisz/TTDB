using System;
using System.Collections.Generic;

using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Data.Interfaces
{
    public interface IBaseObject
    {
        Guid Id { get; set; }
        string ExtId { get; set; }
        IBaseObject Reference { get; set; }
        Guid ReferenceId { get; }
        IBaseObjectType Type { get; set; } 
        IEnumerable<IBaseObjectValue> Values { get; }
        IBaseObjectValue GetValue();
        IBaseObjectValue GetValue(TimePoint timePoint);
        IBaseObjectValue GetValue(int version);
    }
}
