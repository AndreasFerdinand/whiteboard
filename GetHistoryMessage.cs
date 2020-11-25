namespace wssserver
{
    public class GetHistoryMessage : Message
    {
        public const string MessageType = "GetHistory";

        public override string getOutboundMessageAsJson()
        {
            return room.getHistoryMessage();
        }

        public override void processInbound()
        {

        }

        public override void processOutbound()
        {
            sourceSocket.Send(getOutboundMessageAsJson());
        }
    }

}