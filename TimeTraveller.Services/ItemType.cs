namespace TimeTraveller.Services
{
    public enum ItemType
    {
        CaseFileSpecification = 1,
        CaseFile = 2,
        Entity = 3,
        ObjectModel = 4,
        Relation = 5,
        Representation = 6,
        Resource = 7,
        RuleSet = 8
    }

    public sealed class ItemTypeHelper
    {
        private ItemTypeHelper()
        {
        }

        public static int Convert(IBaseObjectType type)
        {
            return (int)type.Id;
        }

        public static int Convert(ItemType type)
        {
            return (int)type;
        }

        public static ItemType Convert(int type)
        {
            return (ItemType)type;
        }
    }
}
