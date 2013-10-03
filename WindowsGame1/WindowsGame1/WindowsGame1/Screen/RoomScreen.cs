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
        Texture2D test_img, backgroundTexture, quit_img, start_img;
        Border div_border, chat_box_border, div_info_border;
        Border[] div_char_border = new Border[8];
        Rectangle div, chat_box, div_info, startRec, quitRec;
        Rectangle[] div_char = new Rectangle[8];
        Background backGround;
        ImageButton avatar_img, start, quit;
        Boolean play_animation_state = false;
        Color borderColor = Color.MediumAquamarine;
        #endregion

        #region Load Content
        public RoomScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("HostScreen", theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");
            div = new Rectangle(20, 20, 900, 500);

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
            startRec = new Rectangle(980,540, 180, 70);
             quitRec = new Rectangle(980,620, 180, 70);

            div_border = new Border("player_border", borderColor, 2, div, this);
            chat_box_border = new Border("chat_box", borderColor, 2, chat_box, this);
            div_info_border = new Border("div_info", borderColor, 2, div_info, this);

            backgroundTexture = Content.Load<Texture2D>("Resource/background");
            backGround = new Background(this, backgroundTexture);

            for (int i = 0; i < div_char.Length; i++)
            {
                String name = "div_char" + i;
                div_char_border[i] = new Border(name, borderColor, 2, div_char[i], this);
            }

            test_img = Content.Load<Texture2D>("Resource/avatar_default");
            start_img = Content.Load<Texture2D>("Resource/start_button");
            quit_img = Content.Load<Texture2D>("Resource/quit_button");

            avatar_img = new ImageButton("Player", test_img, div_char[0], this);
            avatar_img.OnClick += avatar_clicked;

            start = new ImageButton("Start", start_img, startRec, this);
            start = new ImageButton("Start", quit_img, quitRec, this);

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
            int test = (270 - avatar_img.rec.Y) + (avatar_img.rec.X - 255);
            avatar_img.rec.Y += test;
            play_animation_state = true;
        }
        #endregion

        #region Update's function
        private void play_animation(ref Rectangle rec)
        {
            int x = 255;
            int y = 270;
            int speed = 2;
            if (rec.X == x && rec.Y == y)
                play_animation_state = false;
            else
            {
                My_Extension.move_rec(ref rec, x, y, speed, speed);
            }
        }

        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            if (play_animation_state) play_animation(ref avatar_img.rec);
            base.Update(theTime);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
            backGround.DrawBG(spriteBatch);
            //spriteBatch.Draw(border_texture, div1, Color.White);
            /*foreach (Rectangle rec in div_char)
            {
                spriteBatch.Draw(border_texture, rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1f);
            }*/
            //avatar_img.Draw(spriteBatch, gameTime);
            //spriteBatch.Draw(border_texture, chat_box, Color.White);
            //spriteBatch.Draw(border_texture, div_info, Color.White);
            //spriteBatch.Draw(test_img, div_char[0], Color.White);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
