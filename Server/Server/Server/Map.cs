using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Server
{
    //Server side map, same as the client side one
    class Map
    {
            public static int[][] MapData;

            public static int Width = 64;
            public static int Height = 64;
            int TileSize = 32;

            //Loading variables
            static char[] Delimiter = new char[1];

            public static void LoadMap(int LevelNumber)
            {
                MapData = new int[Width][];
                for (int i = 0; i < Width; i++)
                {
                    MapData[i] = new int[Height];
                    for (int j = 0; j < Height; j++)
                    {
                        MapData[i][j] = 0;

                    }
                }
                Delimiter[0] = ' ';

                string EntityFile = Game1.ContentMan.Load<string>("Level" + LevelNumber.ToString());
                string[] EntitySplit = EntityFile.Split(Delimiter);
                int Count = 0;
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        string[] Temp = EntitySplit[Count++].Split(Delimiter);
                        MapData[i][j] = int.Parse(Temp[0]);
                    }
                }
                Console.AddMessage("Map Loaded");
            }

            public void init()
            {
                MapData = new int[64][];
                for (int i = 0; i < 64; i++)
                {
                    MapData[i] = new int[64];
                    for (int j = 0; j < 64; j++)
                    {
                        MapData[i][j] = 200;
                    }
                }
            }

            public static bool GetWalkable(Vector2 Position)
            {
                int X = (int)Position.X / 32;
                int Y = (int)Position.Y / 32;

                if (Y >= 0 && X >= 0)
                {
                    if (MapData[X][Y] < 189)//Wall rocks and trees
                    {
                        return false;
                    }
                    if (MapData[X][Y] > 314 && MapData[X][Y] < 351)//Water
                    {
                        return false;
                    }
                }
                return true;
            }
        }
}
