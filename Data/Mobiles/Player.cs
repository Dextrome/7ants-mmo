using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Data.Graphics;
using Data.World;

namespace Data.Mobiles
{
    public class Player: Mobile
    {
        private static int PlayerTextureWidth = 96;
        private static int PlayerTextureHeight = 192;
        public bool TargetHover = false;
        public bool Targetted = false;
        public Rectangle ScreenPosition;       
        public string Message { get; set; }
        public DateTime MessageTime = DateTime.Now;
        public TimeSpan MessageTimeToPass = new TimeSpan(0, 0, 0, 5, 0);
        public NetConnection Connection { get; set; }
        public _7ANTSMMO.SpriteSheetRuntime.SpriteSheet spriteSheet;
        Boolean reloop = false;
        TimeSpan NonMovingAnimationDelay = new TimeSpan(0, 0, 0, 0, 100);
        DateTime LastNonMovingAnimationStep = DateTime.Now;
        public string UserName { get; set; }

        public Player(string name, int x, int y, int z, NetConnection conn, int r, int g, int b, _7ANTSMMO.SpriteSheetRuntime.SpriteSheet SpriteSheet)
        {
            UserName = name;
            X = x;
            Y = y;
            Z = z;
            Connection = conn;
            colorR = r;
            colorB = b;
            colorG = g;
            spriteSheet = SpriteSheet;
        }

        public Player(string name, int x, int y, int z, NetConnection conn, int r, int g, int b)
        {
            UserName = name;
            X = x;
            Y = y;
            Z = z;
            Connection = conn;
            colorR = r;
            colorB = b;
            colorG = g;
        }

        public Player(string name, int x, int y, int z, NetConnection conn, Color col, _7ANTSMMO.SpriteSheetRuntime.SpriteSheet SpriteSheet)
        {
            UserName = name;
            X = x;
            Y = y;
            Z = z;
            Connection = conn;
            spriteSheet = SpriteSheet;
        }
        public Player(string name, int x, int y, int z, _7ANTSMMO.SpriteSheetRuntime.SpriteSheet SpriteSheet)
        {
            UserName = name;
            X = x;
            Y = y;
            Z = z;
            spriteSheet = SpriteSheet;
        }
        public Player() { }

