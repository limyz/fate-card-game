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
        private Texture2D texture;
        private Color _color;
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                this.Set_Rect(this.Rect, value);
            }
        }

        public Border(string name, Color color, float width, RectangleF rec, Screen parent)
            : base(name, parent, typeof(Border), rec)
        {
            texture = Game1.whiteTexture;
            this.color = color;
            this.Width = width;

            this.Set_Rect(rec, width);
        }

        public void Set_Rect(RectangleF rec, float width)
        {
            this.Rect = new RectangleF(this.Rect.X - width, this.Rect.Y - width, this.Rect.Width + width * 2, this.Rect.Height + width * 2);
        }

        #region Draw's function
        private void Draw_Horizontal_Line(SpriteBatch sb, float y)
        {
            sb.Draw(texture, new RectangleF(Rect.X, y, Rect.Width, Width), color);
        }
        private void Draw_Vertical_Line(SpriteBatch sb, float x)
        {
            sb.Draw(texture, new RectangleF(x, Rect.Y, Width, Rect.Height), color);
        }
        #endregion

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            //uncomment these lines to draw negative width/height rectangle
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone);
            Draw_Horizontal_Line(sb, Rect.Y);
            Draw_Horizontal_Line(sb, Rect.Y + Rect.Height - Width);
            Draw_Vertical_Line(sb, Rect.X);
            Draw_Vertical_Line(sb, Rect.X + Rect.Width - Width);
            //uncomment these lines to draw negative width/height rectangle
            //sb.End();
            //sb.Begin();
        }
    }
}
