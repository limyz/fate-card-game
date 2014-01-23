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
        public Vector2 Location;//The sprite origin; the default is (0,0) which represents the upper-left corner.
        public SpriteEffects effects = SpriteEffects.None;//Effects to apply.
        public Vector2 Scale;
        public Vector2 Move;

        public Image(string name, Texture2D texture, Rectangle rec
            , float draw_order, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.texture = texture;
            this.DrawOrder = draw_order;
            this.Parent = parent;
            this.Rect = rec;
            this.Location.X = rec.X;
            this.Location.Y = rec.Y;
            this.Scale = new Vector2((float)Rect.Width / (float)texture.Width, (float)Rect.Height / (float)texture.Height);
            this.Move = this.Location;
        }

        public override void Update(GameTime theTime)
        {
            if (Convert.ToInt32(this.Move.X) != Convert.ToInt32(this.Location.X) && Convert.ToInt32(this.Move.Y) != Convert.ToInt32(this.Location.Y))
            {
                Vector2 speed = Helper_Direction.MoveTowards(this.Location, this.Move, 2f);
                this.Location.X += speed.X;
                this.Location.Y += speed.Y;
                Console.WriteLine("X:" + Convert.ToInt32(this.Location.X) + "/Y:" + Convert.ToInt32(this.Location.Y));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //spriteBatch.Draw(texture, Rect, Source_Rectangle, Color.White, Rotation, Origin, effects, DrawOrder);
            spriteBatch.Draw(texture, Location, Source_Rectangle, Color.White, Rotation, new Vector2(0, 0), Scale, effects, DrawOrder);
        }
    }
}
