using System;
using System.Collections.Generic;
using System.IO;

using Luminis.Its.Services;
using Luminis.Its.Services.Impl;
using Luminis.Its.Services.Data;
using Luminis.Its.Services.Resources;
using Luminis.Logging;
using Luminis.Patterns.Range;
using Luminis.Unity;

namespace Luminis.Its.Services.Resources.Impl
{
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
        public override Resource Convert(IBaseObjectValue objectValue)
        {
            Resource result = new Resource()
            {
                Content = objectValue.Content,
                ContentType = objectValue.ContentType,
                BaseObjectValue = objectValue
            };
            return result;
        }

        public override Resource Get(string id)
        {
            Resource result = GetBuiltInResource(id);
            if (result == null)
            {
                result = base.Get(id);
            }
            return result;
        }

        public override Resource Get(string id, TimePoint timePoint)
        {
            Resource result = GetBuiltInResource(id);
            if (result == null)
            {
                result = base.Get(id, timePoint);
            }
            return result;
        }

        public override Resource Get(string id, int versionNumber)
        {
            Resource result = GetBuiltInResource(id);
            if (result == null)
            {
                result = base.Get(id, versionNumber);
            }
            return result;
        }

        public override IEnumerable<Resource> GetEnumerable()
        {
            List<Resource> result = new List<Resource>(base.GetEnumerable(string.Empty));
            result.AddRange(_builtInResources.Values);
            return result;
        }

        public override void Store(string id, Resource item, IBaseObjectValue referenceObjectValue, WebHttpHeaderInfo info)
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
                if (baseObject != null)
                {
                    if (referenceObjectValue != null)
                    {
                        objectValue = DataService.InsertValue(null, TimePoint.Now, baseObject, referenceObjectValue, info);
                    }
                    else
                    {
                        objectValue = DataService.InsertValue(null, TimePoint.Now, baseObject, info);
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
                    InternalBaseObject baseObject = result.BaseObjectValue.Parent as InternalBaseObject;
                    result.Content = ReadResourceContent(baseObject.InternalResourceId);
                }
            }
            return result;
        }

        private void PopulateBuiltInResources()
        {
            _builtInResources = new Dictionary<string, Resource>();

            IBaseObjectType resourceType = new InternalBaseObjectType(BaseObjectType);

            Resource builtInResource = new Resource()
            {
                ContentType = "image/png",
                BaseObjectValue = new InternalBaseObjectValue()
                {
                    Parent = new InternalBaseObject()
                    {
                        ExtId = "images/luminis.png",
                        InternalResourceId = "Images.luminis.png",
                        Type = resourceType
                    }
                }
            };
            _builtInResources.Add(builtInResource.ExtId, builtInResource);

            builtInResource = new Resource()
            {
                ContentType = "text/plain; charset=utf-8",
                BaseObjectValue = new InternalBaseObjectValue()
                {
                    Parent = new InternalBaseObject()
                    {
                        ExtId = "styles/luminis.css",
                        InternalResourceId = "Styles.luminis.css",
                        Type = resourceType
                    }
                }
            };
            _builtInResources.Add(builtInResource.ExtId, builtInResource);

            builtInResource = new Resource()
            {
                ContentType = "text/plain; charset=utf-8",
                BaseObjectValue = new InternalBaseObjectValue()
                {
                    Parent = new InternalBaseObject()
                    {
                        ExtId = "scripts/summary.xslt",
                        InternalResourceId = "Scripts.Summary.xslt",
                        Type = resourceType
                    }
                }
            };
            _builtInResources.Add(builtInResource.ExtId, builtInResource);
        }

        private byte[] ReadResourceContent(string resourceId)
        {
            Logger.DebugFormat("Reading resource {0}", resourceId);

            MemoryStream result = new MemoryStream();
            Stream manifestResourceStream = this.GetType().Assembly.GetManifestResourceStream(typeof(IResourceService), resourceId);
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
