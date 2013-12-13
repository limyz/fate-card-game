using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsGame1
{
    public class RoomScreen : Screen
    {
        #region variable decleration
        SpriteFont font, arial14Bold, arial12Bold;
        Border div_border, chat_box_border, chat_input_border, div_info_border;
        Border[] div_char_border = new Border[8];
        Rectangle[] div_char = new Rectangle[8];
        Label[] playerName = new Label[8];
        Label roomInfoContent, roomInfoTitle;
        Background backGround;
        ImageButton start_button, quit_button;
        List<Image> avatar_img = new List<Image>();
        Color borderColor = Color.MediumAquamarine;
        TextBox chat, chatDisplay;
        Div roomInfoDiv;
        Texture2D avatarDefault, caret, highlighted_textbox, white_textbox;
        public Room room;
        public int numberOfPlayer = 0;
        #endregion

        #region broadcast Thread
        UdpClient sendingClient;
        Thread broadcastingThread;
        /*public static byte[] ReadFully(Stream input)
        {
            input.Position = 0;
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }*/
        private void Broadcaster()
        {
            while (true)
            {
                MemoryStream stream = new MemoryStream();
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, room);
                byte[] data = stream.ToArray();
                sendingClient.Send(data, data.Length, "255.255.255.255", 51001);

                Thread.Sleep(1000);
            }
        }
        public void Start_Broadcast()
        {
            sendingClient = new UdpClient();
            sendingClient.EnableBroadcast = true;

            ThreadStart ts = new ThreadStart(Broadcaster);
            broadcastingThread = new Thread(ts);
            broadcastingThread.IsBackground = true;
            broadcastingThread.Start();
        }
        public void End_Broadcast()
        {
            broadcastingThread.Abort();
            sendingClient.Close();
        }
        #endregion

        #region Load Content
        public RoomScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("RoomScreen", theEvent, parent)
        {
            white_textbox = Content.Load<Texture2D>("Resource/white_textbox");
            highlighted_textbox = Content.Load<Texture2D>("Resource/Highlighted_textbox");
            caret = Content.Load<Texture2D>("Resource/caret");
            avatarDefault = Content.Load<Texture2D>("Resource/avatar_default");
            font = Content.Load<SpriteFont>("SpriteFont1");
            arial12Bold = Content.Load<SpriteFont>("Resource/font/Arial_12_Bold");
            arial14Bold = Content.Load<SpriteFont>("Resource/font/Arial_14_Bold");

            div_char[0] = new Rectangle(55, 40, 180, 180);
            div_char[1] = new Rectangle(265, 40, 180, 180);
            div_char[2] = new Rectangle(475, 40, 180, 180);
            div_char[3] = new Rectangle(685, 40, 180, 180);
            div_char[4] = new Rectangle(55, 270, 180, 180);
            div_char[5] = new Rectangle(265, 270, 180, 180);
            div_char[6] = new Rectangle(475, 270, 180, 180);
            div_char[7] = new Rectangle(685, 270, 180, 180);

            div_border = new Border("player_border", borderColor, 2
               , new Rectangle(20, 20, 900, 480), this);
            chat_box_border = new Border("chat_screen", borderColor, 2
                , new Rectangle(20, 520, 950, 130), this);
            chat_input_border = new Border("chat_input", borderColor, 2
                , new Rectangle(20, 660, 950, 24), this);
            div_info_border = new Border("div_info", borderColor, 2
                , new Rectangle(930, 20, 250, 480), this);

            backGround = new Background(Content.Load<Texture2D>("Resource/background"), this);

            for (int i = 0; i < div_char.Length; i++)
            {
                String name = "div_char" + i;
                div_char_border[i] = new Border(name, borderColor, 2, div_char[i], this);
                if (i < 4)
                {
                    playerName[i] = new Label("playerNameLabel" + i, arial12Bold, ""
                    , 55 + (i * 210), 230, 180, Color.White, this);
                    playerName[i].center_align = true;
                }
                else
                {
                    playerName[i] = new Label("playerNameLabel" + i, arial12Bold, ""
                    , 55 + ((i - 4) * 210), 460, 180, Color.White, this);
                    playerName[i].center_align = true;
                }
            }

            start_button = new ImageButton("Start", Content.Load<Texture2D>("Resource/start_button")
                , new Rectangle(980, 540, 180, 70), this);
            start_button.OnClick += Start_button_clicked;

            quit_button = new ImageButton("Quit", Content.Load<Texture2D>("Resource/quit_button")
                , new Rectangle(980, 620, 180, 70), this);
            quit_button.OnClick += Quit_button_clicked;

            chat = new TextBox("Chat Input"
                , white_textbox, highlighted_textbox, caret
                , font, new Rectangle(22, 662, 946, 20), this);

            chatDisplay = new TextBox("Chat Display"
                , white_textbox, highlighted_textbox, caret
                , font, new Rectangle(22, 522, 946, 126), this);
            chatDisplay.ReadOnly = true;

            roomInfoDiv = new Div("Room Info", new Rectangle(932, 22, 246, 476), Color.DarkRed, this);
            roomInfoContent = new Label("Room Info", font, "", 960, 80, 300, Color.White, this);
            roomInfoTitle = new Label("Info Label", arial14Bold, "Room Information", 970, 50, 300, Color.White, this);


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
                {
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                }
                return;
            }
        }

        private void Start_button_clicked(object sender, FormEventData e)
        {
            String s = "Owner index: " + room.owner_index + "\n";
            s += "Player List Count: " + room.Player_List.Count + "\n";
            s += "Room name: " + room.Room_name + "\n";
            s += "Number of Player: " + room.Number_of_Player + "\n";
            s += "Player List:\n";
            foreach (Player p in room.Player_List)
            {
                s += "+ " + p.Player_name + " - " + p.Address + "\n";
            }
            Game1.MessageBox(new IntPtr(0), s, "Room info", 0);
            //avatar_img.Delete();
            //avatar_img = null;
        }

        private void Quit_button_clicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
        }

        /*bool play_animation_state = false;
        private void avatar_clicked(object sender, FormEventData e)
        {
            int test = (270 - avatar_img.rec.Y) + (avatar_img.rec.X - 255);
            avatar_img.rec.Y += test;
            play_animation_state = true;
        }*/
        #endregion

        #region Update's function
        /*private void play_animation(ref Rectangle rec)
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
        }*/

        private void playerNumberChange()
        {
            String s = "Owner index: " + room.owner_index + "\n";
            s += "Player List Count: " + room.Player_List.Count + "\n";
            s += "Room name: " + room.Room_name + "\n";
            s += "Number of Player: " + room.Number_of_Player + "\n";
            s += "Player List:\n";
            foreach (Player p in room.Player_List)
            {
                s += "+ " + p.Player_name + " - " + p.Address + "\n";
            }
            roomInfoContent.Text = s;
            foreach (var item in avatar_img) item.Delete();
            avatar_img.Clear();
            for (int i = 0; i < room.Player_List.Count; i++)
            {
                String playerNameStr = room.Player_List[i].Player_name;
                playerName[i].Text = playerNameStr;
                Image newAvatar = new Image(playerNameStr, avatarDefault, div_char[i], 0.5f, this);
                avatar_img.Add(newAvatar);
            }
            numberOfPlayer = room.Player_List.Count;
        }
        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            //if (play_animation_state) play_animation(ref avatar_img.rec);            
            if (room.Player_List.Count != numberOfPlayer) playerNumberChange();
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
