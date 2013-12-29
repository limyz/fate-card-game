using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public delegate void TextBoxEvent(TextBox sender);

    public class TextBox : SivForm,IKeyboardSubscriber
    {
        Texture2D _textBoxTexture;
        Texture2D _HighlightedTexture;
        Texture2D _caretTexture;
        Texture2D _scrollbarBackground;
        Texture2D _scrollbarTexture;
        RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };
        SpriteFont _font;

        int caret_pos = 0;
        int hscrollbar_width;
        float hscrollbar_offset = 0;
        Rectangle hscrollbar_rec;
        int vscrollbar_height;
        float vscrollbar_offset = 0;
        Rectangle vscrollbar_rec;

        public bool Highlighted { get; set; }
        public bool PasswordBox { get; set; } 
       
        string _text = "";
        public int select_start=-1;
        public int select_end=-1;
        public bool hscrollable = false;
        public bool vscrollable = false;
        public String Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == null)
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
                        if (_font.Characters.Contains(c) || c == '\n' || c == '\r' || c == '\t')
                        {
                            filtered += c;
                            caret_pos += 1;
                        }
                    }

                    if (hscrollable)
                    {
                        if (Rect.Width <= _font.MeasureString(filtered).X)
                        {
                            hscrollbar_width = (int)(Rect.Width * (Rect.Width / _font.MeasureString(filtered).X));
                            float maxoffset = 1 - (float)hscrollbar_width / (float)Rect.Width;
                            float newoffset = hscrollbar_offset * (_font.MeasureString(filtered).X / _font.MeasureString(_text).X);
                            hscrollbar_offset = Math.Min(newoffset, maxoffset);
                        }
                        else
                        {
                            hscrollbar_offset = 0;
                            hscrollbar_width = Rect.Width;
                        }
                    }
                    if (vscrollable)
                    {
                        if (Rect.Height <= _font.MeasureString(filtered).Y)
                        {
                            vscrollbar_height = (int)(Rect.Height * (Rect.Height / _font.MeasureString(filtered).Y));
                            float maxoffset = 1 - (float)vscrollbar_height / (float)Rect.Height; 
                            float newoffset = vscrollbar_offset * (_font.MeasureString(filtered).Y / _font.MeasureString(_text).Y);
                            vscrollbar_offset = Math.Min(newoffset, maxoffset);
                        }
                        else
                        {
                            vscrollbar_offset = 0;
                            vscrollbar_height = Rect.Height;
                        }
                    }
                    _text = filtered;
                }
            }
        }
        bool _readonly = false;
        public bool ReadOnly
        {
            get
            {
                return _readonly;
            }
            set
            {
                if (_readonly == value)
                {
                    return;
                }
                _readonly = value;
                if (_readonly)
                {
                    this.OnClick -= textbox_clicked;
                    this.OnClick += textbox_clicked_Readonly;
                    this.OnMouseEnter -= textbox_OnMouseEnter;
                    this.OnMouseLeave -= textbox_OnMouseLeave;
                }
                else
                {
                    this.OnClick -= textbox_clicked_Readonly;
                    this.OnClick += textbox_clicked;
                    this.OnMouseEnter += textbox_OnMouseEnter;
                    this.OnMouseLeave += textbox_OnMouseLeave;
                }
            }
        }

        public TextBox(string name, Texture2D textBoxTexture, Texture2D highlightedTexture
            , Texture2D caretTexture, Texture2D scrollbarBackground
            , Texture2D scrollbarTexture, SpriteFont font, Rectangle rec
            , Screen parent)
            : base(name, parent, typeof(TextBox),rec)
        {
            _textBoxTexture = textBoxTexture;
            _HighlightedTexture = highlightedTexture;
            _caretTexture = caretTexture;
            _scrollbarBackground = scrollbarBackground;
            _scrollbarTexture = scrollbarTexture;
            _font = font;           
            hscrollbar_width = rec.Width;
            vscrollbar_height = rec.Height;
            this.Parent = parent;  
            //Form event register
            OnClick += new FormEventHandler(textbox_clicked);
            OnMouseEnter += new FormEventHandler(textbox_OnMouseEnter);
            OnMouseLeave += new FormEventHandler(textbox_OnMouseLeave);
            OnMouseScroll += new FormEventHandler(textbox_OnMouseScroll);
        }

        public TextBox(string name, Texture2D textBoxTexture, Texture2D highlightedTexture
            , Texture2D caretTexture, SpriteFont font, Rectangle rec
            , Screen parent)
            : base(name, parent, typeof(TextBox), rec)
        {
            _textBoxTexture = textBoxTexture;
            _HighlightedTexture = highlightedTexture;
            _caretTexture = caretTexture;
            _font = font;
            this.Parent = parent;
            //Form event register
            OnClick += new FormEventHandler(textbox_clicked);
            OnMouseEnter += new FormEventHandler(textbox_OnMouseEnter);
            OnMouseLeave += new FormEventHandler(textbox_OnMouseLeave);
            OnMouseScroll += new FormEventHandler(textbox_OnMouseScroll);
        }

        bool on_hscrollbar_drag = false;
        bool on_vscrollbar_drag = false;
        public override void Update(GameTime gameTime)
        {
            if (hscrollable)
            {
                if (!on_hscrollbar_drag)
                {
                    if (Parent.main_game.left_mouse_click(hscrollbar_rec))
                    {
                        on_hscrollbar_drag = true;
                    }
                }
                if (on_hscrollbar_drag)
                {
                    int move = Parent.main_game.mouse_state.X - Parent.main_game.last_mouse_state.X;
                    float percentage = 0;
                    if (move != 0)
                    {
                       percentage = (float)move / (float)Rect.Width;
                    }
                    float maxoffset = 1 - (float)hscrollbar_width / (float)Rect.Width; 
                    hscrollbar_offset = Math.Max(Math.Min(hscrollbar_offset + percentage, maxoffset), 0);
                }
                int offset = (int)(hscrollbar_offset * Rect.Width);
                hscrollbar_rec = new Rectangle(Rect.X + offset, Rect.Y + Rect.Height - _font.LineSpacing, hscrollbar_width, _font.LineSpacing);
                if (on_hscrollbar_drag)
                {
                    if (Parent.main_game.mouse_state.LeftButton == ButtonState.Released)
                    {
                        on_hscrollbar_drag = false;
                    }
                }
            }

            if (vscrollable)
            {
                if (!on_vscrollbar_drag)
                {
                    if (Parent.main_game.left_mouse_click(vscrollbar_rec))
                    {                        
                        on_vscrollbar_drag = true;
                    }
                }
                if (on_vscrollbar_drag)
                {
                    int move = Parent.main_game.mouse_state.Y - Parent.main_game.last_mouse_state.Y;
                    float percentage = 0;
                    if (move != 0)
                    {
                        percentage = (float)move / (float)Rect.Height;
                    }
                    float maxoffset = 1 - (float)vscrollbar_height / (float)Rect.Height; 
                    vscrollbar_offset = Math.Max(Math.Min(vscrollbar_offset + percentage, maxoffset), 0);
                }
                int offset = (int)(vscrollbar_offset * Rect.Height);
                vscrollbar_rec = new Rectangle(Rect.X + Rect.Width - _font.LineSpacing, Rect.Y + offset, _font.LineSpacing, vscrollbar_height);
                if (on_vscrollbar_drag)
                {
                    if (Parent.main_game.mouse_state.LeftButton == ButtonState.Released)
                    {
                        on_vscrollbar_drag = false;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bool caretVisible = true;
            if ((gameTime.TotalGameTime.TotalMilliseconds % 1000) < 500)
                caretVisible = false;
            else
                caretVisible = true;
            String toDraw = _text;
            if (PasswordBox)
            {
                toDraw = "";
                for (int i = 0; i < Text.Length; i++)
                    toDraw += (char)0x2022; //bullet character (make sure you include it in the font!!!!)
            }
            Rectangle _textbox_rec = new Rectangle(Rect.X, Rect.Y, Rect.Width - (vscrollable ? _font.LineSpacing : 0), Rect.Height - (hscrollable ? _font.LineSpacing : 0));

            spriteBatch.End();//end current screen's spriteBatch.Begin
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(_textBoxTexture, _textbox_rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.41f);
            spriteBatch.Draw(Highlighted ? _HighlightedTexture : _textBoxTexture, _textbox_rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f);
            if (hscrollable)
            {
                Rectangle hscroll_region_rec = new Rectangle(Rect.X, Rect.Y + Rect.Height - _font.LineSpacing, Rect.Width, _font.LineSpacing);
                spriteBatch.Draw(_scrollbarBackground, hscroll_region_rec, Color.White);
                spriteBatch.Draw(_scrollbarTexture, hscrollbar_rec, Color.White);
            }
            if (vscrollable)
            {
                Rectangle vscroll_region_rec = new Rectangle(Rect.X + Rect.Width - _font.LineSpacing, Rect.Y, _font.LineSpacing, Rect.Height);
                spriteBatch.Draw(_scrollbarBackground, vscroll_region_rec, Color.White);
                spriteBatch.Draw(_scrollbarTexture, vscrollbar_rec, Color.White);
            }
            spriteBatch.End();
            Vector2 size = _font.MeasureString(toDraw);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState);
            Rectangle currentRect = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = _textbox_rec;
            if (caretVisible && Selected)
                spriteBatch.Draw(_caretTexture, new Vector2(Rect.X + (int)size.X + 2, Rect.Y + (int)size.Y - ((int)size.Y == 0 ? 0 : _font.LineSpacing)), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.22f); //my caret texture was a simple vertical line, 4 pixels smaller than font size.Y
            //shadow first, then the actual text
            float hoffset = hscrollbar_offset * _font.MeasureString(_text).X * -1f;
            float voffset = vscrollbar_offset * _font.MeasureString(_text).Y * -1f;
            spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X + hoffset, Rect.Y + voffset) + Vector2.One, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.21f);
            spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X + hoffset, Rect.Y + voffset), Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.2f);
            //spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X, Rect.Y) + Vector2.One, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.21f);
            //spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X, Rect.Y), Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.2f);
            spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
            spriteBatch.End();
            spriteBatch.Begin();//Begin a new one(just to be ended immediatly) 
        }

        public void RecieveTextInput(char inputChar)
        {
            Text = Text + inputChar;
        }
        public void RecieveTextInput(string text)
        {
            Text = Text + text;
        }
        public void RecieveCommandInput(char command)
        {
            switch (command)
            {
                case '\b': //backspace
                    if (select_end >= select_start)
                    {
                        
                    }
                    if (Text.Length > 0)
                        Text = Text.Substring(0, Text.Length - 1);
                    break;
                case '\r': //return
                    if (Parent.main_game.keyboard_state.IsKeyDown(Keys.LeftShift) || Parent.main_game.keyboard_state.IsKeyDown(Keys.RightShift))
                    {
                        if (OnShift_EnterPressed != null)
                        OnShift_EnterPressed(this);
                    }
                    else if (OnEnterPressed != null)
                    {
                        OnEnterPressed(this);
                    }
                    break;
                case '\t': //tab
                    if (OnTabPressed != null)
                        OnTabPressed(this);
                    break;
                /*case (char)1://ctrl-a
                    select_start = 0;
                    select_end = wrapedText.Length - 1;
                    break;*/
                default:
                    break;
            }
        }
        public void RecieveSpecialInput(Keys key)
        {   
            //if (key == Keys.Down) Y += _font.LineSpacing;
            //if (key == Keys.Up) Y -= _font.LineSpacing;
        }

        #region FormEventHandler
        private void textbox_clicked(object sender, FormEventData e)
        {
            Parent.main_game.keyboard_text_dispatcher.Subscriber = (TextBox)sender;
            Parent.ActiveForm = (SivForm)sender;
        }
        private void textbox_clicked_Readonly(object sender, FormEventData e)
        {
            Parent.ActiveForm = (SivForm)sender;
        }
        private void textbox_OnMouseEnter(object sender, FormEventData e)
        {
            Highlighted = true;
        }
        private void textbox_OnMouseLeave(object sender, FormEventData e)
        {
            Highlighted = false;
        }
        private void textbox_OnMouseScroll(object sender, FormEventData e)
        {
            if (vscrollable)
            {
                int change = (int)e.args;
                Console.WriteLine(change);
                float maxoffset = 1 - (float)vscrollbar_height / (float)Rect.Height;
                float offset = 0;
                if (change < 0)
                {
                    offset = vscrollbar_offset + ((float)_font.LineSpacing / _font.MeasureString(_text).Y);
                }
                else
                {
                    offset = vscrollbar_offset - ((float)_font.LineSpacing / _font.MeasureString(_text).Y);
                }
                vscrollbar_offset = Math.Max(Math.Min(offset, maxoffset), 0);
            }
            else if (hscrollable)
            {
                int change = (int)e.args;
                float maxoffset = 1 - (float)hscrollbar_width / (float)Rect.Width;
                float offset = 0;
                if (change < 0)
                {
                    offset = hscrollbar_offset + ((float)_font.LineSpacing / _font.MeasureString(_text).X);
                }
                else
                {
                    offset = hscrollbar_offset - ((float)_font.LineSpacing / _font.MeasureString(_text).X);
                }
                hscrollbar_offset = Math.Max(Math.Min(offset, maxoffset), 0);
            }
        }
        #endregion

        public event TextBoxEvent OnEnterPressed;
        public event TextBoxEvent OnTabPressed;
        public event TextBoxEvent OnShift_EnterPressed;

        public bool Selected
        {
            get;
            set;
        }
    }
}
