using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Data.Items;

namespace Data.GUI
{
    public class InventoryItem
    {
        public Item Item;
        public int Amount;
        public bool HighLight;
        public int ArrayX;
        public int ArrayY;

        public InventoryItem(Item Item, int Amount, int ArrayX, int ArrayY)
        {
            this.Item = Item;
            this.Amount = Amount;
            this.ArrayX = ArrayX;
            this.ArrayY = ArrayY;
        }

        public static Rectangle GetScreenRectangle(int x, int y)
        {
            if (x <= Inventory.ColumnsShown - 1)
            {
                return new Rectangle((int)Inventory.Position.X + 73 + (32 * x), (int)Inventory.Position.Y + 80 + (32 * y), 32, 32);
            }
            else
            {
                return new Rectangle(0,0,0,0);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheetTexture, Rectangle SourceRectangle, Rectangle HighLightRectangle, int x, int y, SpriteFont InventoryItemFont)
        {
            spriteBatch.Draw(spriteSheetTexture, new Vector2(Inventory.Position.X + 73 + (32 * x), Inventory.Position.Y + 80 + (32 * y)), SourceRectangle, 
                             Item.Color, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

            if (Amount < 10)
                spriteBatch.DrawString(InventoryItemFont, Amount.ToString(), new Vector2(Inventory.Position.X + 73 + (32 * x) + 23, Inventory.Position.Y + 80 + (32 * y) + 18), Color.White);
            else
                spriteBatch.DrawString(InventoryItemFont, Amount.ToString(), new Vector2(Inventory.Position.X + 73 + (32 * x) + 18, Inventory.Position.Y + 80 + (32 * y) + 18), Color.White);

            if (HighLight)
                spriteBatch.Draw(spriteSheetTexture, new Vector2(Inventory.Position.X + 73 + (32 * x), Inventory.Position.Y + 80 + (32 * y)), HighLightRectangle, 
                             Color.White * 0.1f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheetTexture, Texture2D Texture, Rectangle HighLightRectangle, int x, int y, SpriteFont InventoryItemFont)
        {
            spriteBatch.Draw(Texture, new Vector2(Inventory.Position.X + 65 + (32 * x), Inventory.Position.Y + 10 + (32 * y)), new Rectangle(0, 0, Texture.Width, Texture.Height),
                             Item.Color, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

            if (Amount < 10)
                spriteBatch.DrawString(InventoryItemFont, Amount.ToString(), new Vector2(Inventory.Position.X + 73 + (32 * x) + 23, Inventory.Position.Y + 80 + (32 * y) + 18), Color.White);
            else
                spriteBatch.DrawString(InventoryItemFont, Amount.ToString(), new Vector2(Inventory.Position.X + 73 + (32 * x) + 18, Inventory.Position.Y + 80 + (32 * y) + 18), Color.White);

            if (HighLight)
                spriteBatch.Draw(spriteSheetTexture, new Vector2(Inventory.Position.X + 73 + (32 * x), Inventory.Position.Y + 80 + (32 * y)), HighLightRectangle,
                             Color.White * 0.1f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }


        //public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheetTexture, Texture2D Texture, Rectangle HighLightRectangle, int x, int y, SpriteFont InventoryItemFont)
        //{
        //    spriteBatch.Draw(Texture, new Vector2(Inventory.Position.X + 73 + (32 * x), Inventory.Position.Y + 80 + (32 * y)), new Rectangle(0, 0, Texture.Width, Texture.Height),
        //                     Item.Color, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

        //    if (Amount < 10)
        //        spriteBatch.DrawString(InventoryItemFont, Amount.ToString(), new Vector2(Inventory.Position.X + 73 + (32 * x) + 23, Inventory.Position.Y + 80 + (32 * y) + 18), Color.White);
        //    else
        //        spriteBatch.DrawString(InventoryItemFont, Amount.ToString(), new Vector2(Inventory.Position.X + 73 + (32 * x) + 18, Inventory.Position.Y + 80 + (32 * y) + 18), Color.White);

        //    if (HighLight)
        //        spriteBatch.Draw(spriteSheetTexture, new Vector2(Inventory.Position.X + 73 + (32 * x), Inventory.Position.Y + 80 + (32 * y)), HighLightRectangle,
        //                     Color.White * 0.1f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        //}
    }
}
