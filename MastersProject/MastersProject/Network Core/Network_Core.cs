using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;
using ArbitorClient;

namespace MastersProject
{
    class Network_Core
    {
        static TcpClient Client = new TcpClient();

        static ASCIIEncoding asen = new ASCIIEncoding();
        static NetworkStream Stream;

        static TimeSpan LastPacket = TimeSpan.FromDays(0);

        static int disconnectcount = 0;

        public static int Connect(string IP)
        {
            try
            {
                IPAddress Address = IPAddress.Parse(IP);
                Client.Connect(Address, 2222);
                Stream = Client.GetStream();
                BaseGame.CurrentState = States.Game;
                //We need to refresh the qued flag so that the player can request queing from the server again
                GameState.PlayerList[0].qued = false;
                //Successful connect, pass the Arbitor the IP address
                Arbitor_Core.IP = IP;
            }
            catch
            {
                return 300;
            }
            Packet NamePacket = new Packet();
            NamePacket.String1 = GameState.PlayerList[0].Name;

            SendPacket(NamePacket);

            Arbitor_Core.NetPacketInterpret = InterpretPacket;
            return 0;
        }

        public static void Update(GameTime Time)
        {
            if (!Arbitor_Core.ArbitorActive)
            {
                //send
                if (LastPacket.TotalMilliseconds < Time.TotalGameTime.TotalMilliseconds - 200)//Player Data PacketFrequnecy
                {
                    Packet CurrentPlayer = new Packet();
                    CurrentPlayer.PlayerID = GameState.PlayerList[0].PlayerUID;
                    CurrentPlayer.Vector = GameState.PlayerList[0].TargetPosition;
                    CurrentPlayer.String1 = GameState.PlayerList[0].Name;
                    SendPacket(CurrentPlayer);
                    LastPacket = Time.TotalGameTime;
                }
                //rechieve
                try
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
                        disconnectcount = 0;
                    }
                }
                catch
                {
                    disconnectcount++;
                    if (disconnectcount > 100)
                    {
                        BaseGame.CurrentState = States.Logon;
                        Client.Close();
                        Client = new TcpClient();
                    }
                }
            }
            else
            {
                //Send our updates to the Arbitor
                if (LastPacket.TotalMilliseconds < Time.TotalGameTime.TotalMilliseconds - 300)//Player Data PacketFrequnecy
                {
                    ArbitorClient.Packet CurrentPlayer = new ArbitorClient.Packet();
                    CurrentPlayer.PlayerID = GameState.PlayerList[0].PlayerUID;
                    CurrentPlayer.Vector = GameState.PlayerList[0].TargetPosition;
                    CurrentPlayer.String1 = GameState.PlayerList[0].Name;

                    CurrentPlayer.Int1 = GameState.PlayerList[0].Health;
                    CurrentPlayer.Int2 = GameState.PlayerList[0].Level;
                    CurrentPlayer.Int3 = GameState.PlayerList[0].XP;
                    Arbitor_Core.SendUDP(CurrentPlayer);
                    LastPacket = Time.TotalGameTime;
                }
            }
            //Update the Arbitor core
            Arbitor_Core.Update();
        }

        public static void SendChat(string message)
        {
            Packet MessagePacket = new Packet();
            MessagePacket.PlayerID = GameState.PlayerList[0].PlayerUID;
            MessagePacket.PacketID = 1;
            MessagePacket.String1 = message;
            if (Arbitor_Core.ArbitorActive)
            {
                MessagePacket.String1 = GameState.PlayerList[0].Name + ": " + message;
                Arbitor_Core.SendUDP(MessagePacket);
                InterpretPacket(MessagePacket);//We want to see our own chat messages
            }
            else
            {
                SendPacket(MessagePacket);
            }
        }

        public static bool InterpretPacket(Packet Message)
        {
            switch (Message.PacketID)
            {
                case 0://Player updates
                    bool FoundPlayer = false;
                    if (GameState.PlayerList[0].PlayerUID == Message.PlayerID)
                    {
                        if ((GameState.PlayerList[0].TargetPosition - Message.Vector).Length() > 100)
                        {
                            GameState.PlayerList[0].TargetPosition = Message.Vector;

                        }
                        GameState.PlayerList[0].Health = Message.Int1;
                        if (Message.Int2 > 0 || Message.Int3 > 0 )
                        {
                            GameState.PlayerList[0].Level = Message.Int2;
                            GameState.PlayerList[0].XP = Message.Int3;
                        }
                        FoundPlayer = true;
                    }
                    for (int i = 1; i < GameState.PlayerList.Count; i++)
                    {
                        if (GameState.PlayerList[i].PlayerUID == Message.PlayerID)
                        {
                            //GameState.PlayerList[i].Position = Message.Vector;
                            GameState.PlayerList[i].TargetPosition = Message.Vector;
                            GameState.PlayerList[i].Health = Message.Int1;
                            if (Message.Int2 > 0)
                            {
                                GameState.PlayerList[i].Level = Message.Int2;
                            }
                            FoundPlayer = true;
                            break;
                        }
                    }
                    if (!FoundPlayer)
                    {
                        if (Message.String1 != GameState.PlayerList[0].Name)
                        {
                            Player NewPlayer = new Player();
                            NewPlayer.Init(false);
                            NewPlayer.PlayerUID = Message.PlayerID;
                            NewPlayer.Name = Message.String1;
                            GameState.PlayerList.Add(NewPlayer);
                        }
                    }
                    break;

                case 1://Chat message
                    ChatMessage TempChat = new ChatMessage();
                    TempChat.Countdown = 1000;
                    TempChat.Message = Message.String1;
                    GameState.ChatConsole.Add(TempChat);
                    break;

                case 2://Entity Updates
                    bool FoundEntity = false;
                    for (int i = 0; i < GameState.EntityList.Count; i++)
                    {
                        if (GameState.EntityList[i].UED == Message.Int1)
                        {
                            GameState.EntityList[i].Update(Message.Vector,int.Parse(Message.String1));
                            FoundEntity = true;
                        }
                    }
                    if (!FoundEntity)
                    {
                        Entity NewEntity = new Entity();
                        NewEntity.UED = Message.Int1;
                        NewEntity.id = Message.Int2;
                        NewEntity.Position = Message.Vector;
                        NewEntity.Health = int.Parse(Message.String1);
                        GameState.EntityList.Add(NewEntity);
                    }
                    break;

                case 3://Logon Message
                    GameState.PlayerList[0].Position = Message.Vector;
                    GameState.PlayerList[0].TargetPosition = Message.Vector;
                    GameState.PlayerList[0].PlayerUID = Message.PlayerID;
                    Packet Loaded = new Packet();
                    Loaded.PacketID = 3;
                    Loaded.String1 = GameState.PlayerList[0].Name;
                    SendPacket(Loaded);
                    break;

                case 4:
                    if (Message.PlayerID == GameState.PlayerList[0].PlayerUID)
                    {
                        try
                        {
                            GameState.EntityList[Message.Int1 - 1].Attack();
                            GameState.PlayerList[0].Health -= Message.Int2;
                        }
                        catch
                        {
                        }    
                    }
                    else if (Arbitor_Core.ArbitorActive)
                    {
                        try
                        {
                            for (int i = 0; i < GameState.EntityList.Count; i++)
                            {
                                if (GameState.EntityList[i].UED == Message.Int1)
                                {
                                    GameState.EntityList[i].Health -= Message.Int2;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    break;

                case 5: //Arbitor Entity update
                    FoundEntity = false;
                    for (int i = 0; i < GameState.EntityList.Count; i++)
                    {
                        if (GameState.EntityList[i].UED == Message.PlayerID)
                        {
                            //GameState.EntityList[i].Update(Message.Vector, int.Parse(Message.String1));
                            GameState.EntityList[i].Position = Message.Vector;
                            GameState.EntityList[i].Health = Message.Int1;
                            GameState.EntityList[i].SetTarget(Message.Int2);
                            FoundEntity = true;
                        }
                    }
                    break;

                case 6:
                    for (int i = 0; i < GameState.PlayerList.Count; i++)
                       {
                            if (GameState.PlayerList[i].PlayerUID == Message.PlayerID)
                            {
                                GameState.PlayerList[i].Health -= Message.Int2;
                            }
                        }

                    break;
            }
            return true;
        }
        public static void SendPacket(Packet p)
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
