using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;

namespace ArbitorServer
{
    public class ClientTCPThread
    {
        public Thread ControlThread;
        public TcpClient Client;

        public int UAID = -1;//Which server am I playing on?
        public int UPID = -1;

        public bool TCPConectionAccepted = false;

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
                        if (InPacket.PacketID == 100)
                        {
                            UPID = InPacket.PlayerID;
                        }
                        ArbitorServer_Core.SendArbitorPacket(UAID, InPacket);
                    }
                }
                if (UPID == -1)
                {
                    Packet IDrequest = new Packet();
                    IDrequest.PacketID = 100;
                    SendPacket(IDrequest);
                    Thread.Sleep(1000);
                }
                //Find our 
                if (UAID == -1)
                {
                        //determine which arbitor i'm talking to
                        bool found = false;
                        for (int i = 0; i < ArbitorServer_Core.ArbitorList.Count && !found; i++)
                        {
                            for (int j = 0; j < ArbitorServer_Core.ArbitorList[i].Clients.Count && !found; j++)
                            {
                                if (UPID == ArbitorServer_Core.ArbitorList[i].Clients[j])
                                {
                                    UAID = ArbitorServer_Core.ArbitorList[i].id;
                                    TCPConectionAccepted = true;
                                    found = true;
                                }
                            }
                        }
                }
                Thread.Sleep(10);
            }
        }

        public void SendPacket(Packet p)
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
