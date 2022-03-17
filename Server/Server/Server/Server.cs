using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ArbitorServer;

namespace Server
{
    class Server
    {
        TcpListener TcpServer = new TcpListener(2222);

        public static List<TCPThread> ClientList = new List<TCPThread>();
        
        public static List<Entity> EntityList = new List<Entity>();

        ASCIIEncoding asen = new ASCIIEncoding();


        public void Init()
        {
            TcpServer.Start();
            ArbitorServer_Core.Init();
            //Randomly create monsters
            Random TempRand = new Random();
            for (int i = 0; i < 10; i++)
            {
                Entity TestMonster = new Entity();
                TestMonster.Position = new Vector2(500 + TempRand.Next(1000), 500 + TempRand.Next(1000));
                TestMonster.id = 4;
                TestMonster.UED = 1 + i;
                TestMonster.Health = 100;
                EntityList.Add(TestMonster);
            }
        }

        public void Update(GameTime Time)
        {
            if (TcpServer.Pending())
            {
                TCPThread NewThread = new TCPThread();
                NewThread.Client = TcpServer.AcceptTcpClient();
                NewThread.ControlThread = new System.Threading.Thread(NewThread.Update);
                NewThread.ControlThread.Start();
                ClientList.Add(NewThread);
                Console.AddMessage(Time.TotalRealTime.ToString() + " Client Connected");
            }
            for (int k = 0; k < EntityList.Count; k++)
            {
                EntityList[k].Update(Time);
            }
            //Restore the players health after 5 seconds out of combat
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (ClientList[i].Player != null && ClientList[i].Player.Name != "NULL" && !ClientList[i].Instance)
                {
                    if (ClientList[i].Player.Health < 100)
                    {
                        if (ClientList[i].Player.LastAttacked < Time.TotalRealTime - TimeSpan.FromSeconds(5))
                        {
                            ClientList[i].Player.Health++;
                        }
                    }
                }
            }
            ArbitorServer_Core.Update();
            SendRefresh();
        }

        //Send a packet to everyone connected
        public static void BroadcastPacket(Packet Pack)
        {
            for (int j = 0; j < ClientList.Count; j++)
            {
                ClientList[j].SendPacket(Pack);
            }
        }

        public static void ClearTargets()
        {
            //Clears all the AI targets, used when a client disconnects
            for (int k = 0; k < EntityList.Count; k++)
            {
                EntityList[k].PlayerTarget = -1;
            }
        }

        //Sends a complete update
        public void SendRefresh()
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (ClientList[i].Client.Connected == false)
                {
                    ClientList.RemoveAt(i);
                    i--;
                    continue;
                }
                //Player Updates
                if (ClientList[i].Player != null && ClientList[i].Player.Name != "NULL")
                {
                    
                    Packet PlayerPacket = new Packet();
                    PlayerPacket.PlayerID = ClientList[i].Player.UniqurePID;
                    PlayerPacket.PacketID = 0;
                    PlayerPacket.Int1 = ClientList[i].Player.Health;
                    PlayerPacket.Int2 = ClientList[i].Player.Level;
                    PlayerPacket.Int3 = ClientList[i].Player.XP;

                    PlayerPacket.String1 = ClientList[i].Player.Name;
                    PlayerPacket.Vector = ClientList[i].Player.Position;
                    for (int j = 0; j < ClientList.Count; j++)
                    {
                        ClientList[j].SendPacket(PlayerPacket);
                    }
                }
                //Entity Updates
                for (int k = 0; k < EntityList.Count; k++)
                {
                    Packet EntityUpdate = new Packet();
                    EntityUpdate.PacketID = 2;
                    EntityUpdate.Int1 = EntityList[k].UED;
                    EntityUpdate.Int2 = EntityList[k].id;
                    EntityUpdate.Vector = EntityList[k].Position;
                    EntityUpdate.String1 = EntityList[k].Health.ToString();
                    for (int j = 0; j < ClientList.Count; j++)
                    {
                        ClientList[j].SendPacket(EntityUpdate);
                    }
                }
            }
        }
    }
}
