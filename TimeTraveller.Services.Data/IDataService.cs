using System;
using System.Collections.Generic;
using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.Data
{
    public interface IDataService
    {
        void Clean();
        IBaseObject CreateBaseObject(IBaseObjectType type);
        IBaseObjectJournal CreateBaseObjectJournal();
        IBaseObjectValue CreateBaseObjectValue();
        void DeleteBaseObject(IBaseObject deleteObject);
        IBaseObject GetBaseObject(Guid id);
        IBaseObject GetBaseObject(string key, IBaseObjectType type);
        IBaseObject GetBaseObject(string key, IBaseObjectType type, IBaseObject parent);
        IEnumerable<IBaseObject> GetBaseObjects(IBaseObjectType type);
        IEnumerable<IBaseObject> GetBaseObjects(string extIdFilter, IBaseObjectType type);
        IBaseObjectType GetBaseObjectType(int id);
        IBaseObjectType GetBaseObjectType(string name);
        IBaseObjectValue GetBaseObjectValue(Guid id);
        IEnumerable<IJournalEntry> GetJournal(IBaseObject baseObject, TimePointRange range);
        IEnumerable<IRelationObject> GetRelations(IBaseObject baseObject, string relation);
        IBaseObjectValue GetValue(Guid baseObjectId);
        IBaseObjectValue GetValue(Guid baseObjectId, TimePoint timePoint);
        IBaseObjectValue GetValue(Guid baseObjectId, int version);
        IBaseObjectValue GetValue(string extId, IBaseObjectType type);
        IBaseObjectValue GetValue(string extId, IBaseObjectType type, TimePoint timePoint);
        IBaseObjectValue GetValue(string extId, IBaseObjectType type, int version);
        IEnumerable<IBaseObjectValue> GetValues(string extIdFilter, IBaseObjectType type);
        IEnumerable<IBaseObjectValue> GetValues(IBaseObjectValue referenceObjectValue, string extId, IBaseObjectType type);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType);
        IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, WebHttpHeaderInfo journalInfo);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType);
        IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, WebHttpHeaderInfo journalInfo);
        void SaveChanges();
    }
}