        public void Update()
        {
            Rectangle rec; int Zdiff = PreviousZ - Z;

            if (Animation != "waiting")
            {
                string sourceRec = "";

                switch (Direction)
                {
                    case Statics.Direction.WEST:      sourceRec = "000"; break;
                    case Statics.Direction.NORTHWEST: sourceRec = "100"; break;
                    case Statics.Direction.NORTH:     sourceRec = "200"; break;
                    case Statics.Direction.NORTHEAST: sourceRec = "300"; break;
                    case Statics.Direction.EAST:      sourceRec = "400"; break;
                    case Statics.Direction.SOUTHEAST: sourceRec = "500"; break;
                    case Statics.Direction.SOUTH:     sourceRec = "600"; break;
                    case Statics.Direction.SOUTHWEST: sourceRec = "700"; break;
                }

                if (AnimationCycle == Statics.AnimationCycle.NONE)
                {
                    if (HP > 0)
                    {
                        if (Combat)
                        {
                            if (DateTime.Now - LastNonMovingAnimationStep > NonMovingAnimationDelay)
                            {
                                int i = 0;

                                if (reloop)
                                    i = 4 - AnimationStep;
                                else
                                    i = 1 + AnimationStep;

                                sourceRec += "0" + i.ToString();
                                rec = spriteSheet.SourceRectangle(sourceRec);
                                AnimationOffset = new Vector2(0, 0);
                                AnimationPosition = new Vector2(rec.X, rec.Y);
                                AnimationStep += 1;

                                if (AnimationStep > 3)
                                {
                                    AnimationStep = 0;
                                    if (reloop)
                                        reloop = false;
                                    else
                                        reloop = true;
                                }

                                LastNonMovingAnimationStep = DateTime.Now;
                            }
                        }
                        else
                        {
                            reloop = false;
                            AnimationStep = 0;
                            sourceRec += "33";
                            rec = spriteSheet.SourceRectangle(sourceRec);
                            AnimationPosition = new Vector2(rec.X, rec.Y);
                            AnimationOffset = new Vector2(0, 0);
                        }
                    }
                    else
                    {
                        reloop = false;
                        AnimationStep = 0;
                        sourceRec += "24";
                        rec = spriteSheet.SourceRectangle(sourceRec);
                        AnimationPosition = new Vector2(rec.X, rec.Y);
                        AnimationOffset = new Vector2(0, 0);
                    }
                }
                else if (AnimationCycle == Statics.AnimationCycle.HIT)
                {
                    if (AnimationStep < 4)
                    {
                        if (DateTime.Now - LastNonMovingAnimationStep > NonMovingAnimationDelay)
                        {
                            int i = 13 + AnimationStep;
                            sourceRec += i.ToString();
                            rec = spriteSheet.SourceRectangle(sourceRec);
                            AnimationPosition = new Vector2(rec.X, rec.Y);
                            AnimationStep++;
                            LastNonMovingAnimationStep = DateTime.Now;
                        }
                    }
                    else
                    {
                        AnimationStep = 0;
                        AnimationCycle = Statics.AnimationCycle.NONE;
                    }
                }
                else if (AnimationCycle == Statics.AnimationCycle.GETHIT)
                {
                    if (AnimationStep < 2)
                    {
                        if (DateTime.Now - LastNonMovingAnimationStep > NonMovingAnimationDelay)
                        {
                            int i = 18 - AnimationStep;
                            sourceRec += i.ToString();
                            rec = spriteSheet.SourceRectangle(sourceRec);
                            AnimationPosition = new Vector2(rec.X, rec.Y);
                            AnimationStep++;
                            LastNonMovingAnimationStep = DateTime.Now;
                        }
                    }
                    else
                    {
                        AnimationStep = 0;
                        AnimationCycle = Statics.AnimationCycle.NONE;
                    }
                }
                else if (AnimationCycle == Statics.AnimationCycle.DIE)
                {
                    if (AnimationStep < 6)
                    {
                        if (DateTime.Now - LastNonMovingAnimationStep > NonMovingAnimationDelay)
                        {
                            int i = 19 + AnimationStep;
                            sourceRec += i.ToString();
                            rec = spriteSheet.SourceRectangle(sourceRec);
                            AnimationPosition = new Vector2(rec.X, rec.Y);
                            AnimationStep++;
                            LastNonMovingAnimationStep = DateTime.Now;
                        }
                    }
                    else
                    {
                        AnimationStep = 0;
                        AnimationCycle = Statics.AnimationCycle.NONE;
                    }
                }
                else if (AnimationCycle == Statics.AnimationCycle.RUN)
                {
                    if (AnimationStep < 8)
                    {
                        int i = 5 + AnimationStep;

                        if (i < 10)
                            sourceRec += "0" + i.ToString();
                        else
                            sourceRec += i.ToString();

                        rec = spriteSheet.SourceRectangle(sourceRec);
                        AnimationPosition = new Vector2(rec.X, rec.Y);

                        switch (Direction)
                        {
                            case Statics.Direction.NORTH:
                                AnimationOffset = new Vector2(0, -MapTile.Height + Zdiff) + new Vector2(0, AnimationStep * ((MapTile.Height - Zdiff) / 8));
                                break;
                            case Statics.Direction.SOUTH:
                                AnimationOffset = new Vector2(0, MapTile.Height + Zdiff) + new Vector2(0, -AnimationStep * ((MapTile.Height + Zdiff) / 8));
                                break;
                            case Statics.Direction.EAST:
                                AnimationOffset = new Vector2(MapTile.Width, 0 + Zdiff) + new Vector2(-AnimationStep * (MapTile.Width / 8), AnimationStep * ((0 - Zdiff) / 8));
                                break;
                            case Statics.Direction.WEST:
                                AnimationOffset = new Vector2(-MapTile.Width, 0 + Zdiff) + new Vector2(AnimationStep * (MapTile.Width / 8), AnimationStep * ((0 - Zdiff) / 8));
                                break;
                            case Statics.Direction.NORTHEAST:
                                AnimationOffset = new Vector2((MapTile.Width / 2), -(MapTile.Height / 2) + Zdiff) + new Vector2(-AnimationStep * ((MapTile.Width / 2) / 8), AnimationStep * (((MapTile.Height / 2) - Zdiff) / 8));
                                break;
                            case Statics.Direction.NORTHWEST:
                                AnimationOffset = new Vector2(-(MapTile.Width / 2), -(MapTile.Height / 2) + Zdiff) + new Vector2(AnimationStep * ((MapTile.Width / 2) / 8), AnimationStep * (((MapTile.Height / 2) - Zdiff) / 8));
                                break;
                            case Statics.Direction.SOUTHEAST:
                                AnimationOffset = new Vector2((MapTile.Width / 2), (MapTile.Height / 2) + Zdiff) + new Vector2(-AnimationStep * ((MapTile.Width / 2) / 8), -AnimationStep * (((MapTile.Height / 2) + Zdiff) / 8));
                                break;
                            case Statics.Direction.SOUTHWEST:
                                AnimationOffset = new Vector2(-(MapTile.Width / 2), (MapTile.Height / 2) + Zdiff) + new Vector2(AnimationStep * ((MapTile.Width / 2) / 8), -AnimationStep * (((MapTile.Height / 2) + Zdiff) / 8));
                                break;
                        }

                        AnimationStep++;
                    }
                    else
                    {
                        Animation = "";
                        AnimationStep = 0;
                        AnimationCycle = Statics.AnimationCycle.NONE;
                    }
                }
            }
        }

