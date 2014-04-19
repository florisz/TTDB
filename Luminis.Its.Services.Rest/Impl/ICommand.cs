using System.IO;

namespace Luminis.Its.Services.Rest.Impl
{
    public interface ICommand
    {
        Stream Execute(CommandContext context, IFormatter formatter);
    }
}
