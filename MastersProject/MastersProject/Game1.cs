using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using ArbitorClient;

namespace MastersProject
{
    public enum States
    {
        Logon,
        Game
    }

    public class BaseGame : Microsoft.Xna.Framework.Game
    {
        Graphics_Core Graphics;
        GameState Game;
        LogonState Logon;
        
        public static ContentManager ContentMan;
        
        public static States CurrentState = States.Logon;

        public BaseGame()
        {
            Graphics = new Graphics_Core(this);
            Game = new GameState();
            Logon = new LogonState();
        }

        protected override void Initialize()
        {
            Logon.Init();
            Game.Init();
            base.Initialize();

        }

        protected override void LoadContent()
        {
            Graphics.LoadContent();
            ContentMan = Content;
            Map.LoadMap(1);
        }

        protected override void UnloadContent()
        {
            Packet DisconnectPacket = new Packet();
            DisconnectPacket.PacketID = -1;
            Network_Core.SendPacket(DisconnectPacket);
        }

        protected override void Update(GameTime gameTime)
        {
            Graphics.Update(gameTime);
            switch (CurrentState)
            {
                case States.Logon:
                    Logon.Update(gameTime);
                    break;

                case States.Game:
                    Game.Update(gameTime);
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
