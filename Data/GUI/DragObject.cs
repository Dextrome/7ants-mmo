using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Data;
using Data.World;
using Data.Items;
using Data.GUI.Stratums;

namespace Data.GUI
{
    public class DragObject
    {
        public Object Object { get; set; }
        public Vector2 DrawOffSet { get; set; }
        public bool NoOrigin = false;

        public DragObject(){}

        public DragObject(Object Object, Vector2 DrawOffSet)
        {
            this.Object = Object;
            this.DrawOffSet = DrawOffSet;
        }

        public DragObject(Object Object, Vector2 DrawOffset, bool NoOrigin)
        {
            this.Object = Object;
            this.DrawOffSet = DrawOffSet;
            this.NoOrigin = NoOrigin;
        }


        public void Draw(GraphicsDevice GraphicsDevice, SpriteBatch SpriteBatch, MouseState MouseState)
        {
            Texture2D Texture = new Texture2D(GraphicsDevice, 1, 1);

            if (Object.GetType() == typeof(MapObject))
            {
                MapObject MapObject = (MapObject)Object;
                if (MapObject.Object.GetType().BaseType == typeof(Item))
                {
                    Item Item = (Item)MapObject.Object;
                    Texture = Item.Texture;
                }

            }
            else if (Object.GetType() == typeof(InventoryItem))
            {
                InventoryItem InventoryItem = (InventoryItem)Object;
                Texture = InventoryItem.Item.Texture;
            }
            else if (Object.GetType().BaseType == typeof(Stratum))
            {
                Stratum Stratum = (Stratum)Object;
                Texture = Stratum.BackgroundTexture;
            }
            else
            {
                Item i = (Item)Object;
                Texture = i.Texture;
            }

            if (Object.GetType().BaseType == typeof(Stratum))
            {
                Stratum Stratum = (Stratum)Object;
                SpriteBatch.Draw(Texture, new Vector2(MouseState.X, MouseState.Y) - DrawOffSet, new Rectangle(0, 0, Stratum.Width, Stratum.Height), Color.Yellow * 0.6f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
            else
            {
                SpriteBatch.Draw(Texture, new Vector2(MouseState.X, MouseState.Y) - DrawOffSet, new Rectangle(0, 0, Texture.Width, Texture.Height), Color.Yellow * 0.6f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
