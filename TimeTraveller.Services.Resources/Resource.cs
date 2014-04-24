using System.Xml.Serialization;
using TimeTraveller.Services.Items;

namespace TimeTraveller.Services.Resources
{
    public class Resource: AbstractItem, IItem
    {
        #region Public Properties
        [XmlIgnore()]
        public string ContentType { get; set; }

        [XmlIgnore()]
        public byte[] Content { get; set; }
	    #endregion

        #region IItem/AbstractItem Members
        [XmlIgnore()]
        public override string SelfUri { get; set; }
        #endregion
    }
}
