using System;

namespace Luminis.Its.Tools.Sparx.ObjectModelGen
{
    public class ObjectModelGenEventArgs : EventArgs
    {
        public int Count { get; set; }
        public int Total { get; set; }
        public string Message { get; set; }
        public ObjectModelGenEventArgs(string message, int count, int total)
        {
            this.Count = (count < total)? count : total;
            this.Total = total;
            this.Message = message;
        }
    }
}
