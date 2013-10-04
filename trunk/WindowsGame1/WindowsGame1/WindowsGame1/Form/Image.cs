using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Image : SivForm
    {
        public Texture2D texture;
        float draw_order = 0.5f;

        public Image(string name, Texture2D texture, Rectangle rec
            , float draw_order, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.texture = texture;
            this.draw_order = draw_order;
            this.parent = parent;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, draw_order);
        }
    }
}
