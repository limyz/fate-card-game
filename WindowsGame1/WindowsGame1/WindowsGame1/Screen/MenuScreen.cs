using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class MenuScreen : Screen
    {
        #region variable decleration
        RectangleF bg_rec;
        Texture2D bg_texture;
        ImageButton start_button;
        ImageButton host_button;
        ImageButton join_button;
        ImageButton quit_button;
        SpriteFont font;
        Menu ContextMenu;
        #endregion

        #region load content
        public MenuScreen(GraphicsDeviceManager graphics, ContentManager theContent, SivEventHandler theScreenEvent,Game1 parent)
            : base("MenuScreen",theScreenEvent,parent)
        {
            font = theContent.Load<SpriteFont>("SpriteFont1");

            start_button = new ImageButton("start_button", theContent.Load<Texture2D>("Resource/button/start"), new RectangleF(38, 653, 173, 51), 0.9f, this);
            start_button.OnClick += Menu_button_click_handler;

            host_button = new ImageButton("host_button"
                , theContent.Load<Texture2D>("Resource/button/hostgame")
                , new RectangleF(246, 653, 173, 51), 0.9f, this);
            host_button.OnClick += Menu_button_click_handler;

            join_button = new ImageButton("join_button"
                , theContent.Load<Texture2D>("Resource/button/joingame")
                , new RectangleF(783, 653, 173, 51), 0.9f, this);
            join_button.OnClick += Menu_button_click_handler;

            quit_button = new ImageButton("quit_button"
                , theContent.Load<Texture2D>("Resource/button/quit")
                , new RectangleF(987, 653, 173, 51), 0.9f, this);
            quit_button.OnClick += Menu_button_click_handler;

            bg_rec = new RectangleF(0, 0, main_game.window_width, main_game.window_height);
            bg_texture = theContent.Load<Texture2D>("Resource/menu_background_2");

            List<string> ls = new List<string>();
            ls.Add("30 fps");
            ls.Add("60 fps");
            ls.Add("120 fps");
            ls.Add("Unlimited fps");
            ContextMenu = new Menu("ContextMenu", font, ls, this);
            ContextMenu.MenuItemSelected += ContextMenu_Item_Selected;

            #region Screen_RegisterHandler 
            this.OnRightMouseClick += MenuScreen_OnRightMouseClick;
            #endregion
        }
        #endregion

        #region Handler
        private void MenuScreen_OnRightMouseClick(MouseState mouseState, MouseState lastMouseState)
        {
            ContextMenu.Show(mouseState.X, mouseState.Y);
        }
        private void ContextMenu_Item_Selected(object sender, int index)
        {
            if (index == 3)
            {
                this.main_game.IsFixedTimeStep = false;
            }
            else
            {
                this.main_game.IsFixedTimeStep = true;
                float fps = 0f;
                if (index == 0)
                    fps = 30f;
                else if (index == 1)
                    fps = 60f;
                else if (index == 2)
                    fps = 120f;

                double d = 1d / fps;
                long l = (long)(10000000L * d);
                this.main_game.TargetElapsedTime = new TimeSpan(l);
            }
            ContextMenu.Hide();
        }
        bool moved = false;
        private void Menu_button_click_handler(object sender, FormEventData e = null)
        {
            if (sender as ImageButton == start_button)
            {
                //ScreenEvent.Invoke(this, new SivEventArgs(1));
                if (!moved)
                {
                    this.start_button.MoveBySpeed(100, 100, 300);
                    moved = true;
                }
                else
                {
                    this.start_button.MoveBySecond(38, 653, 3);
                    moved = false;
                }
                return;
            }
            else if (sender as ImageButton == host_button)
            {
                ScreenEvent.Invoke(this, new SivEventArgs(2));
                return;
            }
            else if (sender as ImageButton == join_button)
            {
                ScreenEvent.Invoke(this, new SivEventArgs(3));
            }
            else if (sender as ImageButton == quit_button)
            {
                main_game.Exit();
            }
        }
        #endregion

        #region update
        public override void Update(GameTime theTime)
        {
            base.Update(theTime);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(bg_texture, bg_rec,null,Color.White,0f,new Vector2(0,0),SpriteEffects.None,1f);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }    
}
