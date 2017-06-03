using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Data
{
    public static class Statics
    {
        public static Texture2D blank;
        public static Texture2D grassTexture;
        public static Texture2D dirtTexture;
        public enum MoveDirection { UP, UPLEFT, UPRIGHT, DOWN, DOWNLEFT, DOWNRIGHT, LEFT, RIGHT, NONE }
        public enum PacketTypes { LOGIN, NEWPLAYER, MOVE, WORLDSTATE, MAP, TEXT, COMMAND, MOBILETARGET, MOBILEHIT, COMBATCHANGE }
        public enum CommandType { INCREASE, DECREASE, GET, SET, DIGCORNER, DIGTILE, RAISECORNER, RAISETILE, PLANT }
        public enum TileType { NULL, GRASS1, GRASS2, GRASS3, DIRT1, WATER, ROCK, BLANK }
        public enum TargetType { TILE, CORNER, PLAYER }
        public enum Direction { SOUTH, NORTH, WEST, EAST, NORTHWEST, SOUTHWEST, NORTHEAST, SOUTHEAST }
        public enum ScreenDirection { NORTHWEST, WESTSOUTH, SOUTHEAST, EASTNORTH }
        public enum StratumControlType { TEXTBOX, LABEL, BUTTON, IMAGE }
        public enum AnimationCycle { NONE, WALK, RUN, HIT, SHOOT, CAST, GETHIT, DIE }
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            return path;
        }
        public static int GetRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}
