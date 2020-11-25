using System.Text.Json;
using Fleck;

namespace wssserver
{
    public abstract class Message : IMessage
    {
        protected Room room;
        protected IWebSocketConnection sourceSocket;

        public string type {get; set;}
        public string user {get;set;}

        public abstract void processInbound();
        public abstract string getOutboundMessageAsJson();

        public void setRoom(Room room)
        {
            this.room = room;
        }

        public void setSourceSocket(IWebSocketConnection sourceSocket)
        {
            this.sourceSocket = sourceSocket;
            this.user = room.getUserNameFromSender(sourceSocket);
        }

        public virtual void processOutbound()
        {
            room.broadCastMessage(getOutboundMessageAsJson());
        }
    }
}