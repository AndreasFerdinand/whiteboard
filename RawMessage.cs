namespace wssserver
{
    class RawMessage : IOutboundMessage
    {
        string message;

        public RawMessage(string message)
        {
            this.message = message;
        }

        public string Message { get => message; set => message = value; }

        public string getOutboundMessageAsJson()
        {
            return Message;
        }

        public void processOutbound()
        {
            
        }
    }

}