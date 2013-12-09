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
        SpriteFont font, arialFontBold;
        Border div_border, chat_box_border, div_info_border;
        Border[] div_char_border = new Border[8];
        Rectangle[] div_char = new Rectangle[8];
        Label[] playerName = new Label[8];
        Label roomInfo, infoLabel;
        Background backGround;
        ImageButton start_button, quit_button;
        Image avatar_img;
        Color borderColor = Color.MediumAquamarine;
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

                Thread.Sleep(500);
            }
        }
        public void start_broadcast()
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
            : base("HostScreen", theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");
            arialFontBold = Content.Load<SpriteFont>("Resource/font/Arial_14_Bold");            

            div_char[0] = new Rectangle(55, 40, 180, 180);
            div_char[1] = new Rectangle(265, 40, 180, 180);
            div_char[2] = new Rectangle(475, 40, 180, 180);
            div_char[3] = new Rectangle(685, 40, 180, 180);
            div_char[4] = new Rectangle(55, 270, 180, 180);
            div_char[5] = new Rectangle(265, 270, 180, 180);
            div_char[6] = new Rectangle(475, 270, 180, 180);
            div_char[7] = new Rectangle(685, 270, 180, 180);

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
                if (i < 4)
                {
                    playerName[i] = new Label("playerNameLabel"+i, font, ""
                    , 110 + (i * 210), 230, 150, Color.White, this);
                }
                else
                {
                    playerName[i] = new Label("playerNameLabel" + i, font, ""
                    , 110 + ((i - 4) * 210), 460, 150, Color.White, this);
                }
                //String playerNameStr = "Player " + (i + 1);
                //if (i < 4)
                //{
                //    playerName[i] = new Label(playerNameStr, font, playerNameStr
                //    , 110 + (i * 210), 230, 150, Color.White, this);
                //}
                //else
                //{
                //    playerName[i] = new Label(playerNameStr, font, playerNameStr
                //    , 110 + ((i - 4) * 210), 460, 150, Color.White, this);
                //}
            }

            avatar_img = new Image("Player 1", Content.Load<Texture2D>("Resource/avatar_default")
                , div_char[0], 0.5f, this);
            //avatar_img.OnClick += avatar_clicked;

            infoLabel = new Label("Info Label", arialFontBold, "Room Information", 970, 50, 300, Color.White, this);
            roomInfo = new Label("Room Info", font, "", 970, 80, 300, Color.White, this);

            start_button = new ImageButton("Start", Content.Load<Texture2D>("Resource/start_button")
                , new Rectangle(980, 540, 180, 70), this);
            start_button.OnClick += Start_button_clicked;

            quit_button = new ImageButton("Quit", Content.Load<Texture2D>("Resource/quit_button")
                , new Rectangle(980, 620, 180, 70), this);
            quit_button.OnClick += Quit_button_clicked;

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
            avatar_img.Delete();
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

        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            //if (play_animation_state) play_animation(ref avatar_img.rec);            
            if (room.Player_List.Count != numberOfPlayer)
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
                roomInfo.text = s;

                for (int i = 0; i < room.Player_List.Count; i++)
                {
                    String playerNameStr = room.Player_List[i].Player_name;
                    playerName[i].text = playerNameStr;
                }  
              
                numberOfPlayer = room.Player_List.Count;
            }
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
