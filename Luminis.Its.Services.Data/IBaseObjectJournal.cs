using System;

using Luminis.Its.Services;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Data
{
    public interface IBaseObjectJournal : IJournalEntry
    {
        new int Id { get; }
        IBaseObject Parent { get; set; }
        new TimePoint When { get; set; }
        new string Username { get; set; }
        new object Before { get; set; }
        new object After { get; set; }
    }
}
