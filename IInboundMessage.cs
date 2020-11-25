using Fleck;

namespace wssserver
{
    public interface IInboundMessage
    {
        void processInbound();
        void setRoom(Room room);
        void setSourceSocket(IWebSocketConnection sourceSocket);
    }

}