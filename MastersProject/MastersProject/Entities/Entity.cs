using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ArbitorClient;

namespace MastersProject
{
    //Server side object
    class Entity
    {
        public Vector2 Position;
        public int id;
        public int UED;
        public int Health;

        public int Animation;
        public int frame = 0;
        TimeSpan Ani = TimeSpan.FromSeconds(0);
        public int attacking = 0;

        public void Update(Vector2 iPosition, int ihealth)
        {
            Vector2 Temp = Position - iPosition;
            Position = iPosition;
            Health = ihealth;
            if (attacking == 0)
            {
                Temp.Normalize();
                if (Temp.X > 0.5f)
                {
                    if (Temp.Y > 0)
                    {
                        Animation = 8;
                    }
                    if (Temp.Y < 0)
                    {
                        Animation = 6;
                    }
                }
                if (Temp.X < -0.5f)
                {
                    if (Temp.Y > 0)
                    {
                        Animation = 1;
                    }
                    if (Temp.Y < 0)
                    {
                        Animation = 3;
                    }
                }
            }
        }


        TimeSpan AttackCooldown = TimeSpan.FromSeconds(0);
        TimeSpan AliveTime = TimeSpan.FromSeconds(0);
        public int PlayerTarget = -1;
        public int PlayerTargetID = -1;
        
        public void SetTarget(int id)
        {
            for (int i = 0; i < GameState.PlayerList.Count; i++)
            {
                if (id == GameState.PlayerList[i].PlayerUID)
                {
                    PlayerTarget = i;
                    PlayerTargetID = id;
                    return;
                }
            }
        }
        public void AI(GameTime Time)
        {
            //Handles the AI on the client side
            if (Health > 0)
            {
                AliveTime = Time.TotalGameTime + TimeSpan.FromSeconds(15);
                if (PlayerTarget == -1)
                {
                    for (int i = 0; i < GameState.PlayerList.Count; i++)
                    {
                        if (GameState.PlayerList[i] != null)
                        {
                            if ((GameState.PlayerList[i].Position - Position).Length() < 200)
                            {
                                PlayerTarget = i;
                                PlayerTargetID = GameState.PlayerList[i].PlayerUID;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if ((GameState.PlayerList[PlayerTarget].Position - Position).Length() > 250)
                    {
                        PlayerTarget = -1;
                        return;
                    }
                    if ((GameState.PlayerList[PlayerTarget].Position - Position).Length() > 30)
                    {
                        Vector2 Direction = (GameState.PlayerList[PlayerTarget].Position - Position);
                        Direction.Normalize();
                        Position += Direction * 0.5f;
                    }
                    else
                    {
                        if (AttackCooldown < Time.TotalRealTime)
                        {
                            //Attack
                            Packet AttackPack = new Packet();
                            AttackPack.PlayerID = PlayerTargetID;
                            AttackPack.Int1 = UED;
                            AttackPack.Int2 = 2;
                            AttackPack.PacketID = 6;
                            Arbitor_Core.SendUDP(AttackPack);
                            AttackCooldown = Time.TotalRealTime + TimeSpan.FromSeconds(1);

                            if (PlayerTargetID == GameState.PlayerList[0].PlayerUID)
                            {
                                GameState.PlayerList[0].Health -= 2;
                            }
                        }
                    }
                }
            }
        }

        public void Attack()
        {
            attacking = 4;
        }

        public void UpdateAnimation(GameTime Time)
        {
            if (Time.TotalGameTime > Ani)
            {
                frame++;
                Ani = Time.TotalGameTime + TimeSpan.FromMilliseconds(150);
                if (frame > 4)
                {
                    frame = 0;
                }
                if (attacking > 0)
                {
                    frame = 5 + (4 - attacking);
                    attacking--;
                }
            }


        }
    }
}
