using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EventTextInput;
using System.Net;
using System.Net.Sockets;

#region MyExtension
namespace WindowsGame1
{
    public static class My_Extension
    {
        /*[DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);*/

        public static void move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex) newIndex--;
            // the actual index could have shifted due to the removal
            list.Insert(newIndex, item);
        }
        public static void swap<T>(this List<T> list, int x, int y)
        {
            var temp = list[x];
            list[x] = list[y];
            list[y] = temp;
        }
        public static void shuffle<T>(this List<T> list)
        {
            Random rand = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int rd = rand.Next(i, list.Count);
                list.move<T>(rd, i);
            }
            //MessageBox(new IntPtr(0), list.Count.ToString(), "", 0);
        }

        /*public static Rectangle move(this Rectangle rec, int new_x, int new_y, int speed_x, int speed_y)
        {
            if (rec.X == new_x && rec.Y == new_y)
            {
                return rec;
            }
            if (rec.X < new_x)
            {
                rec.X += Math.Min(new_x - rec.X, speed_x);
            }
            else if (rec.X > new_x)
            {
                rec.X -= Math.Min(rec.X - new_x, speed_x);
            }

            if (rec.Y < new_y)
            {
                rec.Y += Math.Min(new_y - rec.Y, speed_y);
            }
            else if (rec.Y > new_y)
            {
                rec.Y -= Math.Min(rec.Y - new_y, speed_y);
            }
            return rec;
        }
        public static void move_rec(ref Rectangle rec, int new_x, int new_y, int speed_x, int speed_y)
        {
            if (rec.X == new_x && rec.Y == new_y)
            {
                return;
            }
            if (rec.X < new_x)
            {
                rec.X += Math.Min(new_x - rec.X, speed_x);
            }
            else if (rec.X > new_x)
            {
                rec.X -= Math.Min(rec.X - new_x, speed_x);
            }

            if (rec.Y < new_y)
            {
                rec.Y += Math.Min(new_y - rec.Y, speed_y);
            }
            else if (rec.Y > new_y)
            {
                rec.Y -= Math.Min(rec.Y - new_y, speed_y);
            }
            return;
        }*/

        public static void Draw(this SpriteBatch sb, Texture2D texture, RectangleF destinationRectangle, Color color)
        {
            sb.Draw(texture, destinationRectangle.getXY(), null, color, 0f, new Vector2(0), destinationRectangle.getScale(texture), SpriteEffects.None, 0.5f);
        }

        public static void Draw(this SpriteBatch sb, Texture2D texture, RectangleF destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            sb.Draw(texture, destinationRectangle.getXY(), sourceRectangle, color, 0f, new Vector2(0), destinationRectangle.getScale(texture), SpriteEffects.None, 0.5f);
        }

        public static void Draw(this SpriteBatch sb, Texture2D texture, RectangleF destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            sb.Draw(texture, destinationRectangle.getXY(), sourceRectangle, color, rotation, origin, destinationRectangle.getScale(texture), effects, layerDepth);
        }
        public static void Draw(this SpriteBatch sb, Texture2D texture, RectangleF destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            sb.Draw(texture, destinationRectangle.getXY(), sourceRectangle, color, rotation, origin, destinationRectangle.getScale(texture)* scale, effects, layerDepth);
        }
        public static void DrawLayer(this SpriteBatch sb, Texture2D texture, RectangleF destinationRectangle, Color color)
        {
            Rectangle source = texture.Bounds;//|
            Rectangle topLeft = new Rectangle(0, 0, source.Width / 4, source.Height / 4);
            Rectangle middleLeft = new Rectangle(0, topLeft.Y + topLeft.Height, (source.Width / 4), (source.Height / 4) * 2);
            Rectangle bottomLeft = new Rectangle(0, middleLeft.Y + middleLeft.Height, source.Width / 4, source.Height / 4);
            Rectangle topCenter = new Rectangle(topLeft.X + topLeft.Width, 0, (source.Width / 4) * 2, (source.Height / 4));
            Rectangle middleCenter = new Rectangle(middleLeft.X + middleLeft.Width, topCenter.Y + topCenter.Height, (source.Width / 4) * 2, (source.Height / 4) * 2);
            Rectangle bottomCenter = new Rectangle(bottomLeft.X + bottomLeft.Width, middleCenter.Y + middleCenter.Height, (source.Width / 4) * 2, (source.Height / 4));
            Rectangle topRight = new Rectangle(topCenter.X + topCenter.Width, 0, source.Width / 4, source.Height / 4);
            Rectangle middleRight = new Rectangle(middleCenter.X + middleCenter.Width, topRight.Y + topRight.Height, (source.Width / 4), (source.Height / 4) * 2);
            Rectangle bottomRight = new Rectangle(bottomCenter.X + bottomCenter.Width, middleRight.Y + middleRight.Height, source.Width / 4, source.Height / 4);

            RectangleF destinationTopLeft = new RectangleF(destinationRectangle.X, destinationRectangle.Y, source.Width / 4, source.Height / 4);
            RectangleF destinationBottomLeft = new RectangleF(destinationRectangle.X,
                (destinationRectangle.Y + destinationRectangle.Height) - (source.Height / 4), source.Width / 4, source.Height / 4);
            RectangleF destinationTopRight = new RectangleF((destinationRectangle.X + destinationRectangle.Width) - (source.Width / 4),
                destinationRectangle.Y, source.Width / 4, source.Height / 4);
            RectangleF destinationBottomRight = new RectangleF((destinationRectangle.X + destinationRectangle.Width) - (source.Width / 4),
                (destinationRectangle.Y + destinationRectangle.Height) - (source.Height / 4), source.Width / 4, source.Height / 4);

            RectangleF destinationMiddleLeft = new RectangleF(destinationRectangle.X, destinationRectangle.Y + destinationTopLeft.Height,
                destinationTopLeft.Width, destinationRectangle.Height - (destinationTopLeft.Height + destinationBottomLeft.Height));
            RectangleF destinationTopCenter = new RectangleF(destinationRectangle.X + destinationTopLeft.Width, destinationRectangle.Y,
                destinationRectangle.Width - (destinationTopLeft.Width + destinationTopRight.Width), destinationTopLeft.Height);
            RectangleF destinationMiddleRight = new RectangleF((destinationRectangle.X + destinationRectangle.Width) - destinationTopRight.Width,
                destinationRectangle.Y + destinationTopRight.Height, destinationTopRight.Width, destinationRectangle.Height - (destinationTopRight.Height + destinationBottomRight.Height));
            RectangleF destinationBottomCenter = new RectangleF(destinationRectangle.X + destinationBottomLeft.Width,
                (destinationRectangle.Y + destinationRectangle.Height) - destinationBottomLeft.Height, destinationRectangle.Width - (destinationBottomLeft.Width + destinationBottomRight.Width),
                destinationBottomLeft.Height);
            RectangleF destinationMiddleCenter = new RectangleF(destinationRectangle.X + destinationMiddleLeft.Width, destinationRectangle.Y + destinationTopCenter.Height,
               destinationRectangle.Width - (destinationMiddleLeft.Width + destinationMiddleRight.Width), destinationRectangle.Height - (destinationTopCenter.Height + destinationBottomCenter.Height));

            Vector2 v1 = destinationMiddleLeft.getScale(texture);
            Vector2 v2 = destinationTopCenter.getScale(texture);
            Vector2 v3 = destinationMiddleRight.getScale(texture);
            Vector2 v4 = destinationBottomCenter.getScale(texture);
            sb.Draw(texture, destinationTopLeft.getXY(), topLeft, color, 0f, new Vector2(0), destinationTopLeft.getScale(texture)*4, SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationBottomLeft.getXY(), bottomLeft, color, 0f, new Vector2(0), destinationBottomLeft.getScale(texture)*4, SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationTopRight.getXY(), topRight, color, 0f, new Vector2(0), destinationTopRight.getScale(texture)*4, SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationBottomRight.getXY(), bottomRight, color, 0f, new Vector2(0), destinationBottomRight.getScale(texture)*4, SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationMiddleLeft.getXY(), middleLeft, color, 0f, new Vector2(0), new Vector2(v1.X * 4, v1.Y * 2), SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationTopCenter.getXY(), topCenter, color, 0f, new Vector2(0), new Vector2(v2.X * 2, v2.Y * 4), SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationMiddleRight.getXY(), middleRight, color, 0f, new Vector2(0), new Vector2(v3.X * 4, v3.Y * 2), SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationBottomCenter.getXY(), bottomCenter, color, 0f, new Vector2(0), new Vector2(v4.X * 2, v4.Y * 4), SpriteEffects.None, 0.5f);
            sb.Draw(texture, destinationMiddleCenter.getXY(), middleCenter, color, 0f, new Vector2(0), destinationMiddleCenter.getScale(texture)*2, SpriteEffects.None, 0.5f);
        }
    }
}
#endregion

