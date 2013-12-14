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
        public Texture2D Texture;
        public float DrawOrder = 0.5f;
        public Color Color;

        public Div(string name, Rectangle rec, Color color
            , float draw_order, Screen parent)
            : base(name, parent, typeof(Div), rec)
        {
            this.Texture = Game1.whiteTexture;
            this.Color = color;
            this.DrawOrder = draw_order;
            this.Parent = parent;
        }
        public Div(string name, Rectangle rec, Color color, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.Texture = Game1.whiteTexture;
            this.Color = color;
            this.Parent = parent;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Rect, null, Color, 0f, new Vector2(0, 0), SpriteEffects.None, DrawOrder);
        }
    }
}
