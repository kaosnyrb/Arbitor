using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using ArbitorServer;

namespace Server
{
    //Thread for handling a connection to a client
    class TCPThread
    {
        public Thread ControlThread;
        public TcpClient Client;
        public PlayerRepresentation Player = null;

        bool Loaded = false;
        public bool Instance = false;

        public void init()
        {
            ControlThread = new Thread(Update);
            ControlThread.Start();
            
        }

        public void Update()
        {
                while (Client.Connected)
                {
                    if (Client.GetStream().DataAvailable)
                    {
                        byte[] Data = new byte[Client.ReceiveBufferSize];
                        Client.GetStream().Read(Data, 0, (int)Client.ReceiveBufferSize);
                        string[] Messages = Encoding.ASCII.GetString(Data).Split('@');
                        for (int i = 0; i < Messages.Length - 1; i++)
                        {
                            Packet InPacket = new Packet();
                            InPacket.BuildPacket(Messages[i]);
                            InterpretPacket(InPacket);
                        }
                    }

                    Thread.Sleep(10);
                }
        }

        void ForceUpdate()
        {
            Packet PlayerPacket = new Packet();
            PlayerPacket.PlayerID = Player.UniqurePID;
            PlayerPacket.PacketID = 3;
            PlayerPacket.String1 = Player.Name;
            PlayerPacket.Vector = Player.Position;
            SendPacket(PlayerPacket);
        }

        void InterpretPacket(Packet inPacket)
        {
            //Console.PrintPacket(inPacket);
            if (Player == null)
            {
                if (inPacket.PacketID == 0)
                {
                    int id = PlayerDatabase.GetUID(inPacket.String1);
                    Player = PlayerDatabase.GetPlayer(id);
                    ForceUpdate();
                    return;
                }
            }
            switch (inPacket.PacketID)            
            {
                case -1://Disconnect
                    Console.AddMessage("Client " + Player.Name + " disconnected");
                    Server.ClearTargets();
                    break;
                case 0://Standard update
                    if (Loaded)
                    {
                        Player.Name = inPacket.String1;
                        //Collision checks
                        if ( Map.GetWalkable(inPacket.Vector ))
                        {
                            Player.Position = inPacket.Vector;
                        }
                    }
                    break;

                case 1://Chat message
                    if (Loaded)
                    {
                        inPacket.String1 = Player.Name + " : " + inPacket.String1;
                        Server.BroadcastPacket(inPacket);
                    }
                    break;
                case 3://Loading complete
                    Loaded = true;
                    Player.Name = inPacket.String1;
                    Console.AddMessage("Client " + Player.Name + " connected");
                    break;

                case 4://Attack packet
                    for (int i = 0; i < Server.EntityList.Count; i++)
                    {
                        if (inPacket.Int1 == Server.EntityList[i].UED)
                        {
                            if ((Player.Position - Server.EntityList[i].Position).Length() < 50)
                            {
                                Server.EntityList[i].Health -= inPacket.Int2;
                                if (Server.EntityList[i].Health < 1)
                                {
                                    Player.XP += 10;
                                }
                            }
                        }
                    }
                    break;

                case 66://Que for dungeon
                    Console.AddMessage("Player " + Player.UniqurePID + " qued for raid");
                    ArbitorServer_Core.LFGClientList.Add(Player.UniqurePID);
                    Instance = true;
                    break;
            }
        
        }

        public void SendPacket(Packet p)
        {
            if (!Instance)
            {
                char[] Temp = p.GetData().ToCharArray();
                if (Client.Connected)
                {
                    try
                    {
                        Client.GetStream().Write(Encoding.ASCII.GetBytes(Temp), 0, Temp.Length);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