#region MyCustomEventArgs class
public delegate void SivEventHandler(object sender, SivEventArgs e);
public class SivEventArgs : EventArgs
{
    public SivEventArgs(decimal c)
    {
        command_code = c;
    }

    public SivEventArgs(decimal c, object d)
    {
        command_code = c;
        data = d;
    }

    private object data;
    public object Data
    {
        get { return data; }
        set { data = value; }
    }

    private decimal command_code;
    public decimal Command_code
    {
        get { return command_code; }
        set { command_code = value; }
    }
}
public delegate void MessageEventHandler(object sender, string message);
public delegate void SivEventHandler2(object sender, object data);

public delegate void MouseScrollEventHandler(int value_change);
public delegate void MouseEventHandler(MouseState mouseState, MouseState lastMouseState);
public delegate void KeyboardEventHandler(Keys[] keys);
public delegate void EventRaiserDelegate();
public delegate void UpdateDelegate(GameTime gametime);
public delegate void DrawDelegate(SpriteBatch spriteBatch, GameTime gameTime);
#endregion



namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variable Decleration
        public MenuScreen mMenuScreen;
        public InGameScreen mInGameScreen;
        public HostScreen mHostScreen;
        public RoomScreen mRoomScreen;
        public JoinScreen mJoinScreen;
        public CharacterSelectScreen mCharacterSelectScreen;

        public Screen mCurrentScreen;
        public Debug debugger = new Debug();

        //static variable
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);
        public static Texture2D whiteTexture, zeroTexture, transparentTextBox, textboxBackground;
        public static Texture2D whiteTextbox, highlightedTextbox, caret, scrollbarBackground, scrollbarBackground_vert, scrollbar, scrollbar_horz, scrollbar_vert;
        public static Texture2D panelTexture, scrollbarHover_Horz, scrollbarHover_Vert;
        public static SpriteFont font, arial14Bold, arial12Bold, arial13Bold, gautami12Regular, gautami14Bold;   
        //end static variable

        public int window_width = 1200;
        public int window_height = 720;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public MouseState last_mouse_state;
        public MouseState mouse_state;
        public Point mouse_pos;
        public KeyboardState last_keyboard_state;
        public KeyboardState keyboard_state;
        public Song Background_Music;

        public EventRaiserDelegate Event_Raiser_Delegate;
        public KeyboardTextDispatcher keyboard_text_dispatcher;

        public RasterizerState rz = new RasterizerState();         
        #endregion

        #region Event raiser function
        private void On_Click_Dispatcher()
        {
            if (last_mouse_state.LeftButton == ButtonState.Released
                && mouse_state.LeftButton == ButtonState.Pressed)
            {
                //MessageBox(new IntPtr(0), "mouseClick", "", 0);
                if (mCurrentScreen.OnClick != null)
                    mCurrentScreen.OnClick.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Mouse_Up_Dispatcher()
        {
            if (last_mouse_state.LeftButton == ButtonState.Pressed
                && mouse_state.LeftButton == ButtonState.Released)
            {
                //MessageBox(new IntPtr(0), "mouseUp", "", 0);
                if (mCurrentScreen.OnMouseUp != null)
                    mCurrentScreen.OnMouseUp.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Mouse_Down_Dispatcher()
        {
            if (mouse_state.LeftButton == ButtonState.Pressed)
            {
                //MessageBox(new IntPtr(0), "mouseDown", "", 0);
                if (mCurrentScreen.OnMouseDown != null)
                    mCurrentScreen.OnMouseDown.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Right_Mouse_Click_Dispatcher()
        {
            if (last_mouse_state.RightButton == ButtonState.Released
                && mouse_state.RightButton == ButtonState.Pressed)
            {
                //MessageBox(new IntPtr(0), "mouseClick", "", 0);
                if (mCurrentScreen.OnRightMouseClick != null)
                    mCurrentScreen.OnRightMouseClick.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Right_Mouse_Up_Dispatcher()
        {
            if (last_mouse_state.RightButton == ButtonState.Pressed
                && mouse_state.RightButton == ButtonState.Released)
            {
                //MessageBox(new IntPtr(0), "mouseUp", "", 0);
                if (mCurrentScreen.OnRightMouseUp != null)
                    mCurrentScreen.OnRightMouseUp.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Right_Mouse_Down_Dispatcher()
        {
            if (mouse_state.RightButton == ButtonState.Pressed)
            {
                //MessageBox(new IntPtr(0), "mouseDown", "", 0);
                if (mCurrentScreen.OnRightMouseDown != null)
                    mCurrentScreen.OnRightMouseDown.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Middle_Mouse_Click_Dispatcher()
        {
            if (last_mouse_state.MiddleButton == ButtonState.Released
                && mouse_state.MiddleButton == ButtonState.Pressed)
            {
                //MessageBox(new IntPtr(0), "mouseClick", "", 0);
                if (mCurrentScreen.OnMiddleMouseClick != null)
                    mCurrentScreen.OnMiddleMouseClick.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Middle_Mouse_Up_Dispatcher()
        {
            if (last_mouse_state.MiddleButton == ButtonState.Pressed
                && mouse_state.MiddleButton == ButtonState.Released)
            {
                //MessageBox(new IntPtr(0), "mouseUp", "", 0);
                if (mCurrentScreen.OnMiddleMouseUp != null)
                    mCurrentScreen.OnMiddleMouseUp.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Middle_Mouse_Down_Dispatcher()
        {
            if (mouse_state.MiddleButton == ButtonState.Pressed)
            {
                //MessageBox(new IntPtr(0), "mouseDown", "", 0);
                if (mCurrentScreen.OnMiddleMouseDown != null)
                    mCurrentScreen.OnMiddleMouseDown.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Mouse_Scroll_Dispatcher()
        {
            int change = mouse_state.ScrollWheelValue - last_mouse_state.ScrollWheelValue;
            if (change != 0)
            {
                if (mCurrentScreen.OnMouseScroll != null)
                    mCurrentScreen.OnMouseScroll.Invoke(change);
            }
        }
        private void On_Mouse_Move_Dispatcher()
        {
            if (mouse_state.X != last_mouse_state.X || mouse_state.Y != last_mouse_state.Y)
            {
                if (mCurrentScreen.OnMouseMove != null)
                    mCurrentScreen.OnMouseMove.Invoke(mouse_state, last_mouse_state);

                var windows_rec = new Rectangle(0, 0, window_width, window_height);
                if (!windows_rec.Contains(new Point(last_mouse_state.X, last_mouse_state.Y))
                    && windows_rec.Contains(new Point(mouse_state.X, mouse_state.Y)))
                {
                    //MessageBox(new IntPtr(0), "mouseEnter", "", 0);
                    if (mCurrentScreen.OnMouseEnter != null)
                        mCurrentScreen.OnMouseEnter.Invoke(mouse_state, last_mouse_state);
                }
                else if (!windows_rec.Contains(new Point(mouse_state.X, mouse_state.Y))
                    && windows_rec.Contains(new Point(last_mouse_state.X, last_mouse_state.Y)))
                {
                    //MessageBox(new IntPtr(0), "mouseLeave", "", 0);
                    if (mCurrentScreen.OnMouseLeave != null)
                        mCurrentScreen.OnMouseLeave.Invoke(mouse_state, last_mouse_state);
                }
            }
        }
        private void On_Mouse_Hover_Dispatvher()
        {
            var windows_rec = new Rectangle(0, 0, window_width, window_height);
            if (windows_rec.Contains(new Point(mouse_state.X, mouse_state.Y)))
            {
                //MessageBox(new IntPtr(0), "mouseOver", "", 0);
                if (mCurrentScreen.OnMouseHover != null)
                    mCurrentScreen.OnMouseHover.Invoke(mouse_state, last_mouse_state);
            }
        }
        private void On_Keys_Pressed_Dispatcher()
        {
            Keys[] temp = keyboard_state.GetPressedKeys();
            if (temp.Length > 0)
                //MessageBox(new IntPtr(0), "keyPress", "", 0);
                if (mCurrentScreen.OnKeysPress != null)
                    mCurrentScreen.OnKeysPress.Invoke(temp);
        }
        private void On_Keys_Down_Dispatcher()
        {
            var t = keyboard_state.GetPressedKeys().Except(last_keyboard_state.GetPressedKeys());
            if (t.Count() > 0)
                //MessageBox(new IntPtr(0), "keyDown", "", 0);
                if (mCurrentScreen.OnKeysDown != null)
                    mCurrentScreen.OnKeysDown.Invoke(t.ToArray());
        }
        private void On_Keys_Up_Dispatcher()
        {
            var t = last_keyboard_state.GetPressedKeys().Except(keyboard_state.GetPressedKeys());
            if (t.Count() > 0)
                //MessageBox(new IntPtr(0), "keyUp", "", 0);
                if (mCurrentScreen.OnKeysUp != null)
                    mCurrentScreen.OnKeysUp.Invoke(t.ToArray());
        }
        #endregion

        #region input's functions
        public void update_input_state()
        {
            last_mouse_state = mouse_state;
            mouse_state = Mouse.GetState();
            mouse_pos = new Point(mouse_state.X, mouse_state.Y);
            last_keyboard_state = keyboard_state;
            keyboard_state = Keyboard.GetState();
        }
        public bool left_mouse_click(Rectangle rec)///check ONE left mouse click
        {
            if (last_mouse_state.LeftButton == ButtonState.Released && mouse_state.LeftButton == ButtonState.Pressed)
            {
                if (rec.Contains(mouse_pos))
                {
                    return true;
                }
            }
            return false;
        }
        public bool left_mouse_click(RectangleF rec)///check ONE left mouse click
        {
            if (last_mouse_state.LeftButton == ButtonState.Released && mouse_state.LeftButton == ButtonState.Pressed)
            {
                if (rec.Contains(mouse_pos))
                {
                    return true;
                }
            }
            return false;
        }
        public bool left_mouse_release(Rectangle rec)///check ONE left mouse release
        {
            if (last_mouse_state.LeftButton == ButtonState.Pressed && mouse_state.LeftButton == ButtonState.Released)
            {
                if (rec.Contains(mouse_pos))
                {
                    return true;
                }
            }
            return false;
        }
        public bool left_mouse_release(RectangleF rec)///check ONE left mouse release
        {
            if (last_mouse_state.LeftButton == ButtonState.Pressed && mouse_state.LeftButton == ButtonState.Released)
            {
                if (rec.Contains(mouse_pos))
                {
                    return true;
                }
            }
            return false;
        }
        public bool mouse_hover(Rectangle rec)///check mouse hover
        {
            if (rec.Contains(mouse_pos))
            {
                return true;
            }
            else return false;
        }
        public bool mouse_hover(RectangleF rec)///check mouse hover
        {
            if (rec.Contains(mouse_pos))
            {
                return true;
            }
            else return false;
        }
        public bool key_press(Keys key)///check ONE key press
        {
            if (last_keyboard_state.IsKeyUp(key) && keyboard_state.IsKeyDown(key))
            {
                return true;
            }
            else return false;
        }
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            graphics.PreferredBackBufferHeight = window_height;
            graphics.PreferredBackBufferWidth = window_width;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;

            double d = 1d / 120;
            long l = (long)(10000000L * d);
            this.TargetElapsedTime = new TimeSpan(l);

            rz.ScissorTestEnable = true;

            keyboard_text_dispatcher = new KeyboardTextDispatcher(this.Window);

            //register for event raiser
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Click_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Mouse_Up_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Mouse_Down_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Right_Mouse_Click_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Right_Mouse_Up_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Right_Mouse_Down_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Middle_Mouse_Click_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Middle_Mouse_Up_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Middle_Mouse_Down_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Mouse_Scroll_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Mouse_Move_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Mouse_Hover_Dispatvher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Keys_Pressed_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Keys_Down_Dispatcher);
            Event_Raiser_Delegate += new EventRaiserDelegate(On_Keys_Up_Dispatcher);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("SpriteFont1");
            arial12Bold = Content.Load<SpriteFont>("Resource/font/Arial_12_Bold");
            arial13Bold = Content.Load<SpriteFont>("Resource/font/Arial_13_Bold");
            arial14Bold = Content.Load<SpriteFont>("Resource/font/Arial_14_Bold");
            gautami12Regular = Content.Load<SpriteFont>("Resource/font/Gautami_12_Regular");
            gautami14Bold = Content.Load<SpriteFont>("Resource/font/Gautami_14_Bold");
            whiteTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            whiteTexture.SetData(new[] { Color.White });
            zeroTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            zeroTexture.SetData(new[] { Color.Transparent });
            whiteTextbox = Content.Load<Texture2D>("Resource/white_textbox");
            textboxBackground = new Texture2D(graphics.GraphicsDevice, 1, 1);
            textboxBackground.SetData(new[] { new Color(0,0,0,150) });
            transparentTextBox = Content.Load<Texture2D>("Resource/textbox");
            highlightedTextbox = Content.Load<Texture2D>("Resource/Highlighted_textbox");
            caret = Content.Load<Texture2D>("Resource/caret");
            scrollbarBackground = Content.Load<Texture2D>("Resource/graphic/ScrollBarRailHorz");
            scrollbarBackground_vert = Content.Load<Texture2D>("Resource/graphic/ScrollBarRailVert");
            scrollbar = Content.Load<Texture2D>("Resource/graphic/ScrollBarButtonHorz");
            scrollbar_horz = Content.Load<Texture2D>("Resource/graphic/ScrollBarButtonHorz");
            scrollbar_vert = Content.Load<Texture2D>("Resource/graphic/ScrollBarButtonVert");
            scrollbarHover_Horz = Content.Load<Texture2D>("Resource/graphic/ScrollBarHoverHorz");
            scrollbarHover_Vert = Content.Load<Texture2D>("Resource/graphic/ScrollBarHoverVert");
            panelTexture = Content.Load<Texture2D>("Resource/graphic/Panel");

            Background_Music = Content.Load<Song>("Resource/Music/Memoria");
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Background_Music);

            mMenuScreen = new MenuScreen(graphics, this.Content, new SivEventHandler(MenuScreenEvent), this);
            mInGameScreen = new InGameScreen(graphics, this.Content, new SivEventHandler(InGameEvent), this);
            mHostScreen = new HostScreen(graphics, this.Content, new SivEventHandler(HostScreenEvent), this);
            mRoomScreen = new RoomScreen(graphics, this.Content, new SivEventHandler(RoomScreenEvent), this);
            mJoinScreen = new JoinScreen(graphics, this.Content, new SivEventHandler(JoinScreenEvent), this);
            mCharacterSelectScreen = new CharacterSelectScreen(graphics, this.Content, new SivEventHandler(CharacterSelectScreenEvent), this);

            mCurrentScreen = mMenuScreen;

            //update_input_state();      
        }

        #region Screens's Event
        //code = 0
        public void MenuScreenEvent(object obj, SivEventArgs e)
        {
            //Menu to Ingame
            if (e.Command_code == 1)
            {
                mCurrentScreen = mCharacterSelectScreen;
                mCharacterSelectScreen.Start(new Command());
                /*mInGameScreen.Start();
                mCurrentScreen = mInGameScreen;*/
            }
            //Menu to Host Screen
            else if (e.Command_code == 2)
            {
                mCurrentScreen = mHostScreen;
            }
            //Menu to Join Screen
            else if (e.Command_code == 3)
            {
                mJoinScreen.InitializeReceiver();
                mCurrentScreen = mJoinScreen;
            }
        }
        //code = 1
        public void InGameEvent(object obj, SivEventArgs e)
        {
            //Ingame to Menu
            if (e.Command_code == 0)
            {
                mInGameScreen.End(new Command());
                mCurrentScreen = mMenuScreen;
            }
        }
        //code = 2
        public void HostScreenEvent(object obj, SivEventArgs e)
        {
            //HostScreen to Menu
            if (e.Command_code == 0)
            {
                mCurrentScreen = mMenuScreen;
            }
            //HostScreen to RoomScreen
            else if (e.Command_code == 4)
            {
                mRoomScreen.room = (Room)e.Data;
                mRoomScreen.Player_ID = ((Room)e.Data).Player_List.First().id;

                mRoomScreen.Start(new Command(CommandCode.Standby,0));
                mCurrentScreen = mRoomScreen;
            }
        }
        //code = 4
        public void RoomScreenEvent(object obj, SivEventArgs e)
        {
            //RoomScreen to Menu
            if (e.Command_code == 0)
            {
                mRoomScreen.End(new Command());
                mCurrentScreen = mMenuScreen;
            }
            //RoomScreen to InGameScreen
            else if (e.Command_code == 1)
            {
                //mInGameScreen.room = mRoomScreen.room;
                //mInGameScreen.Player_ID = mRoomScreen.Player_ID;
                //mRoomScreen.End(new Command());
                //mInGameScreen.Start(new Command());
                //mCurrentScreen = mInGameScreen;
                mCharacterSelectScreen.room = mRoomScreen.room;
                mCharacterSelectScreen.Player_ID = mRoomScreen.Player_ID;
                mRoomScreen.End(new Command());
                mCharacterSelectScreen.Start(new Command());
                mCurrentScreen = mCharacterSelectScreen;
            }
            
        }
        //code = 3
        public void JoinScreenEvent(object obj, SivEventArgs e)
        {
            //JoinScreen to Menu
            if (e.Command_code == 0)
            {
                mJoinScreen.End_Receive();
                mCurrentScreen = mMenuScreen;
            }
            //JoinScreen to HostScreen
            else if (e.Command_code == 2)
            {
                mJoinScreen.End_Receive();
                mCurrentScreen = mHostScreen;
            }
            //JoinScreen to RoomScreen
            else if (e.Command_code == 4)
            {
                mJoinScreen.End_Receive();

                Room _room = (Room)e.Data;                
                mRoomScreen.room = _room;
                mRoomScreen.Player_ID = _room.Player_List.Last().id;

                mRoomScreen.Start(new Command(CommandCode.Standby, 1));
                mCurrentScreen = mRoomScreen;
            }
        }
        //code = 5
        public void CharacterSelectScreenEvent(object obj, SivEventArgs e)
        {
            if (e.Command_code == 0)
            {
                mCharacterSelectScreen.End(new Command());
                mCurrentScreen = mMenuScreen;
            }
            else if (e.Command_code == 1)
            {
                mInGameScreen.room = mCharacterSelectScreen.room;
                mInGameScreen.Player_ID = mCharacterSelectScreen.Player_ID;
                mCharacterSelectScreen.End(new Command());
                mInGameScreen.Start(new Command());
                mCurrentScreen = mInGameScreen;
            }

        }
        #endregion

        public static string getLocalIP()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //white_texture.Dispose();
        }

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                //for fps counter
                elapsedTime += gameTime.ElapsedGameTime;
                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
                //end for fps counter

                //Music Player
                //end Music Player

                update_input_state();
                Event_Raiser_Delegate.Invoke();
                /*if (Screen.keyboard_state.GetPressedKeys().Length > 0)
                {
                    MessageBox(new IntPtr(0), keyboard_state.GetPressedKeys()[0].ToString(), "", 0);
                }*/
                //check keys enum
                if (key_press(Keys.OemTilde))
                {
                    debugger.Show();
                }
                mCurrentScreen.Update(gameTime);
                
                base.Update(gameTime);
            }
        }

        int ingametime_update_1stimer = 1000;
        string ingametime_string;
        protected override void Draw(GameTime gameTime)
        {           
            if (this.IsActive)
            {
                GraphicsDevice.Clear(Color.Black);

                frameCounter++;

                mCurrentScreen.Draw(graphics, spriteBatch, gameTime);

                base.Draw(gameTime);

                ingametime_update_1stimer += gameTime.ElapsedGameTime.Milliseconds;

                spriteBatch.Begin();

                if (ingametime_update_1stimer >= 1000)
                {
                    ingametime_string = "In Game Time: " + gameTime.TotalGameTime.ToString();
                    ingametime_update_1stimer -= 1000;
                }
                spriteBatch.DrawString(arial12Bold, ingametime_string, new Vector2(5, 0), Color.White);
                string fps_string = "FPS: " + frameRate.ToString();
                spriteBatch.DrawString(arial12Bold, fps_string, new Vector2(5, 17), Color.White);
                string mouseposition_string = "Mouse Position: " + mouse_pos.X + ":" + mouse_pos.Y;
                spriteBatch.DrawString(arial12Bold, mouseposition_string, new Vector2(5, 34), Color.White);
                
                spriteBatch.End();
            }
        }
    }
}