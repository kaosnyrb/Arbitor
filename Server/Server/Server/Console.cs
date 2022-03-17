using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Server
{
    //The console output, a list of strings which is simply rendered
    class Console
    {
        static List<string> MessageList = new List<string>();

        public static void AddMessage(string Message)
        {
            MessageList.Add(Message);
        }

        public static void PrintPacket(Packet p)
        {
            if (p.PacketID != 0)
            {
                AddMessage(p.PacketID + " : " + p.PlayerID);
            }
        }

        //Exports the console output
        public static void SaveConsole()
        {
            TextWriter tw = new StreamWriter("Console.txt");
            tw.WriteLine("------------Server Log   " + DateTime.Now + " ----------------");
            for (int i = 0; i < MessageList.Count; i++)
            {
                tw.WriteLine(MessageList[i]);
            }
            tw.Close();
        }

        public static void Draw()
        {
            for ( int i = 0; i < MessageList.Count;i++)
            {
                ScreenTextManager.RenderText(MessageList[i], new Vector2(5, (MessageList.Count - i) * 10), Color.White);
            }
        }
    }
}
