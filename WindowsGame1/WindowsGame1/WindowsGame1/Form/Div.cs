using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Div : SivForm
    {
        public Texture2D texture;
        public float draw_order = 0.5f;
        public Color color;

        public Div(string name, Rectangle rec, Color color
            , float draw_order, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.texture = Game1.white_texture;
            this.color = color;
            this.draw_order = draw_order;
            this.parent = parent;
        }
        public Div(string name, Rectangle rec, Color color, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.texture = Game1.white_texture;
            this.color = color;
            this.parent = parent;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, rec, null, color, 0f, new Vector2(0, 0), SpriteEffects.None, draw_order);
        }
    }
}
