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
    class JoinScreen : Screen
    {
        #region variable decleration
        SpriteFont font;
        Background bg;
        Texture2D bg_img, background_saber, gameList;
        Rectangle saberRec, gameListRec;
        ImageButton back_button, ok_button, host_button;
        #endregion

        #region load content
        public JoinScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("JoinScreen",theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");

            bg_img = Content.Load<Texture2D>("Resource/background");
            background_saber = Content.Load<Texture2D>("Resource/SaberLily_Trans");
            gameList = Content.Load<Texture2D>("Resource/gamelist");

            gameListRec = new Rectangle(50, 50, 700, 650);
            saberRec = new Rectangle(720, 110, 480, 615);
            bg = new Background(this, bg_img);

            ok_button = new ImageButton("Ok", Content.Load<Texture2D>("Resource/ok_button"), new Rectangle(170, 650, 120, 42), this);
            host_button = new ImageButton("Host", Content.Load<Texture2D>("Resource/host_button"), new Rectangle(320, 650, 120, 42), this);
            back_button = new ImageButton("Back", Content.Load<Texture2D>("Resource/back_button"), new Rectangle(470, 650, 120, 42), this);


            #region JoinScreen_RegisterHandler
            OnKeysDown += JoinScreen_OnKeysDown;
            back_button.OnClick += BackAction;
            host_button.OnClick += HostAction;
            #endregion
        }
        #endregion

        #region HANDLER
        private void JoinScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }
        private void BackAction(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
            return;
        }
        private void HostAction(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(2));
            return;
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
            spriteBatch.Begin();
            spriteBatch.DrawString(font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
            bg.DrawBG(spriteBatch);
            spriteBatch.Draw(background_saber, saberRec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1f);
            spriteBatch.Draw(gameList, gameListRec, Color.White);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
