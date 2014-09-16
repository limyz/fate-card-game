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
        public new bool Visible
        {
            set
            {
                base.Visible = value;
                if (!base.Visible) this.BorderImage.Visible = false;
                else this.BorderImage.Visible = true;
            }
            get { return base.Visible; }
        }
        public enum RotateFlipType
        {
            Rotate90FlipNone = 1,
            Rotate180FlipNone = 2,
            Rotate270FlipNone = 3,
            RotateFlipNone = 4,
        };
        public RotateFlipType RotateFlip;
        public Border BorderImage;
        private bool hasBorder;

        public bool HasBorder
        {
            get { return hasBorder; }
            set { 
                hasBorder = value;
                if (hasBorder) this.BorderImage.Visible = true;
                else this.BorderImage.Visible = false;
            }
        }
        public Nullable<Rectangle> Source_Rectangle = null;//A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
        public Color color = Color.White;//The color to tint the sprite. Use Color.White for full color with no tinting
        public Single Rotation = 0f;//Specifies the angle (in radians) to rotate the sprite about its center.
        public Vector2 Origin = new Vector2(0, 0);//The sprite origin; the default is (0,0) which represents the upper-left corner.
        public Vector2 Scale = new Vector2(1, 1);//Scale factor.
        public SpriteEffects effects = SpriteEffects.None;//Effects to apply.
        public float DrawOrder = 0.5f;//The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.
        public Player playerOwner = null;

        public Image(RectangleF rec, Screen parent)
            : base("image", parent, typeof(Image), rec)
        {
            this.texture = Game1.zeroTexture;
            this.DrawOrder = 0.5f;
            this.Parent = parent;
            this.Rect = rec;
            this.BorderImage = new Border("", Color.White, 1, rec, parent);
            this.HasBorder = false;
        }

        public Image(string name, Texture2D texture, RectangleF rec
            , float draw_order, Screen parent)
            : base(name, parent, typeof(Image), rec)
        {
            this.texture = texture;
            this.DrawOrder = draw_order;
            this.Parent = parent;
            this.Rect = rec;
            this.BorderImage = new Border("", Color.White, 1, rec, parent);
            this.HasBorder = false;
        }

        public Rectangle getSourceRect(){
            return (Rectangle)this.Source_Rectangle;
        }

        public override void Update(GameTime theTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //spriteBatch.Draw(texture, Rect, Source_Rectangle, color, Rotation, Origin, effects, DrawOrder);
            //spriteBatch.Draw(texture, Rect, Source_Rectangle, color, Rotation, Origin, Scale, effects, DrawOrder);
            if (this.RotateFlip == RotateFlipType.Rotate90FlipNone)
            {
                this.Rotation = (float)Math.PI * 0.5f;
                this.BorderImage.Rect = new RectangleF(this.Rect.X,
                    this.Rect.Y, this.Rect.Height, this.Rect.Width);
                spriteBatch.Draw(texture, new RectangleF(this.Rect.X + this.Rect.Height,
                    this.Rect.Y, this.Rect.Width, this.Rect.Height),
                    Source_Rectangle, color, Rotation, Origin, Scale, effects, DrawOrder);
            }
            else if (this.RotateFlip == RotateFlipType.Rotate180FlipNone)
            {
                this.Rotation = (float)Math.PI;
                this.BorderImage.Rect = this.Rect;
                spriteBatch.Draw(texture, new RectangleF(this.Rect.X + this.Rect.Width,
                    this.Rect.Y + this.Rect.Height, this.Rect.Width, this.Rect.Height),
                    Source_Rectangle, color, Rotation, Origin, Scale, effects, DrawOrder);
            }
            else if (this.RotateFlip == RotateFlipType.Rotate270FlipNone)
            {
                this.Rotation = (float)Math.PI * 1.5f;
                this.BorderImage.Rect = new RectangleF(this.Rect.X,
                    this.Rect.Y, this.Rect.Height, this.Rect.Width);
                spriteBatch.Draw(texture, new RectangleF(this.Rect.X, this.Rect.Y + this.Rect.Width, this.Rect.Width, this.Rect.Height),
                    Source_Rectangle, color, Rotation, Origin, Scale, effects, DrawOrder);
            }
            else
            {
                this.Rotation = 0f;
                this.BorderImage.Rect = this.Rect;
                //spriteBatch.Draw(texture, Rect, Source_Rectangle, color, Rotation, Origin, effects, DrawOrder);
                spriteBatch.Draw(texture, Rect, Source_Rectangle, color, Rotation, Origin, Scale, effects, DrawOrder);
            }
        }
    }
}
