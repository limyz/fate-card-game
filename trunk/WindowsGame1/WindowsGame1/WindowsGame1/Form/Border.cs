using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Border : SivForm
    {
        public Texture2D texture;
        public Nullable<Rectangle> Source_Rectangle = null;//A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
        public Color color = Color.White;//The color to tint the sprite. Use Color.White for full color with no tinting
        public Single Rotation = 0f;//Specifies the angle (in radians) to rotate the sprite about its center.
        public Vector2 Origin = new Vector2(0, 0);//The sprite origin; the default is (0,0) which represents the upper-left corner.
        public SpriteEffects effects = SpriteEffects.None;//Effects to apply.
        public float DrawOrder = 0.5f;//The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.
        
        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
            }
        }

        public Border(string name, Color color, float width, RectangleF rec, Screen parent)
            : base(name, parent, typeof(Border), rec)
        {
            texture = Game1.whiteTexture;
            this.color = color;
            this.Width = width;
        }

        #region Draw's function
        private void Draw_Horizontal_Line(SpriteBatch sb, float y)
        {
            sb.Draw(texture, new RectangleF(Rect.X - Width, y, Rect.Width + Width * 2, Width), Source_Rectangle, color, Rotation, Origin, effects, DrawOrder);
        }
        private void Draw_Vertical_Line(SpriteBatch sb, float x)
        {
            sb.Draw(texture, new RectangleF(x, Rect.Y - Width, Width, Rect.Height + Width * 2), Source_Rectangle, color, Rotation, Origin, effects, DrawOrder);
        }
        #endregion

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            //uncomment these lines to draw negative width/height rectangle
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone);
            Draw_Horizontal_Line(sb, Rect.Y - Width);
            Draw_Horizontal_Line(sb, Rect.Y + Rect.Height );
            Draw_Vertical_Line(sb, Rect.X - Width);
            Draw_Vertical_Line(sb, Rect.X + Rect.Width );
            //uncomment these lines to draw negative width/height rectangle
            //sb.End();
            //sb.Begin();
        }
    }
}
