namespace wssserver
{
    using System;
    using System.Text;
    using System.Text.Json;
    using Fleck;

    public class MessageFactory
    {
        public static IMessage createMessageFromString(string JsonMessage, Room room, IWebSocketConnection socket)
        {
            IMessage inboundMessage;

            OLDMessage message = JsonSerializer.Deserialize<OLDMessage>(JsonMessage);

            if ( message == null )
            {
                throw new Exception("Unable to deserialize incoming message");
            }

            if ( string.IsNullOrEmpty( message.type ) )
            {
                throw new Exception("Empty Message type");
            }

            Console.WriteLine("CREATING MESSAGE");

            switch (message.type)
            {
                case ChatMessage.MessageType:
                    inboundMessage = JsonSerializer.Deserialize<ChatMessage>(JsonMessage);
                    break;

                case GetHistoryMessage.MessageType:
                    inboundMessage = JsonSerializer.Deserialize<GetHistoryMessage>(JsonMessage);
                    break;

                case AddStickyNoteMessage.MessageType:
                    inboundMessage = JsonSerializer.Deserialize<AddStickyNoteMessage>(JsonMessage);
                    break;

                case UserMessage.MessageType:
                    inboundMessage = JsonSerializer.Deserialize<UserMessage>(JsonMessage);
                    break;

                default:
                    throw new Exception("Unknown Message type");
            }

            inboundMessage.setRoom(room);
            inboundMessage.setSourceSocket(socket);

            return inboundMessage;
        }
    }
}