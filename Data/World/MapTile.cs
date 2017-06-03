using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Data.Graphics.Shapes;
using Data.Graphics;

namespace Data.World
{
    public class MapTile
    {
        public static Texture2D GrassTexture;
        public static Texture2D DirtTexture;
        public static Texture2D WaterTexture;
        public static int Width = 96;
        public static int Height = 48;
        //public static int Height = 48;
        public static int Border = 1;
        public Texture2D Texture;
        public int ArrayX;
        public int ArrayY;
        public double AverageZ;
        public Vector2 Center;
        public Point[] Corners = new Point[4];
        public int[] CornerHeights = new int[4];
        public float DistanceToMouse;
        public Statics.TileType Type;
        public bool MouseOver = false;


        public MapTile(int[] CornerHeights, int ArrayX, int ArrayY) 
        {
            this.CornerHeights = CornerHeights;
            this.ArrayX = ArrayX;
            this.ArrayY = ArrayY;
            this.Type = Statics.TileType.GRASS1;
            this.Texture = GrassTexture;
        }

        public MapTile(int[] CornerHeights, int ArrayX, int ArrayY, Statics.TileType Type)
        {
            this.CornerHeights = CornerHeights;
            this.ArrayX = ArrayX;
            this.ArrayY = ArrayY;
            this.Type = Type;

            switch (Type)
            {
                case Statics.TileType.GRASS1:
                    this.Texture = GrassTexture;
                    break;
                case Statics.TileType.DIRT1:
                    this.Texture = DirtTexture;
                    break;
                case Statics.TileType.WATER:
                    this.Texture = WaterTexture;
                    break;
            }
        }

        public float Distance(Vector2 Origin)
        {
            float Distance = Vector2.Distance(Origin, new Vector2(Corners[0].X, Corners[0].Y));
            return DistanceToMouse;
        }

        public void Update(int x, int y, GraphicsDeviceManager graphics, MouseState mouseState)
        {
            switch (Type)
            {
                case Statics.TileType.GRASS1:
                    this.Texture = GrassTexture;
                    break;
                case Statics.TileType.DIRT1:
                    this.Texture = DirtTexture;
                    break;
                case Statics.TileType.WATER:
                    this.Texture = WaterTexture;
                    break;
            }

            Center = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2) + new Vector2(-Width / 2 * y, Height / 2 * y) + new Vector2(Width / 2 * x, Height / 2 * x);
            Corners[0] = new Point((int)Center.X, ((int)Center.Y + CornerHeights[0]) - (Height / 2));
            Corners[1] = new Point((int)Center.X + (Width / 2), (int)Center.Y + CornerHeights[1]);
            Corners[2] = new Point((int)Center.X, ((int)Center.Y + CornerHeights[2]) + (Height / 2));
            Corners[3] = new Point((int)Center.X - (Width / 2), (int)Center.Y + CornerHeights[3]);
            AverageZ = -(CornerHeights.Max() + CornerHeights.Min()) / 2;
            DistanceToMouse = Vector2.Distance(new Vector2(mouseState.X, mouseState.Y), new Vector2(Corners[0].X, Corners[0].Y));
            MouseOver = false;
        }

        public void Draw(GraphicsDevice graphicsDevice, Vector2 AnimationOffset)
        {
            Color Color;
            if (MouseOver)
                Color = Color.Yellow;
            else
                Color = Color.White;

            //Draw Polygon Outlines
            //Polygon poly = new Polygon(new Vector2[4] { new Vector2(Corners[0].X, Corners[0].Y), new Vector2(Corners[1].X, Corners[1].Y), new Vector2(Corners[2].X, Corners[2].Y), new Vector2(Corners[3].X, Corners[3].Y)});
            //poly.Draw(spriteBatch, graphicsDevice, Color.Black);

            //Drawing Primitives Lines - 3D
            //VertexPositionColor[] verts;
            //verts = new VertexPositionColor[84];
            //verts[0].Position = new Vector3(Corners[0].X, Corners[0].Y, 0);
            //verts[0].Color = Color.White;
            //verts[1].Position = new Vector3(Corners[1].X, Corners[1].Y, 0);
            //verts[1].Color = Color.White;

            //verts[2].Position = new Vector3(Corners[1].X, Corners[1].Y, 0);
            //verts[2].Color = Color.White;
            //verts[3].Position = new Vector3(Corners[2].X, Corners[2].Y, 0);
            //verts[3].Color = Color.White;

            //verts[4].Position = new Vector3(Corners[2].X, Corners[2].Y, 0);
            //verts[4].Color = Color.White;
            //verts[5].Position = new Vector3(Corners[3].X, Corners[3].Y, 0);
            //verts[5].Color = Color.White;

            //verts[6].Position = new Vector3(Corners[3].X, Corners[3].Y, 0);
            //verts[6].Color = Color.White;
            //verts[7].Position = new Vector3(Corners[0].X, Corners[0].Y, 0);
            //verts[7].Color = Color.White;

            //basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            //basicEffect.TextureEnabled = false;
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, verts, 0, 4);

