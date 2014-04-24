using System;

namespace TimeTraveller.Services.Data
{
    public interface IRelationObject : IBaseObject
    {
        IEntityObject Relation1 { get; set; }

        Guid Relation1Id { get; }

        IEntityObject Relation2 { get; set; }

        Guid Relation2Id { get; }
    }
}
