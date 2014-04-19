using System;
namespace Luminis.Its.Tools.Sparx.ObjectModelGen
{
    public class Cardinality
    {
        public int MinOccurs { get; set; }
        public string MaxOccurs { get; set; }

        public Cardinality()
        {
            this.MinOccurs = 0;
            this.MaxOccurs = "unbounded";
        }

        public Cardinality(int minOccurs, string maxOccurs)
        {
            this.MinOccurs = minOccurs;
            this.MaxOccurs = maxOccurs;
        }

        public override string ToString()
        {
            return String.Format("MinOccurs={0}, MaxOccurs={1}", this.MinOccurs, this.MaxOccurs);
        }
    }
}
