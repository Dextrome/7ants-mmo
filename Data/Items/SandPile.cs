using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Data.Items
{
    public class SandPile: Item
    {
        public SandPile(ContentManager Content)
        {
            this.Name = "Pile of sand";
            this.Texture = Content.Load<Texture2D>("BlankPile");
            this.Color = new Color(240, 224, 118);
            this.CorrespondingTile = "Sand";
            this.Polygon = new Point[4];
            this.Polygon[0] = new Point(26, 146);
            this.Polygon[1] = new Point(70, 146);
            this.Polygon[2] = new Point(70, 181);
            this.Polygon[3] = new Point(26, 181);
        }
    }
}
