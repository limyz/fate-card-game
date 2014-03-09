using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Background : SivForm
    {
        public Texture2D BackgroundTexture;

        public Background(Texture2D background_img, Screen parent)
            : base("background", parent, typeof(Background)
            , new RectangleF(0, 0, parent.main_game.window_width, parent.main_game.window_height)
            , 0.1f, 0.5f)
        {
            BackgroundTexture = background_img;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(BackgroundTexture, Rect, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1f);
        }        
    }
}
