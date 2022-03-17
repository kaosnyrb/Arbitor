using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ArbitorClient
{
    public class Packet
    {
        public Vector2 Vector = new Vector2(0, 0);
        public int Int1 = 0;
        public int Int2 = 0;
        public int Int3 = 0;

        public int PacketID = 0;
        public int PlayerID = 0;
        public string String1 = "";

        public string GetData()
        {
            string Data = "";
            Data += PlayerID.ToString() + "|";
            Data += PacketID + "|";
            Data += String1 + "|";
            Data += Int1.ToString() + "|";
            Data += Int2.ToString() + "|";
            Data += Int3.ToString() + "|";

            Data += ((int)Vector.X).ToString() + "," + ((int)Vector.Y).ToString() + "|";
            Data += "@";
            return Data;
        }

        public void BuildPacket(string Message)
        {
            try
            {
                string[] Split = Message.Split('|');
                PlayerID = int.Parse(Split[0]);
                PacketID = int.Parse(Split[1]);
                String1 = Split[2];
                Int1 = int.Parse(Split[3]);
                Int2 = int.Parse(Split[4]);
                Int3 = int.Parse(Split[5]);

                string[] VectorSplit = Split[6].Split(',');
                int x = int.Parse(VectorSplit[0]);
                int y = int.Parse(VectorSplit[1]);
                Vector = new Vector2(x, y);
            }
            catch
            { }
        }
    }
}
