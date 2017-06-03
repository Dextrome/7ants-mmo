using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Data.Items
{
    public class Item
    {
        //public string Texture { get; set; }
        public Texture2D Texture;
        public string Name { get; set; }
        public Color Color { get; set; }
        public string CorrespondingTile { get; set; }
        public Point[] Polygon { get; set; }
    }
}
