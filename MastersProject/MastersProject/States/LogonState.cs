using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MastersProject
{
    class LogonState
    {
        public string Ip = "127.0.0.1";

        public bool SetIP = true;

        public int ConnectionFail = 0;

        public void Init()
        {

        }

        public void Update(GameTime Time)
        {
            SpriteManager.RenderSprite(1,new Vector2(400,300));
            SpriteManager.RenderSprite(3, new Vector2(400, 400));
            SpriteManager.RenderSprite(2, new Vector2(Mouse.GetState().X + 6, Mouse.GetState().Y + 9));

            if (SetIP)
            {
                ScreenTextManager.RenderText(Ip, new Vector2(250, 200), Color.Red);
                ScreenTextManager.RenderText(GameState.PlayerList[0].Name, new Vector2(250, 250), Color.White);
            }
            else
            {
                ScreenTextManager.RenderText(Ip, new Vector2(250, 200), Color.White);
                ScreenTextManager.RenderText(GameState.PlayerList[0].Name, new Vector2(250, 250), Color.Red);
            }

            if (ConnectionFail > 0)
            {
                ScreenTextManager.RenderText("Connection to server failed", new Vector2(250, 430), Color.Red);
                ConnectionFail--;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (Mouse.GetState().Y < 225 && Mouse.GetState().Y > 175)
                {
                    SetIP = true;
                }
                if (Mouse.GetState().Y < 275 && Mouse.GetState().Y > 225)
                {
                    SetIP = false;
                }
                if (Mouse.GetState().Y < 425 && Mouse.GetState().Y > 375 && Mouse.GetState().X > 375 && Mouse.GetState().X < 475)
                {
                    ConnectionFail = Network_Core.Connect(Ip);
                    
                }
            }
            if (KeyCooldown == 0)
            {
                KeyInput();
            }
            else
            {
                KeyCooldown--;
            }
        }

        int KeyCooldown = 0;
        int Cooldown = 8;
        public void KeyInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { if (!SetIP)GameState.PlayerList[0].Name += "a"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.B)) { if (!SetIP)GameState.PlayerList[0].Name += "b"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.C)) { if (!SetIP)GameState.PlayerList[0].Name += "c"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) { if (!SetIP)GameState.PlayerList[0].Name += "d"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.E)) { if (!SetIP)GameState.PlayerList[0].Name += "e"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.F)) { if (!SetIP)GameState.PlayerList[0].Name += "f"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.G)) { if (!SetIP)GameState.PlayerList[0].Name += "g"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.H)) { if (!SetIP)GameState.PlayerList[0].Name += "h"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.I)) { if (!SetIP)GameState.PlayerList[0].Name += "i"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.J)) { if (!SetIP)GameState.PlayerList[0].Name += "j"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.K)) { if (!SetIP)GameState.PlayerList[0].Name += "k"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.L)) { if (!SetIP)GameState.PlayerList[0].Name += "l"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.M)) { if (!SetIP)GameState.PlayerList[0].Name += "m"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.N)) { if (!SetIP)GameState.PlayerList[0].Name += "n"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.O)) { if (!SetIP)GameState.PlayerList[0].Name += "o"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.P)) { if (!SetIP)GameState.PlayerList[0].Name += "p"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) { if (!SetIP)GameState.PlayerList[0].Name += "q"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.R)) { if (!SetIP)GameState.PlayerList[0].Name += "r"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { if (!SetIP)GameState.PlayerList[0].Name += "s"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.T)) { if (!SetIP)GameState.PlayerList[0].Name += "t"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.U)) { if (!SetIP)GameState.PlayerList[0].Name += "u"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.V)) { if (!SetIP)GameState.PlayerList[0].Name += "v"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) { if (!SetIP)GameState.PlayerList[0].Name += "w"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.X)) { if (!SetIP)GameState.PlayerList[0].Name += "x"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.Y)) { if (!SetIP)GameState.PlayerList[0].Name += "y"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) { if (!SetIP)GameState.PlayerList[0].Name += "z"; KeyCooldown = Cooldown; }

            if (Keyboard.GetState().IsKeyDown(Keys.D0)) { if (SetIP)Ip += "0"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) { if (SetIP)Ip += "1"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) { if (SetIP)Ip += "2"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) { if (SetIP)Ip += "3"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) { if (SetIP)Ip += "4"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) { if (SetIP)Ip += "5"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) { if (SetIP)Ip += "6"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) { if (SetIP)Ip += "7"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) { if (SetIP)Ip += "8"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) { if (SetIP)Ip += "9"; KeyCooldown = Cooldown; }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPeriod)) { if (SetIP)Ip += "."; KeyCooldown = Cooldown; }

            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                if (SetIP)
                {
                    if (Ip.Length > 0)
                    {
                        Ip = Ip.Remove(Ip.Length - 1);
                    }
                }
                else
                {
                    if (GameState.PlayerList[0].Name.Length > 0)
                    {
                        GameState.PlayerList[0].Name = GameState.PlayerList[0].Name.Remove(GameState.PlayerList[0].Name.Length - 1);
                    }
                }
                KeyCooldown = Cooldown;
            }

        }
    }
}
