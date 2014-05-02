namespace TimeTraveller.Services.Interfaces
{
    public interface IBaseObjectType
    {
        int Id { get; }
        string Name { get; }
        string RelativeUri { get; }
    }
}
