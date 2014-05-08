using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Data.Interfaces
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
