using Microsoft.WindowsAzure.Storage.Table;

namespace LineBotDKI.Models
{
    public class EventSourceLocation : EventSourceState
    {
        public string Location { get; set; }

        public EventSourceLocation() { }
    }
}