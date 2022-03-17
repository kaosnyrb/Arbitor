using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Server
{
    //Server side object
    class Entity
    {
        public Vector2 Position;
        public int id;
        public int UED;
        public int Health;

        TimeSpan AttackCooldown = TimeSpan.FromSeconds(0);
        TimeSpan AliveTime = TimeSpan.FromSeconds(0);

        public void Update(GameTime Time)
        {
                switch (id)
                {
                    case 4:
                        OrcAI(Time);
                        break;
                }
           
        }


        public int PlayerTarget = -1;

        public void OrcAI(GameTime Time)
        {
            if (Health > 0)
            {
                AliveTime = Time.TotalGameTime + TimeSpan.FromSeconds(15);
                if (PlayerTarget == -1)
                {
                    for (int i = 0; i < Server.ClientList.Count; i++)
                    {
                        if (Server.ClientList[i].Player != null)
                        {
                            if ((Server.ClientList[i].Player.Position - Position).Length() < 200)
                            {
                                PlayerTarget = i;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        if ((Server.ClientList[PlayerTarget].Player.Position - Position).Length() > 30)
                        {
                            Vector2 Direction = (Server.ClientList[PlayerTarget].Player.Position - Position);
                            Direction.Normalize();
                            Position += Direction * 0.5f;
                        }
                        else
                        {
                            if (AttackCooldown < Time.TotalRealTime)
                            {
                                //Attack
                                Packet AttackPack = new Packet();
                                AttackPack.PlayerID = PlayerTarget;
                                AttackPack.Int1 = UED;
                                AttackPack.Int2 = 2;
                                AttackPack.PacketID = 4;
                                Server.BroadcastPacket(AttackPack);
                                AttackCooldown = Time.TotalRealTime + TimeSpan.FromSeconds(1);
                                Server.ClientList[PlayerTarget].Player.Health -= 2;
                                Server.ClientList[PlayerTarget].Player.LastAttacked = Time.TotalRealTime;
                            }
                        }
                    }
                    catch {
                        //During client disconnects there is a brief window where the ai's target is invalid,
                        //if the id is higher than the remaining players then this will crash.
                        //It is fixed when the disconnect message is rechieved and all ai are forced to retarget
                    }
                    if (Server.ClientList.Count > PlayerTarget && PlayerTarget >= 0)
                    {
                        if ((Server.ClientList[PlayerTarget].Player.Position - Position).Length() > 250)
                        {
                            PlayerTarget = -1;
                        }
                    }
                }
            }
            else
            {
                if (Time.TotalGameTime > AliveTime)
                {
                    Console.AddMessage("Orc " + UED + " Respawning");
                    Random TempRand = new Random();
                    Position = new Vector2(500 + TempRand.Next(1000), 500 + TempRand.Next(1000));
                    Health = 100;
                }
            }
        }
    }
}
