using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Data.World;

namespace Data.GUI.Stratums
{
    public class MainMenu: Stratum
    {
        private int p;
        private int p_2;

        public MainMenu()
        {
            this.Width = 250;
            this.Height = 500;
            this.X = 0;
            this.Y = 0;
            this.Layer = 0;
        }

        public MainMenu(int X, int Y, ContentManager content)
        {
            this.Width = 300;
            this.Height = 400;
            this.X = X;
            this.Y = Y;
            this.Controls.Add(new StratumControl("lblTest", Statics.StratumControlType.LABEL, "                          MAIN MENU", new Vector2(0,20)));
            this.Controls.Add(new StratumControl("btnMapTools", Statics.StratumControlType.BUTTON, "        Map Tools", new Vector2(80, 100), 140, 25));
            this.Controls.Add(new StratumControl("btnOptions", Statics.StratumControlType.BUTTON, "        Options", new Vector2(80, 150), 140, 25));
            this.Controls.Add(new StratumControl("btnExit", Statics.StratumControlType.BUTTON, "        Exit Game", new Vector2(80, 330), 140, 25));
            this.Layer = 0;
            this.BackgroundTexture = content.Load<Texture2D>(@"GUI\stratum_bg");
            this.TopBorderTexture = content.Load<Texture2D>(@"GUI\stratum_bordertop");
            this.BottomBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderbottom");
            this.LeftBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderleft");
            this.RightBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderright");
        }

        public MainMenu(int p, int p_2)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.p_2 = p_2;
        }

        public void btnMapTools(ref List<Stratum> ActiveStratums, ref GraphicsDeviceManager graphics, ref ContentManager Content, ref Map map)
        {
            Stratum menu2remove = new Stratum();

            foreach (Stratum stratum in ActiveStratums)
                if (stratum.GetType() == typeof(MainMenu))
                    menu2remove = stratum;

            ActiveStratums.Add(new MapTools((graphics.PreferredBackBufferWidth / 2) - 125, (graphics.PreferredBackBufferHeight / 2) - 375, Content, ref map));
            ActiveStratums.Remove(menu2remove);
        }

        public void btnOptions()
        { }
    }
}
