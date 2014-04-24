using System.Text;

using TimeTraveller.General.Unity;

namespace TimeTraveller.Services.Rest.Impl
{
    /// <summary>
    /// The CommandFactory can create commands based on the request context and using the Unity-container.
    /// 
    /// The command to lookup is constructed from the context, i.e.:
    /// 
    ///  GETCaseFileTimePoint: 
    ///    GET = the HTTP-method
    ///    CaseFile = the requested type
    ///    TimePoint = the query-parameter ?timepoint=XXX is specified
    /// </summary>
    public sealed class CommandFactory
    {
        #region Factory Method
        public static ICommand Create(IUnity container, CommandContext context)
        {
            string commandId = GetCommandId(context);

            ICommand result = container.Resolve<ICommand>(commandId);

            return result;
        }
        #endregion

        #region Private Methods
        private static string GetCommandId(CommandContext context)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}{1}", context.Request.Method, context.Type);
            if (context.FetchMultiple)
            {
                result.Append("s");
            }
            if (context.IsTimePointSpecified)
            {
                result.Append("TimePoint");
            }
            else if (context.VersionRequested)
            {
                result.Append("Version");
            }

            result.Append("Command");

            return result.ToString();
        }
        #endregion
    }
}
