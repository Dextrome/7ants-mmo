using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;
using _7ANTSMMO.SpriteSheetRuntime;
using Data;
using Data.Items;
using Data.World;
using Data.Graphics.Shapes;
using Data.Graphics;
using Data.GUI;
using Data.GUI.Stratums;
using Data.Mobiles;

namespace Client
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private delegate void LoginDelegate(ref NetClient Client, ref string MyName);

        //SpriteSheets
        _7ANTSMMO.SpriteSheetRuntime.SpriteSheet spriteSheet;
        _7ANTSMMO.SpriteSheetRuntime.SpriteSheet spriteSheetHumanMale;

        //Misc
        bool loggedIn = false;
        Point MouseTileCoords = new Point(0, 0);
        Point ClosestPoint2Mouse = new Point(0, 0);
        bool TilePickingOn = false;
        BasicEffect Effect;
        static Map Map;
        static NetClient Client;
        static string TestName = Statics.GetRandomString();
        static Dictionary<string, Player> GameStateList;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //static string MyName = Statics.GetRandomString();
        string MyName = "";
        static KeyboardState KeyboardState;
        static MouseState MouseState;
        MouseState PreviousMouseState;
        Command MyCommand;
        InventoryItem MyCommandInputItem;
        Dictionary<DateTime, string> SystemMessages = new Dictionary<DateTime, string>();
        TimeSpan SystemMessageDuration = new TimeSpan(0, 0, 5);
        static DateTime AnimationTime = DateTime.Now;
        TimeSpan AnimationDuration = new TimeSpan(0, 0, 0, 0, 40);
        DateTime ServerMessageCheckTime = DateTime.Now;
        TimeSpan ServerMessageCheckTimeSpan = new TimeSpan(0, 0, 0, 0, 150);
        DateTime LastRelease = DateTime.Now;
        DateTime PressedSince = DateTime.Now;
        TimeSpan DoubleClickTimer = new TimeSpan(0, 0, 0, 0, 350);
        DateTime LastKeyboardInput = DateTime.Now;
        TimeSpan KeyboardInputDelay = new TimeSpan(0, 0, 0, 0, 250);
        DateTime LastHit = DateTime.Now;
        TimeSpan HitTimer = new TimeSpan(0, 0, 0, 1);
        Inventory MyInventory;
        DragObject DragObject;
        public List<Stratum> ActiveStratums = new List<Stratum>();
        string TextInput = "";
        Vector2 TextInputPosition;
        Boolean textBoxTyping = false;
        string textBoxInput = "";

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            spriteSheet = Content.Load<_7ANTSMMO.SpriteSheetRuntime.SpriteSheet>("SpriteSheet");
            spriteSheetHumanMale = Content.Load<_7ANTSMMO.SpriteSheetRuntime.SpriteSheet>("SpriteSheetHumanMale");

            NetPeerConfiguration Config = new NetPeerConfiguration("7ANTSMMO");
            Client = new NetClient(Config);
            Client.Start();
            GameStateList = new Dictionary<string, Player>();
            //Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        protected override void Initialize()
        {
            MapTile.GrassTexture = Content.Load<Texture2D>("grasstexture2");
            MapTile.DirtTexture = Content.Load<Texture2D>("dirttexture2");
            MapTile.WaterTexture = Content.Load<Texture2D>("watertexture");
            Map = new Map();
            //WaitForServerApproval(Content);
            Effect = new BasicEffect(GraphicsDevice);
            Effect.VertexColorEnabled = true;
            Effect.TextureEnabled = true;
            Statics.blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Statics.blank.SetData(new[] { Color.White });
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;
            graphics.ApplyChanges();
            MyInventory = new Inventory(Content);
            Inventory.ColumnsShown = 1;
            MyInventory.UpdatePosition(graphics);
            this.IsMouseVisible = true;
            DrawBatch.Effect = Effect;
            DrawBatch.Content = Content;
            DrawBatch.GraphicsDevice = GraphicsDevice;
            UpdateMap();
            //MapObject TestMapObject = new MapObject(new Shovel(Content), 20, 20, Map);
            //Map.Objects.Add(TestMapObject);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            HookKeys();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TextInputPosition.X = (graphics.PreferredBackBufferWidth / 10) + 10;
            TextInputPosition.Y = graphics.PreferredBackBufferHeight - 25 - (graphics.PreferredBackBufferHeight / 10);
            //TextBox.Width = graphics.PreferredBackBufferWidth - 20 - (graphics.PreferredBackBufferWidth / 5);
            ActiveStratums.Add(new LoginScreen(0,0,Content));
        }

        protected void HookKeys()
        {
            OpenTK.GameWindow OTKWindow = null;
            Type type = typeof(OpenTKGameWindow);

            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                OTKWindow = field.GetValue(Window) as OpenTK.GameWindow;
            }

            if (OTKWindow != null)
            {
                OTKWindow.KeyPress += OTKWindow_KeyPress;
                OTKWindow.Keyboard.KeyUp += OTKWindow_KeyUp;
                OTKWindow.Keyboard.KeyDown += OTKWindow_KeyDown;
            }

        }

        private void OTKWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {            /*Console.WriteLine(String.Format("KeyDown={0}\n", e.Key));*/       }

        private void OTKWindow_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {            /*Console.WriteLine(String.Format("KeyUp={0}\n", e.Key));*/       }

        private void OTKWindow_KeyPress(object sender, OpenTK.KeyPressEventArgs e)
        {
            if (textBoxTyping)
            {
                if (char.IsControl(e.KeyChar))
                {
                    if (e.KeyChar == '\b') //backspace
                        if (textBoxInput.Length > 0)
                            textBoxInput = textBoxInput.Substring(0, textBoxInput.Length - 1);
                    else if (e.KeyChar == '\t') //tab
                    { }
                }
                else
                {
                    if (e.KeyChar == '€') { }
                    else
                        textBoxInput += e.KeyChar;
                }
            }
            else
            {
                if (char.IsControl(e.KeyChar))
                {
                    if (e.KeyChar == '\b') //backspace
                        if (TextInput.Length > 0)
                            TextInput = TextInput.Substring(0, TextInput.Length - 1);
                    else if (e.KeyChar == '\t') //tab
                    { }
                }
                else
                {
                    if (e.KeyChar == '€') { }
                    else
                        TextInput += e.KeyChar;
                }
            }
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();
            MyInventory.Update(MouseState);
            
            //CURSORS
            if (MyCommand != null && MyCommand.Target)
                this.IsMouseVisible = false;
            else
                this.IsMouseVisible = true;

            HandleMouseInput();
            HandleKeyboardInput();

            if (loggedIn)
            {
                Combat();
                CheckPlayerMovement();

                if (DateTime.Now - AnimationTime >= AnimationDuration)
                {
                    //foreach (Player p in GameStateList.Values.Where(player => player.Animation != ""))
                    foreach (Player p in GameStateList.Values)
                    { p.Update(); }
                    AnimationTime = DateTime.Now;
                }

                UpdateMap();

                if (DateTime.Now - ServerMessageCheckTime >= ServerMessageCheckTimeSpan)
                {
                    CheckServerMessages(Content);
                    ServerMessageCheckTime = DateTime.Now;
                }
            }

            PreviousMouseState = MouseState;

            //STRATUM-UPDATES
            foreach (Stratum Stratum in ActiveStratums)
            {
                foreach (StratumControl Control in Stratum.Controls)
                    if (Control.Selected)
                        Control.Text = textBoxInput;

                //if (Stratum.GetType() == typeof(MapTools))
                //{
                //    MapTools MapTools = (MapTools)Stratum;
                //    if (textBoxTyping)
                //        MapTools.Controls.Find(control => control.Name == "txtFileName").Text = textBoxInput;
                //}
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (loggedIn)
            {
                int MyX; int MyY; int MyZ;
                if (GameStateList[MyName].Animation == "NorthEast" || GameStateList[MyName].Animation == "NorthWest" || GameStateList[MyName].Animation == "North")
                {
                    MyX = GameStateList[MyName].PreviousX;
                    MyY = GameStateList[MyName].PreviousY;
                    MyZ = GameStateList[MyName].PreviousZ;
                }
                else
                {
                    MyX = GameStateList[MyName].X;
                    MyY = GameStateList[MyName].Y;
                    MyZ = GameStateList[MyName].Z;
                }

                //TILES <= ME."Z"
                for (int x = 0; x <= 40; x++)
                {
                    int tileX = x - 20;
                    for (int y = 0; y <= 40; y++)
                    {
                        int tileY = y - 20;
                        int X; int Y;

                        if (GameStateList[MyName].X + tileX < 0)
                            X = (Map.MaxX + 1) + (GameStateList[MyName].X + tileX);
                        else if (GameStateList[MyName].X + tileX > Map.MaxX)
                            X = (GameStateList[MyName].X + tileX) - (Map.MaxX + 1);
                        else
                            X = GameStateList[MyName].X + tileX;

                        if (GameStateList[MyName].Y + tileY < 0)
                            Y = (Map.MaxY + 1) + (GameStateList[MyName].Y + tileY);
                        else if (GameStateList[MyName].Y + tileY > Map.MaxY)
                            Y = (GameStateList[MyName].Y + tileY) - (Map.MaxY + 1);
                        else
                            Y = GameStateList[MyName].Y + tileY;

                        if (Map.Tiles[X, Y] != null)
                            if ((int)Map.Tiles[X, Y].AverageZ <= MyZ)
                                Map.Tiles[X, Y].Draw(GraphicsDevice, GameStateList[MyName].AnimationOffset);
                            else if (MyX >= MyX + tileX || MyY >= MyY + tileY)
                                Map.Tiles[X, Y].Draw(GraphicsDevice, GameStateList[MyName].AnimationOffset);
                            else if (MyX + tileX != X || MyY + tileY != Y)
                                Map.Tiles[X, Y].Draw(GraphicsDevice, GameStateList[MyName].AnimationOffset);
                    }
                }

                //PLAYERS <= ME.Z
                foreach (Player p in GameStateList.Values.OrderBy(value => value.Z))
                {
                    if (p.UserName == MyName)
                        GameStateList[MyName].Draw(graphics, false, new Vector2(0, 0), new Vector2(0, 0));
                    else
                        p.Draw(graphics, true, new Vector2(-MapTile.Height * (p.Y - GameStateList[MyName].Y), MapTile.Height / 2 * (p.Y - GameStateList[MyName].Y))
                        + new Vector2(MapTile.Width / 2 * (p.X - GameStateList[MyName].X), MapTile.Width / 4 * (p.X - GameStateList[MyName].X)), GameStateList[MyName].AnimationOffset);
                }

                //TILES > ME."Z"
                for (int x = 0; x <= 40; x++)
                {
                    int tileX = x - 20;
                    for (int y = 0; y <= 40; y++)
                    {
                        int tileY = y - 20;
                        if (MyX + tileX >= 0 && MyY + tileY >= 0 && MyX + tileX <= Map.MaxX && MyY + tileY <= Map.MaxY)
                            if (Map.Tiles[MyX + tileX, MyY + tileY] != null)
                                if ((int)Map.Tiles[MyX + tileX, MyY + tileY].AverageZ > MyZ)
                                {
                                    if (MyX <= MyX + tileX && MyY <= MyY + tileY)
                                        Map.Tiles[MyX + tileX, MyY + tileY].Draw(GraphicsDevice, GameStateList[MyName].AnimationOffset);
                                }
                                else if ((int)Map.Tiles[MyX + tileX, MyY + tileY].AverageZ == MyZ)
                                {
                                    if (MyX < MyX + tileX && MyY < MyY + tileY)
                                        Map.Tiles[MyX + tileX, MyY + tileY].Draw(GraphicsDevice, GameStateList[MyName].AnimationOffset);
                                }
                    }
                }

                //PLAYERS > ME.Z
                foreach (Player p in GameStateList.Values.OrderBy(value => value.Z))
                {
                    if (p.UserName != MyName)
                        p.Draw(graphics, true, new Vector2(-MapTile.Height * (p.Y - GameStateList[MyName].Y), MapTile.Height / 2 * (p.Y - GameStateList[MyName].Y))
                        + new Vector2(MapTile.Width / 2 * (p.X - GameStateList[MyName].X), MapTile.Width / 4 * (p.X - GameStateList[MyName].X)), GameStateList[MyName].AnimationOffset);
                }

                //CHAT MESSAGES & HP BARS
                foreach (Player p in GameStateList.Values.OrderBy(value => value.Z))
                {
                    if (p.UserName == MyName)
                    {
                        if (p.Message != "")
                        {
                            if (p.Message.Length >= 6)
                            {
                                if (p.Message.Substring(0, 6) == "SERVER")
                                    spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), p.Message, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2) + new Vector2(-20, -70), Color.PaleVioletRed);
                                else
                                    spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), p.Message, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2) + new Vector2(-20, -70), Color.LightSteelBlue);
                            }
                            else
                                spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), p.Message, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2) + new Vector2(-20, -70), Color.LightSteelBlue);
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), p.Message, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2) + new Vector2(-20, -70)
                                                + new Vector2(-MapTile.Height * (p.Y - GameStateList[MyName].Y), MapTile.Height / 2 * (p.Y - GameStateList[MyName].Y))
                                                + new Vector2(MapTile.Width / 2 * (p.X - GameStateList[MyName].X), MapTile.Width / 4 * (p.X - GameStateList[MyName].X))
                                                + GameStateList[MyName].AnimationOffset - p.AnimationOffset, Color.LightSteelBlue);
                    }
                }

                //TEXT-INPUT
                spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), TextInput, TextInputPosition, Color.White);

                //SYSTEM MESSAGES
                int yOffset = 0;
                List<DateTime> MessagesToRemove = new List<DateTime>();

                foreach (KeyValuePair<DateTime, string> msg in SystemMessages.OrderByDescending(value => value.Key))
                {
                    if (DateTime.Now - msg.Key > SystemMessageDuration)
                        MessagesToRemove.Add(msg.Key);
                    else
                    {
                        spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), msg.Value, new Vector2((graphics.PreferredBackBufferWidth / 10) + 10, graphics.PreferredBackBufferHeight - 75 - (graphics.PreferredBackBufferHeight / 10) - yOffset), Color.DarkRed);
                        yOffset += 15;
                    }
                }

                foreach (DateTime key in MessagesToRemove)
                {
                    SystemMessages.Remove(key);
                }


                //CURSORS
                if (MyCommand != null && MyCommand.Target)
                {
                    Point[] MousePoints = new Point[2];

                    switch (MyCommand.TargetType)
                    {
                        case Statics.TargetType.TILE:
                            TilePickingOn = true;
                            if ((MouseTileCoords.X == GameStateList[MyName].X && MouseTileCoords.Y == GameStateList[MyName].Y))
                                switch (MyCommand.Type)
                                {
                                    case Statics.CommandType.DIGTILE:
                                        spriteBatch.Draw(Content.Load<Texture2D>("Shovel"), new Vector2(GameStateList[MyName].Position.X, GameStateList[MyName].Position.Y) + new Vector2(-10, -60) + new Vector2(48, 72), Color.White);
                                        break;
                                    case Statics.CommandType.RAISETILE:
                                        spriteBatch.Draw(Content.Load<Texture2D>("DirtPile"), new Vector2(GameStateList[MyName].Position.X, GameStateList[MyName].Position.Y) + new Vector2(-20, -40) + new Vector2(48, 72), Color.White);
                                        break;
                                    default:
                                        break;
                                }
                            else
                                spriteBatch.Draw(spriteSheet.Texture, new Vector2(MouseState.X - 24, MouseState.Y - 24), spriteSheet.SourceRectangle("NoTarget"), Color.White);

                            break;
                        case Statics.TargetType.CORNER:
                            if ((ClosestPoint2Mouse.X == GameStateList[MyName].X && ClosestPoint2Mouse.Y == GameStateList[MyName].Y)
                                || (ClosestPoint2Mouse.X == GameStateList[MyName].X + 1 && ClosestPoint2Mouse.Y == GameStateList[MyName].Y)
                                || (ClosestPoint2Mouse.X == GameStateList[MyName].X && ClosestPoint2Mouse.Y == GameStateList[MyName].Y + 1)
                                || (ClosestPoint2Mouse.X == GameStateList[MyName].X + 1 && ClosestPoint2Mouse.Y == GameStateList[MyName].Y + 1))
                                switch (MyCommand.Type)
                                {
                                    case Statics.CommandType.DIGCORNER:
                                        spriteBatch.Draw(spriteSheet.Texture,
                                        new Vector2(Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y].Corners[0].X, Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y].Corners[0].Y) + new Vector2(-10, -60),
                                        spriteSheet.SourceRectangle("Shovel"),
                                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                                        break;
                                    case Statics.CommandType.RAISECORNER:
                                        spriteBatch.Draw(spriteSheet.Texture,
                                        new Vector2(Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y].Corners[0].X, Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y].Corners[0].Y) + new Vector2(-20, -40),
                                        spriteSheet.SourceRectangle("DirtPile"),
                                        Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
                                        break;
                                    default:
                                        break;
                                }
                            else
                                spriteBatch.Draw(spriteSheet.Texture, new Vector2(MouseState.X - 24, MouseState.Y - 24), spriteSheet.SourceRectangle("NoTarget"), Color.White);

                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    TilePickingOn = false;
                }

                //Map Objects
                foreach (MapObject Object in Map.Objects)
                {
                    Object.Draw(graphics, GameStateList[MyName].AnimationOffset);
                }

                //UI
                DrawGUI();

                DrawBatch.Draw();
            }
            else
            {
                foreach (Stratum stratum in ActiveStratums)
                    stratum.Draw(spriteBatch, Content);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Client.Shutdown("bye");
            
            base.OnExiting(sender, args);
        }

        private void DrawGUI()
        {
            spriteBatch.Draw(Content.Load<Texture2D>("black"), new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight / 10), Color.Black);
            spriteBatch.Draw(Content.Load<Texture2D>("black"), new Rectangle(0, graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 10), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight / 10), Color.Black);
            spriteBatch.Draw(Content.Load<Texture2D>("black"), new Rectangle(0, 0, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight), Color.Black);
            spriteBatch.Draw(Content.Load<Texture2D>("black"), new Rectangle(graphics.PreferredBackBufferWidth - (graphics.PreferredBackBufferWidth / 10), 0, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight), Color.Black);

            spriteBatch.Draw(Content.Load<Texture2D>("brownish"), new Rectangle(graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10, (graphics.PreferredBackBufferWidth / 10) * 8, 5), Color.White);
            spriteBatch.Draw(Content.Load<Texture2D>("brownish"), new Rectangle(graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10, 5, (graphics.PreferredBackBufferHeight / 10) * 8), Color.White);
            spriteBatch.Draw(Content.Load<Texture2D>("brownish"), new Rectangle(graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 10), (graphics.PreferredBackBufferWidth / 10) * 8, 5), Color.White);
            spriteBatch.Draw(Content.Load<Texture2D>("brownish"), new Rectangle(graphics.PreferredBackBufferWidth - (graphics.PreferredBackBufferWidth / 10), graphics.PreferredBackBufferHeight / 10, 5, ((graphics.PreferredBackBufferHeight / 10) * 8) + 5), Color.White);

            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "TAB = TOGGLE COMBAT", new Vector2(10, 10), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "F5 = CHANGE MAP DIRECTION", new Vector2(10, 35), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "F6 = TOGGLE TILE BORDERS", new Vector2(10, 60), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "F1 = DIG CORNER", new Vector2(10, 85), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "F2 = RAISE CORNER", new Vector2(10, 110), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "F3 = DIG TILE", new Vector2(10, 135), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "F4 = RAISE TILE", new Vector2(10, 160), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "ESC = MAIN MENU", new Vector2(10, 300), Color.LightGray);
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "ALT+P = PAPERDOLL", new Vector2(10, 325), Color.LightGray);

            if (GameStateList[MyName].Combat && GameStateList[MyName].CombatTarget != null)
                spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "TARGET = " + GameStateList[MyName].CombatTarget.Name, new Vector2(graphics.PreferredBackBufferWidth / 2 + 50, 10), Color.LightGray);

            //hp bar
            spriteBatch.DrawString(Content.Load<SpriteFont>("chatfont"), "HP:", new Vector2(250, 10), Color.LightGray);
            spriteBatch.Draw(spriteSheet.Texture, new Rectangle(290, 10, 100, 19), spriteSheet.SourceRectangle("hp_bar_bg"), Color.White);
            spriteBatch.Draw(spriteSheet.Texture, new Rectangle(290, 10, GameStateList[MyName].HP, 19), spriteSheet.SourceRectangle("hp_bar"), Color.White);

            //inventory
            MyInventory.Draw(spriteSheet.Texture, spriteBatch, spriteSheet.SourceRectangle("inventory"), spriteSheet.SourceRectangle("inventory_highlight"));
            spriteBatch.DrawString(Content.Load<SpriteFont>("inventoryfont"), "F1", new Vector2(graphics.PreferredBackBufferWidth - (32 * Inventory.ColumnsShown) - 17, graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 10) - (32 * 10) + 7), Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("inventoryfont"), "F2", new Vector2(graphics.PreferredBackBufferWidth - (32 * Inventory.ColumnsShown) - 17, graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 10) - (32 * 9) + 7), Color.White);

            for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
                for (int y = 0; y < 10; y++)
                    if (MyInventory.InventoryItems[x, y] != null)
                        MyInventory.InventoryItems[x, y].Draw(spriteBatch, spriteSheet.Texture, MyInventory.InventoryItems[x, y].Item.Texture, spriteSheet.SourceRectangle("inventoryitem_highlight"), x, y, Content.Load<SpriteFont>("InventoryItemFont"));

            //compass
            if (Map.ScreenDirection == Statics.ScreenDirection.NORTHWEST)
                spriteBatch.Draw(Content.Load<Texture2D>("compass"), new Vector2(graphics.PreferredBackBufferWidth - 137, 0), new Rectangle(0, 0, 472, 489), Color.White, 0f, new Vector2(0, 0), 0.3f, SpriteEffects.None, 1f);
            else
                spriteBatch.Draw(Content.Load<Texture2D>("compass"), new Vector2(graphics.PreferredBackBufferWidth - 22, 167), new Rectangle(0, 0, 472, 489), Color.White, (float)Math.PI, new Vector2(0, 0), 0.3f, SpriteEffects.None, 1f);

            //STRATUMS
            foreach (Stratum stratum in ActiveStratums)
                stratum.Draw(spriteBatch, Content);

            //drag object
            if (DragObject != null)
                DragObject.Draw(GraphicsDevice, spriteBatch, MouseState);
        }

        private void HandleMouseInput()
        {
            if (MouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed)
            {
                //DRAGGING RELEASE:
                if (DragObject != null)
                {
                    if (DragObject.Object.GetType().BaseType == typeof(Stratum))
                    {
                        foreach (Stratum Stratum in ActiveStratums)
                        {
                            if (Stratum == DragObject.Object)
                            {
                                Stratum.X = MouseState.X - (int)DragObject.DrawOffSet.X;
                                Stratum.Y = MouseState.Y - (int)DragObject.DrawOffSet.Y;
                                DragObject = null;
                                break;
                            }
                        }
                    }
                    else if (DragObject.Object.GetType() == typeof(InventoryItem))
                    {
                        InventoryItem drag = (InventoryItem)DragObject.Object;
                        bool PaperdollRelease = false;
                        bool InventoryRelease = false;

                        //check if released in paperdoll
                        try
                        {
                            Stratum paperdoll = ActiveStratums.Single(Stratum => Stratum.Title == "Paperdoll");
                            Paperdoll p = (Paperdoll)paperdoll;

                            foreach (StratumControl c in p.Controls.Where(control => control.Name != "Silhouette"))
                            {
                                Rectangle Slot = new Rectangle(paperdoll.X + (int)c.Position.X, paperdoll.Y + (int)c.Position.Y, c.Width, c.Height);

                                if (Slot.Contains(new Point(MouseState.X, MouseState.Y)))
                                {
                                    SystemMessages.Add(DateTime.Now, drag.Item.Name + " released in " + c.Name + ".");
                                    p.Equipment.Add(c.Name, drag.Item);
                                    MyInventory.InventoryItems[drag.ArrayX, drag.ArrayY] = null;
                                    DragObject = null;
                                    PaperdollRelease = true;
                                }
                            }
                        }
                        catch (Exception e)
                        { string exception = e.ToString(); }

                        //check if released in inventory
                        if (!PaperdollRelease)
                        {
                            for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
                            {
                                for (int y = 0; y < 10; y++)
                                {
                                    if (InventoryItem.GetScreenRectangle(x, y).Contains(new Point(MouseState.X, MouseState.Y)))
                                    {
                                        int DropArrayX = drag.ArrayX; int DropArrayY = drag.ArrayY;
                                        InventoryItem drop = MyInventory.InventoryItems[x, y];
                                        if (drop != null)
                                        { drop.ArrayX = DropArrayX; drop.ArrayY = DropArrayY; MyInventory.InventoryItems[x, y] = null; }
                                        drag.ArrayX = x; drag.ArrayY = y;
                                        MyInventory.InventoryItems[x, y] = drag;

                                        if (DragObject.NoOrigin)
                                        {
                                            //if (drop != null)
                                            //    DragObject = new DragObject(drop, new Vector2(InventoryItem.GetScreenRectangle(x, y).X, InventoryItem.GetScreenRectangle(x, y).Y), true);
                                            //else
                                            DragObject = null;
                                        }
                                        else
                                        {
                                            MyInventory.InventoryItems[DropArrayX, DropArrayY] = drop;
                                            DragObject = null;
                                        }
                                        InventoryRelease = true;
                                    }
                                }
                            }

                            //check if released on map
                            if (!InventoryRelease)
                            {
                                Map.Objects.Add(new MapObject(MyInventory.InventoryItems[drag.ArrayX, drag.ArrayY].Item, MouseTileCoords.X, MouseTileCoords.Y, Map));
                                MyInventory.InventoryItems[drag.ArrayX, drag.ArrayY] = null;
                                DragObject = null;
                            }
                        }
                    }
                    else if (DragObject.Object.GetType().BaseType == typeof(Item))
                    {
                        //Dragged item originating from an equipment slot
                        Item drag = (Item)DragObject.Object;
                        bool PaperdollRelease = false;
                        bool InventoryRelease = false;

                        //check if released in paperdoll
                        try
                        {
                            Stratum paperdoll = ActiveStratums.Single(Stratum => Stratum.Title == "Paperdoll");
                            Paperdoll p = (Paperdoll)paperdoll;

                            foreach (StratumControl c in p.Controls.Where(control => control.Name != "Silhouette"))
                            {
                                Rectangle Slot = new Rectangle(paperdoll.X + (int)c.Position.X, paperdoll.Y + (int)c.Position.Y, c.Width, c.Height);

                                if (Slot.Contains(new Point(MouseState.X, MouseState.Y)))
                                {
                                    SystemMessages.Add(DateTime.Now, drag.Name + " released in " + c.Name + ".");
                                    p.Equipment.Add(c.Name, drag);
                                    DragObject = null;
                                    PaperdollRelease = true;
                                }
                            }
                        }
                        catch (Exception e)
                        { string exception = e.ToString(); }

                        if (!PaperdollRelease)
                        {
                            //check if released in inventory
                            for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
                            {
                                for (int y = 0; y < 10; y++)
                                {
                                    if (InventoryItem.GetScreenRectangle(x, y).Contains(new Point(MouseState.X, MouseState.Y)))
                                    {
                                        InventoryItem drop = MyInventory.InventoryItems[x, y];
                                        MyInventory.InventoryItems[x, y] = new InventoryItem(drag, 1, x, y);

                                        if (drop != null)
                                            DragObject = new DragObject(drop, new Vector2(InventoryItem.GetScreenRectangle(x, y).X, InventoryItem.GetScreenRectangle(x, y).Y), true);
                                        else
                                            DragObject = null;

                                        InventoryRelease = true;
                                    }
                                }
                            }

                            //check if released on map
                            if (!InventoryRelease)
                            {
                                Map.Objects.Add(new MapObject(drag, MouseTileCoords.X, MouseTileCoords.Y, Map));
                                DragObject = null;
                            }
                        }
                    }
                    else if (DragObject.Object.GetType() == typeof(MapObject))
                    {
                        MapObject drag = (MapObject)DragObject.Object;
                        bool PaperdollRelease = false;
                        bool InventoryRelease = false;

                        //check if released in paperdoll
                        try
                        {
                            Stratum paperdoll = ActiveStratums.Single(Stratum => Stratum.Title == "Paperdoll");
                            Paperdoll p = (Paperdoll)paperdoll;

                            foreach (StratumControl c in p.Controls.Where(control => control.Name != "Silhouette"))
                            {
                                Rectangle Slot = new Rectangle(paperdoll.X + (int)c.Position.X, paperdoll.Y + (int)c.Position.Y, c.Width, c.Height);

                                if (Slot.Contains(new Point(MouseState.X, MouseState.Y)))
                                {
                                    Item dragItem = (Item)drag.Object;
                                    SystemMessages.Add(DateTime.Now, dragItem.Name + " released in " + c.Name + ".");
                                    p.Equipment.Add(c.Name, dragItem);
                                    DragObject = null;
                                    PaperdollRelease = true;
                                }
                            }
                        }
                        catch (Exception e)
                        { string exception = e.ToString(); }

                        if (!PaperdollRelease)
                        {
                            //check if released in inventory
                            for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
                            {
                                for (int y = 0; y < 10; y++)
                                {
                                    if (InventoryItem.GetScreenRectangle(x, y).Contains(new Point(MouseState.X, MouseState.Y)))
                                    {
                                        InventoryItem drop = MyInventory.InventoryItems[x, y];
                                        MyInventory.InventoryItems[x, y] = new InventoryItem((Item)drag.Object, 1, x, y);

                                        if (drop != null)
                                            DragObject = new DragObject(drop, new Vector2(InventoryItem.GetScreenRectangle(x, y).X, InventoryItem.GetScreenRectangle(x, y).Y), true);
                                        else
                                            DragObject = null;

                                        InventoryRelease = true;
                                    }
                                }
                            }

                            //check if released on map
                            if (!InventoryRelease)
                            {
                                if (MouseTileCoords.X > GameStateList[MyName].X + 1 || MouseTileCoords.X < GameStateList[MyName].X - 1 ||
                                    MouseTileCoords.Y > GameStateList[MyName].Y + 1 || MouseTileCoords.Y < GameStateList[MyName].Y - 1)
                                    SystemMessages.Add(DateTime.Now, "TOO FAR AWAY.");
                                else
                                {
                                    drag.X = MouseTileCoords.X;
                                    drag.Y = MouseTileCoords.Y;
                                    Map.Objects.Add(drag);
                                    DragObject = null;
                                }
                            }
                        }
                    }
                }

                //DOUBLECLICK:
                if (DateTime.Now - LastRelease < DoubleClickTimer)
                {
                    //players
                    foreach (Player p in GameStateList.Values.Where(player => player.HP > 0 && player.UserName != MyName))
                    {
                        if (p.ScreenPosition.Contains(new Point(MouseState.X, MouseState.Y)))
                        {
                            if (GameStateList[MyName].CombatTarget != null)
                            {
                                Player CombatTarget = (Player)GameStateList[MyName].CombatTarget;
                                GameStateList[CombatTarget.UserName].Targetted = false;
                            }

                            //SystemMessages.Add(DateTime.Now, "TARGETTED PLAYER = " + p.UserName);
                            GameStateList[MyName].CombatTarget = p;
                            p.Targetted = true;
                            //SystemMessages.Add(DateTime.Now, p.UserName + " [" + p.X + "," + p.Y + "]");
                            NetOutgoingMessage outmsg = Client.CreateMessage();
                            outmsg.Write((byte)Statics.PacketTypes.MOBILETARGET);
                            outmsg.Write(p.UserName);
                            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                        }
                    }

                    //world
                    MapObject MouseOverMapObject = Map.Objects.Where(Obj => Obj.MouseOver == true).FirstOrDefault();
                    if (MouseOverMapObject != null)
                        SystemMessages.Add(DateTime.Now, MouseOverMapObject.Object.GetType().ToString() + " [" + MouseOverMapObject.X + "," + MouseOverMapObject.Y + "]");

                    //inventory
                    for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            if (MyInventory.InventoryItems[x, y] != null)
                                if (InventoryItem.GetScreenRectangle(x, y).Contains(new Point(MouseState.X, MouseState.Y)))
                                {
                                    if (MyInventory.InventoryItems[x, y].Item.Name == "Shovel")
                                    {
                                        MyCommand = new Command("dig");
                                    }
                                    else if (MyInventory.InventoryItems[x, y].Item.Name == "Pile of dirt")
                                    {
                                        MyCommand = new Command("raise", MyInventory.InventoryItems[x, y].Item);
                                        MyCommandInputItem = MyInventory.InventoryItems[x, y];
                                    }
                                    else if (MyInventory.InventoryItems[x, y].Item.Name == "Pile of sand")
                                    {
                                        MyCommand = new Command("raise", MyInventory.InventoryItems[x, y].Item);
                                    }
                                }
                        }
                    }
                }

                //CLICK STRATUM BUTTON
                if (DateTime.Now - LastRelease > new TimeSpan(500))
                {
                    try
                    {
                        foreach (Stratum Stratum in ActiveStratums)
                        {
                            if (new Rectangle(Stratum.X, Stratum.Y, Stratum.Width, Stratum.Height).Contains(new Point(MouseState.X, MouseState.Y)))
                            {
                                foreach (StratumControl Control in Stratum.Controls.Where(Obj => Obj.Type == Statics.StratumControlType.BUTTON || Obj.Type == Statics.StratumControlType.TEXTBOX))
                                {
                                    if (new Rectangle(Stratum.X + (int)Control.Position.X, Stratum.Y + (int)Control.Position.Y, Control.Width, Control.Height).Contains(new Point(MouseState.X, MouseState.Y)))
                                    {
                                        if (Control.Type == Statics.StratumControlType.TEXTBOX)
                                        {
                                            foreach (StratumControl c in Stratum.Controls)
                                                c.Selected = false;

                                            Control.Selected = true;
                                            textBoxInput = Control.Text;
                                            textBoxTyping = true;
                                        }
                                        else
                                        {
                                            if (Control.Name == "btnExit")
                                                Exit();
                                            else if (Control.Name == "btnLogin")
                                            {
                                                LoginScreen l = (LoginScreen)Stratum;
                                                MethodInfo methodInf = typeof(LoginScreen).GetMethod(Control.Name);
                                                LoginDelegate ld = (LoginDelegate)Delegate.CreateDelegate(typeof(LoginDelegate), l, methodInf);
                                                ld.Invoke(ref Client, ref MyName);
                                                WaitForServerApproval(Content);
                                            }
                                            else if (Control.Name == "btnMap")
                                            {
                                                MethodInfo methodInf = typeof(MapTools).GetMethod(Control.Name);
                                                methodInf.Invoke(Stratum, new object[1] { Control.Text });
                                            }
                                            else if (Control.Name == "btnMapTools")
                                            {
                                                MethodInfo methodInf = typeof(MainMenu).GetMethod(Control.Name);
                                                methodInf.Invoke(Stratum, new object[4] { ActiveStratums, graphics, Content, Map });
                                            }
                                            else if (Control.Name == "btnBackToMenu")
                                            {
                                                MethodInfo methodInf = typeof(MapTools).GetMethod(Control.Name);
                                                methodInf.Invoke(Stratum, new object[3] { ActiveStratums, graphics, Content });
                                            }
                                            else if ((Control.Name == "btnSaveMap" && Stratum.GetType() == typeof(MapTools)) || (Control.Name == "btnLogin" && Stratum.GetType() == typeof(LoginScreen)))
                                                textBoxTyping = false;
                                            //else if (Control.Name == "btnSave" && Stratum.GetType() == typeof(MapTools))
                                            //    textBoxTyping = true;
                                            
                                            MethodInfo methodInfo = typeof(Stratum).GetMethod(Control.Name);

                                            if (Stratum.GetType() == typeof(MainMenu))
                                                methodInfo = typeof(MainMenu).GetMethod(Control.Name);
                                            else if (Stratum.GetType() == typeof(MapTools))
                                                methodInfo = typeof(MapTools).GetMethod(Control.Name);
                                            else if (Stratum.GetType() == typeof(Paperdoll))
                                                methodInfo = typeof(Paperdoll).GetMethod(Control.Name);

                                            methodInfo.Invoke(Stratum, null);
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.ToString());  }
                }

                LastRelease = DateTime.Now;
            }

            if (MouseState.LeftButton == ButtonState.Pressed)
            {
                if (LastRelease < PressedSince)
                {
                    if (DateTime.Now - PressedSince > new TimeSpan(0, 0, 0, 0, 25) 
                        && MouseState.X >= PreviousMouseState.X - 5 && MouseState.X <= PreviousMouseState.X + 5
                        && MouseState.Y >= PreviousMouseState.Y - 5 && MouseState.Y <= PreviousMouseState.Y + 5)
                    {
                        //DRAGGING START:
                        if (DragObject == null)
                        {
                            //CHECK FOR STRATUM DRAGGING:
                            foreach (Stratum Stratum in ActiveStratums)
                            {
                                if (new Rectangle(Stratum.X, Stratum.Y, Stratum.Width, Stratum.Height).Contains(new Point(MouseState.X, MouseState.Y)))
                                {
                                    DragObject = new DragObject(Stratum, new Vector2(MouseState.X - Stratum.X, MouseState.Y - Stratum.Y));

                                    foreach (StratumControl Control in Stratum.Controls.Where(Obj => Obj.Name != "Silhouette"))
                                    {
                                        if (new Rectangle(Stratum.X + (int)Control.Position.X, Stratum.Y + (int)Control.Position.Y, Control.Width, Control.Height).Contains(new Point(MouseState.X, MouseState.Y)))
                                            DragObject = null;
                                    }

                                    break;
                                }
                            }

                            if (DragObject == null)
                            {
                                //CHECK FOR EQUIPMENT DRAGGING
                                try
                                {
                                    Paperdoll paperdoll = (Paperdoll)ActiveStratums.Single(stratum => stratum.Title == "Paperdoll");
                                    foreach (StratumControl EquipmentSlot in ActiveStratums.Single(stratum => stratum.Title == "Paperdoll").Controls.Where(control => control.Name != "Silhouette"))
                                    {
                                        Rectangle EquipmentSlotRectangle = new Rectangle(paperdoll.X + (int)EquipmentSlot.Position.X, paperdoll.Y + (int)EquipmentSlot.Position.Y, EquipmentSlot.Width, EquipmentSlot.Height);

                                        if (EquipmentSlotRectangle.Contains(new Point(MouseState.X, MouseState.Y)))
                                        {
                                            try
                                            {
                                                Item EquipmentItem = paperdoll.Equipment.Single(kvp => kvp.Key == EquipmentSlot.Name).Value;
                                                DragObject = new DragObject(EquipmentItem, new Vector2(MouseState.X - EquipmentSlotRectangle.X, MouseState.Y - EquipmentSlotRectangle.Y));
                                                paperdoll.Equipment.Remove(EquipmentSlot.Name);
                                                break;
                                            }
                                            catch { break; }
                                        }
                                    }
                                }
                                catch { }

                                if (DragObject == null)
                                {
                                    //CHECK FOR INVENTORY ITEM DRAGGING:
                                    for (int x = 0; x <= Inventory.ColumnsShown - 1; x++)
                                    {
                                        for (int y = 0; y < 10; y++)
                                        {
                                            if (MyInventory.InventoryItems[x, y] != null)
                                                if (InventoryItem.GetScreenRectangle(x, y).Contains(new Point(MouseState.X, MouseState.Y)))
                                                {
                                                    DragObject = new DragObject(MyInventory.InventoryItems[x, y], new Vector2(MouseState.X - InventoryItem.GetScreenRectangle(x, y).X + 24, MouseState.Y - InventoryItem.GetScreenRectangle(x, y).Y + 144));
                                                    break;
                                                }
                                        }
                                    }

                                    if (DragObject == null)
                                    {
                                        //CHECK FOR MAP OBJECT DRAGGING:
                                        MapObject MouseOverMapObject = Map.Objects.Where(Obj => Obj.MouseOver == true).FirstOrDefault();

                                        if (MouseOverMapObject != null)
                                        {
                                            if (MouseOverMapObject.X > GameStateList[MyName].X + 1 || MouseOverMapObject.X < GameStateList[MyName].X - 1 ||
                                                MouseOverMapObject.Y > GameStateList[MyName].Y + 1 || MouseOverMapObject.Y < GameStateList[MyName].Y - 1)
                                                SystemMessages.Add(DateTime.Now, "TOO FAR AWAY.");
                                            else
                                            {
                                                DragObject = new DragObject(MouseOverMapObject, new Vector2(MouseState.X - (int)MouseOverMapObject.Position.X, MouseState.Y - (int)MouseOverMapObject.Position.Y));
                                                Map.Objects.Remove(MouseOverMapObject);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    PressedSince = DateTime.Now;
                }

                //CLICK INVENTORY EXPAND BUTTON:
                if (MyInventory.ExpandButtonRectangle.Contains(new Point(MouseState.X, MouseState.Y)))
                {
                    if (Inventory.ColumnsShown < 4)
                        Inventory.ColumnsShown += 1;
                    else
                        Inventory.ColumnsShown = 1;

                    MyInventory.UpdatePosition(graphics);
                }

                //TARGETTING:
                if (MyCommand != null && MyCommand.Target)
                {
                    Point[] MousePoints = new Point[2];

                    switch (MyCommand.TargetType)
                    {
                        case Statics.TargetType.TILE:
                            MyCommand.SendTarget(GameStateList[MyName].X, GameStateList[MyName].Y, Client, Map, GameStateList[MyName]);
                            MyCommand = null;
                            break;
                        case Statics.TargetType.CORNER:
                            if (MyCommand.Type == Statics.CommandType.DIGCORNER)
                            {
                                //you could check for max slope to dig here
                            }
                            else if (MyCommand.Type == Statics.CommandType.RAISECORNER)
                            {
                                //you could check for max slope to raise here
                            }

                            if ((ClosestPoint2Mouse.X == GameStateList[MyName].X && ClosestPoint2Mouse.Y == GameStateList[MyName].Y)
                            || (ClosestPoint2Mouse.X == GameStateList[MyName].X + 1 && ClosestPoint2Mouse.Y == GameStateList[MyName].Y)
                            || (ClosestPoint2Mouse.X == GameStateList[MyName].X && ClosestPoint2Mouse.Y == GameStateList[MyName].Y + 1)
                            || (ClosestPoint2Mouse.X == GameStateList[MyName].X + 1 && ClosestPoint2Mouse.Y == GameStateList[MyName].Y + 1))
                            {
                                if (MyCommand.Type == Statics.CommandType.DIGCORNER)
                                {
                                    //player gains 1 dirt

                                }
                                else if (MyCommand.Type == Statics.CommandType.RAISECORNER)
                                {
                                    //player loses 1 dirt
                                    //MyCommandInputItem.Amount -= 1;
                                    /*if (MyCommandInputItem.Amount <= 0) 
                                    {
                                        //delete item from MyInventory

                                    }*/
                                }

                                MyCommand.SendTarget(ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y, Client, Map, GameStateList[MyName]);
                                MyCommand = null;
                            }
                            else
                            {
                                SystemMessages.Add(DateTime.Now, "The target is out of range.");
                                MyCommand = null;
                            }
                            break;
                    }
                }
            }
        }

        private void HandleKeyboardInput()
        {
            if (DateTime.Now - LastKeyboardInput >= KeyboardInputDelay)
            {
                if (KeyboardState.IsKeyDown(Keys.Enter) && TextInput != "")
                {
                    if (TextInput == "/exit")
                    {
                        this.Exit();
                    }
                    else if (TextInput == "/hit")
                    {
                        GameStateList[MyName].AnimationCycle = Statics.AnimationCycle.HIT;
                    }
                    else if (TextInput.Substring(0, 1) == "/")
                    {
                        //Command
                        MyCommand = new Command(TextInput.Substring(1));
                    }
                    else
                    {
                        //Chat
                        NetOutgoingMessage outmsg = Client.CreateMessage();
                        outmsg.Write((byte)Statics.PacketTypes.TEXT);
                        outmsg.Write(TextInput);
                        Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                    }

                    TextInput = "";
                }
                else if (KeyboardState.IsKeyDown(Keys.F1))
                    MyCommand = new Command("digc");
                else if (KeyboardState.IsKeyDown(Keys.F2))
                    MyCommand = new Command("raic", new DirtPile(Content));
                else if (KeyboardState.IsKeyDown(Keys.F3))
                    MyCommand = new Command("digt");
                else if (KeyboardState.IsKeyDown(Keys.F4))
                    MyCommand = new Command("rait");
                else if (KeyboardState.IsKeyDown(Keys.F5))
                {
                    Map.ChangeDirection();
                    GameStateList[MyName].X = 49 - GameStateList[MyName].X;
                    GameStateList[MyName].Y = 49 - GameStateList[MyName].Y;
                    LastKeyboardInput = DateTime.Now;
                }
                else if (KeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    //if (AnimationDuration == new TimeSpan(0, 0, 0, 0, 60))
                    //{
                    //    AnimationDuration = new TimeSpan(0, 0, 0, 0, 20);
                    //}
                    //else
                    //{
                    //    AnimationDuration = new TimeSpan(0, 0, 0, 0, 60);
                    //}
                    //LastKeyboardInput = DateTime.Now;
                }
                else if (KeyboardState.IsKeyDown(Keys.F6))
                {
                    MapTile.Border = Math.Abs(MapTile.Border - 1);
                    LastKeyboardInput = DateTime.Now;
                }
                else if (KeyboardState.IsKeyDown(Keys.Tab))
                {
                    NetOutgoingMessage outmsg = Client.CreateMessage();
                    outmsg.Write((byte)Statics.PacketTypes.COMBATCHANGE);
                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);

                    if (GameStateList[MyName].Combat)
                    {
                        SystemMessages.Add(DateTime.Now, "COMBAT MODE = OFF");
                        GameStateList[MyName].CombatTarget = null;
                        foreach (Player p in GameStateList.Values)
                        {
                            p.TargetHover = false;
                            p.Targetted = false;
                        }
                    }
                    else
                        SystemMessages.Add(DateTime.Now, "COMBAT MODE = ON");

                    LastKeyboardInput = DateTime.Now;
                }
                else if (KeyboardState.IsKeyDown(Keys.Escape))
                {
                    bool display = true;
                    Stratum menu2remove = new Stratum();

                    foreach (Stratum stratum in ActiveStratums)
                    {
                        if (stratum.GetType() == typeof(MainMenu))
                        {
                            display = false;
                            menu2remove = stratum;
                        }
                    }

                    if (display)
                    {
                        ActiveStratums.Add(new MainMenu((graphics.PreferredBackBufferWidth / 2) - 125, (graphics.PreferredBackBufferHeight / 2) - 375, Content));
                        LastKeyboardInput = DateTime.Now;
                    }
                    else
                        ActiveStratums.Remove(menu2remove);

                    LastKeyboardInput = DateTime.Now;
                }
                else if (KeyboardState.IsKeyDown(Keys.LeftAlt) && KeyboardState.IsKeyDown(Keys.P))
                {
                    bool display = true;
                    Stratum menu2remove = new Stratum();

                    foreach (Stratum stratum in ActiveStratums)
                    {
                        if (stratum.GetType() == typeof(Paperdoll))
                        {
                            display = false;
                            menu2remove = stratum;
                        }
                    }

                    if (display)
                    {
                        ActiveStratums.Add(new Paperdoll((graphics.PreferredBackBufferWidth / 2) - 125, (graphics.PreferredBackBufferHeight / 2) - 375, Content, GameStateList[MyName].Equipment));
                        LastKeyboardInput = DateTime.Now;
                    }
                    else
                        ActiveStratums.Remove(menu2remove);

                    LastKeyboardInput = DateTime.Now;
                }
                else if (KeyboardState.IsKeyDown(Keys.LeftAlt) && KeyboardState.IsKeyDown(Keys.M))
                {
                    bool display = true;
                    Stratum menu2remove = new Stratum();

                    foreach (Stratum stratum in ActiveStratums)
                    {
                        if (stratum.GetType() == typeof(MapTools))
                        {
                            display = false;
                            menu2remove = stratum;
                        }
                    }

                    if (display)
                    {
                        ActiveStratums.Add(new MapTools((graphics.PreferredBackBufferWidth / 2) - 125, (graphics.PreferredBackBufferHeight / 2) - 375, Content, ref Map));
                        LastKeyboardInput = DateTime.Now;
                    }
                    else
                        ActiveStratums.Remove(menu2remove);

                    LastKeyboardInput = DateTime.Now;
                }
            }
        }

        private void Combat()
        {
            if (GameStateList[MyName].Combat)
            {
                foreach (Player p in GameStateList.Values)
                {
                    if (p.ScreenPosition.Contains(new Point(MouseState.X, MouseState.Y)))
                        p.TargetHover = true;
                    else
                        p.TargetHover = false;
                }

                if (GameStateList[MyName].CombatTarget != null && Math.Abs(GameStateList[MyName].CombatTarget.X - GameStateList[MyName].X) <= 1 && Math.Abs(GameStateList[MyName].CombatTarget.Y - GameStateList[MyName].Y) <= 1
                    && DateTime.Now - LastHit >= HitTimer)
                {
                    //send message to server to damage combat target
                    if (GameStateList[MyName].AnimationCycle == Statics.AnimationCycle.NONE)
                    {
                        //GameStateList[MyName].AnimationCycle = Statics.AnimationCycle.HIT;
                        NetOutgoingMessage outmsg = Client.CreateMessage();
                        outmsg.Write((byte)Statics.PacketTypes.MOBILEHIT);
                        Player target = (Player)GameStateList[MyName].CombatTarget;
                        outmsg.Write(target.UserName);
                        outmsg.Write(10);
                        Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                        LastHit = DateTime.Now;

                        if (target.HP - 10 <= 0)
                            GameStateList[MyName].CombatTarget = null;
                    }
                }
            }
        }

        private void CheckPlayerMovement()
        {
            if (GameStateList[MyName].Animation == "" && GameStateList[MyName].HP > 0)
            {
                Statics.MoveDirection MoveDir = Statics.MoveDirection.NONE;

                if (KeyboardState.IsKeyDown(Keys.NumPad7))
                    MoveDir = Statics.MoveDirection.UPLEFT;
                else if (KeyboardState.IsKeyDown(Keys.NumPad8) || KeyboardState.IsKeyDown(Keys.Up))
                    MoveDir = Statics.MoveDirection.UP;
                else if (KeyboardState.IsKeyDown(Keys.NumPad9))
                    MoveDir = Statics.MoveDirection.UPRIGHT;
                else if (KeyboardState.IsKeyDown(Keys.NumPad4) || KeyboardState.IsKeyDown(Keys.Left))
                    MoveDir = Statics.MoveDirection.LEFT;
                else if (KeyboardState.IsKeyDown(Keys.NumPad6) || KeyboardState.IsKeyDown(Keys.Right))
                    MoveDir = Statics.MoveDirection.RIGHT;
                else if (KeyboardState.IsKeyDown(Keys.NumPad1))
                    MoveDir = Statics.MoveDirection.DOWNLEFT;
                else if (KeyboardState.IsKeyDown(Keys.NumPad2) || KeyboardState.IsKeyDown(Keys.Down))
                    MoveDir = Statics.MoveDirection.DOWN;
                else if (KeyboardState.IsKeyDown(Keys.NumPad3))
                    MoveDir = Statics.MoveDirection.DOWNRIGHT;
                else if (MouseState.RightButton == ButtonState.Pressed)
                {
                    if (MouseTileCoords.Y < GameStateList[MyName].Y && MouseTileCoords.X < GameStateList[MyName].X)
                    { MoveDir = Statics.MoveDirection.UP; }
                    else if (MouseTileCoords.Y > GameStateList[MyName].Y && MouseTileCoords.X > GameStateList[MyName].X)
                    { MoveDir = Statics.MoveDirection.DOWN; }
                    else if (MouseTileCoords.Y > GameStateList[MyName].Y && MouseTileCoords.X < GameStateList[MyName].X)
                    { MoveDir = Statics.MoveDirection.LEFT; }
                    else if (MouseTileCoords.Y < GameStateList[MyName].Y && MouseTileCoords.X > GameStateList[MyName].X)
                    { MoveDir = Statics.MoveDirection.RIGHT; }
                    else if (MouseTileCoords.X < GameStateList[MyName].X)
                        MoveDir = Statics.MoveDirection.UPLEFT;
                    else if (MouseTileCoords.Y < GameStateList[MyName].Y)
                        MoveDir = Statics.MoveDirection.UPRIGHT;
                    else if (MouseTileCoords.Y > GameStateList[MyName].Y)
                        MoveDir = Statics.MoveDirection.DOWNLEFT;
                    else if (MouseTileCoords.X > GameStateList[MyName].X)
                        MoveDir = Statics.MoveDirection.DOWNRIGHT;
                }

                if (MoveDir != Statics.MoveDirection.NONE)
                {
                    GameStateList[MyName].AnimationCycle = Statics.AnimationCycle.RUN;
                    GameStateList[MyName].PreviousX = GameStateList[MyName].X;
                    GameStateList[MyName].PreviousY = GameStateList[MyName].Y;
                    GameStateList[MyName].PreviousZ = GameStateList[MyName].Z;

                    if (MoveDir == Statics.MoveDirection.UP)
                    { GameStateList[MyName].Y--; GameStateList[MyName].X--; GameStateList[MyName].Animation = "North"; GameStateList[MyName].Direction = Statics.Direction.NORTH; }
                    if (MoveDir == Statics.MoveDirection.DOWN)
                    { GameStateList[MyName].X++; GameStateList[MyName].Y++; GameStateList[MyName].Animation = "South"; GameStateList[MyName].Direction = Statics.Direction.SOUTH; }
                    if (MoveDir == Statics.MoveDirection.LEFT)
                    { GameStateList[MyName].X--; GameStateList[MyName].Y++; GameStateList[MyName].Animation = "West"; GameStateList[MyName].Direction = Statics.Direction.WEST; }
                    if (MoveDir == Statics.MoveDirection.RIGHT)
                    { GameStateList[MyName].X++; GameStateList[MyName].Y--; GameStateList[MyName].Animation = "East"; GameStateList[MyName].Direction = Statics.Direction.EAST; }
                    if (MoveDir == Statics.MoveDirection.UPLEFT)
                    { GameStateList[MyName].X--; GameStateList[MyName].Animation = "NorthWest"; GameStateList[MyName].Direction = Statics.Direction.NORTHWEST; }
                    if (MoveDir == Statics.MoveDirection.UPRIGHT)
                    { GameStateList[MyName].Y--; GameStateList[MyName].Animation = "NorthEast"; GameStateList[MyName].Direction = Statics.Direction.NORTHEAST; }
                    if (MoveDir == Statics.MoveDirection.DOWNLEFT)
                    { GameStateList[MyName].Y++; GameStateList[MyName].Animation = "SouthWest"; GameStateList[MyName].Direction = Statics.Direction.SOUTHWEST; }
                    if (MoveDir == Statics.MoveDirection.DOWNRIGHT)
                    { GameStateList[MyName].X++; GameStateList[MyName].Animation = "SouthEast"; GameStateList[MyName].Direction = Statics.Direction.SOUTHEAST; }

                    int X; int Y;

                    if (GameStateList[MyName].X < 0)
                        X = (Map.MaxX + 1) + (GameStateList[MyName].X);
                    else if (GameStateList[MyName].X > Map.MaxX)
                        X = (GameStateList[MyName].X) - (Map.MaxX + 1);
                    else
                        X = GameStateList[MyName].X;

                    if (GameStateList[MyName].Y < 0)
                        Y = (Map.MaxY + 1) + (GameStateList[MyName].Y);
                    else if (GameStateList[MyName].Y > Map.MaxY)
                        Y = (GameStateList[MyName].Y) - (Map.MaxY + 1);
                    else
                        Y = GameStateList[MyName].Y;

                    GameStateList[MyName].X = X;
                    GameStateList[MyName].Y = Y;

                    if (Map.Tiles[X, Y].Type == Statics.TileType.WATER)
                    //HEIGHT CHECK --> || (int)Map.Tiles[GameStateList[MyName].X, GameStateList[MyName].Y].AverageZ > GameStateList[MyName].Z + 25)
                    {
                        GameStateList[MyName].X = GameStateList[MyName].PreviousX;
                        GameStateList[MyName].Y = GameStateList[MyName].PreviousY;
                        GameStateList[MyName].Animation = "";
                        MoveDir = Statics.MoveDirection.NONE;
                    }
                    else
                    {
                        GameStateList[MyName].Z = (int)Map.Tiles[GameStateList[MyName].X, GameStateList[MyName].Y].AverageZ;
                        NetOutgoingMessage outmsg = Client.CreateMessage();
                        outmsg.Write((byte)Statics.PacketTypes.MOVE);
                        outmsg.Write((byte)MoveDir);
                        Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                        MoveDir = Statics.MoveDirection.NONE;
                        GameStateList[MyName].Update();
                    }
                }
            }
        }

        private void UpdateMap()
        {
            if (loggedIn)
            {
                int ClosestX = 0; int ClosestY = 0;
                Map.Tiles[ClosestX, ClosestY].DistanceToMouse = 999999;
                for (int x = 0; x <= 40; x++)
                {
                    int tileX = x - 20;
                    for (int y = 0; y <= 40; y++)
                    {
                        int tileY = y - 20;
                        int X; int Y;

                        if (GameStateList[MyName].X + tileX < 0)
                            X = (Map.MaxX + 1) + (GameStateList[MyName].X + tileX);
                        else if (GameStateList[MyName].X + tileX > Map.MaxX)
                            X = (GameStateList[MyName].X + tileX) - (Map.MaxX + 1);
                        else
                            X = GameStateList[MyName].X + tileX;

                        if (GameStateList[MyName].Y + tileY < 0)
                            Y = (Map.MaxY + 1) + (GameStateList[MyName].Y + tileY);
                        else if (GameStateList[MyName].Y + tileY > Map.MaxY)
                            Y = (GameStateList[MyName].Y + tileY) - (Map.MaxY + 1);
                        else
                            Y = GameStateList[MyName].Y + tileY;

                        if (Map.Tiles[X, Y] != null)
                        {
                            Map.Tiles[X, Y].Update(tileX, tileY, graphics, MouseState);
                            if (Map.Tiles[X, Y].DistanceToMouse < Map.Tiles[ClosestX, ClosestY].DistanceToMouse)
                            {
                                ClosestX = X;
                                ClosestY = Y;
                                if (ClosestX > Map.MaxX)
                                    ClosestX = Map.MaxX;
                                else if (ClosestX < 0)
                                    ClosestX = 0;
                                else if (ClosestY > Map.MaxY)
                                    ClosestY = Map.MaxY;
                                else if (ClosestY < 0)
                                    ClosestY = 0;
                            }
                        }
                    }
                }

                ClosestPoint2Mouse = new Point(ClosestX, ClosestY);

                //TILEPICKING
                if (ClosestPoint2Mouse.X > 0 && ClosestPoint2Mouse.Y > 0 && ClosestPoint2Mouse.X < Map.MaxX && ClosestPoint2Mouse.Y < Map.MaxY)
                {
                    if (Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y].pointInTile(new Point(MouseState.X, MouseState.Y)))
                    {
                        MouseTileCoords = new Point(ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y);
                        if (TilePickingOn)
                            Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y].MouseOver = true;
                    }
                    else if (Map.Tiles[ClosestPoint2Mouse.X - 1, ClosestPoint2Mouse.Y].pointInTile(new Point(MouseState.X, MouseState.Y)))
                    {
                        MouseTileCoords = new Point(ClosestPoint2Mouse.X - 1, ClosestPoint2Mouse.Y);
                        if (TilePickingOn)
                            Map.Tiles[ClosestPoint2Mouse.X - 1, ClosestPoint2Mouse.Y].MouseOver = true;
                    }
                    else if (Map.Tiles[ClosestPoint2Mouse.X - 1, ClosestPoint2Mouse.Y - 1].pointInTile(new Point(MouseState.X, MouseState.Y)))
                    {
                        MouseTileCoords = new Point(ClosestPoint2Mouse.X - 1, ClosestPoint2Mouse.Y - 1);
                        if (TilePickingOn)
                            Map.Tiles[ClosestPoint2Mouse.X - 1, ClosestPoint2Mouse.Y - 1].MouseOver = true;
                    }
                    else if (Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y - 1].pointInTile(new Point(MouseState.X, MouseState.Y)))
                    {
                        MouseTileCoords = new Point(ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y - 1);
                        if (TilePickingOn)
                            Map.Tiles[ClosestPoint2Mouse.X, ClosestPoint2Mouse.Y - 1].MouseOver = true;
                    }
                }

                //RUNNING/WALKING (depends on the distance between the mouse and the player)
                //if (MouseTileCoords.X > GameStateList[MyName].X + 2 || MouseTileCoords.Y > GameStateList[MyName].Y + 2 ||
                //    MouseTileCoords.X < GameStateList[MyName].X - 2 || MouseTileCoords.Y < GameStateList[MyName].Y - 2)
                //    AnimationDuration = new TimeSpan(0, 0, 0, 0, 20);
                //else
                //    AnimationDuration = new TimeSpan(0, 0, 0, 0, 60);

                //UPDATE MAP OBJECTS
                foreach (MapObject Object in Map.Objects)
                {
                    Object.Update(Map);
                }

                //CHECK MOUSE OVER MAP OBJECTS
                //foreach (MapObject Object in Map.Objects.Where(Obj => Obj.X == MouseTileCoords.X && Obj.Y == MouseTileCoords.Y))
                foreach (MapObject Object in Map.Objects)
                {
                    if (Object.pointInPolygon(new Point(MouseState.X, MouseState.Y)))
                        Object.MouseOver = true;
                }
            }
        }

        private void WaitForServerApproval(ContentManager Content)
        {
            bool ReceivedWorldState = false;
            //bool ReceivedMap = false;
            NetIncomingMessage inc;

            while (!ReceivedWorldState)
            {
                if ((inc = Client.ReadMessage()) != null)
                {
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            if (inc.ReadByte() == (byte)Statics.PacketTypes.LOGIN)
                            {
                                GameStateList.Clear();
                                int count = 0;
                                count = inc.ReadInt32();

                                for (int i = 0; i < count; i++)
                                {
                                    Player p = new Player();
                                    inc.ReadAllProperties(p);
                                    p.spriteSheet = spriteSheetHumanMale;
                                    p.X = inc.ReadInt32();
                                    p.Y = inc.ReadInt32();
                                    GameStateList.Add(p.UserName, p);
                                }

                                for (int x = 0; x <= Map.MaxX; x++)
                                {
                                    for (int y = 0; y <= Map.MaxY; y++)
                                    {
                                        Statics.TileType type = (Statics.TileType)inc.ReadByte();

                                        if (type != Statics.TileType.NULL)
                                        {
                                            int[] corners = new int[4] { inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32() };
                                            Map.Tiles[x, y] = new MapTile(corners, x, y, type);
                                        }
                                        else
                                        {
                                            Map.Tiles[x, y] = null;
                                        }
                                    }
                                }

                                //remove loginscreen
                                Stratum menu2remove = new Stratum();

                                foreach (Stratum stratum in ActiveStratums)
                                    if (stratum.GetType() == typeof(LoginScreen))
                                        menu2remove = stratum;

                                ActiveStratums.Remove(menu2remove);
                                loggedIn = true;

                                Window.AllowUserResizing = true;
                                ReceivedWorldState = true;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void CheckServerMessages(ContentManager Content)
        {
            NetIncomingMessage inc;

            while ((inc = Client.ReadMessage()) != null)
            {
                if (inc.MessageType == NetIncomingMessageType.Data)
                {
                    byte PacketType = inc.ReadByte();

                    if (PacketType == (byte)Statics.PacketTypes.WORLDSTATE)
                    {
                        //Game State Update (only used for chat messages right now)
                        int jii = 0;
                        jii = inc.ReadInt32();
                        for (int i = 0; i < jii; i++)
                        {
                            GameStateList[inc.ReadString()].Message = inc.ReadString();
                        }
                    }
                    else if (PacketType == (byte)Statics.PacketTypes.NEWPLAYER)
                    {
                        Player p = new Player();
                        inc.ReadAllProperties(p);
                        //p.Texture = Content.Load<Texture2D>("character");
                        p.spriteSheet = spriteSheetHumanMale;
                        p.X = inc.ReadInt32();
                        p.Y = inc.ReadInt32();
                        GameStateList.Add(p.UserName, p);
                    }
                    else if (PacketType == (byte)Statics.PacketTypes.MOVE)
                    {
                        string name = inc.ReadString();

                        if (name != MyName)
                        {
                            byte b = inc.ReadByte();
                            GameStateList[name].PreviousX = GameStateList[name].X;
                            GameStateList[name].PreviousY = GameStateList[name].Y;
                            GameStateList[name].PreviousZ = GameStateList[name].Z;

                            if ((byte)Statics.MoveDirection.UP == b)
                            { GameStateList[name].Y--; GameStateList[name].X--; GameStateList[name].Animation = "North"; GameStateList[name].Direction = Statics.Direction.NORTH; } 
                            if ((byte)Statics.MoveDirection.DOWN == b)
                            { GameStateList[name].X++; GameStateList[name].Y++; GameStateList[name].Animation = "South"; GameStateList[name].Direction = Statics.Direction.SOUTH; } 
                            if ((byte)Statics.MoveDirection.LEFT == b)
                            { GameStateList[name].X--; GameStateList[name].Y++; GameStateList[name].Animation = "West"; GameStateList[name].Direction = Statics.Direction.WEST; }
                            if ((byte)Statics.MoveDirection.RIGHT == b)
                            { GameStateList[name].X++; GameStateList[name].Y--; GameStateList[name].Animation = "East"; GameStateList[name].Direction = Statics.Direction.EAST; }
                            if ((byte)Statics.MoveDirection.UPLEFT == b)
                            { GameStateList[name].X--; GameStateList[name].Animation = "NorthWest"; GameStateList[name].Direction = Statics.Direction.NORTHWEST; }
                            if ((byte)Statics.MoveDirection.UPRIGHT == b)
                            { GameStateList[name].Y--; GameStateList[name].Animation = "NorthEast"; GameStateList[name].Direction = Statics.Direction.NORTHEAST; }
                            if ((byte)Statics.MoveDirection.DOWNLEFT == b)
                            { GameStateList[name].Y++; GameStateList[name].Animation = "SouthWest"; GameStateList[name].Direction = Statics.Direction.SOUTHWEST; }
                            if ((byte)Statics.MoveDirection.DOWNRIGHT == b)
                            { GameStateList[name].X++; GameStateList[name].Animation = "SouthEast"; GameStateList[name].Direction = Statics.Direction.SOUTHEAST; }

                            GameStateList[name].Z = (int)Map.Tiles[GameStateList[name].X, GameStateList[name].Y].AverageZ;
                            GameStateList[name].AnimationCycle = Statics.AnimationCycle.RUN;
                            GameStateList[name].Update();
                        }
                    }
                    else if (PacketType == (byte)Statics.PacketTypes.MOBILEHIT)
                    {
                        string aggressor = inc.ReadString();
                        string target = inc.ReadString();
                        int hp = inc.ReadInt32();
                        int hplost = GameStateList[target].HP - hp;
                        GameStateList[target].HP = hp;
                        try
                        {
                            SystemMessages.Add(DateTime.Now, aggressor + " hit " + target + " for " + hplost + "HP.");
                        }
                        catch { }

                        GameStateList[aggressor].AnimationCycle = Statics.AnimationCycle.HIT;
                        if (GameStateList[target].HP <= 0)
                        {
                            GameStateList[target].AnimationCycle = Statics.AnimationCycle.DIE;
                            GameStateList[target].Targetted = false;
                        }
                        else
                            GameStateList[target].AnimationCycle = Statics.AnimationCycle.GETHIT;
                    }
                    else if (PacketType == (byte)Statics.PacketTypes.MOBILETARGET)
                    {
                        string aggressor = inc.ReadString();
                        if (aggressor != MyName)
                        {
                            SystemMessages.Add(DateTime.Now, aggressor + " is attacking you!");
                        }
                    }
                    else if (PacketType == (byte)Statics.PacketTypes.COMBATCHANGE)
                    {
                        string player = inc.ReadString();

                        if (GameStateList[player].Combat)
                            GameStateList[player].Combat = false;
                        else
                            GameStateList[player].Combat = true;
                    }
                    else if (PacketType == (byte)Statics.PacketTypes.COMMAND)
                    {
                        Byte CommandType = inc.ReadByte();
                        int x = inc.ReadInt32();
                        int y = inc.ReadInt32();
                        int z = inc.ReadInt32();
                        string playerName = inc.ReadString();
                        GameStateList[playerName].PreviousZ = GameStateList[playerName].Z; GameStateList[playerName].Z = z;

                        if (CommandType == (byte)Statics.CommandType.DIGCORNER)
                        {
                            if (Map.Tiles[x, y] != null)
                            {
                                Map.Tiles[x, y].CornerHeights[0] += 1;
                                Map.Tiles[x, y].Type = Statics.TileType.DIRT1;
                            }

                            if (Map.Tiles[x, y - 1] != null)
                            {
                                Map.Tiles[x, y - 1].CornerHeights[3] += 1;
                                Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;
                            }

                            if (Map.Tiles[x - 1, y] != null)
                            {
                                Map.Tiles[x - 1, y].CornerHeights[1] += 1;
                                Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;
                            }

                            if (Map.Tiles[x - 1, y - 1] != null)
                            {
                                Map.Tiles[x - 1, y - 1].CornerHeights[2] += 1;
                                Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;
                            }
                        }
                        else if (CommandType == (byte)Statics.CommandType.RAISECORNER)
                        {
                            if (Map.Tiles[x, y] != null)
                            {
                                Map.Tiles[x, y].CornerHeights[0] -= 1;
                                Map.Tiles[x, y].Type = Statics.TileType.DIRT1;
                            }

                            if (Map.Tiles[x, y - 1] != null)
                            {
                                Map.Tiles[x, y - 1].CornerHeights[3] -= 1;
                                Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;
                            }

                            if (Map.Tiles[x - 1, y] != null)
                            {
                                Map.Tiles[x - 1, y].CornerHeights[1] -= 1;
                                Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;
                            }

                            if (Map.Tiles[x - 1, y - 1] != null)
                            {
                                Map.Tiles[x - 1, y - 1].CornerHeights[2] -= 1;
                                Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;
                            }
                        }
                        else if (CommandType == (byte)Statics.CommandType.DIGTILE)
                        {
                            Map.Tiles[x, y].CornerHeights[0] += 1;
                            Map.Tiles[x, y].CornerHeights[1] += 1;
                            Map.Tiles[x, y].CornerHeights[2] += 1;
                            Map.Tiles[x, y].CornerHeights[3] += 1;
                            Map.Tiles[x, y].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x + 1, y + 1].CornerHeights[0] += 1;
                            Map.Tiles[x + 1, y + 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x + 1, y].CornerHeights[0] += 1;
                            Map.Tiles[x + 1, y].CornerHeights[3] += 1;
                            Map.Tiles[x + 1, y].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x + 1, y - 1].CornerHeights[3] += 1;
                            Map.Tiles[x + 1, y - 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x, y - 1].CornerHeights[2] += 1;
                            Map.Tiles[x, y - 1].CornerHeights[3] += 1;
                            Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x - 1, y - 1].CornerHeights[2] += 1;
                            Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x - 1, y].CornerHeights[1] += 1;
                            Map.Tiles[x - 1, y].CornerHeights[2] += 1;
                            Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x - 1, y + 1].CornerHeights[1] += 1;
                            Map.Tiles[x - 1, y + 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x, y + 1].CornerHeights[0] += 1;
                            Map.Tiles[x, y + 1].CornerHeights[1] += 1;
                            Map.Tiles[x, y + 1].Type = Statics.TileType.DIRT1;
                        }
                        else if (CommandType == (byte)Statics.CommandType.RAISETILE)
                        {
                            Map.Tiles[x, y].CornerHeights[0] -= 1;
                            Map.Tiles[x, y].CornerHeights[1] -= 1;
                            Map.Tiles[x, y].CornerHeights[2] -= 1;
                            Map.Tiles[x, y].CornerHeights[3] -= 1;
                            Map.Tiles[x, y].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x + 1, y + 1].CornerHeights[0] -= 1;
                            Map.Tiles[x + 1, y + 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x + 1, y].CornerHeights[0] -= 1;
                            Map.Tiles[x + 1, y].CornerHeights[3] -= 1;
                            Map.Tiles[x + 1, y].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x + 1, y - 1].CornerHeights[3] -= 1;
                            Map.Tiles[x + 1, y - 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x, y - 1].CornerHeights[2] -= 1;
                            Map.Tiles[x, y - 1].CornerHeights[3] -= 1;
                            Map.Tiles[x, y - 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x - 1, y - 1].CornerHeights[2] -= 1;
                            Map.Tiles[x - 1, y - 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x - 1, y].CornerHeights[1] -= 1;
                            Map.Tiles[x - 1, y].CornerHeights[2] -= 1;
                            Map.Tiles[x - 1, y].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x - 1, y + 1].CornerHeights[1] -= 1;
                            Map.Tiles[x - 1, y + 1].Type = Statics.TileType.DIRT1;

                            Map.Tiles[x, y + 1].CornerHeights[0] -= 1;
                            Map.Tiles[x, y + 1].CornerHeights[1] -= 1;
                            Map.Tiles[x, y + 1].Type = Statics.TileType.DIRT1;
                        }
                    }
                }
            }
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = GraphicsDevice.Viewport.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Viewport.Height;
            graphics.ApplyChanges();
            MyInventory.UpdatePosition(graphics);
            TextInputPosition.X = (graphics.PreferredBackBufferWidth / 10) + 10;
            TextInputPosition.Y = graphics.PreferredBackBufferHeight - 25 - (graphics.PreferredBackBufferHeight / 10);
            //TextBox.Width = graphics.PreferredBackBufferWidth - 20 - (graphics.PreferredBackBufferWidth / 5);
        } 
    }
}