        public void Draw(GraphicsDeviceManager GraphicsDeviceManager, Boolean OtherPlayer, Vector2 OtherPlayerOffset, Vector2 MainPlayerAnimationOffset)
        {
            if (spriteSheet != null)
            {
                VertexPositionColorTexture[] verts;
                verts = new VertexPositionColorTexture[6];
                verts[0].Color = Color.White;
                verts[1].Color = Color.White;
                verts[2].Color = Color.White;
                verts[3].Color = Color.White;
                verts[4].Color = Color.White;
                verts[5].Color = Color.White;

                if (OtherPlayer)
                {
                    if (TargetHover)
                    {
                        verts[0].Color = Color.Red;
                        verts[1].Color = Color.Red;
                        verts[2].Color = Color.Red;
                        verts[3].Color = Color.Red;
                        verts[4].Color = Color.Red;
                        verts[5].Color = Color.Red;
                    }
                    Position = new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth / 2, GraphicsDeviceManager.PreferredBackBufferHeight / 2) + new Vector2(-48, -144 - Z) + OtherPlayerOffset;
                    verts[0].Position = new Vector3(Position.X + MainPlayerAnimationOffset.X - AnimationOffset.X, Position.Y + MainPlayerAnimationOffset.Y - AnimationOffset.Y, 0);
                    verts[1].Position = new Vector3(Position.X + PlayerTextureWidth + MainPlayerAnimationOffset.X - AnimationOffset.X, Position.Y + MainPlayerAnimationOffset.Y - AnimationOffset.Y, 0);
                    verts[2].Position = new Vector3(Position.X + MainPlayerAnimationOffset.X - AnimationOffset.X, Position.Y + PlayerTextureHeight + MainPlayerAnimationOffset.Y - AnimationOffset.Y, 0);
                    verts[3].Position = new Vector3(Position.X + PlayerTextureWidth + MainPlayerAnimationOffset.X - AnimationOffset.X, Position.Y + MainPlayerAnimationOffset.Y - AnimationOffset.Y, 0);
                    verts[4].Position = new Vector3(Position.X + PlayerTextureWidth + MainPlayerAnimationOffset.X - AnimationOffset.X, Position.Y + PlayerTextureHeight + MainPlayerAnimationOffset.Y - AnimationOffset.Y, 0);
                    verts[5].Position = new Vector3(Position.X + MainPlayerAnimationOffset.X - AnimationOffset.X, Position.Y + PlayerTextureHeight + MainPlayerAnimationOffset.Y - AnimationOffset.Y, 0);
                }
                else
                {
                    Position = new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth / 2, GraphicsDeviceManager.PreferredBackBufferHeight / 2) + new Vector2(-48, -144 - Z);
                    verts[0].Position = new Vector3(Position.X, Position.Y, 0);
                    verts[1].Position = new Vector3(Position.X + PlayerTextureWidth, Position.Y, 0);
                    verts[2].Position = new Vector3(Position.X, Position.Y + PlayerTextureHeight, 0);
                    verts[3].Position = new Vector3(Position.X + PlayerTextureWidth, Position.Y, 0);
                    verts[4].Position = new Vector3(Position.X + PlayerTextureWidth, Position.Y + PlayerTextureHeight, 0);
                    verts[5].Position = new Vector3(Position.X, Position.Y + PlayerTextureHeight, 0);
                }

