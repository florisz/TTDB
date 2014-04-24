using System.Xml.Linq;
using System.Collections.Generic;

namespace Luminis.Its.Client.Model
{
    /// <summary>
    /// Compare function used in the linq query to check whether one of the attributes 
    /// in a collection has a specified name. Its value is irrelevant and therefore 
    /// left out of the equation.
    /// </summary>
    class AttributeComparer : IEqualityComparer<System.Xml.Linq.XAttribute>
    {
        #region IEqualityComparer<XAttribute> Members

        public bool Equals(System.Xml.Linq.XAttribute x, System.Xml.Linq.XAttribute y)
        {
            return (x.Name == y.Name);
        }

        public int GetHashCode(System.Xml.Linq.XAttribute attr)
        {
            // Check whether the object is null.
            if (attr == null) return 0;

            // Get the hash code for the Name field if it is not null.
            int hashAttrName = attr.Name == null ? 0 : attr.Name.GetHashCode();

            // Calculate the hash code for the product.
            return hashAttrName;
        }

        #endregion
    }
}
