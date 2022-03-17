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
    public static class ArbitorServer_Core
    {
        static TcpListener TcpServer = new TcpListener(3333);
        public static List<Arbitor> ArbitorList = new List<Arbitor>();
        public static List<ClientTCPThread> ClientList = new List<ClientTCPThread>();

        public static List<int> LFGClientList = new List<int>();

        //Unique Arbitor ID, the reference for which instance we want.
        public static int UAID = 0;

        public static void Init()
        {
            TcpServer.Start();
        }

        public static void Kill()
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                ClientList[i].ControlThread.Abort();
            }
        }

        public static void CreateArbitor(List<int> ClientList)
        {
            Arbitor NewArbitor = new Arbitor();
            NewArbitor.id = UAID++;
            NewArbitor.Clients = new List<int>();
            for (int i = 0; i < ClientList.Count; i++)
            {
                NewArbitor.Clients.Add(ClientList[i]);
            }
            NewArbitor.Init();
            ArbitorList.Add(NewArbitor);
        }

        public static void SendArbitorPacket(int UAID, Packet p)
        {
            for (int i = 0; i < ArbitorList.Count; i++)
            {
                if (ArbitorList[i].id == UAID)
                {
                    ArbitorList[i].GetPacket(p);
                    break;
                }
            }
        }

        public static void Update()
        {
            if (TcpServer.Pending())
            {
                ClientTCPThread NewThread = new ClientTCPThread();
                NewThread.Client = TcpServer.AcceptTcpClient();
                NewThread.ControlThread = new System.Threading.Thread(NewThread.Update);
                NewThread.ControlThread.Start();
                ClientList.Add(NewThread);
            }
            if (LFGClientList.Count > 1)
            {
                CreateArbitor(LFGClientList);
                LFGClientList.RemoveRange(0, LFGClientList.Count);
            }
            for (int i = 0; i < ArbitorList.Count; i++)
            {
                ArbitorList[i].Update();
            }
        }

        public static void SendPlayerPacket(int UPID, Packet p)
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (ClientList[i].UPID == UPID)
                {
                    ClientList[i].SendPacket(p);
                }
            }
        }
    }

    public class Arbitor
    {
        public int id = -1;
        public List<int> Clients;
        public List<Arbitor_Player_Rep> ClientReps = new List<Arbitor_Player_Rep>();
        public List<Arbitor_EntityRep> EntityReps = new List<Arbitor_EntityRep>();

        public bool LoadingStage = true;

        public void Init()
        {
            bool AllClientsConnected = false;
            while (!AllClientsConnected)
            {
                AllClientsConnected = true;
                for (int j = 0; j < ArbitorServer_Core.ClientList.Count; j++)
                {
                    if (ArbitorServer_Core.ClientList[j].UPID == -1)
                    {
                        AllClientsConnected = false;
                    }
                }
            }
            //Connect the clients UDP's
            for (int i = 0; i < Clients.Count; i++)
            {
                for (int j = 0; j < ArbitorServer_Core.ClientList.Count; j++)
                {
                    if (Clients[i] == ArbitorServer_Core.ClientList[j].UPID)
                    {
                        EndPoint ipend = ArbitorServer_Core.ClientList[j].Client.Client.RemoteEndPoint;
                        string temp = ipend.ToString();
                        string[] AdressTemp = temp.Split(':');
                        for (int k = 0; k < Clients.Count; k++)
                        {
                            if (k != i)
                            {
                                Packet AdressPacket = new Packet();
                                AdressPacket.String1 = AdressTemp[0];
                                AdressPacket.PacketID = 101;
                                AdressPacket.PlayerID = ArbitorServer_Core.ClientList[j].UPID;
                                ArbitorServer_Core.SendPlayerPacket(Clients[k], AdressPacket);
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        {
            bool Broadcast = true;
            for (int i = 0; i < ClientReps.Count; i++)
            {
                if (!ClientReps[i].Updated)
                {
                    Broadcast = false;
                }
            }
            //All clients have rechieved an update since last broadcast, broadcast updates
            if (Broadcast)
            {
                for (int i = 0; i < ClientReps.Count; i++)
                {
                    Packet UpdatePacket = new Packet();
                    UpdatePacket.PacketID = 0;//Player Update Packet
                    UpdatePacket.PlayerID = ClientReps[i].PlayerID;
                    //Figure best Vector
                    Vector2 Average = new Vector2();
                    for (int k = 0; k < ClientReps[i].VectorFeedbacks.Count; k++)
                    {
                        Average.X += ClientReps[i].VectorFeedbacks[k].Vector.X;
                        Average.Y += ClientReps[i].VectorFeedbacks[k].Vector.Y;
                    }
                    Average.X = Average.X / ClientReps[i].VectorFeedbacks.Count;
                    Average.Y = Average.Y / ClientReps[i].VectorFeedbacks.Count;
                    UpdatePacket.Vector = Average;

                    //Figure best Health
                    int HealthAverage = 0;
                    for (int k = 0; k < ClientReps[i].HealthFeedbacks.Count; k++)
                    {
                        HealthAverage += ClientReps[i].HealthFeedbacks[k].Int;
                    }
                    HealthAverage = HealthAverage / ClientReps[i].HealthFeedbacks.Count;
                    UpdatePacket.Int1 = HealthAverage;

                    UpdatePacket.Int2 = -1;//Tells the client to keep track of own xp

                    for (int j = 0; j < Clients.Count; j++)
                    {
                        ArbitorServer_Core.SendPlayerPacket(Clients[j], UpdatePacket);
                    }
                    ClientReps[i].Updated = false;
                }
            }

            //Now update the entities
            Broadcast = true;

            for (int i = 0; i < EntityReps.Count; i++)
            {
                if (!EntityReps[i].Updated)
                {
                    Broadcast = false;
                }
            }
            if (Broadcast)
            {
                for (int i = 0; i < EntityReps.Count; i++)
                {
                    Packet EntityUpdatePacket = new Packet();
                    EntityUpdatePacket.PacketID = 5;

                    EntityUpdatePacket.PlayerID = EntityReps[i].UED;

                    //Figure best Vector
                    Vector2 Average = new Vector2();
                    for (int k = 0; k < EntityReps[i].Position.Count; k++)
                    {
                        Average.X += EntityReps[i].Position[k].Vector.X;
                        Average.Y += EntityReps[i].Position[k].Vector.Y;
                    }
                    Average.X = Average.X / EntityReps[i].Position.Count;
                    Average.Y = Average.Y / EntityReps[i].Position.Count;
                    EntityUpdatePacket.Vector = Average;

                    //Figure best Health
                    int HealthMin = 100;
                    for (int k = 0; k < EntityReps[i].Health.Count; k++)
                    {
                        if (EntityReps[i].Health[k].Int < HealthMin)
                        {
                            HealthMin = EntityReps[i].Health[k].Int;
                        }
                    }
                    EntityUpdatePacket.Int1 = HealthMin;

                    //Figure best target
                    EntityUpdatePacket.Int2 = EntityReps[i].PlayerTargetID[0].Int;
                    //TODO: This is not as accurate as it could be

                    for (int j = 0; j < Clients.Count; j++)
                    {
                        ArbitorServer_Core.SendPlayerPacket(Clients[j], EntityUpdatePacket);
                    }
                    EntityReps[i].Updated = false;
                }
            }
        }

        public void GetPacket(Packet p)
        {
            switch (p.PacketID)
            {
                case 22:
                    //A player update packet
                    bool FoundClient = false;
                    for (int i = 0; i < ClientReps.Count; i++)
                    {
                        if (ClientReps[i].PlayerID == p.PlayerID)
                        {
                            FoundClient = true;
                            ClientReps[i].Updated = true;
                            //Set the reported vector for the player
                            bool SetVector = false;
                            for (int j = 0; j < ClientReps[i].VectorFeedbacks.Count; j++)
                            {
                                if (ClientReps[i].VectorFeedbacks[j].PlayerID == p.Int2)
                                {
                                    ClientReps[i].VectorFeedbacks[j].Vector = p.Vector;
                                    SetVector = true;
                                }
                            }
                            //Vector not reported before, create
                            if (!SetVector)
                            {
                                Vector2_PlayerLink NewVec = new Vector2_PlayerLink();
                                NewVec.PlayerID = p.Int2;
                                NewVec.Vector = p.Vector;
                                ClientReps[i].VectorFeedbacks.Add(NewVec);
                            }
                            //Set the reported health value
                            bool SetHealth = false;
                            for (int j = 0; j < ClientReps[i].HealthFeedbacks.Count; j++)
                            {
                                if (ClientReps[i].HealthFeedbacks[j].PlayerID == p.Int2)
                                {
                                    ClientReps[i].HealthFeedbacks[j].Int = p.Int1;
                                    SetHealth = true;
                                }
                            }
                            if (!SetHealth)
                            {
                                Int_PlayerLink NewInt = new Int_PlayerLink();
                                NewInt.PlayerID = p.Int2;
                                NewInt.Int = p.Int1;
                                ClientReps[i].HealthFeedbacks.Add(NewInt);
                            }
                        }
                    }
                    if (!FoundClient)
                    {
                        Arbitor_Player_Rep NewPlayer = new Arbitor_Player_Rep();
                        NewPlayer.Name = p.String1;
                        NewPlayer.PlayerID = p.PlayerID;

                        Vector2_PlayerLink NewVec = new Vector2_PlayerLink();
                        NewVec.PlayerID = p.Int2;
                        NewVec.Vector = p.Vector;
                        NewPlayer.VectorFeedbacks.Add(NewVec);

                        Int_PlayerLink NewInt = new Int_PlayerLink();
                        NewInt.PlayerID = p.Int2;
                        NewInt.Int = p.Int1;
                        NewPlayer.HealthFeedbacks.Add(NewInt);

                        ClientReps.Add(NewPlayer);
                    }
                    break;

                case 23:
                    //An entity update packet
                    bool FoundEntity = false;
                    for (int i = 0; i < EntityReps.Count; i++)
                    {
                        if (EntityReps[i].UED == p.PlayerID)
                        {
                            EntityReps[i].Updated = true;
                            FoundEntity = true;
                            //Position
                            bool SetVector = false;
                            for (int j = 0; j < EntityReps[i].Position.Count; j++)
                            {
                                if (EntityReps[i].Position[j].PlayerID == p.Int3)
                                {
                                    SetVector = true;
                                    EntityReps[i].Position[j].Vector = p.Vector;
                                }
                            }
                            if (!SetVector)
                            {
                                Vector2_PlayerLink NewVec = new Vector2_PlayerLink();
                                NewVec.PlayerID = p.Int3;
                                NewVec.Vector = p.Vector;
                                EntityReps[i].Position.Add(NewVec);
                            }

                            //Health
                            bool SetHealth = false;
                            for (int j = 0; j < EntityReps[i].Health.Count; j++)
                            {
                                if (EntityReps[i].Health[j].PlayerID == p.Int3)
                                {
                                    SetHealth = true;
                                    EntityReps[i].Health[j].Int = p.Int1;
                                }
                            }
                            if (!SetHealth)
                            {
                                Int_PlayerLink NewHealth = new Int_PlayerLink();
                                NewHealth.Int = p.Int1;
                                NewHealth.PlayerID = p.Int3;
                                EntityReps[i].Health.Add(NewHealth);
                            }

                            //Attacking Target
                            bool SetTarget = false;
                            for (int j = 0; j < EntityReps[i].PlayerTargetID.Count; j++)
                            {
                                if (EntityReps[i].PlayerTargetID[j].PlayerID == p.Int3)
                                {
                                    SetTarget = true;
                                    EntityReps[i].PlayerTargetID[j].Int = p.Int2;
                                }
                            }
                            if (!SetTarget)
                            {
                                Int_PlayerLink NewTarget = new Int_PlayerLink();
                                NewTarget.Int = p.Int2;
                                NewTarget.PlayerID = p.Int3;
                                EntityReps[i].PlayerTargetID.Add(NewTarget);
                            }
                        }
                    }
                    if (!FoundEntity)
                    {
                        Arbitor_EntityRep NewEntity = new Arbitor_EntityRep();
                        NewEntity.UED = p.PlayerID;
                        NewEntity.Updated = true;

                        NewEntity.Position = new List<Vector2_PlayerLink>();
                        Vector2_PlayerLink NewVec = new Vector2_PlayerLink();
                        NewVec.PlayerID = p.Int3;
                        NewVec.Vector = p.Vector;
                        NewEntity.Position.Add(NewVec);

                        NewEntity.Health = new List<Int_PlayerLink>();
                        Int_PlayerLink NewHealth = new Int_PlayerLink();
                        NewHealth.Int = p.Int1;
                        NewHealth.PlayerID = p.Int3;
                        NewEntity.Health.Add(NewHealth);

                        NewEntity.PlayerTargetID = new List<Int_PlayerLink>();
                        Int_PlayerLink NewTarget = new Int_PlayerLink();
                        NewTarget.Int = p.Int2;
                        NewTarget.PlayerID = p.Int3;
                        NewEntity.PlayerTargetID.Add(NewTarget);

                        EntityReps.Add(NewEntity);
                    }
                    break;
            }
        }
    }

    //Player Representation
    public class Arbitor_Player_Rep
    {
        public int PlayerID;
        public string Name;
        public List<Vector2_PlayerLink> VectorFeedbacks = new List<Vector2_PlayerLink>();
        public List<Int_PlayerLink> HealthFeedbacks = new List<Int_PlayerLink>();

        //Updated since last broadcast
        public bool Updated = false;
    }

    //Holds a position reported by a player
    public class Vector2_PlayerLink
    {
        public Vector2 Vector;
        public int PlayerID;
    }

    //Holds a int reported by a player
    public class Int_PlayerLink
    {
        public int Int;
        public int PlayerID;
    }

    //Entity Representation
    public class Arbitor_EntityRep
    {
        public int UED = -1;                    //Unique Entity Identifier
        public List<Int_PlayerLink> Health;           //Entities Health
        public List<Int_PlayerLink> PlayerTargetID;   //Who they are attacking
        public List<Vector2_PlayerLink> Position;     //Position
        public bool Updated = false;
    }
}
