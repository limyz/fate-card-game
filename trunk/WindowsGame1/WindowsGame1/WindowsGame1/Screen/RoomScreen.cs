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
        Texture2D avatarDefault;
        public Room room;
        public Player mainPlayer;
        public int numberOfPlayer = 0;
        #endregion

        #region broadcast Thread
        UdpClient sendingClient;
        Thread broadcastingThread;
        public string IPAddress = "";

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
                sendingClient.Send(data, data.Length, IPAddress, 51001);

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
            if (sendingClient != null)
            {
                broadcastingThread.Abort();
                sendingClient.Close();
            }
        }
        #endregion

        #region Synchonize Thread
        Thread joinResponseThread;
        NetworkStream networkStream;
        TcpClient tcpClient;
        bool synchronizeRun = true;

        private bool isHost()
        {
            if (room.Player_List[room.owner_index].Address == mainPlayer.Address && room.Player_List[room.owner_index].Player_name == mainPlayer.Player_name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ServerRespond()
        {
            try
            {
                while (synchronizeRun)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryFormatter bformatter = new BinaryFormatter();
                    bformatter.Serialize(stream, room);
                    byte[] data = stream.ToArray();
                    foreach (Player p in room.Player_List)
                    {
                        if (p.Player_name != mainPlayer.Player_name)
                        {
                            tcpClient = new TcpClient(p.Address, 51003);
                            networkStream = tcpClient.GetStream();
                            networkStream.Write(data, 0, data.Length);
                            tcpClient.Close();
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        private void ClientRespond()
        {
            try
            {
                while (synchronizeRun)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryFormatter bformatter = new BinaryFormatter();
                    bformatter.Serialize(stream, mainPlayer);
                    byte[] data = stream.ToArray();
                    tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                    networkStream = tcpClient.GetStream();
                    networkStream.Write(data, 0, data.Length);
                    tcpClient.Close();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        public void StartSynch()
        {
            if (!isHost())
            {
                Console.WriteLine("Run Client Respond");
                joinResponseThread = new Thread(() => ClientRespond());
            }
            else
            {
                Console.WriteLine("Run Server Respond");
                joinResponseThread = new Thread(() => ServerRespond());
            }
            joinResponseThread.IsBackground = true;
            joinResponseThread.Start();
        }

        public void EndSynch()
        {
            synchronizeRun = false;
            //if (tcpClient != null)
            //{
                //joinResponseThread.Abort();
                //tcpClient.Close();
            //}
        }
        #endregion

        #region receiver Thread
        TcpListener receiveTcp = null;
        Thread receivingThread;
        bool receiverRun = true;

        public void InitializeReceiver()
        {
            if (isHost())
            {
                Console.WriteLine("Run Server Receiver");
                IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Any, 51002);
                receiveTcp = new TcpListener(endPoint);
                receiveTcp.Start();
                ThreadStart start = new ThreadStart(ServerReceiver);
                receivingThread = new Thread(start);
                receivingThread.IsBackground = true;
                receivingThread.Start();
            }
            else
            {
                Console.WriteLine("Run Client Receiver");
                IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Any, 51003);
                receiveTcp = new TcpListener(endPoint);
                receiveTcp.Start();
                ThreadStart start = new ThreadStart(ClientReceiver);
                receivingThread = new Thread(start);
                receivingThread.IsBackground = true;
                receivingThread.Start();
            }

        }

        private void ServerReceiver()
        {
            try
            {
                Byte[] bytes = new Byte[1024];
                Player _player = null;
                while (receiverRun)
                {
                    TcpClient client = receiveTcp.AcceptTcpClient();
                    _player = null;
                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(bytes);
                        mem.Position = 0;
                        _player = (Player)bin.Deserialize(mem);
                        _player.Address = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                        bool found = false;
                        foreach (Player p in room.Player_List)
                        {
                            if (p.Address == _player.Address && p.Player_name == _player.Player_name)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            this.room.Player_List.Add(_player);
                            chatDisplay.Text += _player.Player_name + " had joined the room!" + "\n";
                        }
                    }
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        private void ClientReceiver()
        {
            try
            {
                //IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Any, port);
                Byte[] bytes = new Byte[1024];
                while (receiverRun)
                {
                    TcpClient client = receiveTcp.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(bytes);
                        mem.Position = 0;
                        room = (Room)bin.Deserialize(mem);
                        playerNumberChange();
                    }
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        public void End_Receive()
        {
            receiverRun = false;
            if (receiveTcp != null)
            {
                receiveTcp.Stop();
                //receivingThread.Abort();
            }
        }

        #endregion

        #region Load Content
        public RoomScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("RoomScreen", theEvent, parent)
        {
            #region Load Resource
            avatarDefault = Content.Load<Texture2D>("Resource/avatar_default");
            backGround = new Background(Content.Load<Texture2D>("Resource/background"), this);
            #endregion

            #region Player Content
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

            #endregion

            #region Room Information
            div_info_border = new Border("div_info", borderColor, 2
                , new Rectangle(930, 20, 250, 480), this);
            roomInfoDiv = new Div("Room Info", new Rectangle(932, 22, 246, 476), Color.DarkRed, this);
            roomInfoContent = new Label("Room Info", Game1.font, "", 960, 80, 300, Color.White, this);
            roomInfoTitle = new Label("Info Label", Game1.arial14Bold, "Room Information", 970, 50, 300, Color.White, this);
            #endregion

            #region Player Name Label
            for (int i = 0; i < div_char.Length; i++)
            {
                //String name = "div_char" + i;
                div_char_border[i] = new Border(name, borderColor, 2, div_char[i], this);
                if (i < 4)
                {
                    playerName[i] = new Label("playerNameLabel" + i, Game1.arial12Bold, ""
                    , 55 + (i * 210), 230, 180, Color.White, this);
                    playerName[i].CenterAlign = true;
                }
                else
                {
                    playerName[i] = new Label("playerNameLabel" + i, Game1.arial12Bold, ""
                    , 55 + ((i - 4) * 210), 460, 180, Color.White, this);
                    playerName[i].CenterAlign = true;
                }
            }
            #endregion

            #region Button
            start_button = new ImageButton("Start", Content.Load<Texture2D>("Resource/start_button")
                , new Rectangle(980, 540, 180, 70), this);
            start_button.OnClick += Start_button_clicked;

            quit_button = new ImageButton("Quit", Content.Load<Texture2D>("Resource/quit_button")
                , new Rectangle(980, 620, 180, 70), this);
            quit_button.OnClick += Quit_button_clicked;
            #endregion

            #region Chat
            //Border
            chat_box_border = new Border("chat_screen", borderColor, 2
                , new Rectangle(20, 520, 950, 130), this);
            chat_input_border = new Border("chat_input", borderColor, 2
                , new Rectangle(20, 660, 950, 24), this);

            //Textbox
            chat = new TextBox("Chat Input"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.font, new Rectangle(22, 662, 946, 20), this);

            chatDisplay = new TextBox("Chat Display"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.font, new Rectangle(22, 522, 946, 126), this);
            chatDisplay.ReadOnly = true;
            #endregion

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
            ScreenEvent.Invoke(this, new SivEventArgs(0, mainPlayer));
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

        public void playerNumberChange()
        {
            String s = "Owner index: " + room.owner_index + "\n";
            s += "Main Player: " + mainPlayer.Player_name + "\n";
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
            foreach (var item in playerName)
            {
                item.Text = "";
                item.Visible = false;
            }
            for (int i = 0; i < room.Player_List.Count; i++)
            {
                String playerNameStr = room.Player_List[i].Player_name;
                playerName[i].Text = playerNameStr;
                playerName[i].Visible = true;
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
            if (this.room.Player_List.Count != numberOfPlayer)
            {
                playerNumberChange();
            }
            base.Update(theTime);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.DrawString(Game1.font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
