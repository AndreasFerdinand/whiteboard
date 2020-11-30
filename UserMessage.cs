namespace wssserver
{
    using System.Text.Json;
    using System.Web;

    public class UserMessage : Message
    {
        public const string MessageType = "User";

        public string name {get; set;}

        public override string getOutboundMessageAsJson()
        {
            return "";
        }

        public override void processInbound()
        {
            room.setUser(sourceSocket,name);
        }
    }
}