            //VertexPositionColor[] verts2;
            //verts2 = new VertexPositionColor[84];
            //verts2[0].Position = new Vector3(Corners[0].X + 1, Corners[0].Y + 1, 0);
            //verts2[0].Color = Color.White;
            //verts2[1].Position = new Vector3(Corners[1].X - 1, Corners[1].Y, 0);
            //verts2[1].Color = Color.White;

            //verts2[2].Position = new Vector3(Corners[1].X - 1, Corners[1].Y, 0);
            //verts2[2].Color = Color.White;
            //verts2[3].Position = new Vector3(Corners[2].X - 1, Corners[2].Y - 1, 0);
            //verts2[3].Color = Color.White;

            //verts2[4].Position = new Vector3(Corners[2].X - 1, Corners[2].Y - 1, 0);
            //verts2[4].Color = Color.White;
            //verts2[5].Position = new Vector3(Corners[3].X + 1, Corners[3].Y, 0);
            //verts2[5].Color = Color.White;

            //verts2[6].Position = new Vector3(Corners[3].X + 1, Corners[3].Y, 0);
            //verts2[6].Color = Color.White;
            //verts2[7].Position = new Vector3(Corners[0].X + 1, Corners[0].Y + 1, 0);
            //verts2[7].Color = Color.White;

            //basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            //basicEffect.TextureEnabled = false;
            //basicEffect.CurrentTechnique.Passes[0].Apply();
            //graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, verts2, 0, 4);


            //Fill Polygon With Texture (Primitive Triangles - 3D)
            VertexPositionColorTexture[] verts;
            verts = new VertexPositionColorTexture[6];
            verts[0].Position = new Vector3(Corners[3].X + AnimationOffset.X, Corners[3].Y + AnimationOffset.Y, 0);
            verts[0].TextureCoordinate = new Vector2(0, 256 + CornerHeights[3]) * new Vector2((1f / Texture.Width), (1f / (Texture.Height - 48)));
            verts[0].Color = Color;
            verts[1].Position = new Vector3(Corners[0].X + AnimationOffset.X, Corners[0].Y + AnimationOffset.Y + Border, 0);
            verts[1].TextureCoordinate = new Vector2(48, 232 + CornerHeights[0]) * new Vector2((1f / 96), (1f / (Texture.Height - 48)));
            verts[1].Color = Color;
            verts[2].Position = new Vector3(Corners[2].X + AnimationOffset.X, Corners[2].Y + AnimationOffset.Y - Border, 0);
            verts[2].TextureCoordinate = new Vector2(48, 280 + CornerHeights[2]) * new Vector2((1f / 96), (1f / (Texture.Height - 48)));
            verts[2].Color = Color;
            verts[3].Position = new Vector3(Corners[0].X + AnimationOffset.X, Corners[0].Y + AnimationOffset.Y + Border, 0);
            verts[3].TextureCoordinate = new Vector2(48, 232 + CornerHeights[0]) * new Vector2((1f / 96), (1f / (Texture.Height - 48)));
            verts[3].Color = Color;
            verts[4].Position = new Vector3(Corners[1].X + AnimationOffset.X, Corners[1].Y + AnimationOffset.Y, 0);
            verts[4].TextureCoordinate = new Vector2(96, 256 + CornerHeights[1]) * new Vector2((1f / 96), (1f / (Texture.Height - 48)));
            verts[4].Color = Color;
            verts[5].Position = new Vector3(Corners[2].X + AnimationOffset.X, Corners[2].Y + AnimationOffset.Y - Border, 0);
            verts[5].TextureCoordinate = new Vector2(48, 280 + CornerHeights[2]) * new Vector2((1f / 96), (1f / (Texture.Height - 48)));
            verts[5].Color = Color;
            DrawBatch.Add(verts, new DrawData(Texture, PrimitiveType.TriangleList, 2));
        }

        public bool pointInTile(Point p)
        {
            // Taken from http://social.msdn.microsoft.com/forums/en-US/winforms/thread/95055cdc-60f8-4c22-8270-ab5f9870270a/
            Point p1, p2;
            bool inside = false;
            if (Corners.Length < 3)
            {
                return inside;
            }
            Point oldPoint = new Point(Corners[Corners.Length - 1].X, Corners[Corners.Length - 1].Y);

            for (int i = 0; i < Corners.Length; i++)
            {
                Point newPoint = new Point(Corners[i].X, Corners[i].Y);
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
            return inside;
        }
    }
}
