using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Data.Items;
using Data.Graphics;

namespace Data.World
{
    public class MapObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Object Object { get; set; }
        public Vector2 Position { get; set; }
        public bool MouseOver = false;

        public MapObject() { }

        public MapObject(Object Object, int X, int Y, Map Map) 
        {
            this.Object = Object;
            this.X = X;
            this.Y = Y;
            this.Position = new Vector2(Map.Tiles[X, Y].Corners[3].X, Map.Tiles[X, Y].Corners[0].Y) + new Vector2(0, -144);
        }

        public void Update(Map Map)
        {
            MouseOver = false;
            this.Position = new Vector2(Map.Tiles[X, Y].Corners[3].X, Map.Tiles[X, Y].Corners[0].Y) + new Vector2(0, -144);
        }

        public void Draw(GraphicsDeviceManager GraphicsDeviceManager, Vector2 AnimationOffset)
        {
            Color Color;
            if (MouseOver)
                Color = Color.Yellow;
            else
                Color = Color.White;

            if (Object.GetType().BaseType == typeof(Item))
            {
                Item Item = (Item)Object;
                VertexPositionColorTexture[] verts;
                verts = new VertexPositionColorTexture[6];
                verts[0].Position = new Vector3(Position.X + AnimationOffset.X, Position.Y + AnimationOffset.Y, 0);
                verts[0].TextureCoordinate = new Vector2(0, 0) * new Vector2((1f / Item.Texture.Width), (1f / (Item.Texture.Height)));
                verts[0].Color = Color;
                verts[1].Position = new Vector3(Position.X + AnimationOffset.X + Item.Texture.Width, Position.Y + AnimationOffset.Y, 0);
                verts[1].TextureCoordinate = new Vector2(Item.Texture.Width, 0) * new Vector2((1f / Item.Texture.Width), (1f / (Item.Texture.Height)));
                verts[1].Color = Color;
                verts[2].Position = new Vector3(Position.X + AnimationOffset.X, Position.Y + AnimationOffset.Y + Item.Texture.Height, 0);
                verts[2].TextureCoordinate = new Vector2(0, Item.Texture.Height) * new Vector2((1f / Item.Texture.Width), (1f / (Item.Texture.Height)));
                verts[2].Color = Color;
                verts[3].Position = new Vector3(Position.X + AnimationOffset.X + Item.Texture.Width, Position.Y + AnimationOffset.Y, 0);
                verts[3].TextureCoordinate = new Vector2(Item.Texture.Width, 0) * new Vector2((1f / Item.Texture.Width), (1f / (Item.Texture.Height)));
                verts[3].Color = Color;
                verts[4].Position = new Vector3(Position.X + AnimationOffset.X + Item.Texture.Width, Position.Y + AnimationOffset.Y + Item.Texture.Height, 0);
                verts[4].TextureCoordinate = new Vector2(Item.Texture.Width, Item.Texture.Height) * new Vector2((1f / Item.Texture.Width), (1f / (Item.Texture.Height)));
                verts[4].Color = Color;
                verts[5].Position = new Vector3(Position.X + AnimationOffset.X, Position.Y + AnimationOffset.Y + Item.Texture.Height, 0);
                verts[5].TextureCoordinate = new Vector2(0, Item.Texture.Height) * new Vector2((1f / Item.Texture.Width), (1f / (Item.Texture.Height)));
                verts[5].Color = Color;
                DrawBatch.Add(verts, new DrawData(Item.Texture, PrimitiveType.TriangleList, 2));
            }
        }

        public bool pointInPolygon(Point p)
        {
            bool inside = false;

            if (Object.GetType().BaseType == typeof(Item))
            {
                Item Item = (Item)Object;
                // Taken from http://social.msdn.microsoft.com/forums/en-US/winforms/thread/95055cdc-60f8-4c22-8270-ab5f9870270a/
                Point p1, p2;
                if (Item.Polygon.Length < 3)
                {
                    return inside;
                }
                Point oldPoint = new Point((int)Position.X + Item.Polygon[Item.Polygon.Length - 1].X, (int)Position.Y + Item.Polygon[Item.Polygon.Length - 1].Y);

                for (int i = 0; i < Item.Polygon.Length; i++)
                {
                    Point newPoint = new Point((int)Position.X + Item.Polygon[i].X, (int)Position.Y + Item.Polygon[i].Y);
                    if (newPoint.X > oldPoint.X)
                    {
                        p1 = oldPoint;
                        p2 = newPoint;
                    }
                    else
                    {
                        p1 = newPoint;
                        p2 = oldPoint;
                    }

                    if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                        && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)
                        < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X))
                    {
                        inside = !inside;
                    }
                    oldPoint = newPoint;
                }
            }

            return inside;
        }
    }
}
