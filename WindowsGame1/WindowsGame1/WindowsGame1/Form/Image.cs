using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Image : SivForm
    {
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public float DrawOrder = 0.5f;

        public Image(string name, Texture2D texture, Rectangle rec
            , float draw_order, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.texture = texture;
            this.DrawOrder = draw_order;
            this.Parent = parent;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, Rect, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, DrawOrder);
        }
    }
}
