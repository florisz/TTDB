using System.IO;

namespace TimeTraveller.Services.Rest.Impl
{
    public interface ICommand
    {
        Stream Execute(CommandContext context, IFormatter formatter);
    }
}
