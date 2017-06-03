using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Data.Items;
using Data.World;
using Data.Mobiles;

namespace Data
{
    public class Command
    {
        public bool Target;
        public Statics.TargetType TargetType;
        public Statics.CommandType Type;
        public Item InputItem;
        string Text;

        public Command(string command)
        {
            DetermineCommand(command, null);
        }

        public Command(string command, Item Item)
        {
            DetermineCommand(command, Item);
        }

        private void DetermineCommand(string command, Item item)
        {
            if (command.Length > 3)
            {
                Text = command;
                switch (command.Substring(0, 4))
                {
                    case "incr":
                        Type = Statics.CommandType.INCREASE;
                        Target = true;
                        break;
                    case "decr":
                        Type = Statics.CommandType.DECREASE;
                        Target = true;
                        break;
                    case "get ":
                        Type = Statics.CommandType.GET;
                        Target = true;
                        break;
                    case "set ":
                        Type = Statics.CommandType.SET;
                        Target = true;
                        break;
                    case "digc":
                        Type = Statics.CommandType.DIGCORNER;
                        Target = true;
                        TargetType = Statics.TargetType.CORNER;
                        break;
                    case "digt":
                        Type = Statics.CommandType.DIGTILE;
                        Target = true;
                        TargetType = Statics.TargetType.TILE;
                        break;
                    case "raic":
                        Type = Statics.CommandType.RAISECORNER;
                        InputItem = item;
                        Target = true;
                        TargetType = Statics.TargetType.CORNER;
                        break;
                    case "rait":
                        Type = Statics.CommandType.RAISETILE;
                        InputItem = item;
                        Target = true;
                        TargetType = Statics.TargetType.TILE;
                        break;
                    case "plan":
                        Type = Statics.CommandType.PLANT;
                        Target = true;
                        TargetType = Statics.TargetType.TILE;
                        break;
                    default:
                        break;
                }
            }
        }

        //public WorldMap SendTarget(int x, int y, int z, NetClient Client, WorldMap map, Player player)
        public void SendTarget(int x, int y, NetClient Client, Map map, Player player)
        {
            NetOutgoingMessage outmsg = Client.CreateMessage();
            //WorldMap Map = map;
            Map Map = map;
            switch (Type)
            {
                case Statics.CommandType.INCREASE:
                    break;
                case Statics.CommandType.DECREASE:
                    break;
                case Statics.CommandType.GET:
                    break;
                case Statics.CommandType.SET:
                    break;
                case Statics.CommandType.DIGCORNER:
                    outmsg.Write((byte)Statics.PacketTypes.COMMAND);
                    outmsg.Write((byte)Statics.CommandType.DIGCORNER);
                    outmsg.Write(x);
                    outmsg.Write(y);
                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    break;
                case Statics.CommandType.RAISECORNER:
                    outmsg.Write((byte)Statics.PacketTypes.COMMAND);
                    outmsg.Write((byte)Statics.CommandType.RAISECORNER);
                    outmsg.Write(x);
                    outmsg.Write(y);
                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    break;
                case Statics.CommandType.DIGTILE:
                    outmsg.Write((byte)Statics.PacketTypes.COMMAND);
                    outmsg.Write((byte)Statics.CommandType.DIGTILE);
                    outmsg.Write(x);
                    outmsg.Write(y);

                    if (Map.Tiles[x, y] != null)
                        if (Map.Tiles[x, y].CornerHeights[0] >= 200 || Map.Tiles[x, y].CornerHeights[1] >= 200 || Map.Tiles[x, y].CornerHeights[2] >= 200 || Map.Tiles[x, y].CornerHeights[3] >= 200)
                            break;

                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    break;
                case Statics.CommandType.RAISETILE:
                    outmsg.Write((byte)Statics.PacketTypes.COMMAND);
                    outmsg.Write((byte)Statics.CommandType.RAISETILE);
                    outmsg.Write(x);
                    outmsg.Write(y);

                    if (Map.Tiles[x, y] != null)
                        if (Map.Tiles[x, y].CornerHeights[0] <= -225 || Map.Tiles[x, y].CornerHeights[1] <= -225 || Map.Tiles[x, y].CornerHeights[2] <= -225 || Map.Tiles[x, y].CornerHeights[3] <= -225)
                            break;
 
                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    break;
                case Statics.CommandType.PLANT:
                    //outmsg.Write((byte)Statics.PacketTypes.COMMAND);
                    //outmsg.Write((byte)Statics.CommandType.PLANT);
                    //outmsg.Write(x);
                    //outmsg.Write(y);
                    //Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    //Map.TileArray[x, y, Map.getHighestTile(x, y, z)].Type = Statics.TileType.GRASS1;
                    //Map.TileArray[x, y, Map.getHighestTile(x, y, z)].Corners = "TEMP";
                    break;
            }
        }
    }
}
