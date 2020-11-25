namespace wssserver
{
    using System;
    using System.Collections.Generic;
    using Fleck;

    public class Client
    {
        IWebSocketConnection socket;
        IUser user;

        public IUser User { get => user; set => user = value; }

        public Client(IWebSocketConnection socket)
        {
            this.socket = socket;
        }

        public void sendMessage(string message)
        {
            socket.Send(message);
        }

        public void closeClient()
        {
            socket.Close();
        }
    }


}