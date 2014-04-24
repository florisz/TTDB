using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Linq;
using System.Text;

using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.Data.Impl
{

    public class DataService : IDataService
    {
        #region Private properties
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;
        private const string _defaultContentType = "application/xml; charset=utf-8";

        private ItsDb _dbContext = new ItsDb(); 
        private ILogger _logger;
        #endregion

        #region Constructors
        public DataService(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region IDataService Members
        public void Clean()
        {
            bool connectionOpenedHere = false;
            EntityConnection entityConnection = _dbContext.Connection as EntityConnection;
            DbConnection connection = entityConnection.StoreConnection;
            
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    connectionOpenedHere = true;
                }
                DbCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM ObjectJournal";
                _logger.Debug(command.CommandText);
                command.ExecuteNonQuery();

                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM ObjectValue";
                _logger.Debug(command.CommandText);
                command.ExecuteNonQuery();

                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Object";
                _logger.Debug(command.CommandText);
                command.ExecuteNonQuery();
            }
            finally
            {
                if (connectionOpenedHere)
                {
                    connection.Close();
                }
            }
        }

        public IBaseObject CreateBaseObject(IBaseObjectType type)
        {
            _logger.DebugFormat("CreateBaseObject({0})", type.Name);

            IBaseObject result = new BaseObject()
            {
                Type = type
            };

            return result;
        }

        public IBaseObjectJournal CreateBaseObjectJournal()
        {
            _logger.Debug("CreateBaseObjectJournal()");

            return new BaseObjectJournal();
        }

        public IBaseObjectValue CreateBaseObjectValue()
        {
            _logger.Debug("CreateBaseObjectValue()");

            return new BaseObjectValue();
        }

        public void DeleteBaseObject(IBaseObject deleteObject)
        {
            _dbContext.Attach(deleteObject as BaseObject);
            _dbContext.DeleteObject(deleteObject as BaseObject);
        }

        public IBaseObject GetBaseObject(Guid id)
        {
            var result = GetBaseObject(b => b.Id.Equals(id));

            _logger.DebugFormat("GetBaseObject({0}): {1}", id, result);

            return result;
        }

        public IBaseObject GetBaseObject(string key, IBaseObjectType type)
        {
            var result = GetBaseObject(b => b.ExtId.Equals(key) && b.BaseObjectType.Id.Equals(type.Id));

            _logger.DebugFormat("GetBaseObject({0}, {1}): {2}", key, type, result);

            return result;
        }

        public IBaseObject GetBaseObject(string key, IBaseObjectType type, IBaseObject parent)
        {
            var result = GetBaseObject(b => b.ExtId.Equals(key) && b.BaseObjectType.Id.Equals(type.Id) && b.Reference != null && b.Reference.Id.Equals(parent.Id));

            _logger.DebugFormat("GetBaseObject({0}, {1}, {2}): {3}", key, type, parent.Id, result);

            return result;
        }

        public IEnumerable<IBaseObject> GetBaseObjects(IBaseObjectType type)
        {
            var result = _dbContext.BaseObjectTypeSet.Include("BaseObjects").
                                    Where(b => b.Equals(type)).First().BaseObjects;

            _logger.DebugFormat("GetBaseObjects({0}): {1}", type, result.Count());

            return result.ToArray().Cast<IBaseObject>();
        }

        public IEnumerable<IBaseObject> GetBaseObjects(string extIdFilter, IBaseObjectType type)
        {
            var result = GetBaseObjects(b => b.ExtId.StartsWith(extIdFilter) && b.BaseObjectType.Id.Equals(type.Id));

            _logger.DebugFormat("GetBaseObjects({0}, {1}): {2}", extIdFilter, type, result.Count());

            return result.ToArray();
        }

        public IBaseObjectType GetBaseObjectType(int id)
        {
            var result = _dbContext.BaseObjectTypeSet.
                                    Where(b => b.Id.Equals(id)).First();

            _logger.DebugFormat("GetBaseObjectType({0}): {1}", id, result);

            return result;
        }

        public IBaseObjectType GetBaseObjectType(string name)
        {
            var result = _dbContext.BaseObjectTypeSet.
                                    Where(b => b.Name.Equals(name)).First();

            _logger.DebugFormat("GetBaseObjectType({0}): {1}", name, result);

            return result;
        }

        public IBaseObjectValue GetBaseObjectValue(Guid id)
        {
            var result = _dbContext.BaseObjectValueSet.Include("ParentBaseObject").Include("ParentBaseObject.BaseObjectType").
                                    Where(b => b.Id.Equals(id)).First();

            _logger.DebugFormat("GetBaseObjectValue({0}): {1}", id, result);

            return result;
        }

        public IEnumerable<IJournalEntry> GetJournal(IBaseObject baseObject, TimePointRange range)
        {
            DateTime startTimestamp = range.Start.TimeValue;
            if (startTimestamp <= System.Data.SqlTypes.SqlDateTime.MinValue.Value)
            {
                startTimestamp = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            }
            DateTime endTimestamp = range.End.TimeValue;
            if (endTimestamp >= System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
            {
                endTimestamp = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
            }

            BaseObject baseObjectImpl = baseObject as BaseObject;
            var result = _dbContext.BaseObjectJournalSet.Include("BaseObject").Include("BaseObjectValueBefore").Include("BaseObjectValueAfter").
                          Where(journalEntry => journalEntry.BaseObject.Id.Equals(baseObject.Id)
                             && journalEntry.Timestamp >= startTimestamp
                             && journalEntry.Timestamp <= endTimestamp);

            return result.ToArray().Cast<IJournalEntry>();
        }

        public IEnumerable<IRelationObject> GetRelations(IBaseObject relation1BaseObject, string relation)
        {
            IBaseObjectType relationType = GetBaseObjectType("Relation");
            var result = _dbContext.BaseObjectSet.Include("BaseObjectType").Include("BaseObjectReference").Include("BaseObjectRelation1").Include("BaseObjectRelation2").Include("BaseObjectValues").
                                    Where(baseObject => baseObject.BaseObjectType.Id.Equals(relationType.Id)
                                          && baseObject.ExtId.Equals(relation)
                                          && baseObject.BaseObjectRelation1.Id.Equals(relation1BaseObject.Id));

            _logger.DebugFormat("GetRelations({0}, {1}): {2}", relation1BaseObject.Id, relation, result.Count());

            return result.ToArray().Cast<IRelationObject>();
        }

        public IBaseObjectValue GetValue(Guid baseObjectId)
        {
            IBaseObject baseObject = GetBaseObject(baseObjectId);
            if (baseObject == null)
            {
                return null;
            }
            IBaseObjectValue objectValue = baseObject.Values.Last();

            _logger.DebugFormat("GetValue({0}): {1}", baseObjectId, objectValue);

            return objectValue;
        }

        public IBaseObjectValue GetValue(Guid baseObjectId, TimePoint timePoint)
        {
            IBaseObject baseObject = GetBaseObject(baseObjectId);
            if (baseObject == null)
            {
                return null;
            }
            return baseObject.GetValue(timePoint);
        }

        public IBaseObjectValue GetValue(Guid baseObjectId, int version)
        {
            IBaseObject baseObject = GetBaseObject(baseObjectId);
            if (baseObject == null)
            {
                return null;
            }
            return baseObject.GetValue(version);
        }

        public IBaseObjectValue GetValue(string extId, IBaseObjectType type)
        {
            IBaseObject baseObject = GetBaseObject(extId, type);
            if (baseObject == null)
            {
                _logger.DebugFormat("GetValue({0}, {1}): null", extId, type);

                return null;
            }
            IBaseObjectValue objectValue = baseObject.Values.Last();

            _logger.DebugFormat("GetValue({0}, {1}): {2}", extId, type, objectValue);

            return objectValue;
        }

        public IBaseObjectValue GetValue(string extId, IBaseObjectType type, TimePoint timePoint)
        {
            IBaseObject baseObject = GetBaseObject(extId, type);
            if (baseObject == null)
            {
                _logger.DebugFormat("GetValue({0}, {1}, {2}): null", extId, type, timePoint.TimeValue.ToString("O"));

                return null;
            }

            return baseObject.GetValue(timePoint);
        }

        public IBaseObjectValue GetValue(string extId, IBaseObjectType type, int version)
        {
            IBaseObject baseObject = GetBaseObject(extId, type);
            if (baseObject == null)
            {
                _logger.DebugFormat("GetValue({0}, {1}, {2}): null", extId, type, version);

                return null;
            }

            return baseObject.GetValue(version);
        }

        public IEnumerable<IBaseObjectValue> GetValues(string extIdFilter, IBaseObjectType type)
        {
            var result = _dbContext.BaseObjectValueSet.Include("ParentBaseObject").Include("ParentBaseObject.BaseObjectType").
                                    Where(baseObjectValue => baseObjectValue.ParentBaseObject.ExtId.Substring(0, extIdFilter.Length).Equals(extIdFilter)
                                          && baseObjectValue.ParentBaseObject.BaseObjectType.Id.Equals(type.Id));

            return result.ToArray().Cast<IBaseObjectValue>();
        }

        public IEnumerable<IBaseObjectValue> GetValues(IBaseObjectValue referenceObjectValue, string extId, IBaseObjectType type)
        {
            var result = _dbContext.BaseObjectValueSet.Include("ParentBaseObject").Include("ParentBaseObject.BaseObjectType").
                                    Where(baseObjectValue => baseObjectValue.BaseObjectValueReference.Id.Equals(referenceObjectValue.Id)
                                          && baseObjectValue.ParentBaseObject.ExtId.Equals(extId)
                                          && baseObjectValue.ParentBaseObject.BaseObjectType.Id.Equals(type.Id));

            _logger.DebugFormat("GetValues({0}, {1}, {2}): {3}", referenceObjectValue.Id, extId, type, result.Count());

            return result.ToArray().Cast<IBaseObjectValue>();
        }

        public void InsertBaseObject(IBaseObject newObject)
        {
            _logger.DebugFormat("InsertBaseObject({0}, {1}, {2})", newObject.Id, newObject.ExtId, newObject.Type);
            _dbContext.AddToBaseObjectSet(newObject as BaseObject);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, baseObject, null, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, baseObject, null, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject)
        {
            return InsertValue(content, contentType, timePoint, baseObject, null, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(content, contentType, timePoint, baseObject, null, journalInfo);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, baseObject, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, baseObject, referenceObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(content, contentType, timePoint, baseObject, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            _logger.DebugFormat("InsertValue1({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", content.Length, contentType, timePoint.TimeValue.ToString("O"), baseObject.Id, baseObject.ExtId, baseObject.Type, referenceObjectValue, journalInfo);

            BaseObjectValue lastValue = baseObject.Values.Last() as BaseObjectValue;
            lastValue.End = timePoint.AddSeconds(-1);

            BaseObjectValue newValue = new BaseObjectValue();
            newValue.Parent = baseObject;
            if (referenceObjectValue != null)
            {
                newValue.Reference = referenceObjectValue;
            }
            newValue.Start = timePoint;
            newValue.Content = content;
            newValue.ContentType = contentType;
            newValue.Version = baseObject.Values.Count();

            if (journalInfo != null)
            {
                BaseObjectJournal journal = new BaseObjectJournal();
                journal.After = newValue;
                journal.Before = lastValue;
                journal.Parent = baseObject;
                journal.Username = journalInfo.Username;
                journal.When = timePoint;
                _dbContext.AddToBaseObjectJournalSet(journal);
            }

            _dbContext.AddToBaseObjectValueSet(newValue);

            return newValue;
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, id, extId, type, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, id, extId, type, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, id, extId, type, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, id, extId, type, referenceObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            _logger.DebugFormat("InsertValue2({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", content.Length, contentType, timePoint.TimeValue.ToString("O"), id, extId, type, referenceObjectValue, journalInfo);
            if (referenceObjectValue != null)
            {
                _logger.DebugFormat("InsertValue2(referenceObjectValue.id={0}", referenceObjectValue.Id);
            }

            BaseObject newObject = new BaseObject();
            newObject.Id = id;
            newObject.Type = type;
            newObject.ExtId = extId;
            _dbContext.AddToBaseObjectSet(newObject);
            if (referenceObjectValue != null)
            {
                newObject.Reference = referenceObjectValue.Parent;
            }

            BaseObjectValue newValue = new BaseObjectValue();
            _logger.DebugFormat("newValue.id={0}", newValue.Id);
            newValue.ParentBaseObject = newObject;
            if (referenceObjectValue != null)
            {
                newValue.Reference = referenceObjectValue;
            }
            newValue.Start = timePoint;
            newValue.Content = content;
            newValue.ContentType = contentType;
            newValue.Version = 1;

            if (journalInfo != null)
            {
                BaseObjectJournal journal = new BaseObjectJournal();
                journal.After = newValue;
                journal.Parent = newObject;
                journal.Username = journalInfo.Username;
                journal.When = timePoint;
                _dbContext.AddToBaseObjectJournalSet(journal);
            }

            _dbContext.AddToBaseObjectValueSet(newValue);

            return newValue;
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, id, extId, type, extReferenceId, referenceType, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(_defaultEncoding.GetBytes(content), _defaultContentType, timePoint, id, extId, type, extReferenceId, referenceType, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, extReferenceId, referenceType, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, WebHttpHeaderInfo journalInfo)
        {
            _logger.DebugFormat("InsertValue3({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}. {8})", content.Length, contentType, timePoint.TimeValue.ToString("O"), id, extId, type, extReferenceId, referenceType, journalInfo);

            IBaseObject refObject = GetBaseObject(extReferenceId, referenceType);

            BaseObject newObject = new BaseObject();
            newObject.Id = id;
            newObject.Type = type;
            newObject.ExtId = extId;
            newObject.Reference = refObject;

            BaseObjectValue newValue = new BaseObjectValue();
            newValue.ParentBaseObject = newObject;
            newValue.Start = timePoint;
            newValue.Content = content;
            newValue.ContentType = contentType;
            newValue.Version = 1;

            if (journalInfo != null)
            {
                BaseObjectJournal journal = new BaseObjectJournal();
                journal.After = newValue;
                journal.Parent = newObject;
                journal.Username = journalInfo.Username;
                journal.When = timePoint;
                _dbContext.AddToBaseObjectJournalSet(journal);
            }

            _dbContext.AddToBaseObjectValueSet(newValue);

            return newValue;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
        #endregion

        #region Private Methods
        private IBaseObject GetBaseObject(Func<BaseObject, bool> func)
        {
            var result = GetBaseObjects(func);

            return result.FirstOrDefault();
        }

        private IEnumerable<IBaseObject> GetBaseObjects(Func<BaseObject, bool> func)
        {
            var result = _dbContext.BaseObjectSet.Include("BaseObjectType").Include("BaseObjectRelation1").Include("BaseObjectRelation2").Include("BaseObjectValues").Where(func);

            return result.Cast<IBaseObject>();
        }
        #endregion
    }
}
