using System.IO;

namespace Luminis.Its.Services.Rest.Impl
{
    public interface IFormatter
    {
        Stream Format(CommandContext context, object item);
    }
}
