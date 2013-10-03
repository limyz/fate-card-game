using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Background
    {
        private Texture2D backgroundTexture;
        private Rectangle screenRectangle;
        //private RoomScreen roomScreen;
        //public Game1 main_game;

        public Background(Screen parent, Texture2D background_img)
        {
            backgroundTexture = background_img;
            screenRectangle = new Rectangle(0, 0, parent.main_game.window_width, parent.main_game.window_height);
        }

        public void DrawBG(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, screenRectangle, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1f);
            //base.Draw(graphics, spriteBatch, gameTime);
            //spriteBatch.End();
        }
    }
}
