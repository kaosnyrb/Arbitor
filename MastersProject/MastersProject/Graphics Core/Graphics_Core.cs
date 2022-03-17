using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MastersProject
{
    //Handles all rendering subsystems
    class Graphics_Core
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Game GameAccess;
        public Graphics_Core(Game GameInput)
        {
            graphics = new GraphicsDeviceManager(GameInput);
            GameInput.Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            GameAccess = GameInput;
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            SpriteManager.LoadSprite(0, GameAccess.Content, "footman");
            SpriteManager.LoadSprite(1, GameAccess.Content, "LogonScreen");
            SpriteManager.LoadSprite(2, GameAccess.Content, "cursor");
            SpriteManager.LoadSprite(3, GameAccess.Content, "connect");
            SpriteManager.LoadSprite(4, GameAccess.Content, "grunt");
            SpriteManager.LoadSprite(5, GameAccess.Content, "Target");
            SpriteManager.LoadSprite(6, GameAccess.Content, "healthbar");
            SpriteManager.LoadSprite(7, GameAccess.Content, "XPbar");

            SpriteManager.LoadSprite(18, GameAccess.Content, "LFG");
            SpriteManager.LoadSprite(19, GameAccess.Content, "raid");
            SpriteManager.LoadSprite(20, GameAccess.Content, "summertiles");
            SpriteManager.LoadSprite(21, GameAccess.Content, "wintertiles");

            ScreenTextManager.LoadFont(GameAccess.Content, "Arial");
        }

        public void Update(GameTime gameTime)
        {
            SpriteManager.ClearRenderSpriteList();
            ScreenTextManager.ClearText();
        }

        public void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.DarkGreen);
            SpriteManager.Draw(spriteBatch);
            ScreenTextManager.Draw(spriteBatch);
        }
    }
}
