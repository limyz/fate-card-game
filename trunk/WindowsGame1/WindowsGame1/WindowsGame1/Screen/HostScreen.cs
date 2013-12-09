﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace WindowsGame1
{
    public class HostScreen : Screen
    {
        #region variable decleration
        SpriteFont font;

        ImageButton OK_button;
        ImageButton Canel_button;
        Label Host_name_label;
        Label Room_name_label;
        Label Number_of_player_label;
        TextBox Host_name_textbox;
        TextBox Room_name_textbox;
        TextBox Number_of_player_textbox;
        Background bg;
        Image saber, dialog;

        #endregion

        #region load content
        public HostScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("HostScreen",theEvent, parent)
        {
            font = Content.Load<SpriteFont>("Resource/font/TNRoman_12_Bold");
            Texture2D white_textbox = Content.Load<Texture2D>("Resource/white_textbox");
            Texture2D highlighted_textbox = Content.Load<Texture2D>("Resource/Highlighted_textbox");
            Texture2D caret = Content.Load<Texture2D>("Resource/caret");

            bg = new Background(Content.Load<Texture2D>("Resource/background"), this);

            saber = new Image("saber2", Content.Load<Texture2D>("Resource/saber_trans")
            , new Rectangle(-50, 0, 700, 735), 0.99f, this);

            dialog = new Image("dialog", Content.Load<Texture2D>("Resource/dialog_menu")
            , new Rectangle(400, 120, 400, 300), 0.98f, this);

            Host_name_label = new Label("host_name_label",
                font, "Player name"
                , 470, 200, 150, Color.White, this);

            Host_name_textbox = new TextBox("Host_name_textbox"
                , white_textbox, highlighted_textbox, caret
                , font, new Rectangle(620, 200, 100, 20), this);

            Room_name_label = new Label("Room_name_label"
                , font, "Room name"
                , 470, 230, 150, Color.White, this);

            Room_name_textbox = new TextBox("Room_name_textbox"
                , white_textbox, highlighted_textbox, caret
                , font, new Rectangle(620, 230, 100, 20), this);

            Number_of_player_label = new Label("Number_of_player_label"
                , font, "Number of Player"
                , 470, 260, 150, Color.White, this);

            Number_of_player_textbox = new TextBox("Number_of_player_textbox"
                , white_textbox, highlighted_textbox, caret
                , font, new Rectangle(620, 260, 100, 20), this);

            OK_button = new ImageButton("OK_button"
                , Content.Load<Texture2D>("Resource/ok_button")
                , new Rectangle(480, 300, 120, 42), this);
            OK_button.OnClick += Ok_button_clicked;

            Canel_button = new ImageButton("Cancel_button"
                , Content.Load<Texture2D>("Resource/back_button")
                , new Rectangle(600, 300, 120, 42), this);
            Canel_button.OnClick += Cancel_button_clicked;

            #region HostScreen_RegisterHandler
            OnKeysDown += HostScreen_OnKeysDown;
            #endregion
        }
        #endregion

        #region HANDLER
        private void HostScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }
        public void Ok_button_clicked(object sender, FormEventData e)
        {
            try
            {
                Room r = new Room(
                    new Player(Host_name_textbox.Text, "0.0.0.0")
                    , Room_name_textbox.Text, int.Parse(Number_of_player_textbox.Text));
                ScreenEvent.Invoke(this, new SivEventArgs(4, r));
            }
            catch(Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }
        public void Cancel_button_clicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
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
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
