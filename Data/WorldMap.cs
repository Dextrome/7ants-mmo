using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _7ANTSMMO.Data
{
    public class WorldMap
    {
        public int X = 49;
        public int Y = 49;
        public int Z = 19;
        public Tile[, ,] TileArray = new Tile[50, 50, 20];
        public Texture2D mouseMap;
        public int getHighestTile(int x, int y, int playerZ)
        {
            int z = -1;

            try
            {
                for (int getZ = 0; getZ <= Z; getZ++)
                {
                    if (TileArray[x, y, getZ] != null && TileArray[x, y, getZ].Type != Statics.TileType.WATER && !TileArray[x, y, getZ].Corners.Contains("2"))
                        z++;
                    else
                        break;
                }
            }
            catch
            {
                z = -1;
            }

            return z;
        }

        public WorldMap(Texture2D mouseMap)
        {
            this.mouseMap = mouseMap;
            GenerateMap2();
        }

        public WorldMap()
        {
            GenerateMap2();
        }

        public void GenerateMap1()
        {
            for (int z = 0; z <= 8; z++)
            {
                for (int x = 0; x <= X; x++)
                {
                    for (int y = 0; y <= Y; y++)
                    {
                        TileArray[x, y, z] = new Tile(Statics.TileType.BLANK, "1111", "Dirt");
                    }
                }
            }

            for (int x = 0; x <= X; x++)
            {
                for (int y = 0; y <= Y; y++)
                {
                    TileArray[x, y, 9] = new Tile(Statics.TileType.GRASS1, "1111");
                }
            }

            TileArray[44, 45, 10] = new Tile(Statics.TileType.GRASS1, "3300");
            TileArray[44, 45, 11] = new Tile(Statics.TileType.GRASS1, "1122");
            TileArray[43, 48, 9] = new Tile(Statics.TileType.WATER, "1111");
            TileArray[42, 48, 9] = new Tile(Statics.TileType.WATER, "1111");
            TileArray[43, 47, 9] = new Tile(Statics.TileType.WATER, "1111");
            TileArray[42, 47, 9] = new Tile(Statics.TileType.WATER, "1111");
            TileArray[39, 40, 7] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[39, 40, 8] = null;
            TileArray[39, 40, 9] = null;
            TileArray[40, 39, 7] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[40, 39, 8] = null;
            TileArray[40, 39, 9] = null;
            TileArray[39, 39, 7] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[39, 39, 8] = null;
            TileArray[39, 39, 9] = null;
            TileArray[40, 40, 6] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[40, 40, 7] = null;
            TileArray[40, 40, 8] = null;
            TileArray[40, 40, 9] = null;
            TileArray[40, 46, 5] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[40, 46, 6] = null;
            TileArray[40, 46, 7] = null;
            TileArray[40, 46, 8] = null;
            TileArray[40, 46, 9] = null;
            TileArray[49, 48, 6] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[49, 48, 7] = null;
            TileArray[49, 48, 8] = null;
            TileArray[49, 48, 9] = null;
            TileArray[49, 49, 4] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[49, 49, 5] = null;
            TileArray[49, 49, 6] = null;
            TileArray[49, 49, 7] = null;
            TileArray[49, 49, 8] = null;
            TileArray[49, 49, 9] = null;
            TileArray[49, 46, 9].Corners = "1001";
            TileArray[47, 48, 9] = null;
            TileArray[47, 47, 9] = null;
            TileArray[48, 48, 9] = null;
            TileArray[48, 47, 9].Corners = "1100";
            TileArray[49, 47, 9].Corners = "1000";
            TileArray[48, 45, 9] = null;
            TileArray[49, 45, 9] = null;
            TileArray[48, 42, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[48, 42, 10].Corners = "0110";
            TileArray[49, 43, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[49, 43, 10].Corners = "1100";
            TileArray[49, 42, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[49, 41, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[49, 41, 10].Corners = "0011";
            TileArray[48, 43, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[48, 43, 10].Corners = "0100";
            TileArray[48, 44, 9] = null;
            TileArray[49, 44, 9] = null;
            TileArray[47, 45, 9] = null;
            TileArray[47, 46, 9] = null;
            TileArray[49, 45, 9] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[49, 45, 9].Corners = "0001";
            TileArray[48, 45, 9] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[48, 45, 9].Corners = "0011";
            TileArray[48, 41, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[48, 41, 10].Corners = "0010";
            TileArray[45, 48, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[45, 48, 10].Corners = "1001";
            TileArray[45, 47, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[45, 47, 10].Corners = "1001";
            TileArray[47, 40, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[47, 40, 10].Corners = "0010";
            TileArray[43, 44, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 44, 11] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 43, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 43, 11] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 43, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 43, 11] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 43, 12] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 44, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 44, 11] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 11] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 11] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 12] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 12] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 13] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 13] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 14] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 14] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 15] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 15] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 16] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 16] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 17] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 17] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 18] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 18] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[41, 43, 19] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[42, 43, 19] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 37, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[45, 37, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 37, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 38, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 36, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[35, 46, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[35, 46, 10].Corners = "0101";
            TileArray[35, 47, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[35, 47, 10].Corners = "1001";
            TileArray[34, 46, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[34, 46, 10].Corners = "0011";
            TileArray[34, 47, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[35, 45, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[35, 45, 10].Corners = "0110";
            TileArray[37, 47, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[37, 47, 10].Corners = "1100";
            TileArray[38, 47, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[38, 47, 10].Corners = "1010";
            TileArray[37, 46, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[38, 46, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[38, 46, 10].Corners = "1001";
            TileArray[39, 48, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[38, 48, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[38, 48, 10].Corners = "0110";
            TileArray[39, 47, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[39, 47, 10].Corners = "0011";
            TileArray[45, 36, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[45, 36, 10].Corners = "1011";
            TileArray[45, 38, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[45, 38, 10].Corners = "1101";
            TileArray[43, 36, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 36, 10].Corners = "0111";
            TileArray[43, 38, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 38, 10].Corners = "1110";
            TileArray[44, 39, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[44, 39, 10].Corners = "1100";
            TileArray[45, 39, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[45, 39, 10].Corners = "1000";
            TileArray[43, 39, 10] = new Tile(Statics.TileType.GRASS1, "1111");
            TileArray[43, 39, 10].Corners = "0100";
        }

        public void GenerateMap2()
        {
            for (int z = 0; z <= 2; z++)
            {
                for (int x = 0; x <= X; x++)
                {
                    for (int y = 0; y <= Y; y++)
                    {
                        TileArray[x, y, z] = new Tile(Statics.TileType.BLANK, "1111", "Dirt");
                    }
                }
            }

            for (int x = 0; x <= X; x++)
            {
                for (int y = 0; y <= Y; y++)
                {
                    TileArray[x, y, 3] = new Tile(Statics.TileType.GRASS1, "1111");
                }
            }
        }

        public Point[] WorldToMapCell(Point worldPoint, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, int X, int Y, int Z)
        {
            Point[] returnPoints = new Point[2];
            int localPointX = graphics.PreferredBackBufferWidth / 2;
            int localPointY = (graphics.PreferredBackBufferHeight / 2) + (mouseMap.Height / 2) - 2;
            Point returnPoint = new Point(localPointX, localPointY);
            Point tilePoint = new Point(X, Y);
            uint[] myUint = new uint[1];

            for (int x = 0; x <= 20; x++)
            {
                int ScreenX = x - 10;
                for (int y = 0; y <= 30; y++)
                {
                    int ScreenY = y - 15;

                    //spriteBatch.Draw(mouseMap, new Rectangle(localPointX + ((mouseMap.Width) * ScreenX), localPointY + ((mouseMap.Height) * ScreenY), mouseMap.Width, mouseMap.Height), Color.White * 0.2f);

                    //Z handling
                    //ge geeft de Z waarde van de speler al mee in de worldpoint parameter, dus op basis daarvan worde uw tiles bepaalt eigelijk en kunt ge enkel tiles op die Z hoogte targetten
                    //which is fine... en ook iets make da ne speler dan maar 1-3 tiles verder kan targetten naarmate wa voor iets da hij wilt doen eh
                    //en ook checke per targettable tile of de Z waarde van de hoogste tile ni hoger is dan de Z waarde waarop we aan het targetten zijn.

                    if (new Rectangle(localPointX + ((mouseMap.Width) * ScreenX), localPointY + ((mouseMap.Height) * ScreenY), mouseMap.Width, mouseMap.Height).Contains(worldPoint.X, worldPoint.Y))
                    {
                        tilePoint.X += ScreenY;
                        tilePoint.Y += ScreenY;
                        tilePoint.X += ScreenX;
                        tilePoint.Y -= ScreenX;

                        try
                        {
                            int mouseMapPointX = worldPoint.X % (localPointX + ((mouseMap.Width) * ScreenX));
                            int mouseMapPointY = worldPoint.Y % (localPointY + ((mouseMap.Height) * ScreenY));
                            mouseMap.GetData(0, new Rectangle(mouseMapPointX, mouseMapPointY, 1, 1), myUint, 0, 1);
                            returnPoint.X += (mouseMap.Width) * ScreenX;
                            returnPoint.Y += (mouseMap.Height) * ScreenY;

                            if (myUint[0] == 0xFF0000FF) // Red
                            {
                                tilePoint.X -= 1;
                                returnPoint.X -= (mouseMap.Width / 2);
                                returnPoint.Y -= (mouseMap.Height / 2);
                            }

                            if (myUint[0] == 0xFF00FF00) // Green
                            {
                                tilePoint.Y += 1;
                                returnPoint.X -= (mouseMap.Width / 2);
                                returnPoint.Y += (mouseMap.Height / 2);

                            }

                            if (myUint[0] == 0xFF00FFFF) // Yellow
                            {
                                tilePoint.Y -= 1;
                                returnPoint.X += (mouseMap.Width / 2);
                                returnPoint.Y -= (mouseMap.Height / 2);
                            }

                            if (myUint[0] == 0xFFFF0000) // Blue
                            {
                                tilePoint.X += 1;
                                returnPoint.X += (mouseMap.Width / 2);
                                returnPoint.Y += (mouseMap.Height / 2);
                            }
                        }
                        catch { }
                    }
                }
            }

            returnPoints[0] = returnPoint;
            returnPoints[1] = tilePoint;
            return returnPoints;
        }

        public Point[] WorldToMapCellCorner(Point worldPoint, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, int X, int Y, int Z)
        {
            Point[] returnPoints = new Point[2];
            int localPointX = graphics.PreferredBackBufferWidth / 2;
            int localPointY = (graphics.PreferredBackBufferHeight / 2) - 2;
            Point returnPoint = new Point(localPointX, localPointY);
            Point tilePoint = new Point(X, Y);
            uint[] myUint = new uint[1];

            for (int x = 0; x <= 20; x++)
            {
                int ScreenX = x - 10;
                for (int y = 0; y <= 30; y++)
                {
                    int ScreenY = y - 15;

                    //spriteBatch.Draw(mouseMap, new Rectangle(localPointX + ((mouseMap.Width) * ScreenX), localPointY + ((mouseMap.Height) * ScreenY), mouseMap.Width, mouseMap.Height), Color.White * 0.2f);

                    //Z handling
                    //ge geeft de Z waarde van de speler al mee in de worldpoint parameter, dus op basis daarvan worde uw tiles bepaalt eigelijk en kunt ge enkel tiles op die Z hoogte targetten
                    //which is fine... en ook iets make da ne speler dan maar 1-3 tiles verder kan targetten naarmate wa voor iets da hij wilt doen eh
                    //en ook checke per targettable tile of de Z waarde van de hoogste tile ni hoger is dan de Z waarde waarop we aan het targetten zijn.

                    if (new Rectangle(localPointX + ((mouseMap.Width) * ScreenX), localPointY + ((mouseMap.Height) * ScreenY), mouseMap.Width, mouseMap.Height).Contains(worldPoint.X, worldPoint.Y))
                    {
                        tilePoint.X += ScreenY;
                        tilePoint.Y += ScreenY;
                        tilePoint.X += ScreenX;
                        tilePoint.Y -= ScreenX;

                        try
                        {
                            int mouseMapPointX = worldPoint.X % (localPointX + ((mouseMap.Width) * ScreenX));
                            int mouseMapPointY = worldPoint.Y % (localPointY + ((mouseMap.Height) * ScreenY));
                            mouseMap.GetData(0, new Rectangle(mouseMapPointX, mouseMapPointY, 1, 1), myUint, 0, 1);
                            returnPoint.X += (mouseMap.Width) * ScreenX;
                            returnPoint.Y += (mouseMap.Height) * ScreenY;

                            if (myUint[0] == 0xFF0000FF) // Red
                            {
                                tilePoint.X -= 1;
                                returnPoint.X -= (mouseMap.Width / 2);
                                returnPoint.Y -= (mouseMap.Height / 2);
                            }

                            if (myUint[0] == 0xFF00FF00) // Green
                            {
                                tilePoint.Y += 1;
                                returnPoint.X -= (mouseMap.Width / 2);
                                returnPoint.Y += (mouseMap.Height / 2);

                            }

                            if (myUint[0] == 0xFF00FFFF) // Yellow
                            {
                                tilePoint.Y -= 1;
                                returnPoint.X += (mouseMap.Width / 2);
                                returnPoint.Y -= (mouseMap.Height / 2);
                            }

                            if (myUint[0] == 0xFFFF0000) // Blue
                            {
                                tilePoint.X += 1;
                                returnPoint.X += (mouseMap.Width / 2);
                                returnPoint.Y += (mouseMap.Height / 2);
                            }
                        }
                        catch { }
                    }
                }
            }

            returnPoints[0] = returnPoint;
            returnPoints[1] = tilePoint;
            return returnPoints;
        }
    }
}
