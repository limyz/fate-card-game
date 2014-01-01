﻿using System;
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

        int hscrollbar_width;
        float hscrollbar_offset = 0;
        Rectangle hscrollbar_rec;
        int vscrollbar_height;
        float vscrollbar_offset = 0;
        Rectangle vscrollbar_rec;

        public bool Highlighted { get; set; }
        public bool PasswordBox { get; set; } 
       
        string _text = "";
        public int caret_position = 0;
        public int select_start = 0;
        public int select_count = 0;
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
        public override void Update(GameTime gameTime)
        {
            if (hscrollable)
            {
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

                    if (Parent.main_game.mouse_state.LeftButton == ButtonState.Released)
                        on_hscrollbar_drag = false;
                }
                int offset = (int)(hscrollbar_offset * Rect.Width);
                hscrollbar_rec = new Rectangle(Rect.X + offset, Rect.Y + Rect.Height - _font.LineSpacing, hscrollbar_width, _font.LineSpacing);
            }

            if (vscrollable)
            {
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

                    if (Parent.main_game.mouse_state.LeftButton == ButtonState.Released)
                        on_vscrollbar_drag = false;
                }
                int offset = (int)(vscrollbar_offset * Rect.Height);
                vscrollbar_rec = new Rectangle(Rect.X + Rect.Width - _font.LineSpacing, Rect.Y + offset, _font.LineSpacing, vscrollbar_height);
            }

            if (on_select_drag)
            {
                Point p = new Point(Parent.main_game.mouse_state.X, Parent.main_game.mouse_state.Y);
                caret_position = this.GetTextIndexByPoint(p);
                select_count = this.GetTextIndexByPoint(p) - select_start;

                if (Parent.main_game.mouse_state.LeftButton == ButtonState.Released)
                {
                    on_select_drag = false;
                    /*if (select_count < 0)
                    {
                        select_start += select_count;
                        select_count *= -1;
                    }
                    Game1.MessageBox(new IntPtr(0), _text.Substring(select_start, select_count), "", 0);*/
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
            //spriteBatch.Draw(Highlighted ? _HighlightedTexture : _textBoxTexture, _textbox_rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f);
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
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState);
            Rectangle currentRect = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = _textbox_rec;
            
            float hoffset = hscrollbar_offset * _font.MeasureString(_text).X * -1f;
            float voffset = vscrollbar_offset * _font.MeasureString(_text).Y * -1f;

            if (select_count > 0)
            {
                int first_line_offset = 0;
                if (select_start > 0)
                {
                    int y_line_position = 0;
                    for (int i = select_start - 1; i >= 0; i--)
                    {
                        if (_text[i] == '\n')
                        {
                            y_line_position = i + 1;
                            break;
                        }
                    }
                    first_line_offset = (int)_font.MeasureString(_text.Substring(y_line_position, select_start - y_line_position)).X;
                }
                List<Rectangle> select_recs = new List<Rectangle>();
                int new_line_position = select_start;
                for (int i = select_start; i < select_start + select_count; i++)
                {
                    if (_text[i] == '\n' || i == select_start + select_count - 1)
                    {
                        Vector2 WaH = _font.MeasureString(_text.Substring(new_line_position, i - new_line_position + 1));
                        if (select_recs.Count == 0)
                        {
                            select_recs.Add(new Rectangle(Rect.X + (int)hoffset + first_line_offset, Rect.Y + (int)voffset + select_recs.Count * _font.LineSpacing, (int)WaH.X, _font.LineSpacing));
                        }
                        else
                        {
                            select_recs.Add(new Rectangle(Rect.X + (int)hoffset, Rect.Y + (int)voffset + select_recs.Count * _font.LineSpacing, (int)WaH.X, _font.LineSpacing));
                        }
                        new_line_position = i;
                    }
                }
                foreach (Rectangle select_rec in select_recs)
                {
                    spriteBatch.Draw(_HighlightedTexture, select_rec, null, Color.Aquamarine, 0f, new Vector2(0, 0), SpriteEffects.None, 0.23f);
                }
                //spriteBatch.Draw(_HighlightedTexture, select_rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.23f);
            }
            else if (select_count < 0)
            {
                int first_line_offset = 0;
                if (select_start + select_count > 0)
                {
                    int y_line_position = 0;
                    for (int i = select_start + select_count - 1; i >= 0; i--)
                    {
                        if (_text[i] == '\n')
                        {
                            y_line_position = i + 1;
                            break;
                        }
                    }
                    first_line_offset = (int)_font.MeasureString(_text.Substring(y_line_position, select_start + select_count - y_line_position)).X;
                }
                List<Rectangle> select_recs = new List<Rectangle>();
                int new_line_position = select_start + select_count;
                for (int i = select_start + select_count; i < select_start; i++)
                {
                    if (_text[i] == '\n' || i == select_start - 1)
                    {
                        Vector2 WaH = _font.MeasureString(_text.Substring(new_line_position, i - new_line_position + 1));
                        if (select_recs.Count == 0)
                        {
                            select_recs.Add(new Rectangle(Rect.X + (int)hoffset + first_line_offset, Rect.Y + (int)voffset + select_recs.Count * _font.LineSpacing, (int)WaH.X, _font.LineSpacing));
                        }
                        else
                        {
                            select_recs.Add(new Rectangle(Rect.X + (int)hoffset, Rect.Y + (int)voffset + select_recs.Count * _font.LineSpacing, (int)WaH.X, _font.LineSpacing));
                        }
                        new_line_position = i;
                    }
                }
                foreach (Rectangle select_rec in select_recs)
                {
                    spriteBatch.Draw(_HighlightedTexture, select_rec, null, Color.Aquamarine, 0f, new Vector2(0, 0), SpriteEffects.None, 0.23f);
                }
            }
            if (caretVisible && Selected)
            {
                Vector2 size = new Vector2(0, 0);
                if (caret_position > 0)
                {
                    int y_line_position = 0;
                    for (int i = caret_position - 1; i >= 0; i--)
                    {
                        if (_text[i] == '\n')
                        {
                            y_line_position = i + 1;
                            break;
                        }
                    }
                    size.X = _font.MeasureString(_text.Substring(y_line_position, caret_position - y_line_position)).X;
                    size.Y = _font.MeasureString(_text.Substring(0, caret_position)).Y - _font.LineSpacing;
                }

                spriteBatch.Draw(_caretTexture, new Vector2(Rect.X + (int)size.X + hoffset, Rect.Y + (int)size.Y + voffset), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.22f); 
            }
            //shadow first, then the actual text            
            spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X + hoffset, Rect.Y + voffset) + Vector2.One, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.21f);
            spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X + hoffset, Rect.Y + voffset), Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.2f);

            spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
            spriteBatch.End();
            spriteBatch.Begin();//Begin a new one(just to be ended immediatly) 
        }

        public void RecieveTextInput(char inputChar)
        {
            if (select_count != 0)
            {
                if (select_count < 0)
                {
                    select_start += select_count;
                    select_count *= -1;
                }
                Text = Text.Remove(select_start, select_count);
                caret_position = select_start;
                select_count = 0;
            }
            Text = Text.Insert(caret_position, inputChar.ToString());
            caret_position += 1;
        }
        public void RecieveTextInput(string text)
        {
            if (select_count != 0)
            {
                if (select_count < 0)
                {
                    select_start += select_count;
                    select_count *= -1;
                }
                Text = Text.Remove(select_start, select_count);
                caret_position = select_start;
                select_count = 0;
            }
                Text = Text.Insert(caret_position, text);
                caret_position += text.Length;
        }
        public void RecieveCommandInput(char command)
        {
            switch (command)
            {
                case '\b': //backspace
                    /*if (select_end > select_start)
                    {   
                    }*/
                    if (Text.Length > 0)
                    {
                        if (select_count != 0)
                        {
                            if (select_count < 0)
                            {
                                select_start += select_count;
                                select_count *= -1;
                            }
                            Text = Text.Remove(select_start, select_count);
                            caret_position = select_start;
                            select_count = 0;
                        }
                        else if (caret_position > 0)
                        {
                            Text = Text.Remove(caret_position - 1, 1);
                            caret_position -= 1;
                        }

                    }
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
            if (key == Keys.Left)
            {
                if (caret_position > 0)
                {
                    caret_position --;
                    select_count = 0;
                }
            }
            else if (key == Keys.Right)
            {
                if (caret_position < _text.Length)
                {
                    caret_position ++;
                    select_count = 0;
                }
            }
        }

        #region FormHandlerFunction
        bool on_hscrollbar_drag = false;
        bool on_vscrollbar_drag = false;
        bool on_select_drag = false;
        private void check_textbox_clicked(MouseState ms)
        {
            Point mouse_position = new Point(ms.X, ms.Y);
            if (hscrollable)
            {
                if (!on_hscrollbar_drag)
                {
                    if (hscrollbar_rec.Contains(mouse_position))
                    {
                        on_hscrollbar_drag = true;
                    }
                }
            }
            if (vscrollable)
            {
                if (!on_vscrollbar_drag)
                {
                    if (vscrollbar_rec.Contains(mouse_position))
                    {
                        on_vscrollbar_drag = true;
                    }
                }
            }
            if (!on_hscrollbar_drag && !on_vscrollbar_drag)
            {
                caret_position = this.GetTextIndexByPoint(mouse_position);
                if (!on_select_drag)
                {
                    select_start = this.GetTextIndexByPoint(mouse_position);
                    /*if (select_start == _text.Length)
                    {
                        select_count = 0;
                    }*/
                    on_select_drag = true;
                }
            }
        }
        private int GetTextIndexByPoint(Point p)
        {
            int relative_x = p.X - Rect.X + (int)(hscrollbar_offset * _font.MeasureString(_text).X);
            int relative_y = p.Y - Rect.Y + (int)(vscrollbar_offset * _font.MeasureString(_text).Y);

            int y_line_position = 0;
            int test_newline = _font.LineSpacing;
            if (test_newline < relative_y)
            {
                for (int i = 0; i < _text.Length; i++)
                {
                    if (_text[i] == '\n')
                    {
                        test_newline += _font.LineSpacing;
                        if (test_newline >= relative_y)
                        {
                            y_line_position = i + 1;
                            goto OuterLabel;
                        }
                    }
                }
                return _text.Length;
            }
        OuterLabel:
            String test_char = "";
            for (int i = y_line_position; i < _text.Length; i++)
            {
                if (_text[i] == '\n')
                {
                    return Math.Max(i, 0);
                }
                test_char += _text[i];
                if (_font.MeasureString(test_char).X >= relative_x)
                {
                    return i;
                }
            }
            return _text.Length;
        }
        #endregion
        #region FormEventHandler
        private void textbox_clicked(object sender, FormEventData e)
        {
            Parent.main_game.keyboard_text_dispatcher.Subscriber = (TextBox)sender;
            Parent.ActiveForm = (SivForm)sender;
            check_textbox_clicked((MouseState)e.args);
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
