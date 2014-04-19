using System.Text;
using System.Linq;

namespace Luminis.Its.Client.Model
{
    public class ObjectModelService
    {
        #region Public Methods
        /// <summary>
        /// Converts a objectmodel xml to a objectmodel object
        /// </summary>
        /// <param name="objectModelXml"></param>
        /// <param name="encoding"></param>
        /// <returns>reference to the newly instantiated ObjectModel object</returns>
        public static ObjectModel Convert(string objectModelXml)
        {
            // fill all standard (meta) attributes through XML mapping
            ObjectModel objectModel = XmlHelper.FromXml<ObjectModel>(objectModelXml, Encoding.UTF8);

            ConnectRelationSourceAndTarget(objectModel);

            ConnectEntityRelations(objectModel);

            return objectModel;
        }

        public static string GetXML(ObjectModel model)
        {
            return XmlHelper.ToXml<ObjectModel>(model, Encoding.UTF8);
        }

        public static string GetXML(ObjectModel model, Encoding enc)
        {
            return XmlHelper.ToXml<ObjectModel>(model, enc);
        }

        public static string GetXML(ObjectModel model, Encoding enc, bool omitEncodingPreamble)
        {
            return XmlHelper.ToXml<ObjectModel>(model, enc, omitEncodingPreamble);
        }

        #endregion

        #region Private Methods
        private static void ConnectEntityRelations(ObjectModel objectModel, ObjectDefinition sourceObjectDefinition)
        {
            // Select all object definitions where the ObjectType=entity or relation
            var objectDefinitionNames = (from definition in objectModel.ObjectDefinitions
                                         select definition.Name);

            // Select all relations where the sourceObjectDefinition is the "Source" and the "Target" is an entity of relation
            var relations = (from relation in objectModel.ObjectRelations
                             where relation.Source.Equals(sourceObjectDefinition.Name)
                             && objectDefinitionNames.Contains(relation.Target)
                             select relation);

            sourceObjectDefinition.EntityRelations = relations.ToArray();
        }

        private static void ConnectEntityRelations(ObjectModel objectModel)
        {
            if (objectModel.ObjectRelations != null)
            {
                foreach (ObjectDefinition objectDefinition in objectModel.ObjectDefinitions)
                {
                    ConnectEntityRelations(objectModel, objectDefinition);
                }
            }
        }

        private static void ConnectRelationSourceAndTarget(ObjectModel objectModel, ObjectRelation objectRelation)
        {
            objectRelation.SourceObjectDefinition = (from definition in objectModel.ObjectDefinitions
                                                     where definition.Name.Equals(objectRelation.Source)
                                                     select definition).First();

            objectRelation.TargetObjectDefinition = (from definition in objectModel.ObjectDefinitions
                                                     where definition.Name.Equals(objectRelation.Target)
                                                     select definition).First();
        }

        private static void ConnectRelationSourceAndTarget(ObjectModel objectModel)
        {
            if (objectModel.ObjectRelations != null)
            {
                foreach (ObjectRelation objectRelation in objectModel.ObjectRelations)
                {
                    ConnectRelationSourceAndTarget(objectModel, objectRelation);
                }
            }
        }
        #endregion
    }
}
