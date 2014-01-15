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

        public float DrawOrder = 0.5f;//The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.
        public Nullable<Rectangle> Source_Rectangle = null;//A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
        public Single Rotation = 0f;//Specifies the angle (in radians) to rotate the sprite about its center.
        public Vector2 Origin = new Vector2(0, 0);//The sprite origin; the default is (0,0) which represents the upper-left corner.
        public SpriteEffects effects = SpriteEffects.None;//Effects to apply.

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
            spriteBatch.Draw(texture, Rect, Source_Rectangle, Color.White, Rotation, Origin, effects, DrawOrder);
        }
    }
}
