using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Data.GUI.Stratums
{
    public class Paperdoll: Stratum
    {
        private int p;
        private int p_2;
        public Dictionary<string, Items.Item> Equipment = new  Dictionary<string, Items.Item>();

        public Paperdoll()
        {
            this.Width = 168;
            this.Height = 397;
            this.X = 0;
            this.Y = 0;
            this.Layer = 0;
        }

        public Paperdoll(int X, int Y, ContentManager content, Dictionary<string, Items.Item> Equipment)
        {
            this.Equipment = Equipment;
            this.Width = 168;
            this.Height = 397;
            this.X = X;
            this.Y = Y;
            this.Title = "Paperdoll";
            this.Controls.Add(new StratumControl("Silhouette", Statics.StratumControlType.IMAGE, @"GUI\Male-Silhouette", new Vector2(5, 5), 158, 387));
            this.Controls.Add(new StratumControl("RightHand", Statics.StratumControlType.IMAGE, @"GUI\equipment_slot", new Vector2(5, 195), 34, 34));
            this.Controls.Add(new StratumControl("LeftHand", Statics.StratumControlType.IMAGE, @"GUI\equipment_slot", new Vector2(129, 195), 34, 34));
            this.Controls.Add(new StratumControl("Head", Statics.StratumControlType.IMAGE, @"GUI\equipment_slot", new Vector2(62, 12), 34, 34));
            this.Layer = 0;
            this.BackgroundTexture = content.Load<Texture2D>(@"GUI\paperdoll_bg");
            this.TopBorderTexture = content.Load<Texture2D>(@"GUI\stratum_bordertop");
            this.BottomBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderbottom");
            this.LeftBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderleft");
            this.RightBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderright");
        }

        public Paperdoll(int p, int p_2)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.p_2 = p_2;
        }
    }
}
