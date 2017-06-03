using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Data.Graphics;
using Data.World;


namespace Data.Mobiles
{

    public class Mobile
    {
        public Dictionary<string, Items.Item> Equipment = new Dictionary<string, Items.Item>();
        public int HP = 100;
        public int Mana = 100;
        public int Stamina = 100;
        public bool Combat = false;
        public Mobile CombatTarget;
        public int X { get; set; }
        public int PreviousX { get; set; }
        public int Y { get; set; }
        public int PreviousY { get; set; }
        public int Z { get; set; }
        public int PreviousZ { get; set; }
        public string Name { get; set; }
        public Texture2D Texture;
        public int colorR { get; set; }
        public int colorG { get; set; }
        public int colorB { get; set; }
        public int AnimationStep = 0;
        public Vector2 Position { get; set; }
        public Vector2 AnimationPosition = new Vector2(0, 0);
        public Vector2 AnimationOffset = new Vector2(0, 0);
        public string Animation = "";
        public Statics.AnimationCycle AnimationCycle = Statics.AnimationCycle.NONE;
        public Statics.Direction Direction = Statics.Direction.SOUTH;
    }
}
