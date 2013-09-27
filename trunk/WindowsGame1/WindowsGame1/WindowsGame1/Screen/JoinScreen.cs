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
        Border test_border;
        Rectangle test_rec = new Rectangle(150, 100, 200, 200);
        ImageButton test_img;
        #endregion

        #region load content
        public JoinScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("JoinScreen",theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");

            test_img = new ImageButton("blahblah", Content.Load<Texture2D>("Resource/Test"), test_rec, this);                ;
            test_border = new Border("test_border", Color.Blue, 3, test_rec, this);

            #region JoinScreen_RegisterHandler
            OnKeysDown += JoinScreen_OnKeysDown;
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
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
