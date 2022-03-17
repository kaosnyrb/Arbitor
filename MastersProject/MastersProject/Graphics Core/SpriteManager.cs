using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace MastersProject
{
    public class Sprite
    {
        //The texture object used when drawing the sprite
        public Texture2D mSpriteTexture;

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
        }
        //Draw the sprite to the screen
        public void Draw(SpriteBatch theSpriteBatch, Vector2 Position)
        {
            theSpriteBatch.Draw(mSpriteTexture, Position, Color.White);
        }
        public void Draw(SpriteBatch theSpriteBatch, Vector2 Position, float Rotation)
        {
            theSpriteBatch.Draw(mSpriteTexture, Position, null, Color.White, Rotation, new Vector2(mSpriteTexture.Width / 2, mSpriteTexture.Height / 2), 1, SpriteEffects.None, 1);
        }
        public void DrawMini(SpriteBatch theSpriteBatch, Vector2 Position)
        {
            float Multiplyer = 0;
            if (mSpriteTexture.Height > mSpriteTexture.Width)
            {
                Multiplyer = 64.0f / (float)mSpriteTexture.Height;
            }
            else
            {
                Multiplyer = 64.0f / (float)mSpriteTexture.Width;
            }
            if ((int)(mSpriteTexture.Width * Multiplyer) > 35)
            {
                theSpriteBatch.Draw(mSpriteTexture, new Rectangle((int)Position.X, (int)Position.Y,
                     (int)(mSpriteTexture.Width * Multiplyer), (int)(mSpriteTexture.Height * Multiplyer)), Color.White);
            }
            else
            {
                theSpriteBatch.Draw(mSpriteTexture, new Rectangle((int)Position.X, (int)Position.Y,
                     35, (int)(mSpriteTexture.Height * Multiplyer)), Color.White);

            }
        }

        public void Draw(SpriteBatch theSpriteBatch, Vector2 Position, float Rotation, Color RenderColor, int frame, int Animation)
        {
            Rectangle AniRect = new Rectangle((Animation * 75), (frame * 55), 75, 55);
            theSpriteBatch.Draw(mSpriteTexture, Position, AniRect, RenderColor, Rotation,  new Vector2(35,25), 1, SpriteEffects.None, 0);
        }

        //For a 32 by 32 grid
        public void DrawTile(SpriteBatch theSpriteBatch, Vector2 Position, float Rotation, Color RenderColor, int x, int y)
        {
            Rectangle AniRect = new Rectangle((x * 33), (y * 33), 33, 33);
            theSpriteBatch.Draw(mSpriteTexture, Position, AniRect, RenderColor, Rotation, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        }
    }

    public class RenderSprite
    {
        public bool Active = false;
        public Vector2 Position = new Vector2(0, 0);
        public int SpriteID = 0;
        public float Rotation = 0;
        public Color RenderColor = Color.White;
        public bool Mini = false;
        public bool Animated = false;
        public int Frame = 0;
        public int Animation = 0;
        public bool Tile = false;
    }

    public static class SpriteManager
    {
        public static Sprite[] SpriteList;
        static RenderSprite[] RenderSpriteList;
        public static int MAXSPRITES = 50;
        public static int MAXRENDERSPRITES = 2000;

        static SpriteManager()
        {
            SpriteList = new Sprite[MAXSPRITES];
            for (int i = 0; i < MAXSPRITES; i++)
            {
                SpriteList[i] = new Sprite();
            }
            RenderSpriteList = new RenderSprite[MAXRENDERSPRITES];
            for (int i = 0; i < MAXRENDERSPRITES; i++)
            {
                RenderSpriteList[i] = new RenderSprite();
            }
        }

        public static void LoadSprite(int Index, ContentManager theContentManager, string theAssetName)
        {
            SpriteList[Index].LoadContent(theContentManager, theAssetName);
        }

        public static void ClearRenderSpriteList()
        {
            for (int i = 0; i < MAXRENDERSPRITES; i++)
            {
                RenderSpriteList[i].Active = false;
            }
        }

        public static void RenderSprite(int SpriteID, Vector2 Position)
        {
            bool SetRenderSprite = false;
            for (int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++)
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    RenderSpriteList[i].Rotation = 0;
                    RenderSpriteList[i].RenderColor = Color.White;
                    RenderSpriteList[i].Mini = false;
                    SetRenderSprite = true;
                    RenderSpriteList[i].Animated = false;
                    RenderSpriteList[i].Tile = false;
                }
            }
        }

        public static void RenderMiniSprite(int SpriteID, Vector2 Position)
        {
            bool SetRenderSprite = false;
            for (int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++)
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    RenderSpriteList[i].Rotation = 0;
                    RenderSpriteList[i].RenderColor = Color.White;
                    RenderSpriteList[i].Mini = true;
                    SetRenderSprite = true;
                    RenderSpriteList[i].Animated = false;
                    RenderSpriteList[i].Tile = false;
                }
            }
        }
        public static void RenderSprite(int SpriteID, Vector2 Position, Color DrawColour)
        {
            bool SetRenderSprite = false;
            for (int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++)
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    RenderSpriteList[i].RenderColor = DrawColour;
                    RenderSpriteList[i].Rotation = 0;
                    RenderSpriteList[i].Mini = false;
                    SetRenderSprite = true;
                    RenderSpriteList[i].Animated = false;
                    RenderSpriteList[i].Tile = false;
                }
            }
        }

        public static void RenderSprite(int SpriteID, Vector2 Position, float Rotation)
        {
            bool SetRenderSprite = false;
            for (int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++)
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    RenderSpriteList[i].Rotation = Rotation;
                    RenderSpriteList[i].RenderColor = Color.White;
                    RenderSpriteList[i].Mini = false;
                    RenderSpriteList[i].Animated = false;
                    RenderSpriteList[i].Tile = false;
                    SetRenderSprite = true;
                }
            }
        }

        public static void RenderSprite(int SpriteID, Vector2 Position, int Animation, int frame)
        {
            bool SetRenderSprite = false;
            for (int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++)
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    RenderSpriteList[i].Rotation = 0;
                    RenderSpriteList[i].RenderColor = Color.White;
                    RenderSpriteList[i].Mini = false;
                    RenderSpriteList[i].Animated = true;
                    RenderSpriteList[i].Animation = Animation;
                    RenderSpriteList[i].Tile = false;
                    RenderSpriteList[i].Frame = frame;
                    SetRenderSprite = true;
                }
            }
        }

        public static void RenderTileSprite(int SpriteID, Vector2 Position, int x, int y)
        {
            bool SetRenderSprite = false;
            for (int i = 0; i < MAXRENDERSPRITES && !SetRenderSprite; i++)
            {
                if (!RenderSpriteList[i].Active)
                {
                    RenderSpriteList[i].Active = true;
                    RenderSpriteList[i].Position = Position;
                    RenderSpriteList[i].SpriteID = SpriteID;
                    RenderSpriteList[i].Rotation = 0;
                    RenderSpriteList[i].RenderColor = Color.White;
                    RenderSpriteList[i].Mini = false;
                    RenderSpriteList[i].Animated = false;
                    RenderSpriteList[i].Tile = true;
                    RenderSpriteList[i].Animation = x;
                    RenderSpriteList[i].Frame = y;
                    SetRenderSprite = true;
                }
            }
        }
        public static void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Begin();
            for (int i = 0; i < MAXRENDERSPRITES; i++)
            {
                if (RenderSpriteList[i].Active)
                {
                    if (RenderSpriteList[i].Mini)
                    {
                        SpriteList[RenderSpriteList[i].SpriteID].DrawMini(theSpriteBatch, RenderSpriteList[i].Position);
                    }
                    else
                    {
                        if (RenderSpriteList[i].Animated || RenderSpriteList[i].Tile )
                        {
                            if (RenderSpriteList[i].Animated)
                            {
                                SpriteList[RenderSpriteList[i].SpriteID].Draw(theSpriteBatch, RenderSpriteList[i].Position, RenderSpriteList[i].Rotation, RenderSpriteList[i].RenderColor, RenderSpriteList[i].Frame, RenderSpriteList[i].Animation);
                            }
                            if ( RenderSpriteList[i].Tile)
                            {
                                SpriteList[RenderSpriteList[i].SpriteID].DrawTile(theSpriteBatch,RenderSpriteList[i].Position, RenderSpriteList[i].Rotation, RenderSpriteList[i].RenderColor, RenderSpriteList[i].Frame, RenderSpriteList[i].Animation);
                            }
                        }
                        else
                        {
                            SpriteList[RenderSpriteList[i].SpriteID].Draw(theSpriteBatch, RenderSpriteList[i].Position, RenderSpriteList[i].Rotation);
                        }
                    }
                }
            }
            theSpriteBatch.End();
        }

        public static Texture2D GetTexture(int i)
        {
            return SpriteList[i].mSpriteTexture;
        }
    }
}
