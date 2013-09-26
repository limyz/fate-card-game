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
    class RoomScreen : Screen
    {
        #region variable decleration
        SpriteFont font;
        Texture2D test_img;
        Rectangle div1, chat_box, div_info;
        Rectangle[] div_char = new Rectangle[8];
        Texture2D border_texture;
        ImageButton avatar_img;
        Boolean play_animation_state = false;
        float new_x = 0, new_y = 0;
        #endregion

        #region Load Content
        public RoomScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("HostScreen", theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");
            div1 = new Rectangle(20, 20, 900, 500);

            div_char[0] = new Rectangle(45, 40, 200, 200);
            div_char[1] = new Rectangle(255, 40, 200, 200);
            div_char[2] = new Rectangle(465, 40, 200, 200);
            div_char[3] = new Rectangle(675, 40, 200, 200);
            div_char[4] = new Rectangle(45, 270, 200, 200);
            div_char[5] = new Rectangle(255, 270, 200, 200);
            div_char[6] = new Rectangle(465, 270, 200, 200);
            div_char[7] = new Rectangle(675, 270, 200, 200);

            chat_box = new Rectangle(20, 550, 950, 150);
            div_info = new Rectangle(950, 20, 200, 500);

            border_texture = Content.Load<Texture2D>("Resource/Untitled-1");
            test_img = Content.Load<Texture2D>("Resource/avatar_default");

            avatar_img = new ImageButton("Player", test_img, div_char[0], this);
            avatar_img.OnClick += avatar_clicked;

            #region RoomScreen_RegisterHandler
            OnKeysDown += RoomScreen_OnKeysDown;
            #endregion
        }
        #endregion

        #region Handler
        private void RoomScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }
        private void avatar_clicked(object sender, FormEventData e)
        {
            play_animation_state = true;
        }
        #endregion

        #region Update's function
        private void play_animation()
        {
            float x = 255;
            float y = 270;
            float speed_x = (x - 45) / 100;
            float speed_y = (y - 40) / 100;

            System.Console.WriteLine(speed_x);
            System.Console.WriteLine(speed_y);

            Rectangle temp = avatar_img.rec.move(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(speed_x), Convert.ToInt32(speed_y));
            //Rectangle temp = avatar_img.rec.move(x, y, speed_x, speed_y);
            if (new_x == x && new_y == y)
                play_animation_state = false;
            else
            {
                avatar_img.rec = temp;
            }
        }

        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            if (play_animation_state) play_animation();
            base.Update(theTime);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.Draw(border_texture, div1, Color.White);
            foreach (Rectangle rec in div_char)
            {
                spriteBatch.Draw(border_texture, rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1f);
            }
            avatar_img.Draw(spriteBatch, gameTime);
            spriteBatch.Draw(border_texture, chat_box, Color.White);
            spriteBatch.Draw(border_texture, div_info, Color.White);
            //spriteBatch.Draw(test_img, div_char[0], Color.White);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
