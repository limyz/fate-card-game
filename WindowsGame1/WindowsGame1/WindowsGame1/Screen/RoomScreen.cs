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
        Border div_border, chat_box_border, div_info_border;
        Border[] div_char_border = new Border[8];
        Rectangle[] div_char = new Rectangle[8];
        Background backGround;
        ImageButton start, quit;
        Image avatar_img;
        bool play_animation_state = false;
        Color borderColor = Color.MediumAquamarine;
        #endregion

        #region Load Content
        public RoomScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("HostScreen", theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");

            div_char[0] = new Rectangle(45, 40, 200, 200);
            div_char[1] = new Rectangle(255, 40, 200, 200);
            div_char[2] = new Rectangle(465, 40, 200, 200);
            div_char[3] = new Rectangle(675, 40, 200, 200);
            div_char[4] = new Rectangle(45, 270, 200, 200);
            div_char[5] = new Rectangle(255, 270, 200, 200);
            div_char[6] = new Rectangle(465, 270, 200, 200);
            div_char[7] = new Rectangle(675, 270, 200, 200);

            div_border = new Border("player_border", borderColor, 2
                , new Rectangle(20, 20, 900, 500), this);
            chat_box_border = new Border("chat_box", borderColor, 2
                , new Rectangle(20, 550, 950, 150), this);
            div_info_border = new Border("div_info", borderColor, 2
                , new Rectangle(950, 20, 200, 500), this);

            backGround = new Background(Content.Load<Texture2D>("Resource/background"), this);

            for (int i = 0; i < div_char.Length; i++)
            {
                String name = "div_char" + i;
                div_char_border[i] = new Border(name, borderColor, 2, div_char[i], this);
            }

            avatar_img = new Image("Player", Content.Load<Texture2D>("Resource/avatar_default")
                , div_char[0], 0.5f, this);
            avatar_img.OnClick += avatar_clicked;

            start = new ImageButton("Start" , Content.Load<Texture2D>("Resource/start_button")
                , new Rectangle(980,540, 180, 70), this);

            quit = new ImageButton("Quit", Content.Load<Texture2D>("Resource/quit_button")
                , new Rectangle(980, 620, 180, 70), this);
            quit.OnClick += quit_button_clicked;

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
        private void quit_button_clicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
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
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
