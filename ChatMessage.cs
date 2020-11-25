namespace wssserver
{
    using System.Text.Json;
    using System.Web;

    public class ChatMessage : Message
    {
        public const string MessageType = "ChatMessage";

        public string content {get; set;}

        public override void processInbound()
        {
            content = HttpUtility.HtmlEncode(content);
            content = content.ReplaceSymbols();
        }

        public override string getOutboundMessageAsJson()
        {
            string message = JsonSerializer.Serialize(this);
            
            return message;
        }
    }
}