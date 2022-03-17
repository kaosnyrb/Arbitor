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

namespace Server
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Server MainServer;
        public static ContentManager ContentMan;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            MainServer = new Server();
            graphics.PreferredBackBufferWidth = 200;
            graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            MainServer.Init();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenTextManager.LoadFont(Content, "Arial");
            PlayerDatabase.LoadDatabase();
            ContentMan = Content;
            Map.LoadMap(1);
        }


        protected override void UnloadContent()
        {
            Console.SaveConsole();
            PlayerDatabase.SaveDatabase();
            foreach (TCPThread t in Server.ClientList)
            {
                t.ControlThread.Abort();
            }
            ArbitorServer.ArbitorServer_Core.Kill();
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenTextManager.ClearText();
            MainServer.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Console.Draw();
            ScreenTextManager.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
