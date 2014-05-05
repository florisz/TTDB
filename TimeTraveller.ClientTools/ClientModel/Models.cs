using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTraveller.ClientModel
{
    public abstract class Specification
	{
        public Specification(string name, string filename)
		{
			this.Name = name;
			this.Filename = filename;
		}
		public string Name { get; set; }
        public string Filename { get; set; }
        public abstract string Path { get; }
    }

    public class ObjectModel : Specification
	{
        public ObjectModel(string name, string filename) : base(name, filename) { }

        public override string Path
        {
            get { return string.Format("specifications/objectmodels/{0}", this.Name); }
        }
    }

	public class CaseFileSpecification : Specification
	{
        public CaseFileSpecification(string name, string filename, ObjectModel objectModel) : base (name, filename)
        {
            this.ObjectModel = objectModel;
        }
        public ObjectModel ObjectModel { get; set; }
        public override string Path
        {
            get { return string.Format("specifications/casefiles/{0}/{1}", this.ObjectModel.Name, this.Name); }
        }
    }

    public abstract class ConcreteSpecification : Specification
    {
        public ConcreteSpecification(string name, string filename, ObjectModel objectModel, CaseFileSpecification caseFileSpec)
            : base(name, filename)
        {
            this.ObjectModel = objectModel;
            this.CaseFileSpec = caseFileSpec;
        }
        public ObjectModel ObjectModel { get; set; }
        public CaseFileSpecification CaseFileSpec { get; set; }
    }

    public class CaseFile : ConcreteSpecification
	{
        public CaseFile(string name, string filename, ObjectModel objectModel, CaseFileSpecification caseFileSpec)
            : base(name, filename, objectModel, caseFileSpec)
        {}

        public override string Path
        {
            get { return string.Format("casefiles/{0}/{1}/{2}", this.ObjectModel.Name, this.CaseFileSpec.Name, this.Name); }
        }
    }

    public class RuleSpec : ConcreteSpecification
    {
        public RuleSpec(string name, string filename, ObjectModel objectModel, CaseFileSpecification caseFileSpec)
            : base(name, filename, objectModel, caseFileSpec)
        {}

        public override string Path
        {
            get { return string.Format("rules/{0}/{1}/{2}", this.ObjectModel.Name, this.CaseFileSpec.Name, this.Name); }
        }
    }

    public class ViewSpec : ConcreteSpecification
    {
        public ViewSpec(string name, string filename, ObjectModel objectModel, CaseFileSpecification caseFileSpec)
            : base(name, filename, objectModel, caseFileSpec)
        {}

        public override string Path
        {
            get { return string.Format("representations/{0}/{1}/{2}", this.ObjectModel.Name, this.CaseFileSpec.Name, this.Name); }
        }
    }
}
