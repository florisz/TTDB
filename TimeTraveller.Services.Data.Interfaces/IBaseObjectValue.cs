using System;
using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.Data.Interfaces
{
    public interface IBaseObjectValue
    {
        Guid Id { get; set; }
        IBaseObject Parent { get; set; }
        Guid ParentId { get; }
        IBaseObjectValue Reference { get; set; }
        Guid ReferenceId { get; }
        TimePointRange Range { get; set; }
        TimePoint Start { get; set; }
        TimePoint End { get; set; }
        string ContentType { get; set; }
        byte[] Content { get; set; }
        string Text { get; set; }
        int Version { get; set; }
    }
}
