namespace wssserver
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Web;
    
    public class AddStickyNoteMessage : Message
    {
        public const string MessageType = "AddStickyNote";
        public const string DefaultNoteColor = "canary";
        public readonly string[] AllowedColors = new string[] {"canary","palepink","paleorange","limegreen","paleblue"};

        public Guid uuid {get;set;}
        public string content {get; set;}
        public string color {get; set;}

        public override string getOutboundMessageAsJson()
        {
            string message = JsonSerializer.Serialize(this);
            
            return message;
        }

        public override void processInbound()
        {
            content = HttpUtility.HtmlEncode(content);
            content = content.ReplaceSymbols();

            if ( !AllowedColors.Contains(color) )
            {
                color = DefaultNoteColor;
            }
        }
    }

}