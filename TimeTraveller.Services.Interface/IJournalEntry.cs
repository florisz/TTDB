using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.Interfaces
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
