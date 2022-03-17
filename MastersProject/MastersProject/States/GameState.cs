using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArbitorClient;

namespace MastersProject
{
    class GameState
    {
        public static List<Player> PlayerList = new List<Player>();
        public static List<ChatMessage> ChatConsole = new List<ChatMessage>();
        public static List<Entity> EntityList = new List<Entity>();
        Map CurrentMap = new Map();

        TimeSpan ArbitorUpdate = TimeSpan.FromSeconds(0);

        public void Init()
        {
            Player LocalPlayer = new Player();
            LocalPlayer.Init(true);
            PlayerList.Add(LocalPlayer);
            CurrentMap.init();
        }

        public void Update(GameTime gameTime)
        {
            Camera.Update();
            CurrentMap.DrawMap();
            Network_Core.Update(gameTime);
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerList[i].Update(gameTime);
            }
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].Health > 0)
                {
                    if (EntityList[i].id == 4)
                    {
                        EntityList[i].UpdateAnimation(gameTime);
                        ScreenTextManager.RenderText(EntityList[i].Health.ToString(), EntityList[i].Position - Camera.CameraPosition - new Vector2(20, 50), Color.White);
                        SpriteManager.RenderSprite(4, EntityList[i].Position - Camera.CameraPosition, EntityList[i].Animation, EntityList[i].frame);
                        if (ArbitorClient.Arbitor_Core.ArbitorActive)
                        {
                            EntityList[i].AI(gameTime);
                        }
                    }
                    else
                    {
                        SpriteManager.RenderSprite(EntityList[i].id, EntityList[i].Position - Camera.CameraPosition);
                    }
                }
            }
            if (ChatConsole.Count > 0)
            {
                for (int i = ChatConsole.Count - 1; i >= 0; i--)
                {
                    ScreenTextManager.RenderText(ChatConsole[i].Message, new Vector2(10, 300 + i * 15), Color.White);
                    ChatConsole[i].Countdown--;
                    if (ChatConsole[i].Countdown < 1)
                    {
                        ChatConsole.RemoveAt(i);
                    }
                }
            }
            if (!ArbitorClient.Arbitor_Core.ArbitorActive)
            {
                SpriteManager.RenderSprite(19, new Vector2(1000, 1000) - Camera.CameraPosition);
            }
            else
            {
                //Arbitor full update, called every second to refresh all data held by the arbitor
                if (ArbitorUpdate < gameTime.TotalRealTime)
                {
                    ArbitorUpdate = gameTime.TotalRealTime + TimeSpan.FromMilliseconds(500);
                    CompileAndSendArbitorUpdate();
                }
            }
            SpriteManager.RenderSprite(2, new Vector2(Mouse.GetState().X + 6, Mouse.GetState().Y + 9));
        }

        public static void CompileAndSendArbitorUpdate()
        {
            List<Packet> UpdateList = new List<Packet>();

            for (int i = 0; i < PlayerList.Count; i++)
            {
                Packet CurrentPlayer = new Packet();
                CurrentPlayer.PacketID = 22;
                CurrentPlayer.PlayerID = PlayerList[i].PlayerUID;
                CurrentPlayer.Vector = PlayerList[i].TargetPosition;
                CurrentPlayer.String1 = PlayerList[i].Name;
                CurrentPlayer.Int1 = PlayerList[i].Health;
                CurrentPlayer.Int2 = PlayerList[0].PlayerUID;
                UpdateList.Add(CurrentPlayer);
            }
            for (int i = 0; i < EntityList.Count; i++)
            {
                Packet EntityPacket = new Packet();
                EntityPacket.PacketID = 23;
                EntityPacket.PlayerID = EntityList[i].UED;
                EntityPacket.Vector = EntityList[i].Position;
                EntityPacket.Int1 = EntityList[i].Health;
                EntityPacket.Int2 = EntityList[i].PlayerTargetID;
                EntityPacket.Int3 = PlayerList[0].PlayerUID;
                UpdateList.Add(EntityPacket);
            }
            ArbitorClient.Arbitor_Core.SendCompleteUpdate(UpdateList);
        }
    }

    class ChatMessage
    {
        public string Message;
        public int Countdown;
    }
}
