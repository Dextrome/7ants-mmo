using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Data.Graphics.Shapes
{
    public class Polygon
    {
        public Vector2[] Vertices;

        public Polygon(Vector2[] Vertices)
        {
            this.Vertices = Vertices;
        }

        public void Draw(SpriteBatch batch, GraphicsDevice graphicsDevice, Color color)
        {
            for (int i = 0; i < Vertices.Count() - 1; i++)
            {
                DrawLine(batch, graphicsDevice, 1, color, Vertices[i], Vertices[i + 1]);
            }

            DrawLine(batch, graphicsDevice, 1, color, Vertices[Vertices.Count() - 1], Vertices[0]);
        }

        void DrawLine(SpriteBatch batch, GraphicsDevice graphicsDevice, float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);
            batch.Draw(Statics.blank, point1, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0f);
        }
    }
}
