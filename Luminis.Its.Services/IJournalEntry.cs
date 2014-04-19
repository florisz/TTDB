using System;

using Luminis.Patterns.Range;

namespace Luminis.Its.Services
{
    public interface IJournalEntry
    {
        int Id { get; }
        TimePoint When { get; }
        string Username { get; }
        object Before { get; }
        object After { get; }
    }
}
