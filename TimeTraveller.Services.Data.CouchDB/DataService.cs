using System;
using TimeTraveller.General.Logging;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;
using TimeTraveller.General.Patterns.Range;

namespace TimeTraveller.Services.Data.CouchDB
{
	public class DataService : IDataService
    {
        #region Constructors

	    public DataService(ILogger logger)
	    {
	        
	    }
        #endregion
        #region IDataService implementation

        public void Clean ()
		{
			throw new NotImplementedException ();
		}

        public void InsertBaseObject(IBaseObject obj)
        {
            throw new NotImplementedException();
        }

        public IBaseObject CreateBaseObject(IBaseObjectType type)
        {
            throw new NotImplementedException();
        }

        public IBaseObjectJournal CreateBaseObjectJournal()
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue CreateBaseObjectValue ()
		{
			throw new NotImplementedException ();
		}

		public void DeleteBaseObject (IBaseObject deleteObject)
		{
			throw new NotImplementedException ();
		}

		public IBaseObject GetBaseObject (Guid id)
		{
			throw new NotImplementedException ();
		}

		public IBaseObject GetBaseObject (string key, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public IBaseObject GetBaseObject (string key, IBaseObjectType type, IBaseObject parent)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<IBaseObject> GetBaseObjects (IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<IBaseObject> GetBaseObjects (string extIdFilter, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectType GetBaseObjectType (int id)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectType GetBaseObjectType (string name)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetBaseObjectValue (Guid id)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<IJournalEntry> GetJournal (IBaseObject baseObject, TimePointRange range)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<IRelationObject> GetRelations (IBaseObject baseObject, string relation)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetValue (Guid baseObjectId)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetValue (Guid baseObjectId, TimePoint timePoint)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetValue (Guid baseObjectId, int version)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetValue (string extId, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetValue (string extId, IBaseObjectType type, TimePoint timePoint)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue GetValue (string extId, IBaseObjectType type, int version)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<IBaseObjectValue> GetValues (string extIdFilter, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<IBaseObjectValue> GetValues (IBaseObjectValue referenceObjectValue, string extId, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, IBaseObject baseObject)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, IBaseObject baseObject, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, IBaseObject baseObject, IBaseObjectValue referenceObjectValue, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, IBaseObjectValue referenceObjectValue, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (string content, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType)
		{
			throw new NotImplementedException ();
		}

		public IBaseObjectValue InsertValue (byte[] content, string contentType, TimePoint timePoint, Guid id, string extId, IBaseObjectType type, string extReferenceId, IBaseObjectType referenceType, IUserInfo journalInfo)
		{
			throw new NotImplementedException ();
		}

		public void SaveChanges ()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

