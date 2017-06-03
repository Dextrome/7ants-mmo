using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Data;
using Data.World;


namespace Data.GUI.Stratums
{
    public class MapTools: Stratum
    {
        private int p;
        private int p_2;
        private Map map;

        public MapTools()
        {
            this.Width = 250;
            this.Height = 500;
            this.X = 0;
            this.Y = 0;
            this.Layer = 0;
        }

        public MapTools(int X, int Y, ContentManager content, ref Map map)
        {
            this.Width = 300;
            this.Height = 400;
            this.X = X;
            this.Y = Y;
            this.map = map;
            this.Controls.Add(new StratumControl("lblTitle", Statics.StratumControlType.LABEL, "                          MAP TOOLS", new Vector2(0,20)));
            this.Controls.Add(new StratumControl("btnReset", Statics.StratumControlType.BUTTON, "       Reset Map", new Vector2(80, 100), 140, 25));
            this.Controls.Add(new StratumControl("btnRandom", Statics.StratumControlType.BUTTON, "      Random Map", new Vector2(80, 150), 140, 25));
            this.Controls.Add(new StratumControl("btnSave", Statics.StratumControlType.BUTTON, "        Save Map", new Vector2(80, 200), 140, 25));
            this.Controls.Add(new StratumControl("btnLoad", Statics.StratumControlType.BUTTON, "        Load Map", new Vector2(80, 250), 140, 25));
            this.Controls.Add(new StratumControl("btnBackToMenu", Statics.StratumControlType.BUTTON, "  Back to Main Menu", new Vector2(80, 350), 140, 25));
            this.BackgroundTexture = content.Load<Texture2D>(@"GUI\stratum_bg");
            this.TopBorderTexture = content.Load<Texture2D>(@"GUI\stratum_bordertop");
            this.BottomBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderbottom");
            this.LeftBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderleft");
            this.RightBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderright");
        }

        public MapTools(int p, int p_2)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.p_2 = p_2;
        }

        public void btnBackToMenu(ref List<Stratum> ActiveStratums, ref GraphicsDeviceManager graphics, ref ContentManager Content)
        {
            Stratum menu2remove = new Stratum();

            foreach (Stratum stratum in ActiveStratums)
                if (stratum.GetType() == typeof(MapTools))
                    menu2remove = stratum;

            ActiveStratums.Add(new MainMenu((graphics.PreferredBackBufferWidth / 2) - 125, (graphics.PreferredBackBufferHeight / 2) - 375, Content));
            ActiveStratums.Remove(menu2remove);
        }

        public void btnReset()
        {
            map.GenerateBasicMap();
        }

        public void btnRandom()
        {
            map.GenerateRandomMap();
        }

        public void btnSave()
        {
            this.Controls.Clear();
            this.Controls.Add(new StratumControl("lblTitle", Statics.StratumControlType.LABEL, "                          SAVE MAP", new Vector2(0, 20)));
            this.Controls.Add(new StratumControl("txtFileName", Statics.StratumControlType.TEXTBOX, "", new Vector2(50, 100), 200, 25));
            this.Controls.Add(new StratumControl("btnSaveMap", Statics.StratumControlType.BUTTON, "              SAVE", new Vector2(80, 150), 140, 25));
            this.Controls.Add(new StratumControl("btnBack", Statics.StratumControlType.BUTTON, "              BACK", new Vector2(80, 350), 140, 25));
        }

        public void btnSaveMap()
        {
            map.filename = Controls.Find(control => control.Name == "txtFileName").Text;
            map.SaveMap();
            btnBack();
        }

        public void btnLoad()
        {
            this.Controls.Clear();
            var filenames = Directory.GetFiles(@"E:\Projects\MonoGame Projects\7ANTSMMO_MONO\7ANTSMMO_MONO\bin\WindowsGL\Debug", "*.xml").
                Select(filename => Path.GetFileNameWithoutExtension(filename)).
                ToArray();
            int counter = 0;
            foreach (string filename in filenames)
            {
                counter++;
                this.Controls.Add(new StratumControl("btnMap", Statics.StratumControlType.BUTTON, filename, new Vector2(80, 20 + (counter * 50)), 140, 25));
            }
            this.Controls.Add(new StratumControl("lblTitle", Statics.StratumControlType.LABEL, "                          LOAD MAP", new Vector2(0, 20)));
            this.Controls.Add(new StratumControl("btnBack", Statics.StratumControlType.BUTTON, "              BACK", new Vector2(80, 350), 140, 25));
        }

        public void btnMap(string filename)
        {
            map.LoadMap(filename);
            btnBack();
        }


        public void btnBack()
        {
            this.Controls.Clear();
            this.Controls.Add(new StratumControl("lblTest", Statics.StratumControlType.LABEL, "                          MAP TOOLS", new Vector2(0, 20)));
            this.Controls.Add(new StratumControl("btnReset", Statics.StratumControlType.BUTTON, "       Reset Map", new Vector2(80, 100), 140, 25));
            this.Controls.Add(new StratumControl("btnRandom", Statics.StratumControlType.BUTTON, "      Random Map", new Vector2(80, 150), 140, 25));
            this.Controls.Add(new StratumControl("btnSave", Statics.StratumControlType.BUTTON, "        Save Map", new Vector2(80, 200), 140, 25));
            this.Controls.Add(new StratumControl("btnLoad", Statics.StratumControlType.BUTTON, "        Load Map", new Vector2(80, 250), 140, 25));
        }
    }
}
