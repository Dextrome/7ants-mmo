using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Data.Graphics
{
    public class DrawData
    {
        public Texture2D Texture { get; set; }
        public PrimitiveType PrimitiveType { get; set; }
        public int PrimitiveCount { get; set; }

        public DrawData() { }
        public DrawData(Texture2D Texture, PrimitiveType PrimitiveType, int PrimitiveCount)
        {
            this.Texture = Texture;
            this.PrimitiveCount = PrimitiveCount;
            this.PrimitiveType = PrimitiveType;
        }
    }
}
