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
            sb.Draw(texture, new RectangleF(Rect.X - Width, y, Rect.Width + Width * 2, Width), color);
        }
        private void Draw_Vertical_Line(SpriteBatch sb, float x)
        {
            sb.Draw(texture, new RectangleF(x, Rect.Y - Width, Width, Rect.Height + Width * 2), color);
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
