using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Data.Items
{
    public class Shovel: Item
    {
        public Shovel(ContentManager Content)
        {
            this.Name = "Shovel";
            this.Texture = Content.Load<Texture2D>("ShovelItem");
            this.Color = Color.White;
            this.Polygon = new Point[4];
            this.Polygon[0] = new Point(18, 162);
            this.Polygon[1] = new Point(73, 162);
            this.Polygon[2] = new Point(73, 192);
            this.Polygon[3] = new Point(18, 192);
        }
    }
}
