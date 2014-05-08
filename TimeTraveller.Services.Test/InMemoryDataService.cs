using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Data.Interfaces;

namespace TimeTraveller.Services.Test
{
    public class InMemoryDataService : IDataService
    {
        #region Private Properties
        private ILogger _logger;
        private Dictionary<Guid, BaseObject> _baseObjectsById = new Dictionary<Guid, BaseObject>();
        private Dictionary<string, BaseObject> _baseObjectsByKey = new Dictionary<string, BaseObject>();
        private Dictionary<ItemType, BaseObjectType> _baseObjectTypes = new Dictionary<ItemType, BaseObjectType>();
        #endregion

        #region Constructors
        public InMemoryDataService(ILogger logger)
        {
            _logger = logger;

            _baseObjectTypes.Add(ItemType.CaseFile, new BaseObjectType()
            {
                Id = (int)ItemType.CaseFile,
                Name = "CaseFile",
                RelativeUri = "/casefiles/"
            });
            _baseObjectTypes.Add(ItemType.CaseFileSpecification, new BaseObjectType()
            {
                Id = (int)ItemType.CaseFileSpecification,
                Name = "CaseFileSpecification",
                RelativeUri = "/specifications/casefiles/"
            });
            _baseObjectTypes.Add(ItemType.Entity, new BaseObjectType()
            {
                Id = (int)ItemType.Entity,
                Name = "Entity"
            });
            _baseObjectTypes.Add(ItemType.ObjectModel, new BaseObjectType()
            {
                Id = (int)ItemType.ObjectModel,
                Name = "ObjectModel",
                RelativeUri = "/specifications/objectmodels/"
            });
            _baseObjectTypes.Add(ItemType.Relation, new BaseObjectType()
            {
                Id = (int)ItemType.Relation,
                Name = "Relation"
            });
            _baseObjectTypes.Add(ItemType.Representation, new BaseObjectType()
            {
                Id = (int)ItemType.Representation,
                Name = "Representation",
                RelativeUri = "/representations/"
            });
            _baseObjectTypes.Add(ItemType.Resource, new BaseObjectType()
            {
                Id = (int)ItemType.Resource,
                Name = "Resource",
                RelativeUri = "/resources/"
            });
            _baseObjectTypes.Add(ItemType.RuleSet, new BaseObjectType()
            {
                Id = (int)ItemType.RuleSet,
                Name = "RuleSet",
                RelativeUri = "/rules/"
            });
        }
        #endregion

        #region Public Methods
        public void LogContent()
        {
            _logger.Debug("DATASERVICE:");
            foreach (Guid id in _baseObjectsById.Keys)
            {
                IBaseObject baseObject = _baseObjectsById[id];
                _logger.DebugFormat("BaseObject: id={0}, extId={1}, type={2}", baseObject.Id, baseObject.ExtId, baseObject.Type.Name);
                foreach (IBaseObjectValue value in baseObject.Values)
                {
                    _logger.DebugFormat("\tBaseObjectValue: id={0}, start={1}, end={2}\r\n\tcontent={3}", value.Id, value.Start.TimeValue,value.End.TimeValue, value.Text);
                }
            }
        }

        #endregion

        #region IDataService Members
        public void Clean()
        {
            _baseObjectsById.Clear();
            _baseObjectsByKey.Clear();
        }

        public IBaseObject CreateBaseObject(IBaseObjectType type)
        {
            IBaseObject result = new BaseObject()
            {
                Type = type
            };

            return result;
        }

        public IBaseObjectJournal CreateBaseObjectJournal()
        {
            return new BaseObjectJournal();
        }

        public IBaseObjectValue CreateBaseObjectValue()
        {
            return new BaseObjectValue();
        }

        public void DeleteBaseObject(IBaseObject deleteObject)
        {
            _baseObjectsById.Remove(deleteObject.Id);
            _baseObjectsByKey.Remove(deleteObject.ExtId);

        }

        public IBaseObject GetBaseObject(Guid id)
        {
            if (_baseObjectsById.ContainsKey(id))
            {
                return _baseObjectsById[id];
            }
            else
            {
                return null;
            }
        }

        public IBaseObject GetBaseObject(string key, IBaseObjectType type)
        {
            if (_baseObjectsByKey.ContainsKey(key))
            {
                return _baseObjectsByKey[key];
            }
            else
            {
                return null;
            }
        }

        public IBaseObject GetBaseObject(string key, IBaseObjectType type, IBaseObject parent)
        {
            var result = (from b in _baseObjectsById.Values
                          where b.ExtId.Equals(key) && b.BaseObjectType.Id.Equals(type.Id) && b.Reference != null && b.Reference.Id.Equals(parent.Id)
                          select b).FirstOrDefault();

            _logger.DebugFormat("GetBaseObject({0}, {1}, {2}): {3}", key, type, parent.Id, result);

            return result;
        }

