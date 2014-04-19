using System;

using Luminis.Patterns.Range;

namespace Luminis.Its.Services
{
    public interface IItem
    {
        IBaseObjectType BaseObjectType { get; }
        string ExtId { get; }
        Guid Id { get; }
        string SelfUri { get; set; }
        ItemType Type { get; }
        int Version { get; }

        IJournalEntry[] GetHistory();
        IJournalEntry[] GetHistory(TimePoint from);
        IJournalEntry[] GetHistory(TimePoint from, TimePoint to);

        string GetUri(Uri baseUri, UriType uriType, params string[] additionalQueryArguments);
        string GetUri(Uri baseUri, UriType uriType, int versionNumber, params string[] additionalQueryArguments);
    }
}
