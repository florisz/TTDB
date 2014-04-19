using System;
using System.Net;

using System.Xml.Serialization;

namespace Luminis.Its.Client.Model
{
    public partial class ObjectModel
    {
        #region Public Methods
        /// 
        /// <summary>
        /// return the object definition specified by its id
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

            if (objectDefinition == null)
            {
                throw new ArgumentException(string.Format("Object definition with type {0} can not be found in this object model", id));
            }

            return objectDefinition;
        }

        /// 
        /// <summary>
        /// return the object definition specified by its type name
        /// </summary>
        /// <param name="type">type name of the object definition to return</param>
        /// <returns>the specified object definition</returns>
        /// 
        public ObjectDefinition GetObjectDefinition(string type)
        {
            ObjectDefinition objectDefinition = null;

            foreach (ObjectDefinition obj in this.ObjectDefinitions)
            {
                if (obj.Name == type)
                {
                    objectDefinition = obj;
                    break;
                }
            }

            if (objectDefinition == null)
            {
                throw new ArgumentException(string.Format("Object definition with type {0} can not be found in this object model", type));
            }

            return objectDefinition;
        }

        /// 
        /// <summary>
        /// return the object relation specified by its source and target names
        /// </summary>
        /// <param name="type">type name of the object relation to return</param>
        /// <returns>the specified object relation</returns>
        /// 
        public ObjectRelation GetObjectRelation(string source, string target)
        {
            ObjectRelation objectRelation = null;

            foreach (ObjectRelation obj in this.ObjectRelations)
            {
                if (obj.Source == source && obj.Target == target)
                {
                    objectRelation = obj;
                    break;
                }
            }

            if (objectRelation == null)
            {
                throw new ArgumentException(string.Format("Object definition with source {0} and target {1} can not be found in this object model", source, target));
            }

            return objectRelation;
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
