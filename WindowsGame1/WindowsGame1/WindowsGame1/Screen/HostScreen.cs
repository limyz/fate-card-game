using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Net;
using System.Net.Sockets;


namespace WindowsGame1
{
    public class HostScreen : Screen
    {
        #region Variable Decleration
        SpriteFont font;
        ImageButton OK_button, Canel_button;
        Label Host_name_label, Number_of_player_label, Room_name_label;
        TextBox Host_name_textbox, Room_name_textbox, Number_of_player_textbox;
        Background bg;
        Image saber, dialog;
        TextBox ipTextBox;
        Menu test_menu;
        #endregion

        #region Load Content
        public HostScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("HostScreen",theEvent, parent)
        {
            #region Load Resource
            font = Content.Load<SpriteFont>("Resource/font/TNRoman_12_Bold");
            bg = new Background(Content.Load<Texture2D>("Resource/background"), this);
            #endregion

            #region Image
            saber = new Image("saber2", Content.Load<Texture2D>("Resource/saber_trans")
            , new Rectangle(-50, 0, 700, 735), 0.99f, this);
            dialog = new Image("dialog", Content.Load<Texture2D>("Resource/dialog_menu")
            , new Rectangle(400, 120, 400, 300), 0.98f, this);
            #endregion

            #region Host Game Panel
            ipTextBox = new TextBox("Hosting IP", Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , font, new Rectangle(470, 160, 250, 20), this);
            ipTextBox.Text = "255.255.255.255";

            Host_name_label = new Label("host_name_label",
                font, "Player name"
                , 470, 200, 150, Color.White, this);

            Host_name_textbox = new TextBox("Host_name_textbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , font, new Rectangle(620, 200, 100, 20), this);
            Host_name_textbox.Text = "AltimaZ";

            Room_name_label = new Label("Room_name_label"
                , font, "Room name"
                , 470, 230, 150, Color.White, this);

            Room_name_textbox = new TextBox("Room_name_textbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , font, new Rectangle(620, 230, 100, 20), this);
            Room_name_textbox.Text = "Room Test";

            Number_of_player_label = new Label("Number_of_player_label"
                , font, "Number of Player"
                , 470, 260, 150, Color.White, this);

            Number_of_player_textbox = new TextBox("Number_of_player_textbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , font, new Rectangle(620, 260, 100, 20), this);
            Number_of_player_textbox.Text = "3";

            OK_button = new ImageButton("OK_button"
                , Content.Load<Texture2D>("Resource/ok_button")
                , new Rectangle(480, 300, 120, 42), this);
            OK_button.OnClick += Ok_button_clicked;

            Canel_button = new ImageButton("Cancel_button"
                , Content.Load<Texture2D>("Resource/back_button")
                , new Rectangle(600, 300, 120, 42), this);
            Canel_button.OnClick += Cancel_button_clicked;
            #endregion

            List<string> ls = new List<string>();
            ls.Add("Sakuraba Neku");
            ls.Add("Yayyyyy");
            ls.Add("I'm so powerful");
            ls.Add("Sakuraba Neku");
            ls.Add("Yayyyyy");
            ls.Add("I'm so powerful");
            ls.Add("Sakuraba Neku");
            ls.Add("Yayyyyy");
            ls.Add("I'm so powerful");
            ls.Add("Sakuraba Neku");
            ls.Add("Yayyyyy");
            ls.Add("I'm so powerful");
            test_menu = new Menu("test_menu", font, ls, this);
            test_menu.MenuItemSelected += Test_Menu_Item_Selected;

            #region HostScreen_RegisterHandler
            OnKeysDown += HostScreen_OnKeysDown;
            OnRightMouseClick += HostScreen_OnRightMouseClick;
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
        private void HostScreen_OnRightMouseClick(MouseState mouseState, MouseState lastMouseState)
        {
            test_menu.Show(mouseState.X, mouseState.Y);
        }
        public void Ok_button_clicked(object sender, FormEventData e)
        {
            if (Host_name_textbox.Text.Length > 12)
            {
                Game1.MessageBox(new IntPtr(0), "Player Name cannot more than 12 characters", "Player Name Invalid", 0);
                return;
            }
            if (Room_name_textbox.Text.Length > 12)
            {
                Game1.MessageBox(new IntPtr(0), "Room Name cannot more than 12 characters", "Room Name Invalid", 0);
                return;
            }
            try
            {
                main_game.mRoomScreen.IPAddress = ipTextBox.Text;
                Player player = new Player(Host_name_textbox.Text, Game1.getLocalIP());
                player.Status = true;
                Room r = new Room(player, Room_name_textbox.Text, 
                    int.Parse(Number_of_player_textbox.Text));
                ScreenEvent.Invoke(this, new SivEventArgs(4, r));
            }
            catch(Exception ex)
            {
                main_game.mRoomScreen.End(new Command());
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }
        public void Cancel_button_clicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
        }
        public void Test_Menu_Item_Selected(object sender, int index)
        {
            Menu m = (Menu)sender;
            Game1.MessageBox(new IntPtr(0), m.ItemsList[index], "Menu Item Selected", 0);
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
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
