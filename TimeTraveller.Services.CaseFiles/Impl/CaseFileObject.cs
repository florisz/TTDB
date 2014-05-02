using System;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.CaseFiles.Impl
{
    public class CaseFileObject
    {
        public Guid Id { get; set; }
        public bool IsRootEntity { get; set; }
        public string ObjectName { get; set; }
        public ItemType ItemType { get; set; }
        public TimePoint Start { get; set; }
        public TimePoint End { get; set; }
        public string Text { get; set; }
        public CaseFileObject Relation1 { get; set; }
        public CaseFileObject Relation2 { get; set; }
        public IBaseObjectValue BaseObjectValue { get; set; }
    }
}
