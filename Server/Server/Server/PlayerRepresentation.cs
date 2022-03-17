using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Server
{
    //Simple data storage representing a player
    class PlayerRepresentation
    {
        public string Name = "NULL";
        public Vector2 Position = new Vector2(500,500);
        public int UniqurePID;
        public int Health = 100;
        public int Level;
        public int XP = 0;

        public TimeSpan LastAttacked = TimeSpan.FromSeconds(0);//Used for regen calculations
    }
}
