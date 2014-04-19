using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Luminis.Its.Services;
using Luminis.Its.Services.Items;

namespace Luminis.Its.Services.ObjectModels
{
    public partial class ObjectModel : AbstractItem, IItem
    {
        #region IItem/AbstractItem Members
        [XmlIgnore()]
        public override string SelfUri
        {
            get
            {
                string result = string.Empty;
                if (this.Link != null)
                {
                    result = this.Link.href;
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    if (this.Link == null)
                    {
                        this.Link = new ObjectModelLink()
                        {
                            rel = ObjectModelLinkRel.self
                        };
                    }
                    this.Link.href = value;
                }
                else
                {
                    this.Link.href = null;
                }
            }
        }
        #endregion

        #region Public Methods
        /// 
        /// <summary>
        /// return the object specified by its id
        /// </summary>
        /// <param name="id">id of the object definition to return</param>
        /// <returns>the specified object definition</returns>
        /// 
        public ObjectDefinition GetObjectDefinition(int id)
        {
            ObjectDefinition objectDefinition = null;

            foreach (ObjectDefinition obj in this.ObjectDefinitions)
            {
                if (obj.Id == id)
                {
                    objectDefinition = obj;
                    break;
                }
            }

            return objectDefinition;
        }

        #endregion
    }

    public partial class ObjectDefinition
    {
        #region Public Properties
        [XmlIgnore]
        public ObjectRelation[] EntityRelations { get; set; }
        [XmlIgnore]
        public int Id { get; set; }
        #endregion
    }

    public partial class ObjectRelation
    {
        #region Public Properties
        [XmlIgnore]
        public ObjectDefinition SourceObjectDefinition { get; set; }

        [XmlIgnore]
        public ObjectDefinition TargetObjectDefinition { get; set; }
        #endregion
    }
}
