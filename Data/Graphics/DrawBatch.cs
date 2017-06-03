using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Data.Graphics
{
    public static class DrawBatch
    {
        public static Dictionary<VertexPositionColorTexture[], DrawData> Queue = new Dictionary<VertexPositionColorTexture[], DrawData>();
        public static BasicEffect Effect;
        public static ContentManager Content;
        public static GraphicsDevice GraphicsDevice;

        //public DrawBatch(BasicEffect Effect, ContentManager Content, GraphicsDevice GraphicsDevice)
        //{
        //    this.Effect = Effect;
        //    this.Content = Content;
        //    this.GraphicsDevice = GraphicsDevice;
        //}

        public static void Add(VertexPositionColorTexture[] Vertex, DrawData DrawData)
        {
            Queue.Add(Vertex, DrawData);
        }

        public static void Draw()
        {
            Texture2D Texture;
            VertexPositionColorTexture[] Vertex;
            int PrimitiveCount;
            PrimitiveType PrimitiveType;

            foreach (KeyValuePair<VertexPositionColorTexture[], DrawData> KeyValuePair in Queue)
            {
                Vertex = KeyValuePair.Key;
                Texture = KeyValuePair.Value.Texture;
                PrimitiveCount = KeyValuePair.Value.PrimitiveCount;
                PrimitiveType = KeyValuePair.Value.PrimitiveType;
                // projection uses CreateOrthographicOffCenter to create 2d projection matrix with 0,0 in the upper left.
                Effect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
                Effect.TextureEnabled = true;
                Effect.Texture = Texture;
                Effect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType, Vertex, 0, PrimitiveCount);
            }

            Queue.Clear();
        }
    }
}
