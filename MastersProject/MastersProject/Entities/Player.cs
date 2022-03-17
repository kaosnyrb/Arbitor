using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ArbitorClient;

namespace MastersProject
{
    class Player
    {
        public Vector2 Position = new Vector2(400, 300);
        public Vector2 TargetPosition = new Vector2(400, 300);
        public string Name = "Player";

        public int Health = 100;
        public int Level;
        public int XP;

        bool LocalPlayer = true;

        public int PlayerUID = -22;
        bool ChatMode = false;

        int PlayerTarget = -1;

        TimeSpan AttackCooldown = TimeSpan.FromSeconds(0);
        TimeSpan Animation = TimeSpan.FromSeconds(0);

        int moving = 0;
        int animation = 0;
        int frame = 0;
        int attack = 0;


        public bool qued = false;

        public void Init(bool isLocalPlayer)
        {
            LocalPlayer = isLocalPlayer;
        }

        public void Update(GameTime Time)
        {
            if (LocalPlayer)
            {
                //Attacking/Targeting
                if (PlayerTarget != -1)
                {
                    //Draw the target box or clear the target if it is dead.
                    if (GameState.EntityList[PlayerTarget].Health > 0)
                    {
                        SpriteManager.RenderSprite(5, GameState.EntityList[PlayerTarget].Position - Camera.CameraPosition);
                    }
                    else
                    {
                        PlayerTarget = -1;
                    }
                }
                //If our swing timer has reset
                if (Time.TotalRealTime > AttackCooldown)
                {
                    if (PlayerTarget != -1)
                    {
                        if ((GameState.EntityList[PlayerTarget].Position - Position).Length() < 30)
                        {
                            //Attack Packet
                            Packet AttackPack = new Packet();
                            AttackPack.PlayerID = PlayerUID;
                            AttackPack.Int1 = GameState.EntityList[PlayerTarget].UED;
                            AttackPack.Int2 = 10;
                            AttackPack.PacketID = 4;
                            AttackCooldown = Time.TotalRealTime + TimeSpan.FromSeconds(1);
                            attack = 4;//Animation Timer
                            if (Arbitor_Core.ArbitorActive)
                            {
                                GameState.EntityList[PlayerTarget].Health -= 10;
                            }
                            else
                            {
                                Network_Core.SendPacket(AttackPack);
                            }
                        }
                    }
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Vector2 MousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    if (!qued)
                    {
                        if ((MousePosition - (new Vector2(1000, 1000) - Camera.CameraPosition)).Length() < 100)
                        {
                            Packet RaidJoin = new Packet();
                            RaidJoin.PlayerID = PlayerUID;
                            RaidJoin.PacketID = 66;
                            Network_Core.SendPacket(RaidJoin);
                            qued = true;
                            //We've flaged ourselves as looking for a group, connect to the Arbitor server
                            Arbitor_Core.Connect(PlayerUID);
                        }
                    }
                    for (int i = 0; i < GameState.EntityList.Count; i++)
                    {
                        if ((MousePosition - (GameState.EntityList[i].Position - Camera.CameraPosition)).Length() < 100
                            && (MousePosition - (GameState.EntityList[i].Position - Camera.CameraPosition)).Length() > -100)
                        {
                            switch (GameState.EntityList[i].id)
                            {
                                //Interact with object
                                case 4:
                                    PlayerTarget = i;
                                    break;
                            }
                        }
                    }
                }
                //Alt targeting
                if (Keyboard.GetState().IsKeyDown(Keys.Tab))
                {
                    int BestTarget = 0;
                    float BestDistance = 100000;
                    for (int i = 0; i < GameState.EntityList.Count; i++)
                    {
                        if ((GameState.EntityList[i].Position - Position).Length() < BestDistance && GameState.EntityList[i].Health > 1)
                        {
                            BestTarget = i;
                            BestDistance = (GameState.EntityList[i].Position - Position).Length();
                        }
                    }
                    PlayerTarget = BestTarget;
                }
                //Movement
                if (!ChatMode)
                {
                    moving = 0;
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        TargetPosition.Y--;
                        moving = 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        TargetPosition.Y++;
                        moving = 2;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        TargetPosition.X++;
                        moving = 3;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        TargetPosition.X--;
                        moving = 4;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        if (PressCooldown == 0)
                        {
                            ChatMode = true;
                            PressCooldown = 8;
                        }
                    }
                    if (!Map.GetWalkable(TargetPosition))
                    {
                        TargetPosition = Position;
                    }
                    if (PressCooldown > 0)
                    {
                        PressCooldown--;
                    }
                    Camera.CameraPosition = Position - new Vector2(400, 300);
                }
                else
                {
                    HandleTextInput();
                    MapEditing(Time);
                }
                //HealthBar
                for (int i = 0; i < Health; i++)
                {
                    SpriteManager.RenderSprite(6, new Vector2(10 + (i * 2), 20));
                }
                //XP
                ScreenTextManager.RenderText("Level " + (XP / 100 + 1).ToString(), new Vector2(70, 37), Color.WhiteSmoke);
                int xpdelta = XP % 100;
                for (int i = 0; i < xpdelta; i++)
                {
                    SpriteManager.RenderSprite(7, new Vector2(10 + (i * 2), 50));
                }
                ScreenTextManager.RenderText(Name, new Vector2(400, 300) - new Vector2((Name.Length / 2) * 10, 60), Color.White);
                //LFG
                if (qued)
                {
                    SpriteManager.RenderSprite(18, new Vector2(250, 50));
                }
                if (Animation < Time.TotalGameTime)
                {
                    switch (moving)
                    {
                        case 0:
                            break;
                        case 1:
                            frame++;
                            animation = 0;
                            break;
                        case 2:
                            frame++;
                            animation = 4;
                            break;
                        case 3:
                            frame++;
                            animation = 2;
                            break;
                        case 4:
                            frame++;
                            animation = 7;
                            break;
                    }
                    Animation = Time.TotalGameTime + TimeSpan.FromMilliseconds(150);
                    if (frame > 4)
                    {
                        frame = 0;
                    }
                    if (attack > 0)
                    {
                        frame = 5 + (4 - attack);
                        attack--;
                    }
                }
                SpriteManager.RenderSprite(0, new Vector2(400, 300), animation, frame);
            }
            else
            {
                Point CheckPoint = new Point((int)Position.X, (int)Position.Y);
                if (Camera.FieldOfView.Contains(CheckPoint))
                {
                    ScreenTextManager.RenderText(Name, Position - new Vector2((Name.Length / 2) * 10, 60) - Camera.CameraPosition, Color.White);
                    SpriteManager.RenderSprite(0, Position - Camera.CameraPosition, 0, 0);
                }
            }
            if ((TargetPosition - Position).Length() > 0.5f)
            {
                if ((TargetPosition - Position).Length() > 50)
                {
                    Position = TargetPosition;
                }
                else
                {
                    Position = Vector2.SmoothStep(Position, TargetPosition, 0.15f);
                    //Animation
                    Vector2 Temp = Position - TargetPosition;
                    Temp.Normalize();
                    if (Temp.X > 0.5f)
                    {
                        if (Temp.Y > 0)
                        {
                            moving = 8;
                        }
                        if (Temp.Y < 0)
                        {
                            moving = 6;
                        }
                    }
                    if (Temp.X < -0.5f)
                    {
                        if (Temp.Y > 0)
                        {
                            moving = 1;
                        }
                        if (Temp.Y < 0)
                        {
                            moving = 3;
                        }
                    }
                    if (Animation < Time.TotalGameTime)
                    {
                        switch (moving)
                        {
                            case 0:
                                break;
                            case 1:
                                frame++;
                                animation = 0;
                                break;
                            case 2:
                                frame++;
                                animation = 4;
                                break;
                            case 3:
                                frame++;
                                animation = 2;
                                break;
                            case 4:
                                frame++;
                                animation = 7;
                                break;
                        }
                        Animation = Time.TotalGameTime + TimeSpan.FromMilliseconds(150);
                        if (frame > 4)
                        {
                            frame = 0;
                        }
                        if (attack > 0)
                        {
                            frame = 5 + (4 - attack);
                            attack--;
                        }
                    }
                }
            }
        }

