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
        Rectangle bg_rec;
        Texture2D bg_texture;
        ImageButton start_button;
        ImageButton host_button;
        ImageButton join_button;
        ImageButton quit_button;
        SpriteFont font;
        #endregion

        #region load content
        public MenuScreen(GraphicsDeviceManager graphics, ContentManager theContent, SivEventHandler theScreenEvent,Game1 parent)
            : base("MenuScreen",theScreenEvent,parent)
        {
            font = theContent.Load<SpriteFont>("SpriteFont1");

            start_button = new ImageButton("start_button", theContent.Load<Texture2D>("Resource/start"), new Rectangle(38, 653, 173, 51), 0.9f, this);
            start_button.OnClick += Menu_button_click_handler;

            host_button = new ImageButton("host_button"
                , theContent.Load<Texture2D>("Resource/hostgame")
                , new Rectangle(246, 653, 173, 51), 0.9f, this);
            host_button.OnClick += Menu_button_click_handler;

            join_button = new ImageButton("join_button"
                , theContent.Load<Texture2D>("Resource/joingame")
                , new Rectangle(783, 653, 173, 51), 0.9f, this);
            join_button.OnClick += Menu_button_click_handler;

            quit_button = new ImageButton("quit_button"
                ,theContent.Load<Texture2D>("Resource/quit")
                , new Rectangle(987, 653, 173, 51), 0.9f, this);
            quit_button.OnClick += Menu_button_click_handler;

            bg_rec = new Rectangle(0, 0, main_game.window_width, main_game.window_height);
            bg_texture = theContent.Load<Texture2D>("Resource/menu_background_2");
        }
        #endregion

        #region forms's handler
        private void Menu_button_click_handler(object sender, FormEventData e = null)
        {
            if (sender as ImageButton == start_button)
            {
                ScreenEvent.Invoke(this, new SivEventArgs(1));
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
            spriteBatch.DrawString(font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.Draw(bg_texture, bg_rec,null,Color.White,0f,new Vector2(0,0),SpriteEffects.None,1f);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }    
}
