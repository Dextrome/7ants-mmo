using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Data.World;

namespace Data.GUI.Stratums
{
    public class LoginScreen: Stratum
    {
        public LoginScreen()
        {
            this.Width = 250;
            this.Height = 500;
            this.X = 0;
            this.Y = 0;
            this.Layer = 0;
        }

        public LoginScreen(int X, int Y, ContentManager content)
        {
            this.Width = 800;
            this.Height = 600;
            this.X = X;
            this.Y = Y;
            this.Controls.Add(new StratumControl("lblTitle", Statics.StratumControlType.LABEL, "                          LOGIN SCREEN", new Vector2(0,20)));
            this.Controls.Add(new StratumControl("txtUsername", Statics.StratumControlType.TEXTBOX, "", new Vector2(80, 100), 200, 25));
            this.Controls.Add(new StratumControl("txtPassword", Statics.StratumControlType.TEXTBOX, "", new Vector2(80, 150), 200, 25));
            this.Controls.Add(new StratumControl("btnLogin", Statics.StratumControlType.BUTTON, "        Login", new Vector2(80, 220), 140, 25));
            this.Controls.Add(new StratumControl("btnExit", Statics.StratumControlType.BUTTON, "        Exit Game", new Vector2(80, 350), 140, 25));
            this.Layer = 0;
            this.BackgroundTexture = content.Load<Texture2D>(@"GUI\stratum_bg");
            this.TopBorderTexture = content.Load<Texture2D>(@"GUI\stratum_bordertop");
            this.BottomBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderbottom");
            this.LeftBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderleft");
            this.RightBorderTexture = content.Load<Texture2D>(@"GUI\stratum_borderright");
        }

        public void btnLogin(ref NetClient Client, ref string myName)
        {
            string username = Controls.Find(control => control.Name == "txtUsername").Text;
            string password = Controls.Find(control => control.Name == "txtPassword").Text;
            myName = username;
            NetOutgoingMessage outmsg = Client.CreateMessage();
            outmsg.Write((byte)Statics.PacketTypes.LOGIN);
            outmsg.Write(username);
            outmsg.Write(password);
            outmsg.Write(23);
            outmsg.Write(23);
            outmsg.Write(0);
            Client.Connect("127.0.0.1", 14242, outmsg);
        }

        public void btnOptions()
        { }
    }
}
