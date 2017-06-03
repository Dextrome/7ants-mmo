using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Data.GUI.Stratums
{
    public class StratumControl
    {
        public string Name;
        public Statics.StratumControlType Type;
        public string Text;
        public Vector2 Position;
        public int Width;
        public int Height;
        public bool Selected;

        public StratumControl()
        {
            this.Name = "";
            this.Text = "";
            this.Position = new Vector2(0, 0);
            this.Width = 0;
            this.Height = 0;
            this.Selected = false;
        }

        public StratumControl(string Name, Statics.StratumControlType Type)
        {
            this.Name = Name;
            this.Type = Type;
            this.Text = "";
            this.Position = new Vector2(0, 0);
            this.Width = 0;
            this.Height = 0;
            this.Selected = false;
        }

        public StratumControl(string Name, Statics.StratumControlType Type, string Text)
        {
            this.Name = Name;
            this.Type = Type;
            this.Text = Text;
            this.Position = new Vector2(0, 0);
            this.Width = 0;
            this.Height = 0;
            this.Selected = false;
        }

        public StratumControl(string Name, Statics.StratumControlType Type, string Text, Vector2 Position)
        {
            this.Name = Name;
            this.Type = Type;
            this.Text = Text;
            this.Position = Position;
            this.Width = 0;
            this.Height = 0;
            this.Selected = false;
        }

        public StratumControl(string Name, Statics.StratumControlType Type, string Text, Vector2 Position, int Width, int Height)
        {
            this.Name = Name;
            this.Type = Type;
            this.Text = Text;
            this.Position = Position;
            this.Width = Width;
            this.Height = Height;
            this.Selected = false;
        }
    }
}
