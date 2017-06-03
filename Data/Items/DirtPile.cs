using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Data.Items
{
    public class DirtPile: Item
    {
        public DirtPile(ContentManager Content)
        {
            this.Name = "Pile of dirt";
            this.Texture = Content.Load<Texture2D>("DirtPile");
            this.Color = Color.White;
            this.CorrespondingTile = "Dirt";
            this.Polygon = new Point[4];
            this.Polygon[0] = new Point(26, 146);
            this.Polygon[1] = new Point(70, 146);
            this.Polygon[2] = new Point(70, 181);
            this.Polygon[3] = new Point(26, 181);
        }
    }
}
