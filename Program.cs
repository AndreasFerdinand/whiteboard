using System;
using Fleck;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace wssserver
{
    class Program
    {
        static void Main()
        {
            FleckLog.Level = LogLevel.Debug;
            //var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://0.0.0.0:8181");

            server.SupportedSubProtocols = new []{ "whiteboard" };

            ConcurrentDictionary<string, Room> rooms = new ConcurrentDictionary<string, Room> ();

            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            Console.WriteLine("OPEN! " + socket.ConnectionInfo.Path );

                            if ( rooms.ContainsKey(socket.ConnectionInfo.Path) )
                            {
                                rooms[socket.ConnectionInfo.Path].addClient(socket);
                            }
                            else
                            {
                                var newRoom = new Room(socket.ConnectionInfo.Path,socket.ConnectionInfo.Path);
                                newRoom.addClient(socket);

                                rooms.TryAdd(socket.ConnectionInfo.Path,newRoom);
                            }
                        };
                    socket.OnClose = () =>
                        {
                            Console.WriteLine("Close! " + socket.ConnectionInfo.Path );

                            if ( rooms.ContainsKey(socket.ConnectionInfo.Path) )
                            {
                                rooms[socket.ConnectionInfo.Path].deleteClient(socket);
                            }

//                            allSockets.Remove(socket);
                        };
                    socket.OnMessage = message =>
                        {
                            Console.WriteLine(message);

                            if ( rooms.ContainsKey(socket.ConnectionInfo.Path) )
                            {
                                //rooms[socket.ConnectionInfo.Path].broadCastMessage(message);
                                rooms[socket.ConnectionInfo.Path].processMessage(message,socket);
                            }

                            //allSockets.ToList().ForEach(s => s.Send(message));
                        };
                });


            var input = Console.ReadLine();
            while (input != ":exit")
            {
/*                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);
                }*/
                if ( input[0] == ':' )
                {
                    if ( input == ":rooms" )
                    {
                        foreach (var room in rooms)
                        {
                            Console.WriteLine( room.Value.Path );
                        }
                    }
                    else if ( input == ":users" )
                    {
                        foreach (var room in rooms)
                        {
                            var user = room.Value.getUser();
                            Console.WriteLine( room.Value.Path + " (" + user.Count + ")");
                            Console.WriteLine( "  " + string.Join(",",room.Value.getUser()));
                        }
                    }
                }
                else
                {
                    foreach (var room in rooms)
                    {
                        room.Value.broadCastMessage(input);
                    }
                }

                input = Console.ReadLine();
            }

                foreach (var room in rooms)
                {
                    room.Value.closeRoom();
                }

        }
    }
}
