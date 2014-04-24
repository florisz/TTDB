namespace TimeTraveller.Services
{
    public interface IBaseObjectType
    {
        int Id { get; }
        string Name { get; }
        string RelativeUri { get; }
    }
}
