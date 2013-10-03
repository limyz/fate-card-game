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
            texture = Game1.white_texture;
            this.color = color;
            this.Width = width;
        }

        #region Draw's function
        private void Draw_Horizontal_Line(SpriteBatch sb, int y)
        {
            sb.Draw(texture, new Rectangle(rec.X, y, rec.Width, Width), color);
        }
        private void Draw_Vertical_Line(SpriteBatch sb, int x)
        {
            sb.Draw(texture, new Rectangle(x, rec.Y, Width, rec.Height), color);
        }
        #endregion

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            //uncomment these lines to draw negative width/height rectangle
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone);
            Draw_Horizontal_Line(sb, rec.Y);
            Draw_Horizontal_Line(sb, rec.Y + rec.Height - Width);
            Draw_Vertical_Line(sb, rec.X);
            Draw_Vertical_Line(sb, rec.X + rec.Width - Width);
            //uncomment these lines to draw negative width/height rectangle
            //sb.End();
            //sb.Begin();
        }
    }
}
