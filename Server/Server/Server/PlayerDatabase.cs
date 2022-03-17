using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Server
{
    //Player infomation storage and retreval
    //Stored in a Text file and loaded on startup
    class PlayerDatabase
    {
        static List<PlayerRepresentation> PlayerDatabaseList = new List<PlayerRepresentation>();

        public static int nextPID = 0;

        public static void LoadDatabase()
        {
            TextReader Read = new StreamReader("PDB.txt");
            string PlayerData;
            while ((PlayerData = Read.ReadLine()) != null)
            {
                PlayerRepresentation PlayerRecord = new PlayerRepresentation();
                string[] Split = PlayerData.Split('|');
                PlayerRecord.Name = Split[0];
                
                string[] VectorSplit = Split[1].Split(',');
                int x = int.Parse(VectorSplit[0]);
                int y = int.Parse(VectorSplit[1]);
                PlayerRecord.Position = new Vector2(x, y);

                PlayerRecord.Level = int.Parse(Split[2]);
                PlayerRecord.Health = int.Parse(Split[3]);
                PlayerRecord.XP = int.Parse(Split[4]);

                PlayerDatabaseList.Add(PlayerRecord);
            }
            nextPID = PlayerDatabaseList.Count;
            Read.Close();
        }

        public static void SaveDatabase()
        {
            TextWriter tw = new StreamWriter("PDB.txt");
            for (int i = 0; i < PlayerDatabaseList.Count; i++)
            {
                tw.WriteLine(PlayerDatabaseList[i].Name + "|" + 
                    PlayerDatabaseList[i].Position.X + "," + PlayerDatabaseList[i].Position.Y 
                    + "|" + PlayerDatabaseList[i].Level 
                    + "|" + PlayerDatabaseList[i].Health
                    + "|" + PlayerDatabaseList[i].XP);
            }
            tw.Close();
        }

        //Returns the UPID for the string or if not found returns the next free number
        public static int GetUID(string name)
        {
            for (int i = 0; i < PlayerDatabaseList.Count; i++)
            {
                if (PlayerDatabaseList[i].Name == name)
                {
                    return i;
                }
            }
            return nextPID++;
        }

        public static PlayerRepresentation GetPlayer(int id)
        {
            if (PlayerDatabaseList.Count <= id)
            {
                PlayerRepresentation PlayerRecord = new PlayerRepresentation();
                PlayerRecord.UniqurePID = id;
                PlayerDatabaseList.Add(PlayerRecord);
            }
            PlayerDatabaseList[id].UniqurePID = id;
            return PlayerDatabaseList[id];
        }
    }
}
