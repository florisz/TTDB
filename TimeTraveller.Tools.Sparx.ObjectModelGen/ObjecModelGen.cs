using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using Luminis.Its.Services.CaseFileSpecifications;
using Luminis.Its.Services.ObjectModels;
using Luminis.Xml;

namespace Luminis.Its.Tools.Sparx.ObjectModelGen
{
    public class ObjectModelGen
    {
        private EA.Repository _repository = new EA.Repository();
        private int _progressTotal;
        private int _progressCount;

        public EventHandler<ObjectModelGenEventArgs> Progress;

        protected void OnProgress(ObjectModelGenEventArgs args)
        {
            if (Progress != null)
            {
                Progress(this, args);
            }
        }

        /// <summary>
        /// Main entry point of the generator
        /// </summary>
        /// <param name="repositoryName">full path of the Enterprise Architect Model file</param>
        /// <param name="outputDirectory">full path to the directory to put the result files in</param>
        /// <returns>true if everything goes well, an exception is thrown otherwise</returns>
        public bool Generate(string repositoryName, string outputDirectory)
        {
            try
            {
                OnProgress(new ObjectModelGenEventArgs("Opening model file", 1, 100));

                EA.Package package = InitializeGeneration(repositoryName);

                if (package != null)
                {
                    _progressCount = 1;
                    OnProgress(new ObjectModelGenEventArgs("Building object structure", _progressCount++, _progressTotal));
                    ObjectModel objectModel = BuildObjectModel(package);

                    OnProgress(new ObjectModelGenEventArgs("Writing XML schemas for object model", _progressCount++, _progressTotal));
                    string filenameObjectModel = outputDirectory + @"\ObjectModel.xml";
                    File.WriteAllText(filenameObjectModel, XmlHelper.ToXml<ObjectModel>(objectModel, Encoding.UTF8, true), Encoding.UTF8);
                    
                    _repository.CloseFile();
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

            return true;
        }

        /// <summary>
        /// Initialise the process of generation and check all possible preconditions
        /// Give an adequate error message if something is wrong and return the reference to the package
        /// with the object model
        /// As a side effect initialise the progress counters
        /// </summary>
        /// <param name="repositoryName">full path name of the EA model file</param>
        /// <returns>reference to the package with the object model</returns>
        private EA.Package InitializeGeneration(string repositoryName)
        {
            EA.Package objectModelPackage = null;
            bool fileOpened = false;

            try
            {
                FileInfo repositoryFile = new FileInfo(repositoryName);
                if (!File.Exists(repositoryFile.FullName))
                {
                    throw new ArgumentException(String.Format("The Enterprise Architect model file ({0}) can not be found. Specify another model file.", repositoryName));
                }
                _repository.OpenFile(repositoryFile.FullName);
                fileOpened = true;

                EA.Package rootModelPackage = (EA.Package)_repository.Models.GetAt(0);
                if (rootModelPackage == null)
                {
                    throw new ArgumentException("RootModel Package does not exist in the model");
                }
                if (rootModelPackage.Packages.Count == 0)
                {
                    throw new ArgumentException("RootModel Package does not contain child packages");
                }
                objectModelPackage = GetPackageByName("Object Model", rootModelPackage);
                if (objectModelPackage == null)
                {
                    throw new ArgumentException("Package with name: 'Object Model' does not exist in the model. Make sure there is a package with this name in the Enterprise Architect Model.");
                }
                string name = TagGetValue(objectModelPackage.Element.TaggedValues, "Name");
                if (name == "")
                    throw new ArgumentException("The package with name 'Object Model' must contain tag with name 'Name' with a non-empty value. This value represents the name of the object model.");


                // initialise the progress counters
                int linkCount = 0;
                int elementCount = objectModelPackage.Elements.Count;
                foreach (EA.Diagram diagram in objectModelPackage.Diagrams)
                    linkCount += diagram.DiagramLinks.Count;

                // as a side effect set the total for progress counting
                _progressTotal = linkCount + elementCount;
            }
            catch (ArgumentException argumentException)
            {
                MessageBox.Show(argumentException.Message, "Generation error", MessageBoxButtons.OK);
                
                if (fileOpened)
                    _repository.CloseFile();

                return null;
            }
            
            return objectModelPackage;
        }

        /// <summary>
        /// The object model in EA consists of classes and relations. In the
        /// generation the following steps are performed:
        /// - set the name
        /// - get and store all object definitions
        /// - get and store all relations between the objects
        /// </summary>
        /// <param name="packageName">name of the package in Enterprise Architect</param>
        /// <param name="rootModelPackage">reference to the EA package in which to look for the specified package</param>
        /// <returns>reference to a newly created ObjectDictionary with the content of the object model</returns>
        private ObjectModel BuildObjectModel(EA.Package package)
        {
            // in the initialization it has been checked whether this tag exists
            string name = TagGetValue(package.Element.TaggedValues, "Name");

            ObjectModel objectModel = new ObjectModel();

            objectModel.Name = name;
            objectModel.SelfUri = "";
            objectModel.ObjectDefinitions = GetObjectModelDefinitions(package);
            objectModel.ObjectRelations = GetObjectModelRelations(objectModel, package);

            return objectModel;
        }

        /// <summary>
        /// Traverse the list of elements in the specified EA package,
        /// all elements of the stereotype "entity" and/or "relation" are object definitions. Each
        /// of these elements result in a new object definition. Set the properties of the object definition
        /// according to the information from the EA element
        /// </summary>
        /// <param name="package">reference to the EA pacage with the object model</param>
        /// <returns>an array with all relevant object definitions</returns>
        private ObjectDefinition[] GetObjectModelDefinitions(EA.Package package)
        {
            List<ObjectDefinition> objectDefinitions = new List<ObjectDefinition>();

            foreach (EA.Element element in package.Elements)
            {
                if (element != null)

                {
                    OnProgress(new ObjectModelGenEventArgs("Writing object model definition", _progressCount++, _progressTotal));

                    ObjectType objectType;
                    if (element.Stereotype == "entity")
                        objectType = ObjectType.entity;
                    else if (element.Stereotype == "relation")
                        objectType = ObjectType.relation;
                    else // unknown elements are ignored
                        continue;

                    // FIXME what about complex properties ?!

                    ObjectDefinition objectDef = GetObjectDefinition(element, objectType);

                    objectDefinitions.Add(objectDef);

                }
            }

            return objectDefinitions.ToArray();
        }

        /// <summary>
        /// Get one object definition,
        /// translate all values from Enterprise Architect to an internal object definition of
        /// the specified object type
        /// </summary>
        /// <param name="element">reference to the source element</param>
        /// <param name="objectType">the target obect type</param>
        /// <returns></returns>
        private ObjectDefinition GetObjectDefinition(EA.Element element, ObjectType objectType)
        {
            ObjectDefinition objectDef = new ObjectDefinition();

            objectDef.Name = element.Name;
            objectDef.Id = element.ElementID;
            objectDef.ObjectType = objectType;
            objectDef.Properties = GetProperties(element.ElementID);

            objectDef.ComplexProperties = GetComplexProperties(element, new List<int>());

            return objectDef;
        }

        /// <summary>
        /// Get the complex properties for the specified element in the Enterprise Architect repository
        /// A complex property is a class of the stereotype "complexproperty". Traverse all relations of
        /// the element and find the elements with stereotype "complexproperty". Add all elements found to the
        /// complexproperty collection if it isn't part of the collection already
        /// </summary>
        /// <param name="element">reference to the element</param>
        /// <param name="processed">list of elements which are processed already</param>
        /// <returns>array with complexproperties if they exist, null otherwise</returns>
        private ObjectDefinitionComplexProperty[] GetComplexProperties(EA.Element element, List<int> processed)
        {
            List<ObjectDefinitionComplexProperty> complexProperties = new List<ObjectDefinitionComplexProperty>();
            
            // foreach relation of this element: if the other end is a complex property add it to the list

            EA.Collection connectors = element.Connectors;
            foreach (EA.Connector connector in connectors)
            {
                EA.Element otherElement;
                EA.ConnectorEnd otherConnectorEnd;

                // first determine who is the supplier and the client
                if (element.ElementID == connector.SupplierID)
                {
                    otherElement = _repository.GetElementByID(connector.ClientID);
                    otherConnectorEnd = connector.ClientEnd;
                }
                else
                {
                    otherConnectorEnd = connector.SupplierEnd;
                    otherElement = _repository.GetElementByID(connector.SupplierID); 
                }
                                             
                if (otherElement.Stereotype == "complexproperty" && ! processed.Contains(otherElement.ElementID))
                {
                    ObjectDefinitionComplexProperty complexProperty = new ObjectDefinitionComplexProperty();
                    complexProperty.Name = otherElement.Name;
                    complexProperty.Properties = GetProperties(otherElement.ElementID);
                    Cardinality cardinality = SetCardinality(otherConnectorEnd.Cardinality);
                    complexProperty.MinOccurs = cardinality.MinOccurs;
                    complexProperty.MaxOccurs = cardinality.MaxOccurs;
                    processed.Add(element.ElementID);
                    complexProperty.ComplexProperties = GetComplexProperties(otherElement, processed);
                    processed.Remove(element.ElementID);
                    complexProperties.Add(complexProperty);
                }
            }
            
            return (complexProperties.Count == 0) ? null : complexProperties.ToArray();
        }

        /// <summary>
        /// Traverse all links in the specified EA package, check the direction of the relation and
        /// create a new object relation from this link. 
        /// </summary>
        /// <param name="objectModel">reference to the object model with all known object definitions</param>
        /// <param name="package">reference to the EA pacage with the object model</param>
        /// <returns>an array with all relevant object relations</returns>
        private ObjectRelation[] GetObjectModelRelations(ObjectModel objectModel, EA.Package package)
        {
            List<ObjectRelation> objectRelations = new List<ObjectRelation>();

            foreach (EA.Diagram diagram in package.Diagrams)
            {
                foreach (EA.DiagramLink link in diagram.DiagramLinks)
                {
                    OnProgress(new ObjectModelGenEventArgs("Writing object model relation", _progressCount++, _progressTotal));

                    EA.Connector connector = _repository.GetConnectorByID(link.ConnectorID);
                    EA.Element sourceElement = null;
                    EA.Element targetElement = null;
                    if (connector.Direction == "Source -> Destination")
                    {
                        sourceElement = _repository.GetElementByID(connector.ClientID);
                        targetElement = _repository.GetElementByID(connector.SupplierID);
                    }
                    else if (connector.Direction == "Destination -> Source")
                    {
                        sourceElement = _repository.GetElementByID(connector.SupplierID);
                        targetElement = _repository.GetElementByID(connector.ClientID);
                    }
                    else
                        throw new ArgumentException("A relation does not have a direction. Each relation must have a direction");

                    ObjectDefinition sourceObjectDef = objectModel.GetObjectDefinition(sourceElement.ElementID);
                    ObjectDefinition targetObjectDef = objectModel.GetObjectDefinition(targetElement.ElementID);

                    // FIXME: better error handling?!
                    if (sourceObjectDef == null || targetObjectDef == null)
                        continue;

                    if (((sourceObjectDef.ObjectType == ObjectType.entity &&
                           targetObjectDef.ObjectType == ObjectType.relation) ||
                          (sourceObjectDef.ObjectType == ObjectType.relation &&
                           targetObjectDef.ObjectType == ObjectType.entity)))
                    {
                        Cardinality cardinality = SetCardinality(connector.SupplierEnd.Cardinality);
                        ObjectRelation objectRelation = new ObjectRelation();

                        objectRelation.Source = sourceObjectDef.Name;
                        objectRelation.Target = targetObjectDef.Name;
                        objectRelation.MinOccurs = cardinality.MinOccurs;
                        objectRelation.MaxOccurs = cardinality.MaxOccurs;

                        objectRelations.Add(objectRelation);
                    }
                }
            }

            return objectRelations.ToArray();
        }

        /// <summary>
        /// Construct a list of ObjectDefinitionProperties, each ObjectDefinitionProperty represents
        /// an attribute of the specified element.
        /// </summary>
        /// <param name="elementId">the element to consider</param>
        /// <returns>array list of ObjectDefinitionProperties</returns>
        private ObjectDefinitionProperty[] GetProperties(int elementId)
        {
            List<ObjectDefinitionProperty> properties = new List<ObjectDefinitionProperty>();

            foreach (EA.Attribute attribute in _repository.GetElementByID(elementId).AttributesEx)
            {
                ObjectDefinitionProperty prop = new ObjectDefinitionProperty();
                prop.Type = attribute.Type;
                prop.Name = attribute.Name;
                prop.Required = attribute.LowerBound.Trim() == "0" ? false : true; 
                prop.RequiredSpecified = true;
                properties.Add(prop);
            }

            return properties.ToArray();
        }

        /// <summary>
        /// Converts the cardinality as used in Enterprise architect to the cardinality
        /// structure which is used internally
        /// The Enterprise Architect version is a user defined string varying from
        /// *, 0, 0..1, 0..*, 1, 1.., 1..* or a fixed numeric number
        /// </summary>
        /// <param name="EACardinality">string with the EA cardinality</param>
        /// <returns>cardinality specified as a Cardinality object</returns>
        private Cardinality SetCardinality(string EACardinality)
        {
            Cardinality cardinality = new Cardinality();

            switch (EACardinality)
            {
                case "*":
                    cardinality.MinOccurs = 0;
                    cardinality.MaxOccurs = "unbounded";
                    break;
                case "0":
                    // actually this a very strange situation but the user asks for it
                    cardinality.MinOccurs = 0;
                    cardinality.MaxOccurs = "0";
                    break;
                case "0..1":
                    cardinality.MinOccurs = 0;
                    cardinality.MaxOccurs = "1";
                    break;
                case "0..*":
                    cardinality.MinOccurs = 0;
                    cardinality.MaxOccurs = "unbounded";
                    break;
                case "1":
                    cardinality.MinOccurs = 1;
                    cardinality.MaxOccurs = "1";
                    break;
                case "1..":
                    cardinality.MinOccurs = 1;
                    cardinality.MaxOccurs = "unbounded";
                    break;
                case "1..*":
                    cardinality.MinOccurs = 1;
                    cardinality.MaxOccurs = "unbounded";
                    break;
                case "":
                    // pick default
                    break;
                default: // format is "x..y"
                    string[] tokens = EACardinality.Split('.');

                    if (tokens.Length != 3)
                        throw new ArgumentException(String.Format("Cardinality {0} is incorrect. Specify according to the format 'x..y'", EACardinality));
                    
                    int minOccurs;
                    if (int.TryParse(tokens[0], out minOccurs))
                        cardinality.MinOccurs = minOccurs;
                    else
                        throw new ArgumentException("Cardinality minOccurs is not an integer value");

                    string maxOccurs = tokens[2];
                    if (maxOccurs == "*")
                        maxOccurs = "unbounded";

                    cardinality.MaxOccurs = maxOccurs;
                    break;
            }

            return cardinality;
        }

        /// <summary>
        /// Return the reference to a child package based on its name
        ///   (this is a replacement of the EA GetByName method. This GetByName method
        ///    throws an 'index out of bounds' exception if a non existing 
        ///    package is requested)
        /// </summary>
        /// <param name="packageName">name of the package to be returned</param>
        /// <param name="rootPackage">parent package to search in</param>
        /// <returns>package reference if found, null otherwise</returns>
        private EA.Package GetPackageByName(string packageName, EA.Package rootPackage)
        {
            EA.Package package = null;

            foreach (EA.Package p in rootPackage.Packages)
            {
                if (p.Name == packageName)
                {
                    package = p;
                    break;
                }
            }

            return package;
        }

        /// <summary>
        /// Returns whether a tag exists in a collection of tag values
        /// </summary>
        /// <param name="taggedValues">collection to be searched</param>
        /// <param name="tagName">name of the tag to search for</param>
        /// <returns>true if tag exists in collection, false otherwise</returns>
        private bool TagExist(EA.Collection taggedValues, string tagName)
        {
            bool rv = false;

            foreach (EA.TaggedValue tv in taggedValues)
            {
                if (tv.Name == tagName)
                {
                    rv = true;
                    break;
                }
            }

            return rv;
        }

        /// <summary>
        /// Returns whether a tag value is true
        /// </summary>
        /// <param name="taggedValues">the collection in which the tag should exist</param>
        /// <param name="tagName">name of the tag to search for</param>
        /// <returns>true if tagvalue is true, false in all other cases (also if the tag is non-existent)</returns>
        private bool TagIsTrue(EA.Collection taggedValues, string tagName)
        {
            bool rv = false;

            foreach (EA.TaggedValue tv in taggedValues)
            {
                if (tv.Name == tagName)
                {
                    rv = (tv.Value.ToLower() == "true");
                    break;
                }
            }

            return rv;
        }

        /// <summary>
        /// Returns the value of a tag from a collection of tags
        /// </summary>
        /// <param name="taggedValues">the collection in which the tag should exist</param>
        /// <param name="tagName">name of the tag to search for</param>
        /// <returns>value of the tag, returns the empty string if the tag does not exist</returns>
        private string TagGetValue(EA.Collection taggedValues, string tagName)
        {
            string rv = "";

            foreach (EA.TaggedValue tv in taggedValues)
            {
                if (tv.Name == tagName)
                {
                    rv = tv.Value;
                    break;
                }
            }

            return rv;
        }

    }        
}
