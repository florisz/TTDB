using System;
using System.Xml;
using System.Xml.Serialization;

using Luminis.Its.Services;
using Luminis.Its.Services.Items;

namespace Luminis.Its.Services.Resources
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