                verts[0].TextureCoordinate = AnimationPosition * new Vector2((1f / spriteSheet.Texture.Width), (1f / (spriteSheet.Texture.Height)));
                verts[1].TextureCoordinate = (AnimationPosition + new Vector2(PlayerTextureWidth, 0)) * new Vector2((1f / spriteSheet.Texture.Width), (1f / (spriteSheet.Texture.Height)));
                verts[2].TextureCoordinate = (AnimationPosition + new Vector2(0, PlayerTextureHeight)) * new Vector2((1f / spriteSheet.Texture.Width), (1f / (spriteSheet.Texture.Height)));
                verts[3].TextureCoordinate = (AnimationPosition + new Vector2(PlayerTextureWidth, 0)) * new Vector2((1f / spriteSheet.Texture.Width), (1f / (spriteSheet.Texture.Height)));
                verts[4].TextureCoordinate = (AnimationPosition + new Vector2(PlayerTextureWidth, PlayerTextureHeight)) * new Vector2((1f / spriteSheet.Texture.Width), (1f / (spriteSheet.Texture.Height)));
                verts[5].TextureCoordinate = (AnimationPosition + new Vector2(0, PlayerTextureHeight)) * new Vector2((1f / spriteSheet.Texture.Width), (1f / (spriteSheet.Texture.Height)));
                ScreenPosition = new Rectangle((int)Position.X, (int)Position.Y, PlayerTextureWidth, PlayerTextureHeight);
                DrawBatch.Add(verts, new DrawData(spriteSheet.Texture, PrimitiveType.TriangleList, 2));
            }
        }

        //public void Draw(SpriteBatch spriteBatch, Vector2 Position)
        //{
        //    if (HP > 0)
        //    {
        //        spriteBatch.Draw(Texture, Position, AnimationRectangle, Color.White);
        //    }
        //    else
        //    {
        //        spriteBatch.Draw(Texture, Position + new Vector2(115, 45), AnimationRectangle, Color.White, 90f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
        //    }
        //}

        //public void Draw(SpriteBatch spriteBatch, Vector2 Position, Texture2D tex)
        //{
        //    ScreenPosition = new Rectangle((int)Position.X + 25, (int)Position.Y + 15, 60, 70);

        //    if (HP > 0)
        //    {
        //        if (TargetHover)
        //            spriteBatch.Draw(tex, Position, AnimationRectangle, Color.Red);
        //        else if (Targetted)
        //            spriteBatch.Draw(tex, Position, AnimationRectangle, Color.MediumVioletRed);
        //        else
        //            spriteBatch.Draw(tex, Position, AnimationRectangle, Color.White);
        //    }
        //    else
        //    {
        //        spriteBatch.Draw(tex, Position + new Vector2(115, 45), AnimationRectangle, Color.White, 90f, new Vector2(0,0), 1f, SpriteEffects.None, 0f);
        //    }
        //}
    }
}