        public IEnumerable<IBaseObject> GetBaseObjects(IBaseObjectType type)
        {
            var result = (from baseObject in _baseObjectsById.Values
                          where baseObject.BaseObjectType.Id.Equals((int)type.Id)
                          select baseObject);

            return result.Cast<IBaseObject>();
        }

        public IEnumerable<IBaseObject> GetBaseObjects(string extIdFilter, IBaseObjectType type)
        {
            var result = (from baseObject in _baseObjectsById.Values
                          where baseObject.BaseObjectType.Id.Equals((int)type.Id)
                          && baseObject.ExtId.StartsWith(extIdFilter)
                          select baseObject);

            return result.Cast<IBaseObject>();
        }

        public IBaseObjectType GetBaseObjectType(int type)
        {
            return _baseObjectTypes[ItemTypeHelper.Convert(type)];
        }

        public IBaseObjectType GetBaseObjectType(string type)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue GetBaseObjectValue(Guid id)
        {
            IBaseObjectValue result = null;
            foreach (IBaseObject baseObject in _baseObjectsById.Values)
            {
                foreach (IBaseObjectValue baseObjectvalue in baseObject.Values)
                {
                    if (baseObjectvalue.Id.Equals(id))
                    {
                        result = baseObjectvalue;
                        break;
                    }
                }
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        public IEnumerable<IJournalEntry> GetJournal(IBaseObject baseObject, TimePointRange range)
        {
            BaseObject baseObjectImpl = baseObject as BaseObject;   
            var result = (from journalEntry in baseObjectImpl.BaseObjectJournal
                          where range.Includes(journalEntry.When)
                          select journalEntry);

            return result.Cast<IJournalEntry>();
        }

        public IEnumerable<IRelationObject> GetRelations(IBaseObject baseObject, string relation)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue GetValue(string extId, IBaseObjectType type)
        {
            IBaseObject baseObject = GetBaseObject(extId, type);
            if (baseObject == null)
            {
                return null;
            }
            IBaseObjectValue objectValue = baseObject.Values.Last();
            return objectValue;
        }

        public IBaseObjectValue GetValue(string extId, IBaseObjectType type, TimePoint timePoint)
        {
            IBaseObject baseObject = GetBaseObject(extId, type);
            if (baseObject == null)
            {
                return null;
            }
            return GetValue(baseObject, timePoint);
        }

        public IBaseObjectValue GetValue(string extId, IBaseObjectType type, int version)
        {
            IBaseObject baseObject = GetBaseObject(extId, type);
            if (baseObject == null)
            {
                return null;
            }
            return GetValue(baseObject, version);
        }

        public IBaseObjectValue GetValue(Guid baseObjectId)
        {
            IBaseObject baseObject = GetBaseObject(baseObjectId);
            if (baseObject == null)
            {
                return null;
            }
            IBaseObjectValue objectValue = baseObject.Values.Last();
            return objectValue;
        }

        public IBaseObjectValue GetValue(Guid baseObjectId, TimePoint timePoint)
        {
            IBaseObject baseObject = GetBaseObject(baseObjectId);
            if (baseObject == null)
            {
                return null;
            }
            return GetValue(baseObject, timePoint);
        }

        public IBaseObjectValue GetValue(Guid baseObjectId, int version)
        {
            IBaseObject baseObject = GetBaseObject(baseObjectId);
            if (baseObject == null)
            {
                return null;
            }
            return GetValue(baseObject, version);
        }

        public IBaseObjectValue GetValue(IBaseObject baseObject, TimePoint timePoint)
        {
            var result = (from baseObjectValue in baseObject.Values
                          where baseObjectValue.Range.Includes(timePoint)
                          select baseObjectValue);
            return result.First();
        }

        public IBaseObjectValue GetValue(IBaseObject baseObject, int version)
        {
            var result = (from baseObjectValue in baseObject.Values
                          where baseObjectValue.Version.Equals(version)
                          select baseObjectValue);
            return result.First();
        }

        public IEnumerable<IBaseObjectValue> GetValues(string extIdFilter, IBaseObjectType type)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBaseObjectValue> GetValues(IBaseObjectValue referenceObjectValue, string extId, IBaseObjectType type)
        {
            IEnumerable<IBaseObject> objectsWithExtId = (from baseObject in _baseObjectsById.Values
                                                         where baseObject.ExtId.Equals(extId)
                                                         select baseObject as IBaseObject);
            List<IBaseObjectValue> result = new List<IBaseObjectValue>();
            foreach (IBaseObject baseObject in objectsWithExtId)
            {
                result.AddRange(from baseObjectValue in baseObject.Values
                                where baseObjectValue.Reference.Id == referenceObjectValue.Id
                                && baseObjectValue.End.Equals(TimePoint.Future)
                                select baseObjectValue);
            }
            return result;
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject)
        {
            return InsertValue(content, timePoint, baseObject, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(content, timePoint, baseObject, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType)
        {
            return InsertValue(Encoding.UTF8.GetBytes(content), "application/xml; charset=utf-8", timePoint, id, extId, type, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(Encoding.UTF8.GetBytes(content), "application/xml; charset=utf-8", timePoint, id, extId, type, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type)
        {
            return InsertValue(content, timePoint, id, extId, type, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(content, timePoint, id, extId, type, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, string extReferenceId, IBaseObjectType type)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, string extReferenceId, IBaseObjectType type, WebHttpHeaderInfo journalInfo)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(Encoding.UTF8.GetBytes(content), "application/xml; charset=utf-8", timePoint, baseObject, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journaInfo)
        {
            return InsertValue(Encoding.UTF8.GetBytes(content), "application/xml; charset=utf-8", timePoint, baseObject, referenceObjectValue, journaInfo);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(Encoding.UTF8.GetBytes(content), "application/xml; charset=utf-8", timePoint, id, extId, type, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(Encoding.UTF8.GetBytes(content), "application/xml; charset=utf-8", timePoint, id, extId, type, referenceObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject)
        {
            return InsertValue(content, contentType, timePoint, baseObject, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(content, contentType, timePoint, baseObject, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, null as IBaseObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, WebHttpHeaderInfo journalInfo)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, null as IBaseObjectValue, journalInfo);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, WebHttpHeaderInfo journalInfo)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(content, contentType, timePoint, baseObject, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            if (referenceObjectValue != null)
            {
                _logger.DebugFormat("InsertValue({1}, {2}, {3}, {4}), content=\r\n{0}", content.Length, contentType, timePoint, baseObject.Id, referenceObjectValue.Id);
            }
            else
            {
                _logger.DebugFormat("InsertValue({1}, {2}, {3}, null), content=\r\n{0}", content.Length, contentType, timePoint, baseObject.Id);
            }
            
            baseObject.Values.Last().End = timePoint.AddSeconds(-1);

            BaseObjectValue newValue = new BaseObjectValue();

            newValue.Parent = baseObject;
            if (referenceObjectValue != null)
            {
                newValue.Reference = referenceObjectValue;
                newValue.ReferenceId = referenceObjectValue.Id;
            }
            newValue.Start = timePoint;
            newValue.Content = content;
            newValue.ContentType = contentType;
            newValue.Version = baseObject.Values.Count();

            return newValue;
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
        {
            return InsertValue(content, contentType, timePoint, id, extId, type, referenceObjectValue, null);
        }

        public IBaseObjectValue InsertValue(byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo journalInfo)
        {
            if (referenceObjectValue != null)
            {
                _logger.DebugFormat("InsertValue({1}, {2}, {3}, {4}), content=\r\n{0}", content.Length, contentType, timePoint, extId, referenceObjectValue.Id);
            }
            else
            {
                _logger.DebugFormat("InsertValue({1}, {2}, {3}, null), content=\r\n{0}", content.Length, contentType, timePoint, extId);
            }

            BaseObject newObject = new BaseObject();
            newObject.Id = id;
            newObject.ExtId = extId;
            newObject.Type = GetBaseObjectType(type.Id);
            _baseObjectsById.Add(newObject.Id, newObject as BaseObject);
            _baseObjectsByKey.Add(newObject.ExtId, newObject as BaseObject);

            BaseObjectValue newValue = new BaseObjectValue();
            newValue.ParentBaseObject = newObject;
            if (referenceObjectValue != null)
            {
                newValue.Reference = referenceObjectValue;
                newObject.Reference = referenceObjectValue.Parent;
                newObject.ReferenceId = referenceObjectValue.Parent.Id;
                newValue.ReferenceId = referenceObjectValue.Id;
            }

            newValue.Start = timePoint;
            newValue.Content = content;
            newValue.ContentType = contentType;
            newValue.Version = 1;

            return newValue;
        }

        public void SaveChanges()
        {
            foreach (BaseObject baseObject in _baseObjectsById.Values)
            {
                foreach (BaseObjectValue baseObjectValue in baseObject.Values)
                {
                    if (baseObjectValue.Id == null)
                    {
                        baseObjectValue.Id = Guid.NewGuid();
                    }
                }
            }
        }

        #endregion
    }
}
