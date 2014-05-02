using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using TimeTraveller.Services.Data;
using TimeTraveller.Services.Items;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.General.Xml;
using TimeTraveller.Services.Interfaces;
using TimeTraveller.Services.Data.Interfaces;

namespace TimeTraveller.Services.Impl
{
    public abstract class AbstractTimeLineService<T> : ITimeLineService<T> 
        where T : IItem 
    {
        #region Public Properties
        public const string _historyXslt = "scripts/itsbrowserhistory.xslt";
        public const string _listXslt = "scripts/itsbrowserlist.xslt";
        public const string _summaryXslt = "scripts/itsbrowsersummary.xslt";
        #endregion

        #region Constructors
        public AbstractTimeLineService(ItemType itemType, ILogger logger, IUnity container, IDataService dataService)
        {
            ItemType = itemType;
            Logger = logger;
            Container = container;
            DataService = dataService;
            BaseObjectType = DataService.GetBaseObjectType((int)ItemType);
        }
        #endregion

        #region Abstract Methods
        public abstract string XmlSchemaName { get; }
        public abstract string XmlSchemaResourceName { get; }

        public abstract T Convert(IBaseObjectValue objectValue, Uri baseUri);

        public virtual void WriteDetailedSummaryInfo(T item, Uri baseUri, XmlWriter xmlWriter)
        {
            // Empty implementation
        }
        #endregion

        #region Public Properties
        public IBaseObjectType BaseObjectType { get; private set; }
        public ItemType ItemType { get; private set; }
        public ILogger Logger { get; private set; }
        public IUnity Container { get; private set; }
        public IDataService DataService { get; private set; }
        #endregion

        #region ITimeLineService Members
        public virtual T Convert(string xml, Encoding encoding)
        {
            try
            {
                Stream schemaStream = GetType().Assembly.GetManifestResourceStream(XmlSchemaResourceName);

                XmlHelper.Validate(xml, encoding, schemaStream);

                T result = XmlHelper.FromXml<T>(xml, encoding);
                
                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public virtual T Get(string id, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("Get<{0}>({1}, {2})", typeof(T).Name, id, baseUri);

                T result = Get(id, TimePoint.Now, baseUri);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public virtual T Get(string id, TimePoint timePoint, Uri baseUri)
        {
            try
            {

                Logger.DebugFormat("Get<{0}>({1}, {2}, {3})", typeof(T).Name, id, timePoint.TimeValue.ToString("O"), baseUri);

                IBaseObjectValue objectValue = DataService.GetValue(id, BaseObjectType, timePoint);
                if (objectValue == null)
                {
                    throw new ArgumentOutOfRangeException(typeof(T).Name, string.Format("Unknown {0} {1}", typeof(T).Name, id));
                }

                T result = Convert(objectValue, baseUri);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }

        }

        public virtual T Get(string id, int versionNumber, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("Get<{0}>({1}, {2})", typeof(T).Name, id, versionNumber);

                IBaseObjectValue objectValue = DataService.GetValue(id, BaseObjectType, versionNumber);
                if (objectValue == null)
                {
                    throw new ArgumentOutOfRangeException(typeof(T).Name, string.Format("Unknown {0} {1}/{2}", typeof(T).Name, id, versionNumber));
                }

                T result = Convert(objectValue, baseUri);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public virtual IEnumerable<T> GetEnumerable(Uri baseUri)
        {
            return GetEnumerable(string.Empty, baseUri);
        }

        public virtual IEnumerable<T> GetEnumerable(string parentId, Uri baseUri)
        {
            try
            {
                Logger.DebugFormat("GetEnumerable<{0}>({1})", typeof(T).Name, parentId);

                List<T> result = new List<T>();
                IEnumerable<IBaseObject> baseObjects = DataService.GetBaseObjects(parentId, BaseObjectType);
                foreach (IBaseObject baseObject in baseObjects)
                {
                    T convertedObjectValue = Convert(baseObject.GetValue(), baseUri);
                    result.Add(convertedObjectValue);
                }
                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public virtual string GetHistory(T item, Uri baseUri, Encoding encoding)
        {
            return GetHistory(item, new TimePointRange(TimePoint.Past, TimePoint.Future), baseUri, encoding);
        }

        public virtual string GetHistory(T item, TimePointRange timePointRange, Uri baseUri, Encoding encoding)
        {
            try
            {
                Logger.DebugFormat("GetHistory<{0}>({1}, {2})", typeof(T).Name, item.ExtId, encoding.HeaderName);

                AbstractItem itemImpl = item as AbstractItem;

                StringBuilder resultXml = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = encoding;
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);
                xmlWriter.WriteStartElement("History");
                xmlWriter.WriteElementString("Name", itemImpl.ExtId);

                IEnumerable<IJournalEntry> journalEntries = DataService.GetJournal(itemImpl.BaseObjectValue.Parent, timePointRange);

                foreach (IJournalEntry journalEntry in journalEntries)
                {
                    xmlWriter.WriteStartElement("Journal");
                    xmlWriter.WriteStartElement("Link");
                    xmlWriter.WriteAttributeString("rel", BaseObjectType.Name);
                    IBaseObjectValue valueAfter = journalEntry.After as IBaseObjectValue;
                    xmlWriter.WriteAttributeString("href", itemImpl.GetUri(baseUri, UriType.Version, valueAfter.Version));
                    xmlWriter.WriteEndElement(); // Link
                    xmlWriter.WriteElementString("Timestamp", journalEntry.When.ToString("MM-dd-yyyy HH:mm ss"));
                    xmlWriter.WriteElementString("Username", journalEntry.Username);
                    xmlWriter.WriteEndElement(); // Journal
                }

                xmlWriter.WriteEndElement(); // History
                xmlWriter.Close();

                Logger.DebugFormat("GetHistory()=\r\n{0}", resultXml);

                string result = resultXml.ToString();

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public virtual string GetList(IEnumerable<T> items, Uri baseUri, Encoding encoding)
        {
            try
            {
                Logger.DebugFormat("GetList<{0}>({1}, {2})", typeof(T).Name, baseUri, encoding.HeaderName);

                StringBuilder resultXml = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = encoding;
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);
                xmlWriter.WriteStartElement("List");
                foreach (IItem item in items)
                {
                    xmlWriter.WriteStartElement("Link");
                    xmlWriter.WriteAttributeString("rel", BaseObjectType.Name);
                    xmlWriter.WriteAttributeString("href", item.GetUri(baseUri, UriType.Latest, AbstractItem.SummaryQueryArgument));
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement(); // List
                xmlWriter.Close();

                Logger.DebugFormat("GetList()=\r\n{0}", resultXml);

                string result = resultXml.ToString();

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }

        }

        public virtual string GetSummary(T item, Uri baseUri, Encoding encoding)
        {
            try
            {
                Logger.DebugFormat("GetSummary<{0}>({1}, {2})", typeof(T).Name, item.ExtId, encoding.HeaderName);

                AbstractItem itemImpl = item as AbstractItem;

                StringBuilder resultXml = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = encoding;
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                XmlWriter xmlWriter = XmlWriter.Create(resultXml, settings);
                xmlWriter.WriteStartElement("Summary");
                xmlWriter.WriteElementString("Name", item.ExtId);

                xmlWriter.WriteStartElement("Link");
                xmlWriter.WriteAttributeString("rel", "latest");
                xmlWriter.WriteAttributeString("href", item.GetUri(baseUri, UriType.Latest));
                xmlWriter.WriteEndElement(); // Link

                xmlWriter.WriteStartElement("Link");
                xmlWriter.WriteAttributeString("rel", "current");
                xmlWriter.WriteAttributeString("href", item.GetUri(baseUri, UriType.Version));
                xmlWriter.WriteEndElement(); // Link

                if (itemImpl.BaseObjectValue != null && itemImpl.BaseObjectValue.Parent != null && itemImpl.BaseObjectValue.Parent.Values != null)
                {
                    foreach (IBaseObjectValue value in itemImpl.BaseObjectValue.Parent.Values)
                    {
                        xmlWriter.WriteStartElement("Link");
                        xmlWriter.WriteAttributeString("rel", "version");
                        xmlWriter.WriteAttributeString("href", item.GetUri(baseUri, UriType.Version, value.Version));
                        xmlWriter.WriteEndElement(); // Link
                    }
                }

                xmlWriter.WriteStartElement("Link");
                xmlWriter.WriteAttributeString("rel", "summary");
                xmlWriter.WriteAttributeString("href", item.GetUri(baseUri, UriType.Latest, AbstractItem.SummaryQueryArgument));
                xmlWriter.WriteEndElement(); // Link

                xmlWriter.WriteStartElement("Link");
                xmlWriter.WriteAttributeString("rel", "history");
                xmlWriter.WriteAttributeString("href", item.GetUri(baseUri, UriType.Latest, AbstractItem.HistoryQueryArgument));
                xmlWriter.WriteEndElement(); // Link

                WriteDetailedSummaryInfo(item, baseUri, xmlWriter);

                xmlWriter.WriteEndElement(); // Summary
                xmlWriter.Close();

                Logger.DebugFormat("Summary=\r\n{0}", resultXml);

                string result = resultXml.ToString();

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }

        }

        public virtual string GetXml(T item, Encoding encoding)
        {
            try
            {
                string result = XmlHelper.ToXml<T>(item, encoding, true);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }

        public virtual string GetXmlSchemaAddress(Uri baseUri)
        {
            string result = string.Format("{0}{1}{2}", baseUri, "/schemas/", GetXmlSchemaName());

            return result;
        }

        public virtual string GetXmlSchemaName()
        {
            return XmlSchemaName;
        }

        public virtual string GetXmlSchemaText()
        {
            MemoryStream schemaBuffer = new MemoryStream();
            Stream xmlSchemaStream = GetType().Assembly.GetManifestResourceStream(XmlSchemaResourceName);
            int byteValue = xmlSchemaStream.ReadByte();
            while (byteValue != -1)
            {
                schemaBuffer.WriteByte((byte)byteValue);
                byteValue = xmlSchemaStream.ReadByte();
            }
            schemaBuffer.Close();

            string result = Encoding.UTF8.GetString(schemaBuffer.ToArray());
            return result;
        }

        public virtual bool Store(string id, T item, Uri baseUri, IHeaderInfo info)
        {
            return Store(id, item, null, baseUri, info);
        }

        public virtual bool Store(string id, T item, IBaseObjectValue referenceObjectValue, Uri baseUri, IHeaderInfo info)
        {
            try
            {
                Logger.DebugFormat("Store<{0}>({1})", typeof(T).Name, id);

                IBaseObject baseObject = DataService.GetBaseObject(id, BaseObjectType);
                IBaseObjectValue objectValue = null;
                string xml = GetXml(item, Encoding.UTF8);

                bool result = (baseObject == null);
                if (baseObject != null)
                {
                    if (referenceObjectValue != null)
                    {
                        objectValue = DataService.InsertValue(xml, TimePoint.Now, baseObject, referenceObjectValue, info);
                    }
                    else
                    {
                        objectValue = DataService.InsertValue(xml, TimePoint.Now, baseObject, info);
                    }
                }
                else
                {
                    if (referenceObjectValue != null)
                    {
                        objectValue = DataService.InsertValue(xml, TimePoint.Now, Guid.NewGuid(), id, BaseObjectType, referenceObjectValue, info);
                    }
                    else
                    {
                        objectValue = DataService.InsertValue(xml, TimePoint.Now, Guid.NewGuid(), id, BaseObjectType, info);
                    }
                }

                DataService.SaveChanges();

                AbstractItem itemImpl = item as AbstractItem;
                itemImpl.BaseObjectValue = objectValue;
                item.GetUri(baseUri, UriType.Version);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Debug("Unexpected exception", exception);
                throw;
            }
        }
        #endregion
    }
}
