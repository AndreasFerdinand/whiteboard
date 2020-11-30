namespace wssserver
{
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using Fleck;
    using System.Linq;
    using System.Text.Json;
    using System;
    using System.Text;

    public class Room
    {
        readonly string roomId;
        readonly string path;

        ConcurrentDictionary<IWebSocketConnection, Client> clients = new ConcurrentDictionary<IWebSocketConnection, Client> ();
        ConcurrentQueue <IOutboundMessage> history = new ConcurrentQueue<IOutboundMessage>();

        public string RoomId => roomId;
        public string Path => path;

        protected DateTime lastHistoryModificationTime;
        protected DateTime lastHistoryBackupTime;

        protected System.Timers.Timer timer;

        public Room(string roomId, string path)
        {
            this.roomId = roomId;
            this.path = path;

            lastHistoryModificationTime = DateTime.Now;
            lastHistoryBackupTime = lastHistoryModificationTime;

            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(BackupHistory);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        public void BackupHistory(Object source, System.Timers.ElapsedEventArgs e)
        {
            if ( lastHistoryBackupTime < lastHistoryModificationTime )
            {
                DateTime backupTime = DateTime.Now;

                System.IO.Directory.CreateDirectory("backup");

                //string historyJson = JsonSerializer.Serialize(history.ToArray());

                System.IO.File.WriteAllText("backup/" + RoomId + "_" + backupTime.ToString(), getHistoryMessage());

                lastHistoryBackupTime = backupTime;
            }
        }

        public void setUser(IWebSocketConnection socketConnection, string user)
        {
            clients[socketConnection].User = new User(user);
        }

        public void addClient(IWebSocketConnection socket)
        {
            clients.TryAdd(socket, new Client(socket));
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
                var tempHistoryMessages = new List<string>();
                string historyMessageContent = "";

                foreach(var msg in history)
                {
                    string messagecontent = msg?.getOutboundMessageAsJson();

                    if ( !string.IsNullOrEmpty(messagecontent))
                    {
                        tempHistoryMessages.Add(messagecontent);
                    }
                }

                if ( tempHistoryMessages.Count > 0 )
                {
                    historyMessageContent = string.Join(',',tempHistoryMessages);
                }

                string historyMessage = "{ \"type\":\"History\", \"history\": [ " + historyMessageContent + " ] }";

                return historyMessage;
        }

        public void processMessage(string message, IWebSocketConnection socket)
        {
            OLDMessage receivedMessage = JsonSerializer.Deserialize<OLDMessage>(message);

            if ( receivedMessage.type == "GetUser"  )
            {
                socket.Send( getUserMessage() );
            }
            else if ( receivedMessage.type == "ChatMessage" || receivedMessage.type == "GetHistory" ||
                      receivedMessage.type == "AddStickyNote" || receivedMessage.type == "User" )
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
            lastHistoryModificationTime = DateTime.Now;
            history.Enqueue(new RawMessage(message));

            clients.ToList().ForEach(s => s.Value?.sendMessage(message));
        }

        public void deleteClient(IWebSocketConnection socket)
        {
            Client client;

            clients.TryRemove(socket,out client);

            client?.closeClient();
        }

        public void closeRoom()
        {
            foreach( var client in clients.Keys )
            {
                deleteClient(client);
            }
        }
    }
}