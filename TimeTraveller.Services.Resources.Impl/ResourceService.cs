using System;
using System.Collections.Generic;
using System.IO;
using TimeTraveller.Services.Impl;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.General.Unity;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Resources.Impl
{
    class ResourceDescriptor
    {
        public string Name { get; set;  }
        public string Extension { get; set; }
        public string Folder { get; set; }
        public string ContentType { get; set; }
        public ResourceDescriptor(string name, string extension, string folder, string contentType)
        {
            this.Name = name;
            this.Extension = extension;
            this.Folder = folder;
            this.ContentType = contentType;
        }
    }

    public class ResourceService : AbstractTimeLineService<Resource>, IResourceService
    {
        #region Private Properties
        private IDictionary<string, Resource> _builtInResources;
        #endregion

        #region Constructors
        public ResourceService(ILogger logger, IUnity container, IDataService dataService)
            : base(ItemType.Resource, logger, container, dataService)
        {
            PopulateBuiltInResources();
        }
        #endregion

        #region Abstract Methods
        public override string XmlSchemaName
        {
            get
            {
                return null;
            }
        }

        public override string XmlSchemaResourceName
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region IResourceService Members
        public override Resource Convert(IBaseObjectValue objectValue, Uri baseUri)
        {
            Resource result = new Resource()
            {
                Content = objectValue.Content,
                ContentType = objectValue.ContentType,
                BaseObjectValue = objectValue
            };

            result.GetUri(baseUri, UriType.Version);

            return result;
        }

        public override Resource Get(string id, Uri baseUri)
        {
            Resource result = GetBuiltInResource(id);
            if (result == null)
            {
                result = base.Get(id, baseUri);
            }
            return result;
        }

        public override Resource Get(string id, TimePoint timePoint, Uri baseUri)
        {
            Resource result = GetBuiltInResource(id);
            if (result == null)
            {
                result = base.Get(id, timePoint, baseUri);
            }
            return result;
        }

        public override Resource Get(string id, int versionNumber, Uri baseUri)
        {
            Resource result = GetBuiltInResource(id);
            if (result == null)
            {
                result = base.Get(id, versionNumber, baseUri);
            }
            return result;
        }

        public override IEnumerable<Resource> GetEnumerable(Uri baseUri)
        {
            List<Resource> result = new List<Resource>(base.GetEnumerable(string.Empty, baseUri));
            result.AddRange(_builtInResources.Values);
            return result;
        }

        public override bool Store(string id, Resource item, IBaseObjectValue referenceObjectValue, Uri baseUri, IUserInfo info)
        {
            try
            {
                Logger.DebugFormat("Store<{0}>({1})", typeof(Resource).Name, id);

                if (_builtInResources.ContainsKey(id))
                {
                    throw new ArgumentException(string.Format("Cannot store built in resource {0}", id));
                }

                IBaseObject baseObject = DataService.GetBaseObject(id, BaseObjectType);
                IBaseObjectValue objectValue = null;
                bool result = (baseObject == null);

                if (baseObject != null)
                {
                    if (referenceObjectValue != null)
                    {
                        objectValue = DataService.InsertValue(item.Content, item.ContentType, TimePoint.Now, baseObject, referenceObjectValue, info);
                    }
                    else
                    {
                        objectValue = DataService.InsertValue(item.Content, item.ContentType, TimePoint.Now, baseObject, info);
                    }
                }
                else
                {
                    if (referenceObjectValue != null)
                    {
                        objectValue = DataService.InsertValue(item.Content, item.ContentType, TimePoint.Now, Guid.NewGuid(), id, BaseObjectType, referenceObjectValue, info);
                    }
                    else
                    {
                        objectValue = DataService.InsertValue(item.Content, item.ContentType, TimePoint.Now, Guid.NewGuid(), id, BaseObjectType, info);
                    }
                }

                DataService.SaveChanges();

                item.BaseObjectValue = objectValue;
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

        #region Private Methods
        private Resource GetBuiltInResource(string id)
        {
            Resource result = null;
            if (_builtInResources.ContainsKey(id))
            {
                Logger.DebugFormat("Internal resource {0} found", id);

                result = _builtInResources[id];
                if (result.Content == null)
                {
                    ReadBuiltInResources();
                }
            }
            return result;
        }

        private void PopulateBuiltInResources()
        {
            _builtInResources = new Dictionary<string, Resource>();
            IBaseObjectType resourceType = new InternalBaseObjectType(BaseObjectType);

            // Populate the list of resources and their extension and target folder
            ResourceDescriptor[] resourceList = new ResourceDescriptor[] { 
                    new ResourceDescriptor( "timetraveller", "png", "Images", "image/png" ), 
                    new ResourceDescriptor( "ItsBrowser", "css", "Styles", "text/plain; charset=utf-8" ), 
                    new ResourceDescriptor( "ItsBrowserHistory", "xslt", "Scripts", "text/plain; charset=utf-8" ), 
                    new ResourceDescriptor( "ItsBrowserHead", "xslt", "Scripts", "text/plain; charset=utf-8" ),
                    new ResourceDescriptor( "ItsBrowserJournal", "xslt", "Scripts", "text/plain; charset=utf-8" ),
                    new ResourceDescriptor( "ItsBrowserPageHeader", "xslt", "Scripts", "text/plain; charset=utf-8" ),
                    new ResourceDescriptor( "ItsBrowserLink", "xslt", "Scripts", "text/plain; charset=utf-8" ),
                    new ResourceDescriptor( "ItsBrowserList", "xslt", "Scripts", "text/plain; charset=utf-8" ),
                    new ResourceDescriptor( "ItsBrowserSummary", "xslt", "Scripts", "text/plain; charset=utf-8" )
                };

            foreach (ResourceDescriptor resource in resourceList)
            {
                Resource builtInResource = new Resource()
                {
                    ContentType = resource.ContentType,
                    BaseObjectValue = new InternalBaseObjectValue()
                    {
                        Parent = new InternalBaseObject()
                        {
                            ExtId = String.Format("{0}/{1}.{2}", resource.Folder, resource.Name, resource.Extension).ToLower(),
                            InternalResourceId = String.Format("{0}.{1}.{2}", resource.Folder, resource.Name, resource.Extension),
                            Type = resourceType
                        }
                    }
                };
                _builtInResources.Add(builtInResource.ExtId, builtInResource);
            }

        }

        private void ReadBuiltInResources()
        {
            foreach (Resource resource in _builtInResources.Values)
            {
                InternalBaseObject baseObject = resource.BaseObjectValue.Parent as InternalBaseObject;
                resource.Content = ReadResourceContent(baseObject.InternalResourceId);
            }
        }

        private byte[] ReadResourceContent(string resourceId)
        {
            Logger.DebugFormat("Reading resource {0}", resourceId);

            MemoryStream result = new MemoryStream();
            Stream manifestResourceStream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), resourceId);
            int byteValue = manifestResourceStream.ReadByte();
            while (byteValue != -1)
            {
                result.WriteByte((byte)byteValue);
                byteValue = manifestResourceStream.ReadByte();
            }
            result.Close();

            return result.ToArray();
        }
        #endregion
    }
}
