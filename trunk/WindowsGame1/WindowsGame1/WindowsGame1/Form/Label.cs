using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Label : SivForm
    {
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                if (_text == null)
                {
                    _text = "";
                }

                else
                {
                    //if you attempt to display a character that is not in your font
                    //you will get an exception, so we filter the characters
                    String filtered = "";
                    foreach (char c in value)
                    {
                        if (Font.Characters.Contains(c) || c == '\n')
                            filtered += c;
                    }
                    _text = filtered;
                }
            }
        }
        public SpriteFont Font;
        public Color Color;
        private bool center_align = false;
        public bool CenterAlign
        {
            get { return center_align; }
            set { center_align = value; }
        }

        public Label(string name, SpriteFont font, string text, int x, int y, int width, Color color
            , Screen parent)
            : base(name, parent, typeof(Label), new Rectangle(x, y, width, font.LineSpacing))
        {
            Font = font;
            Text = text;
            Color = color;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            if (center_align)
            {
                float offset_x = ((float)Rect.Width - Font.MeasureString(Text).X) / 2;
                sb.DrawString(Font, _text, new Vector2(offset_x + Rect.X, Rect.Y), Color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);
            }
            else
            {
                sb.DrawString(Font, _text, new Vector2(Rect.X, Rect.Y), Color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);
            }
        }
    }
}