        int brush = 0;
        TimeSpan Brushchange = TimeSpan.FromMilliseconds(1);
        public void MapEditing(GameTime Time)
        {
            //Map design
            int x = brush % 20;
            int y = brush / 20;

            ScreenTextManager.RenderText(brush.ToString(), new Vector2(100, 100), Color.White);

            SpriteManager.RenderTileSprite(20, new Vector2(500, 10), y, x);

            if (Keyboard.GetState().IsKeyDown(Keys.Home))
            {
                ChatMessage saved = new ChatMessage();
                saved.Countdown = 200;
                saved.Message = "MAP SAVED";
                GameState.ChatConsole.Add(saved);
                Map.Save();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp) && Time.TotalGameTime > Brushchange)
            {
                brush++;
                Brushchange = Time.TotalGameTime + TimeSpan.FromMilliseconds(200);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown) && brush > 0 && Time.TotalGameTime > Brushchange)
            {
                brush--;
                Brushchange = Time.TotalGameTime + TimeSpan.FromMilliseconds(200);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                int tilex = (int)((Mouse.GetState().X + Camera.CameraPosition.X) / 32);
                int tiley = (int)((Mouse.GetState().Y + Camera.CameraPosition.Y) / 32);

                if (tilex >= 0 && tiley >= 0)
                {
                    Map.MapData[tilex][tiley] = brush;
                }
            }
        }

        string CurrentMessage = "";
        int PressCooldown = 0;
        public void HandleTextInput()
        {
            if (PressCooldown == 0)
            {
                Keys[] Pressed = Keyboard.GetState().GetPressedKeys();
                if (Pressed.Length > 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.A)) { CurrentMessage += "a"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.B)) { CurrentMessage += "b"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.C)) { CurrentMessage += "c"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.D)) { CurrentMessage += "d"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.E)) { CurrentMessage += "e"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.F)) { CurrentMessage += "f"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.G)) { CurrentMessage += "g"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.H)) { CurrentMessage += "h"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.I)) { CurrentMessage += "i"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.J)) { CurrentMessage += "j"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.K)) { CurrentMessage += "k"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.L)) { CurrentMessage += "l"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.M)) { CurrentMessage += "m"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.N)) { CurrentMessage += "n"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.O)) { CurrentMessage += "o"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.P)) { CurrentMessage += "p"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Q)) { CurrentMessage += "q"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.R)) { CurrentMessage += "r"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.S)) { CurrentMessage += "s"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.T)) { CurrentMessage += "t"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.U)) { CurrentMessage += "u"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.V)) { CurrentMessage += "v"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.W)) { CurrentMessage += "w"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.X)) { CurrentMessage += "x"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Y)) { CurrentMessage += "y"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Z)) { CurrentMessage += "z"; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Back)) { if (CurrentMessage.Length > 0) CurrentMessage = CurrentMessage.Remove(CurrentMessage.Length - 1); }
                    if (Keyboard.GetState().IsKeyDown(Keys.Space)) { CurrentMessage += " "; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        ChatMode = false;
                        if (CurrentMessage.Length > 0)
                        {
                            Network_Core.SendChat(CurrentMessage);
                        }
                        CurrentMessage = "";
                    }
                    PressCooldown = 8;
                }
            }
            else
            {
                PressCooldown--;
            }
            ScreenTextManager.RenderText(CurrentMessage + "|", new Vector2(10, 500), Color.White);
        }
    }
}
