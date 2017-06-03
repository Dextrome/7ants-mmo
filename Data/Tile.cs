using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _7ANTSMMO.Data
{
    public class Tile
    {
        static public int TileWidth = 96;
        static public int TileHeight = 96;
        static public int TileDepth = 25;
        public string Corners = "1111";
        public Statics.TileType Type = Statics.TileType.BLANK;
        public string SubType = "";

        public Tile(Statics.TileType Type, string Corners)
        {
            this.Type = Type;
            this.Corners = Corners;
        }

        public Tile(Statics.TileType Type, string Corners, string SubType)
        {
            this.Type = Type;
            this.Corners = Corners;
            this.SubType = SubType;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 Position, Rectangle SourceRectangle)
        {
            switch (Type)
            {
                case Statics.TileType.WATER:
                    spriteBatch.Draw(spriteSheet, Position, SourceRectangle, Color.White * 0.7f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    break;
                case Statics.TileType.BLANK:
                    switch (SubType)
                    {
                        case "Dirt":
                            spriteBatch.Draw(spriteSheet, Position, SourceRectangle, new Color(255, 210, 110), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            break;
                        case "Sand":
                            spriteBatch.Draw(spriteSheet, Position, SourceRectangle, new Color(255, 235, 20), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            //spriteBatch.Draw(spriteSheet, Position, SourceRectangle, Color.Yellow, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            break;
                        default:
                            spriteBatch.Draw(spriteSheet, Position, SourceRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                            break;
                    }
                    
                    break;
                default:
                    spriteBatch.Draw(spriteSheet, Position, SourceRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    break;
            }
        }
    }
}
