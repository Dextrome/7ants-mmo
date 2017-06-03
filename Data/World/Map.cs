using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Data;
using Data.Items;

namespace Data.World
{
    public class Map
    {
        public string filename = "map";
        public int MaxX = 49;
        public int MaxY = 49;
        public MapTile[,] Tiles = new MapTile[50, 50];
        public List<MapObject> Objects = new List<MapObject>();
        public MapTile[,] RealTiles = new MapTile[50, 50];
        public Statics.ScreenDirection ScreenDirection = Statics.ScreenDirection.NORTHWEST;

        public Map()
        {
            //GenerateBasicMap();
        }

        public void GenerateBasicMap()
        {
            for (int x = 0; x <= 49; x++)
                for (int y = 0; y <= 49; y++)
                    Tiles[x, y] = new MapTile(new int[] { 0, 0, 0, 0 }, x, y);

            Tiles[25, 25].CornerHeights = new int[] { 10, 0, 0, 0 };
            Tiles[24, 25].CornerHeights = new int[] { 0, 10, 0, 0 };
            Tiles[24, 24].CornerHeights = new int[] { 0, 0, 10, 0 };
            Tiles[25, 24].CornerHeights = new int[] { 0, 0, 0, 10 };

            Tiles[25, 23].CornerHeights = new int[] { -20, 0, 0, 0 };
            Tiles[24, 23].CornerHeights = new int[] { 0, -20, 0, 0 };
            Tiles[24, 22].CornerHeights = new int[] { 0, 0, -20, 0 };
            Tiles[25, 22].CornerHeights = new int[] { 0, 0, 0, -20 };

            Tiles[25, 35].CornerHeights = new int[] { -40, 0, 0, 0 };
            Tiles[24, 35].CornerHeights = new int[] { 0, -40, 0, 0 };
            Tiles[24, 34].CornerHeights = new int[] { 0, 0, -40, 0 };
            Tiles[25, 34].CornerHeights = new int[] { 0, 0, 0, -40 };


            Tiles[46, 46].CornerHeights = new int[] { 0, -70, -30, 0 };
            Tiles[46, 47].CornerHeights = new int[] { 0, -30, -30, 0 };
            Tiles[46, 48].CornerHeights = new int[] { 0, -30, 0, 0 };
            Tiles[47, 46].CornerHeights = new int[] { -70, -70, -30, -30 };
            Tiles[47, 47].CornerHeights = new int[] { -30, -30, -30, -30 };
            Tiles[47, 48].CornerHeights = new int[] { -30, -30, 0, 0 };
            Tiles[48, 48].CornerHeights = new int[] { -30, 0, 0, 0 };
            Tiles[48, 47].CornerHeights = new int[] { -30, 0, 0, -30 };
            Tiles[48, 46].CornerHeights = new int[] { -70, 0, 0, -30 };

            Tiles[47, 45].CornerHeights = new int[] { -70, -70, -70, -70 };
            Tiles[48, 45].CornerHeights = new int[] { -70, 0, 0, -70 };
            Tiles[48, 44].CornerHeights = new int[] { 0, 0, 0, -70 };
            Tiles[47, 44].CornerHeights = new int[] { 0, 0, -70, -70 };
            Tiles[46, 44].CornerHeights = new int[] { 0, 0, -70, 0 };
            Tiles[46, 45].CornerHeights = new int[] { 0, -70, -70, 0 };

            for (int x = 0; x <= 49; x++)
            {
                Tiles[x, 49] = new MapTile(new int[] { 0, 0, 0, 0 }, x, 49, Statics.TileType.WATER);
                if (x < 46)
                    Tiles[x, 48] = new MapTile(new int[] { 0, 0, 0, 0 }, x, 48, Statics.TileType.WATER);
            }

            for (int y = 0; y <= 49; y++)
            {
                Tiles[49, y] = new MapTile(new int[] { 0, 0, 0, 0 }, 49, y, Statics.TileType.WATER);
                if (y < 44)
                    Tiles[48, y] = new MapTile(new int[] { 0, 0, 0, 0 }, 48, y, Statics.TileType.WATER);
            }

            RealTiles = Tiles;
        }

