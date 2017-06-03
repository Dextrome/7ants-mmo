using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Data.GUI.Stratums
{
    public class Stratum
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Layer;
        public string Title;
        public Texture2D BackgroundTexture;
        public Texture2D BottomBorderTexture;
        public Texture2D TopBorderTexture;
        public Texture2D LeftBorderTexture;
        public Texture2D RightBorderTexture;
        public List<StratumControl> Controls = new List<StratumControl>();

        public void Draw(SpriteBatch spriteBatch, ContentManager content)
        {
            spriteBatch.Draw(BackgroundTexture, new Rectangle(X, Y, Width, Height), Color.White);
            spriteBatch.Draw(TopBorderTexture, new Rectangle(X, Y, Width, 5), Color.DarkGray);
            spriteBatch.Draw(BottomBorderTexture, new Rectangle(X, Y + Height - 5, Width, 5), Color.DarkGray);
            spriteBatch.Draw(LeftBorderTexture, new Rectangle(X, Y, 5, Height), Color.DarkGray);
            spriteBatch.Draw(RightBorderTexture, new Rectangle(X + Width - 5, Y, 5, Height), Color.DarkGray);

            foreach (StratumControl Control in Controls)
            {
                switch (Control.Type)
                {
                    case Statics.StratumControlType.LABEL:
                        spriteBatch.DrawString(content.Load<SpriteFont>("chatfont"), Control.Text, new Vector2(X,Y) + Control.Position + new Vector2(3,3), Color.White);
                        break;
                    case Statics.StratumControlType.TEXTBOX:
                        spriteBatch.Draw(content.Load<Texture2D>("hp_bar"), new Rectangle(X + Convert.ToInt16(Control.Position.X), Y + Convert.ToInt16(Control.Position.Y), Control.Width, Control.Height), Color.OliveDrab);
                        if (Control.Selected)
                            spriteBatch.DrawString(content.Load<SpriteFont>("chatfont"), Control.Text + "_", new Vector2(X, Y) + Control.Position + new Vector2(3, 3), Color.White);
                        else
                            spriteBatch.DrawString(content.Load<SpriteFont>("chatfont"), Control.Text, new Vector2(X, Y) + Control.Position + new Vector2(3, 3), Color.White);
                        break;
                    case Statics.StratumControlType.BUTTON:
                        spriteBatch.Draw(content.Load<Texture2D>("hp_bar"), new Rectangle(X + Convert.ToInt16(Control.Position.X), Y + Convert.ToInt16(Control.Position.Y), Control.Width, Control.Height), Color.OliveDrab);
                        spriteBatch.DrawString(content.Load<SpriteFont>("chatfont"), Control.Text, new Vector2(X, Y) + Control.Position + new Vector2(3, 3), Color.Yellow);
                        break;
                    case Statics.StratumControlType.IMAGE:
                        spriteBatch.Draw(content.Load<Texture2D>(Control.Text), new Rectangle(X + Convert.ToInt16(Control.Position.X), Y + Convert.ToInt16(Control.Position.Y), Control.Width, Control.Height), Color.White);
                        Paperdoll p;
                        KeyValuePair<string, Items.Item> kvp;
                        Items.Item i;

                        switch (Control.Name)
                        {
                            case "RightHand":
                                try
                                {
                                    p = (Paperdoll)this;
                                    kvp = p.Equipment.Single(item => item.Key == Control.Name);
                                    i = kvp.Value;
                                    spriteBatch.Draw(i.Texture, new Rectangle(X + Convert.ToInt16(Control.Position.X), Y + Convert.ToInt16(Control.Position.Y) - (Control.Height * 3), Control.Width, Control.Height * 4), Color.White);
                                }
                                catch { }
                                break;
                            case "LeftHand":
                                try
                                {
                                    p = (Paperdoll)this;
                                    kvp = p.Equipment.Single(item => item.Key == Control.Name);
                                    i = kvp.Value;
                                    spriteBatch.Draw(i.Texture, new Rectangle(X + Convert.ToInt16(Control.Position.X), Y + Convert.ToInt16(Control.Position.Y) - (Control.Height * 3), Control.Width, Control.Height * 4), Color.White);
                                }
                                catch { }
                                break;
                            case "Head":
                                try
                                {
                                    p = (Paperdoll)this;
                                    kvp = p.Equipment.Single(item => item.Key == Control.Name);
                                    i = kvp.Value;
                                    spriteBatch.Draw(i.Texture, new Rectangle(X + Convert.ToInt16(Control.Position.X), Y + Convert.ToInt16(Control.Position.Y) - (Control.Height * 3), Control.Width, Control.Height * 4), Color.White);
                                }
                                catch { }
                                break;
                        }

                        break;
                }
            }
        }
    }
}
