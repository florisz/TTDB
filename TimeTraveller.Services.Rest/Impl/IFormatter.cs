using System.IO;

namespace TimeTraveller.Services.Rest.Impl
{
    public interface IFormatter
    {
        Stream Format(CommandContext context, object item);
    }
}
