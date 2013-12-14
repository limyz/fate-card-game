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

        public int caret_pos = 0;
        int hscrollbar_width;
        public int hscrollbar_offset=0;
        Rectangle hscrollbar_rec;

        public bool Highlighted { get; set; }
        public bool PasswordBox { get; set; } 
       
        string _text = "";
        public int select_start=-1;
        public int select_end=-1;
        public bool scrollable = false;
        public String Text
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
                        if (_font.Characters.Contains(c) || c == '\n' || c == '\r' || c == '\t')
                        {
                            filtered += c;
                            caret_pos += 1;
                        }
                    }
                    
                    _text = filtered;
                    if (scrollable)
                    {
                        if (Rect.Width <= _font.MeasureString(_text).X)
                        {
                            hscrollbar_width = (int)(Rect.Width * (Rect.Width / _font.MeasureString(_text).X));
                        }
                        else
                        {
                            hscrollbar_offset = 0;
                            hscrollbar_width = Rect.Width;
                        }
                    }
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
                    this.OnClick -= new FormEventHandler(textbox_clicked);
                    this.OnMouseEnter -= new FormEventHandler(textbox_OnMouseEnter);
                    this.OnMouseLeave -= new FormEventHandler(textbox_OnMouseLeave);
                }
                else
                {
                    this.OnClick += new FormEventHandler(textbox_clicked);
                    this.OnMouseEnter += new FormEventHandler(textbox_OnMouseEnter);
                    this.OnMouseLeave += new FormEventHandler(textbox_OnMouseLeave);
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
            this.Parent = parent;  
            //Form event register
            OnClick += new FormEventHandler(textbox_clicked);
            OnMouseEnter += new FormEventHandler(textbox_OnMouseEnter);
            OnMouseLeave += new FormEventHandler(textbox_OnMouseLeave);
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
        }

        public override void Update(GameTime gameTime)
        {
            if (scrollable)
            {
                hscrollbar_rec = new Rectangle(Rect.X + hscrollbar_offset, Rect.Y + Rect.Height - _font.LineSpacing, hscrollbar_width, _font.LineSpacing);
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
            Rectangle _textbox_rec = new Rectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height -(scrollable?_font.LineSpacing:0));

            spriteBatch.End();//end current screen's spriteBatch.Begin
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);           
            spriteBatch.Draw(_textBoxTexture, _textbox_rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.41f);
            spriteBatch.Draw(Highlighted ? _HighlightedTexture : _textBoxTexture, _textbox_rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f);
            if (scrollable)
            {
                Rectangle hscroll_region_rec = new Rectangle(Rect.X, Rect.Y + Rect.Height - _font.LineSpacing, Rect.Width, _font.LineSpacing);
                spriteBatch.Draw(_scrollbarBackground, hscroll_region_rec, Color.White);
                spriteBatch.Draw(_scrollbarTexture, hscrollbar_rec, Color.White);
            }
            spriteBatch.End();
            Vector2 size = _font.MeasureString(toDraw);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState);
            Rectangle currentRect = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = _textbox_rec;
            if (caretVisible && Selected)
                spriteBatch.Draw(_caretTexture, new Vector2(Rect.X + (int)size.X + 2, Rect.Y + (int)size.Y-((int)size.Y==0?0:_font.LineSpacing)), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.22f); //my caret texture was a simple vertical line, 4 pixels smaller than font size.Y
            //shadow first, then the actual text
            spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X, Rect.Y) + Vector2.One, Color.White,0f,new Vector2(0,0),1f,SpriteEffects.None,0.21f);
            spriteBatch.DrawString(_font, toDraw, new Vector2(Rect.X, Rect.Y), Color.Black,0f, new Vector2(0, 0), 1f, SpriteEffects.None,0.2f);
            //spriteBatch.DrawString(_font, toDraw, new Vector2(X, Y), Color.Black);
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
        }
        private void textbox_OnMouseEnter(object sender, FormEventData e)
        {
            Highlighted = true;
        }
        private void textbox_OnMouseLeave(object sender, FormEventData e)
        {
            Highlighted = false;
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
