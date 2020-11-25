namespace wssserver
{
    public interface IOutboundMessage
    {
        string getOutboundMessageAsJson();

        void processOutbound();
    }

}