        public void GenerateRandomMap()
        {
            int[,] CornerPoints = new int[50, 50];
            Random random = new Random();

            for (int x = 0; x <= 49; x++)
                for (int y = 0; y <= 49; y++)
                {
                    int maxZ = 0; int minZ = 0;
                    if (x == 0 || y == 0)
                    {
                        maxZ = 0; minZ = 0;
                    }
                    else
                    {
                        int maxZ1 = 0; int minZ1 = 0;
                        int maxZ2 = 0; int minZ2 = 0;
                        maxZ1 = CornerPoints[x - 1, y] + 10;
                        minZ1 = CornerPoints[x - 1, y] - 10;
                        maxZ2 = CornerPoints[x, y - 1] + 10;
                        minZ2 = CornerPoints[x, y - 1] - 10;
                        if (maxZ1 < maxZ2)
                            maxZ = maxZ1;
                        else
                            maxZ = maxZ2;
                        if (minZ1 < minZ2)
                            minZ = minZ1;
                        else
                            minZ = minZ2;

                        if (maxZ > 100)
                            maxZ = 100;
                        if (minZ < -125)
                            minZ = -125;
                    }

                    //int Z = random.Next(-125, 100);
                    int Z = random.Next(minZ, maxZ);
                    CornerPoints[x, y] = Z;
                }

            for (int x = 0; x <= 48; x++)
                for (int y = 0; y <= 48; y++)
                    Tiles[x, y] = new MapTile(new int[] { CornerPoints[x, y], CornerPoints[x + 1, y], CornerPoints[x + 1, y + 1], CornerPoints[x, y + 1] }, x, y);

            for (int y = 0; y <= 48; y++)
                Tiles[49, y] = new MapTile(new int[] { CornerPoints[49, y], CornerPoints[0, y], CornerPoints[0, y + 1], CornerPoints[49, y + 1] }, 49, y);

            for (int x = 0; x <= 48; x++)
                Tiles[x, 49] = new MapTile(new int[] { CornerPoints[x, 49], CornerPoints[x + 1, 49], CornerPoints[x + 1, 0], CornerPoints[x, 0] }, x, 49);

            Tiles[49, 49] = new MapTile(new int[] { CornerPoints[49, 49], CornerPoints[0, 49], CornerPoints[0, 0], CornerPoints[49, 0] }, 49, 49);
            RealTiles = Tiles;
        }

        public void ChangeDirection()
        {
            if (ScreenDirection == Statics.ScreenDirection.NORTHWEST)
                ScreenDirection = Statics.ScreenDirection.SOUTHEAST;
            else
                ScreenDirection = Statics.ScreenDirection.NORTHWEST;

            MapTile[,] NewTiles = new MapTile[50, 50];

            for (int x = 0; x <= 49; x++)
                for (int y = 0; y <= 49; y++)
                {
                    NewTiles[x, y] = Tiles[49 - x, 49 - y];
                    NewTiles[x, y].CornerHeights = new int[] { NewTiles[x, y].CornerHeights[2], NewTiles[x, y].CornerHeights[3], NewTiles[x, y].CornerHeights[0], NewTiles[x, y].CornerHeights[1] };
                }

            Tiles = NewTiles;
        }

        public void SaveMap()
        {
            XDocument MapDoc = new XDocument();
            XElement MapElement = new XElement("Map");
            MapElement.Add(new XAttribute("MaxX", MaxX));
            MapElement.Add(new XAttribute("MaxY", MaxY));
            for (int x = 0; x <= MaxX; x++)
                for (int y = 0; y <= MaxY; y++)
                {
                    XElement TileElement = new XElement("Tile");
                    TileElement.Add(new XAttribute("X", x));
                    TileElement.Add(new XAttribute("Y", y));
                    TileElement.Add(new XElement("Corner0",Tiles[x, y].CornerHeights[0]));
                    TileElement.Add(new XElement("Corner1",Tiles[x, y].CornerHeights[1]));
                    TileElement.Add(new XElement("Corner2",Tiles[x, y].CornerHeights[2]));
                    TileElement.Add(new XElement("Corner3",Tiles[x, y].CornerHeights[3]));
                    MapElement.Add(TileElement);
                }
            MapDoc.Add(MapElement);

            MapDoc.Save(filename + ".xml");
        }

        public void LoadMap(string filename)
        {
            this.filename = filename;
            XDocument MapDoc = XDocument.Load(filename + ".xml");
            XElement MapElement = MapDoc.Element("Map");
            this.MaxX = Convert.ToInt16(MapElement.Attribute("MaxX").Value);
            this.MaxY = Convert.ToInt16(MapElement.Attribute("MaxY").Value);
            this.Tiles = new MapTile[MaxX + 1, MaxY + 1];
            this.RealTiles = new MapTile[MaxX + 1, MaxY + 1];

            foreach (XElement TileElement in MapElement.Elements("Tile"))
                this.Tiles[Convert.ToInt16(TileElement.Attribute("X").Value), Convert.ToInt16(TileElement.Attribute("Y").Value)] =
                    new MapTile(new int[] { Convert.ToInt16(TileElement.Element("Corner0").Value), Convert.ToInt16(TileElement.Element("Corner1").Value), 
                        Convert.ToInt16(TileElement.Element("Corner2").Value), Convert.ToInt16(TileElement.Element("Corner3").Value) }, 
                        Convert.ToInt16(TileElement.Attribute("X").Value), Convert.ToInt16(TileElement.Attribute("Y").Value));

            this.RealTiles = this.Tiles;
        }
    }
}
