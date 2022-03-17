using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;

namespace ArbitorClient
{
    public class Arbitor_Core
    {
        static TcpClient ArbitorServer = new TcpClient();
        static ASCIIEncoding asen = new ASCIIEncoding();
        static NetworkStream Stream;

        static List<UDPPlayer> UDPClients = new List<UDPPlayer>();

        //Socket
        static IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        static Socket newsock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
        static IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        static  EndPoint Remote = (EndPoint)(sender);


        static int PlayerUID = -1;
        public static string IP; //Set at logon, the IP of the server
        public static bool ArbitorActive = false;

        public static Func<Packet,bool> NetPacketInterpret;

        public static int Connect(int PUID)
        {
            try
            {
                IPAddress Address = IPAddress.Parse(IP);
                ArbitorServer.Connect(Address, 3333);
                Stream = ArbitorServer.GetStream();
                PlayerUID = PUID;
                //UDP Listen
                newsock.Bind(ipep);
            }
            catch
            {
                return 300;
            }
            return 0;
        }

        public static void Update()
        {
            try
            {
                if (Stream != null)
                {
                    if (Stream.DataAvailable)
                    {
                        byte[] Data = new byte[ArbitorServer.ReceiveBufferSize];
                        ArbitorServer.GetStream().Read(Data, 0, (int)ArbitorServer.ReceiveBufferSize);
                        string[] Messages = Encoding.ASCII.GetString(Data).Split('@');
                        for (int i = 0; i < Messages.Length - 1; i++)
                        {
                            Packet InPacket = new Packet();
                            InPacket.BuildPacket(Messages[i]);
                            InterpretPacket(InPacket);
                        }
                    }
                }
                if (newsock.Available > 0)
                {
                    //Listen to UDP
                    int recv;
                    byte[] data = new byte[1024];
                    recv = newsock.ReceiveFrom(data, ref Remote);
                    InterpretDatagram(data, recv);
                }
            }
            catch
            {
                Console.Beep(40, 1);
            }
        }

        public static void SendDatagram(string Data, UDPPlayer Target)
        {
            //TestHello
            IPEndPoint Testipep = new IPEndPoint(IPAddress.Parse(Target.IP), 9050);
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(Data);
            newsock.SendTo(data, data.Length, SocketFlags.None, Testipep);
        }

        public static void InterpretDatagram(byte[] Dgram, int Length)
        {
            string Message = Encoding.ASCII.GetString(Dgram, 0, Length);
            Packet NewPacket = new Packet();
            NewPacket.BuildPacket(Message);
            NetPacketInterpret(NewPacket);
        }

        public static void SendCompleteUpdate(List<Packet> GameState)
        {
            for (int i = 0; i < GameState.Count; i++)
            {
                SendPacket(GameState[i]);
            }
        }

        public static void SendPacket(Packet p)
        {
            char[] Temp = p.GetData().ToCharArray();
            if (ArbitorServer.Connected)
            {
                try
                {
                    ArbitorServer.GetStream().Write(Encoding.ASCII.GetBytes(Temp), 0, Temp.Length);
                }
                catch
                {

                }
            }
        }

        public static void SendUDP(Packet p)
        {
            for (int i = 0; i < UDPClients.Count; i++)
            {
                SendDatagram(p.GetData(), UDPClients[i]);
            }
        }

        public static void InterpretPacket(Packet p)
        {
            switch (p.PacketID)
            {
                case 0:
                    NetPacketInterpret(p);
                    break;

                case 5:
                    NetPacketInterpret(p);
                    break;

                case 100:
                    Packet PUIDPacket = new Packet();
                    PUIDPacket.PlayerID = PlayerUID;
                    PUIDPacket.PacketID = 100;
                    SendPacket(PUIDPacket);
                    break;

                case 101:
                    //UDP Client 1
                    UDPPlayer Temp = new UDPPlayer();
                    Temp.IP = p.String1;
                    UDPClients.Add(Temp);
                    SendDatagram("Yo yo yo" + PlayerUID, Temp);
                    //We've UDP clients, break from the TCP model
                    ArbitorActive = true;
                    break;

            }
        }
    }

    public class UDPPlayer
    {
        public string IP = "0.0.0.0";
    }

}
