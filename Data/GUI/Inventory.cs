using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Data.Items;

namespace Data.GUI
{
    public class Inventory
    {
        public static int ColumnsShown = 1;
        public Rectangle ExpandButtonRectangle = new Rectangle();
        public bool ExpandButtonHighlight = false;
        public static Vector2 Position;
        public InventoryItem[,] InventoryItems = new InventoryItem[4, 10];        

        public Inventory(ContentManager Content)
        {
            InventoryItems[0, 0] = new InventoryItem(new Shovel(Content), 1, 0, 0);
            InventoryItems[0, 1] = new InventoryItem(new DirtPile(Content), 5, 0, 1);
            InventoryItems[0, 2] = new InventoryItem(new SandPile(Content), 55, 0, 2);
        }

        public void Update(MouseState MouseState)
        {
            if (ExpandButtonRectangle.Contains(new Point(MouseState.X, MouseState.Y)))
                ExpandButtonHighlight = true;
            else
                ExpandButtonHighlight = false;

            for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (InventoryItems[x, y] != null)
                        if (InventoryItem.GetScreenRectangle(x, y).Contains(new Point(MouseState.X, MouseState.Y)))
                            InventoryItems[x, y].HighLight = true;
                        else
                            InventoryItems[x, y].HighLight = false;
                }
            }
        }

        public void UpdatePosition(GraphicsDeviceManager graphics)
        {        
            Position = new Vector2(graphics.PreferredBackBufferWidth - 200 + (32 * (4 - ColumnsShown)), graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 10) - 400);
            ExpandButtonRectangle = new Rectangle((int)Position.X + 28, (int)Position.Y + 206, 22, 66);
        }

        public void Draw(Texture2D SpriteSheetTexture, SpriteBatch spriteBatch, Rectangle SourceRectangle, Rectangle SourceRectangleHighlight)
        {
            spriteBatch.Draw(SpriteSheetTexture, Position, SourceRectangle, Color.White);

            if (ExpandButtonHighlight)
                spriteBatch.Draw(SpriteSheetTexture, Position, SourceRectangleHighlight, Color.White * 0.1f);
        }
    }
}
