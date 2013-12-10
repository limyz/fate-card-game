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
                        if (_font.Characters.Contains(c) || c == '\n')
                            filtered += c;
                    }
                    _text = filtered;
                }
            }
        }    
        public SpriteFont _font;
        public Color _color;

        public Label(string name, SpriteFont font, string text, int x, int y, int width, Color color
            , Screen parent)
            : base(name, parent, typeof(Label), new Rectangle(x, y, width, font.LineSpacing))
        {
            _font = font;
            Text = text;
            _color = color;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            sb.DrawString(_font, _text, new Vector2(rec.X, rec.Y), _color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
