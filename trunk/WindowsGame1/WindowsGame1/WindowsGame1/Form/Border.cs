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
        public int Width;
        public Border(string name, Color color, int width, Rectangle rec, Screen parent)
            : base(name, parent, typeof(Border), rec)
        {
            texture = Game1.whiteTexture;
            this.color = color;
            this.Width = width;
        }

        #region Draw's function
        private void Draw_Horizontal_Line(SpriteBatch sb, int y)
        {
            sb.Draw(texture, new Rectangle(Rect.X, y, Rect.Width, Width), color);
        }
        private void Draw_Vertical_Line(SpriteBatch sb, int x)
        {
            sb.Draw(texture, new Rectangle(x, Rect.Y, Width, Rect.Height), color);
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
