using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Microsoft.Xna.Framework;

namespace MastersProject
{
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

            string EntityFile = BaseGame.ContentMan.Load<string>("Level" + LevelNumber.ToString());
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
        }

        public static void Save()
        {
            StreamWriter SW;
            SW = File.CreateText("LEVEL.XML");
            //Spawn
            SW.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            SW.WriteLine("<XnaContent>");
            string Export = "<Asset Type=\"System.String\">";
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Export += MapData[i][j].ToString() + " ";
                }
            }
            Export += "</Asset>";
            SW.WriteLine(Export);
            SW.WriteLine("</XnaContent>");
            SW.Close();
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

        public void DrawMap()
        {
            Point TileCheck = new Point();
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    TileCheck.X = i * 32;
                    TileCheck.Y = j * 32;

                    int x = MapData[i][j] % 20;
                    int y = MapData[i][j] / 20;

                    if (Camera.FieldOfView.Contains(TileCheck))
                    {
                        //Tile maps are interchangable, gives some visual feedback about the Arbitor state
                        if (ArbitorClient.Arbitor_Core.ArbitorActive)
                        {
                            SpriteManager.RenderTileSprite(21, new Vector2(TileCheck.X - Camera.CameraPosition.X, TileCheck.Y - Camera.CameraPosition.Y), y, x);
                        }
                        else
                        {
                            SpriteManager.RenderTileSprite(20, new Vector2(TileCheck.X - Camera.CameraPosition.X, TileCheck.Y - Camera.CameraPosition.Y), y, x);
                        }
                    }
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
