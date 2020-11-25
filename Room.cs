namespace wssserver
{
    using System.Collections.Generic;
    using Fleck;
    using System.Linq;
    using System.Text.Json;
    using System;
    using System.Text;

    public class Room
    {
        readonly string roomId;
        readonly string path;

        Dictionary<IWebSocketConnection, Client> clients = new Dictionary<IWebSocketConnection, Client> ();
        List <IOutboundMessage> history = new List<IOutboundMessage>();

        public string RoomId => roomId;
        public string Path => path;

        public Room(string roomId, string path)
        {
            this.roomId = roomId;
            this.path = path;
        }

        public void addClient(IWebSocketConnection socket)
        {
            clients.Add(socket, new Client(socket));
        }

        public List<string> getUser()
        {
            List<string> User = new List<string>();

                foreach (KeyValuePair<IWebSocketConnection, Client> client in clients)
                {
                    string currentUserName = client.Value?.User?.getUsername();
                    User.Add((currentUserName == null ? "[unknown]" : currentUserName));
                }

            return User;
        }

        public string getUserNameFromSender(IWebSocketConnection sender)
        {
            string userName = clients[sender]?.User?.getUsername();

            if ( string.IsNullOrEmpty(userName) )
            {
                userName = "[unknown]";
            }

            Console.WriteLine("getUserNameFromSender" + userName);

            return userName;
        }

        public string getUserMessage()
        {
            string userMessage = "";

            List<string> user = getUser();

            if ( user.Count > 0 )
            {
                userMessage = "\"" + string.Join("\",\"",user) + "\"";
            }

            userMessage = "{\"type\":\"UserList\",\"userlist\":[" + userMessage + "]}";

            return userMessage;
        }

        public string getHistoryMessage()
        {
                StringBuilder historyMessageBuilder = new StringBuilder();

                int totalCount = history.Count();
                for (int count = 0; count < totalCount; count++)
                {
                    var result = history[count];

                    if ( (count + 1) == totalCount )
                    {
                        historyMessageBuilder.Append(result.getOutboundMessageAsJson());
                    }
                    else
                    {
                        historyMessageBuilder.Append(result.getOutboundMessageAsJson());
                        historyMessageBuilder.Append(",");
                    }
                }

                string historyMessage = "{ \"type\":\"History\", \"history\": [ " + historyMessageBuilder.ToString() + " ] }";

                return historyMessage;
        }

        public void processMessage(string message, IWebSocketConnection socket)
        {
            OLDMessage receivedMessage = JsonSerializer.Deserialize<OLDMessage>(message);


            if ( receivedMessage.type == "User"  )
            {
                User receivedUser = JsonSerializer.Deserialize<User>(message);

                clients[socket].User = receivedUser;
            }
            else if ( receivedMessage.type == "GetUser"  )
            {
                socket.Send( getUserMessage() );
            }
            else if ( receivedMessage.type == "ChatMessage" || receivedMessage.type == "GetHistory" || receivedMessage.type == "AddStickyNote" )
            {
                IMessage msg = MessageFactory.createMessageFromString(message,this,socket);

                msg.processInbound();
                msg.processOutbound();
            }
            else
            {
                broadCastMessage(message);
            }

        }

        public void broadCastMessage(string message)
        {
            history.Add(new RawMessage(message));

            clients.ToList().ForEach(s => s.Value.sendMessage(message));
        }

        public void deleteClient(IWebSocketConnection socket)
        {
            clients[socket].closeClient();

            clients.Remove(socket);
        }

        public void closeRoom()
        {
            clients.ToList().ForEach(s => s.Value.closeClient());
            clients.Clear();
        }

    }

}