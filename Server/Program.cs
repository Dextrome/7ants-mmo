using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using Lidgren.Network;
using Data;
using Data.World;
using Data.Mobiles;
using Microsoft.Xna;
using Microsoft.Xna.Framework;

namespace Server
{
    class Program
    {
        static NetServer Server;
        static NetPeerConfiguration Config;
        static bool isClosing = false;

        static void Main(string[] args)
        {
            Map Map = new Map();
            Map.LoadMap("world");
            Config = new NetPeerConfiguration("7ANTSMMO");
            Config.Port = 14242;
            Config.MaximumConnections = 20;
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Server = new NetServer(Config);
            Server.Start();
            Console.WriteLine("Server Started @ port " + Config.Port);
            Dictionary<NetConnection, Player> GameWorldState = new Dictionary<NetConnection, Player>();
            NetIncomingMessage IncomingMessage;
            DateTime Time = DateTime.Now;
            TimeSpan TimeToPass = new TimeSpan(0, 0, 0, 0, 200);
            Console.WriteLine("Waiting for new connections and updateing world state to current ones");
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            // Infinite Loop
            while (true)
            {
                if (isClosing)
                {
                    // Exiting Server Application
                    Map.filename = "world";
                    Map.SaveMap();
                    return;
                }
                if (Console.KeyAvailable)
                {
                    string line = Console.ReadLine(); // Get string from user
                    if (line == "save") // Check string
                    {
                        Map.filename = "world";
                        Map.SaveMap();
                        Console.WriteLine("World Saved");
                    }
                }
                if ((IncomingMessage = Server.ReadMessage()) != null)
                {
                    switch (IncomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            // Read the first byte of the packet
                            // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                            if (IncomingMessage.ReadByte() == (byte)Statics.PacketTypes.LOGIN)
                            {
                                Console.WriteLine("Incoming LOGIN");
                                IncomingMessage.SenderConnection.Approve();
                                string username = IncomingMessage.ReadString();
                                string password = IncomingMessage.ReadString();
                                int X = IncomingMessage.ReadInt32();
                                int Y = IncomingMessage.ReadInt32();
                                int Z = IncomingMessage.ReadInt32();
                                bool existingAccount = false;
                                XDocument MapDoc = XDocument.Load("Accounts.xml");
                                XElement Accounts = MapDoc.Element("accounts");
                                foreach (XElement Account in Accounts.Elements("account"))
                                    if (Account.Attribute("username").Value.ToUpper() == username.ToUpper())
                                        if (password.ToUpper() == Account.Element("password").Value.ToUpper())
                                            existingAccount = true;

                                if (existingAccount)
                                {
                                    System.Random RandNum = new System.Random();
                                    Player NewPlayer = new Player(username, X, Y, Z, IncomingMessage.SenderConnection, RandNum.Next(0, 255), RandNum.Next(0, 255), RandNum.Next(0, 255));
                                    GameWorldState.Add(IncomingMessage.SenderConnection, NewPlayer);

                                    //Send New Player info to all connections
                                    NetOutgoingMessage outmsg = Server.CreateMessage();
                                    outmsg.Write((byte)Statics.PacketTypes.NEWPLAYER);
                                    outmsg.WriteAllProperties(NewPlayer);
                                    outmsg.Write(NewPlayer.X);
                                    outmsg.Write(NewPlayer.Y);
                                    if (Server.Connections.Count > 0)
                                        Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                    else
                                        Server.SendMessage(outmsg, IncomingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                                    //Send Players and Map to New Player
                                    outmsg = Server.CreateMessage();
                                    outmsg.Write((byte)Statics.PacketTypes.LOGIN);
                                    outmsg.Write(GameWorldState.Count);

                                    foreach (Player p in GameWorldState.Values)
                                    {
                                        outmsg.WriteAllProperties(p);
                                        outmsg.Write(p.X);
                                        outmsg.Write(p.Y);
                                    }

                                    for (int x = 0; x <= Map.MaxX; x++)
                                    {
                                        for (int y = 0; y <= Map.MaxY; y++)
                                        {
                                            if (Map.Tiles[x, y] != null)
                                            {
                                                outmsg.Write((byte)Map.Tiles[x, y].Type);
                                                outmsg.Write(Map.Tiles[x, y].CornerHeights[0]);
                                                outmsg.Write(Map.Tiles[x, y].CornerHeights[1]);
                                                outmsg.Write(Map.Tiles[x, y].CornerHeights[2]);
                                                outmsg.Write(Map.Tiles[x, y].CornerHeights[3]);
                                            }
                                            else
                                            {
                                                outmsg.Write((byte)Statics.TileType.NULL);
                                            }
                                        }
                                    }

                                    Server.SendMessage(outmsg, IncomingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                    Console.WriteLine("Approved new connection for account '" + username + "' and updated the world status");
                                }
                                else
                                {
                                    Console.WriteLine("Disapproved connection for account '" + username + "'");
                                }
                            }

                            break;
                        // Data type is all messages manually sent from client // ( Approval is automated process )
                        case NetIncomingMessageType.Data:
                            Byte PacketType = IncomingMessage.ReadByte();

                            if (PacketType == (byte)Statics.PacketTypes.MOVE)
                            {
                                Player p = GameWorldState[IncomingMessage.SenderConnection];

                                // Read next byte
                                byte b = IncomingMessage.ReadByte();
                                int previousX = p.X;
                                int previousY = p.Y;

                                // Handle movement. This byte should correspond to some direction
                                if ((byte)Statics.MoveDirection.UP == b)
                                { p.Y--; p.X--; }
                                if ((byte)Statics.MoveDirection.DOWN == b)
                                { p.X++; p.Y++; }
                                if ((byte)Statics.MoveDirection.LEFT == b)
                                { p.X--; p.Y++; }
                                if ((byte)Statics.MoveDirection.RIGHT == b)
                                { p.X++; p.Y--; }
                                if ((byte)Statics.MoveDirection.UPLEFT == b)
                                    p.X--;
                                if ((byte)Statics.MoveDirection.UPRIGHT == b)
                                    p.Y--;
                                if ((byte)Statics.MoveDirection.DOWNLEFT == b)
                                    p.Y++;
                                if ((byte)Statics.MoveDirection.DOWNRIGHT == b)
                                    p.X++;

                                int X; int Y;

                                if (p.X < 0)
                                    X = (Map.MaxX + 1) + (p.X);
                                else if (p.X > Map.MaxX)
                                    X = (p.X) - (Map.MaxX + 1);
                                else
                                    X = p.X;

                                if (p.Y < 0)
                                    Y = (Map.MaxY + 1) + (p.Y);
                                else if (p.Y > Map.MaxY)
                                    Y = (p.Y) - (Map.MaxY + 1);
                                else
                                    Y = p.Y;

                                p.X = X;
                                p.Y = Y;

                                p.PreviousZ = p.Z;
                                p.Z = (int)Map.Tiles[p.X, p.Y].AverageZ;

                                //Send position updates to all players
                                NetOutgoingMessage outmsg = Server.CreateMessage();
                                outmsg.Write((byte)Statics.PacketTypes.MOVE);
                                outmsg.Write(p.UserName);
                                outmsg.Write(b);
                                outmsg.Write(p.X);
                                outmsg.Write(p.Y);
                                outmsg.Write(p.Z);
                                Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                break;
                            }
                            else if (PacketType == (byte)Statics.PacketTypes.TEXT)
                            {
                                GameWorldState[IncomingMessage.SenderConnection].Message = IncomingMessage.ReadString();
                                GameWorldState[IncomingMessage.SenderConnection].MessageTime = DateTime.Now;
                            }
                            else if (PacketType == (byte)Statics.PacketTypes.MOBILETARGET)
                            {
                                string targetname = IncomingMessage.ReadString();
                                Player target = GameWorldState.Values.Where(player => player.UserName == targetname).First();
                                GameWorldState[IncomingMessage.SenderConnection].CombatTarget = target;
                                NetOutgoingMessage outmsg = Server.CreateMessage();
                                outmsg.Write((byte)Statics.PacketTypes.MOBILETARGET);
                                outmsg.Write(GameWorldState[IncomingMessage.SenderConnection].UserName);
                                Server.SendMessage(outmsg, target.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            else if (PacketType == (byte)Statics.PacketTypes.MOBILEHIT)
                            {
                                string target = IncomingMessage.ReadString();
                                GameWorldState.Values.Where(player => player.UserName == target).First().HP -= IncomingMessage.ReadInt32();
                                NetOutgoingMessage outmsg = Server.CreateMessage();
                                outmsg.Write((byte)Statics.PacketTypes.MOBILEHIT);
                                outmsg.Write(GameWorldState[IncomingMessage.SenderConnection].UserName);
                                outmsg.Write(target);
                                outmsg.Write(GameWorldState.Values.Where(player => player.UserName == target).First().HP);
                                Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            else if (PacketType == (byte)Statics.PacketTypes.COMBATCHANGE)
                            {
                                if (GameWorldState[IncomingMessage.SenderConnection].Combat)
                                    GameWorldState[IncomingMessage.SenderConnection].Combat = false;
                                else
                                    GameWorldState[IncomingMessage.SenderConnection].Combat = true;

                                NetOutgoingMessage outmsg = Server.CreateMessage();
                                outmsg.Write((byte)Statics.PacketTypes.COMBATCHANGE);
                                outmsg.Write(GameWorldState[IncomingMessage.SenderConnection].UserName);
                                Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            else if (PacketType == (byte)Statics.PacketTypes.COMMAND)
                            {
                                Player p = GameWorldState[IncomingMessage.SenderConnection];
                                Byte CommandType = IncomingMessage.ReadByte();
                                int x = IncomingMessage.ReadInt32();
                                int y = IncomingMessage.ReadInt32();
                                p.PreviousZ = p.Z; p.Z = (int)Map.Tiles[p.X, p.Y].AverageZ;

                                if (CommandType == (byte)Statics.CommandType.DIGCORNER)
                                {
                                    if (Map.Tiles[x, y] != null)
                                    {
                                        Map.Tiles[x, y].CornerHeights[0] += 1;
                                        Map.Tiles[x, y].Type = Statics.TileType.DIRT1;
                                    }

                                    if (Map.Tiles[x, y - 1] != null)
                                    {
                                        Map.Tiles[x, y - 1].CornerHeights[3] += 1;
                                        Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;
                                    }

                                    if (Map.Tiles[x - 1, y] != null)
                                    {
                                        Map.Tiles[x - 1, y].CornerHeights[1] += 1;
                                        Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;
                                    }

                                    if (Map.Tiles[x - 1, y - 1] != null)
                                    {
                                        Map.Tiles[x - 1, y - 1].CornerHeights[2] += 1;
                                        Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;
                                    }
                                }
                                else if (CommandType == (byte)Statics.CommandType.RAISECORNER)
                                {
                                    if (Map.Tiles[x, y] != null)
                                    {
                                        Map.Tiles[x, y].CornerHeights[0] -= 1;
                                        Map.Tiles[x, y].Type = Statics.TileType.DIRT1;
                                    }

                                    if (Map.Tiles[x, y - 1] != null)
                                    {
                                        Map.Tiles[x, y - 1].CornerHeights[3] -= 1;
                                        Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;
                                    }

                                    if (Map.Tiles[x - 1, y] != null)
                                    {
                                        Map.Tiles[x - 1, y].CornerHeights[1] -= 1;
                                        Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;
                                    }

                                    if (Map.Tiles[x - 1, y - 1] != null)
                                    {
                                        Map.Tiles[x - 1, y - 1].CornerHeights[2] -= 1;
                                        Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;
                                    }
                                }
                                else if (CommandType == (byte)Statics.CommandType.DIGTILE)
                                {
                                    Map.Tiles[x, y].CornerHeights[0] += 1;
                                    Map.Tiles[x, y].CornerHeights[1] += 1;
                                    Map.Tiles[x, y].CornerHeights[2] += 1;
                                    Map.Tiles[x, y].CornerHeights[3] += 1;
                                    Map.Tiles[x, y].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x + 1, y + 1].CornerHeights[0] += 1;
                                    Map.Tiles[x + 1, y + 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x + 1, y].CornerHeights[0] += 1;
                                    Map.Tiles[x + 1, y].CornerHeights[3] += 1;
                                    Map.Tiles[x + 1, y].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x + 1, y - 1].CornerHeights[3] += 1;
                                    Map.Tiles[x + 1, y - 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x, y - 1].CornerHeights[2] += 1;
                                    Map.Tiles[x, y - 1].CornerHeights[3] += 1;
                                    Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x - 1, y - 1].CornerHeights[2] += 1;
                                    Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x - 1, y].CornerHeights[1] += 1;
                                    Map.Tiles[x - 1, y].CornerHeights[2] += 1;
                                    Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x - 1, y + 1].CornerHeights[1] += 1;
                                    Map.Tiles[x - 1, y + 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x, y + 1].CornerHeights[0] += 1;
                                    Map.Tiles[x, y + 1].CornerHeights[1] += 1;
                                    Map.Tiles[x, y + 1].Type = Statics.TileType.DIRT1;
                                }
                                else if (CommandType == (byte)Statics.CommandType.RAISETILE)
                                {
                                    Map.Tiles[x, y].CornerHeights[0] -= 1;
                                    Map.Tiles[x, y].CornerHeights[1] -= 1;
                                    Map.Tiles[x, y].CornerHeights[2] -= 1;
                                    Map.Tiles[x, y].CornerHeights[3] -= 1;
                                    Map.Tiles[x, y].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x + 1, y + 1].CornerHeights[0] -= 1;
                                    Map.Tiles[x + 1, y + 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x + 1, y].CornerHeights[0] -= 1;
                                    Map.Tiles[x + 1, y].CornerHeights[3] -= 1;
                                    Map.Tiles[x + 1, y].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x + 1, y - 1].CornerHeights[3] -= 1;
                                    Map.Tiles[x + 1, y - 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x, y - 1].CornerHeights[2] -= 1;
                                    Map.Tiles[x, y - 1].CornerHeights[3] -= 1;
                                    Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x - 1, y - 1].CornerHeights[2] -= 1;
                                    Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x - 1, y].CornerHeights[1] -= 1;
                                    Map.Tiles[x - 1, y].CornerHeights[2] -= 1;
                                    Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x - 1, y + 1].CornerHeights[1] -= 1;
                                    Map.Tiles[x - 1, y + 1].Type = Statics.TileType.DIRT1;

                                    Map.Tiles[x, y + 1].CornerHeights[0] -= 1;
                                    Map.Tiles[x, y + 1].CornerHeights[1] -= 1;
                                    Map.Tiles[x, y + 1].Type = Statics.TileType.DIRT1;
                                }

                                //Send map update to all players
                                NetOutgoingMessage outmsg = Server.CreateMessage();
                                outmsg.Write((byte)Statics.PacketTypes.COMMAND);
                                outmsg.Write(CommandType);
                                outmsg.Write(x);
                                outmsg.Write(y);
                                outmsg.Write(p.Z);
                                outmsg.Write(p.UserName);
                                Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            // In case status changed it can be one of these:
                            // NetConnectionStatus.Connected;
                            // NetConnectionStatus.Connecting;
                            // NetConnectionStatus.Disconnected;
                            // NetConnectionStatus.Disconnecting;
                            // NetConnectionStatus.None;
                            // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                            Console.WriteLine(IncomingMessage.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)IncomingMessage.SenderConnection.Status);
                            if (IncomingMessage.SenderConnection.Status == NetConnectionStatus.Disconnected || IncomingMessage.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                // Find disconnected character and remove it
                                GameWorldState.Remove(IncomingMessage.SenderConnection);
                            }
                            break;
                        default:
                            // As i statet previously, theres few other kind of messages also, but i dont cover those in this example
                            // Uncommenting next line, informs you, when ever some other kind of message is received
                            Console.WriteLine("Unhandled Message of Type " + IncomingMessage.MessageType +": " + IncomingMessage.ToString());
                            break;
                    }
                }

                if ((Time + TimeToPass) < DateTime.Now)
                {
                    if (Server.ConnectionsCount != 0)
                    {
                        NetOutgoingMessage outmsg = Server.CreateMessage();
                        outmsg.Write((byte)Statics.PacketTypes.WORLDSTATE);
                        outmsg.Write(GameWorldState.Count);

                        foreach (Player p in GameWorldState.Values)
                        {
                            outmsg.Write(p.UserName);
                            outmsg.Write(p.Message);

                            // Checks Message Timers
                            if ((p.MessageTime + p.MessageTimeToPass) < DateTime.Now && p.Message != "")
                            {
                                p.Message = "";
                            }
                        }

                        Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                    }

                    Time = DateTime.Now;
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isClosing = true;
                    Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    isClosing = true;
                    Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isClosing = true;
                    Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isClosing = true;
                    Console.WriteLine("User is logging off!");
                    break;
            }
            return true;
        }

        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }
}
