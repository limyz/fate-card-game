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
        #region Variable Decleration
        public Room room;
        public Guid Player_ID;
        //public int Player_Index = 0;
        public int numberOfPlayer = 0;
        Border div_border, chat_box_border, chat_input_border, div_info_border, MainPlayer_Boder;
        Border[] div_char_border = new Border[8];
        RectangleF[] div_char = new RectangleF[8];
        Label[] playerName = new Label[8];
        Label roomInfoContent, roomInfoTitle;
        Background backGround;
        ImageButton start_button, quit_button;
        List<Image> avatar_img = new List<Image>();
        List<Image> borderAvatarList = new List<Image>();
        Color borderColor = Color.MediumAquamarine;
        TextBox chat, chatDisplay;
        Div roomInfoDiv;
        Texture2D avatarDefault, cancelButtonTexture, readyButtonTexture, startButtonTexture, panelTexture, imageBorderTexture;

        #endregion

        #region Broadcast Thread
        UdpClient sendingClient;
        Thread broadcastingThread;
        public string IPAddress = "";

        public static byte[] ReadFully(Stream input)
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

        private void Broadcaster()
        {
            while (true)
            {
                MemoryStream stream = new MemoryStream();
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, room);
                byte[] data = stream.ToArray();
                sendingClient.Send(data, data.Length, IPAddress, 51001);
                if (IPAddress != "255.255.255.255")
                {
                    sendingClient.Send(data, data.Length, "255.255.255.255", 51001);
                }

                Thread.Sleep(1000);
            }
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
        List<TcpClient> tcpServerClient = new List<TcpClient>();
        bool synchronizeRun = true;
        NetworkStream networkStream;
        TcpClient tcpClient;

        public void StartSynch()
        {
            synchronizeRun = true;
            if (room.isHost(Player_ID))
            {
                int Player_Index = room.findByID(Player_ID);
                for (int i = 0; i < room.Player_List.Count; i++)
                {
                    if (i == Player_Index)
                    {
                        //dummy client for host to balance the size of
                        // Player_List and tcpServerClient
                        tcpServerClient.Add(new TcpClient());
                        continue;
                    }
                    Player temp_player = room.Player_List[i];
                    tcpServerClient.Add(new TcpClient(temp_player.Address, 51003));
                }
                joinResponseThread = new Thread(() => ServerRespond());
                joinResponseThread.IsBackground = true;
                joinResponseThread.Start();
            }
            else
            {
                ClientJoin();
            }

        }

        private void ServerRespond()
        {
            while (synchronizeRun)
            {
                Command c = new Command(CommandCode.Standby);
                byte[] data = c.Serialize();
                int Player_Index = room.findByID(Player_ID);
                bool update_room = false;
                for (int i = 0; i < tcpServerClient.Count; i++)
                {
                    try
                    {
                        if (i == Player_Index) continue;
                        tcpServerClient[i].GetStream().Write(data, 0, data.Length);
                    }
                    //couldn't send message because the client has disconnected
                    //so we remove that tcp client and player from the list
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        string s = room.Player_List[i].Player_name + " had left the room!" + "\n";
                        this.tcpServerClient.RemoveAt(i);
                        this.room.Player_List.RemoveAt(i);
                        i--;
                        UpdateRoom();
                        update_room = true;
                        SendChatMessage(s);
                    }
                }

                if (update_room)
                {
                    Command c2 = new Command(CommandCode.Update_Room, room);
                    byte[] data2 = c2.Serialize();
                    for (int i = 0; i < tcpServerClient.Count; i++)
                    {
                        try
                        {
                            if (i == Player_Index) continue;
                            tcpServerClient[i].GetStream().Write(data2, 0, data2.Length);
                        }
                        catch
                        {
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void ClientJoin()
        {
            Command c = new Command(CommandCode.Join_Game, room.Player_List.Last());
            byte[] data = c.Serialize();

            tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
            networkStream = tcpClient.GetStream();
            networkStream.Write(data, 0, data.Length);
            tcpClient.Close();
        }

        public void EndSynch()
        {
            synchronizeRun = false;
            foreach (TcpClient tcpclient in tcpServerClient)
            {
                tcpclient.Close();
            }
            tcpServerClient.Clear();
            //if (tcpClient != null)
            //{
            //joinResponseThread.Abort();
            //tcpClient.Close();
            //}
        }
        #endregion

        #region Receiver Thread
        TcpListener receiveTcp;
        Thread receivingThread;
        bool receiverRun = true;
        bool connect_to_host = true;
        bool StoppedTcp = false;
        DateTime LastReceiveTimeFromHost = DateTime.Now;

        public void InitializeReceiver()
        {
            receiverRun = true;
            connect_to_host = true;
            StoppedTcp = false;
            LastReceiveTimeFromHost = DateTime.Now;

            if (room.isHost(Player_ID))
            {
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
            Byte[] bytes = new Byte[1024 * 16];
            Player _player = null;
            while (receiverRun)
            {
                if (StoppedTcp)
                {
                    break;
                }
                else if (receiveTcp.Pending())
                {
                    TcpClient client = receiveTcp.AcceptTcpClient();
                    _player = null;
                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Command c = new Command(bytes);
                        if (c.Command_Code == CommandCode.Standby)
                        {
                            //do nothing
                        }
                        if (c.Command_Code == CommandCode.Can_I_Join)
                        {
                            Command temp_c;
                            if (room.Player_List.Count >= room.Number_of_Player)
                            {
                                temp_c = new Command(CommandCode.Cant_Join_Room_Full);
                            }
                            else
                            {
                                temp_c = new Command(CommandCode.OK_to_Join);
                            }
                            byte[] data = temp_c.Serialize();
                            stream.Write(data, 0, data.Length);
                        }
                        if (c.Command_Code == CommandCode.Join_Game)
                        {
                            _player = (Player)c.Data1;
                            _player.Address = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            bool found = false;
                            foreach (Player p in room.Player_List)
                            {
                                if (p.Address == _player.Address && p.id == _player.id)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            bool update_room = false;
                            if (!found)
                            {
                                this.room.Player_List.Add(_player);
                                this.tcpServerClient.Add(new TcpClient(_player.Address, 51003));
                                this.UpdateRoom();
                                SendChatMessage(_player.Player_name + " had joined the room!" + "\n");
                                update_room = true;
                            }

                            if (update_room)
                            {
                                int Player_Index = room.findByID(Player_ID);
                                Command c2 = new Command(CommandCode.Update_Room, room);
                                byte[] data2 = c2.Serialize();
                                for (int i2 = 0; i2 < tcpServerClient.Count; i2++)
                                {
                                    try
                                    {
                                        if (i2 == Player_Index) continue;
                                        tcpServerClient[i2].GetStream().Write(data2, 0, data2.Length);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                        else if (c.Command_Code == CommandCode.Chat_Message)
                        {
                            SendChatMessage(c.Message);
                        }
                        else if (c.Command_Code == CommandCode.Ready)
                        {
                            string ipPlayer = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            foreach (Player p in room.Player_List)
                            {
                                if (p.Address == ipPlayer && p.id == (Guid)c.Data1)
                                {
                                    p.Status = true;
                                    break;
                                }
                            }
                            int Player_Index = room.findByID(Player_ID);
                            Command c2 = new Command(CommandCode.Update_Room, room);
                            byte[] data2 = c2.Serialize();
                            for (int j = 0; j < tcpServerClient.Count; j++)
                            {
                                try
                                {
                                    if (j == Player_Index) continue;
                                    tcpServerClient[j].GetStream().Write(data2, 0, data2.Length);
                                }
                                catch
                                {
                                }
                            }
                            this.UpdateRoom();
                        }
                        else if (c.Command_Code == CommandCode.Cancel)
                        {
                            string ipPlayer = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            foreach (Player p in room.Player_List)
                            {
                                if (p.Address == ipPlayer && p.id == (Guid)c.Data1)
                                {
                                    p.Status = false;
                                    break;
                                }
                            }
                            int Player_Index = room.findByID(Player_ID);
                            Command c2 = new Command(CommandCode.Update_Room, room);
                            byte[] data2 = c2.Serialize();
                            for (int j = 0; j < tcpServerClient.Count; j++)
                            {
                                try
                                {
                                    if (j == Player_Index) continue;
                                    tcpServerClient[j].GetStream().Write(data2, 0, data2.Length);
                                }
                                catch
                                {
                                }
                            }
                            this.UpdateRoom();
                        }
                    }
                    client.Close();
                }
            }
        }

        private void ClientReceiver()
        {
            Byte[] bytes = new Byte[1024 * 16];
            while (receiverRun)
            {
                if (StoppedTcp)
                {
                    break;
                }
                else if (receiveTcp.Pending())
                {
                    LastReceiveTimeFromHost = DateTime.Now;
                    TcpClient client = receiveTcp.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        if (!connect_to_host)
                        {
                            client.Close();
                            break;
                        }
                        /*BinaryFormatter bin = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(bytes);*/
                        Command c = new Command(bytes);
                        if (c.Command_Code == CommandCode.Standby)
                        {
                            //do nothing
                        }
                        else if (c.Command_Code == CommandCode.Update_Room)
                        {
                            room = (Room)c.Data1;
                            room.Player_List[room.owner_index].Address = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            this.UpdateRoom();
                        }
                        else if (c.Command_Code == CommandCode.Chat_Message)
                        {
                            chatDisplay.Text += c.Message;
                        }
                        else if (c.Command_Code == CommandCode.Start_Game)
                        {
                            room = (Room)c.Data1;
                            ScreenEvent.Invoke(this, new SivEventArgs(1));
                        }
                    }
                }
                else if ((DateTime.Now - LastReceiveTimeFromHost) > new TimeSpan(0, 0, 5))
                {
                    try
                    {
                        Command c = new Command(CommandCode.Check_Connect);
                        c.SendData(room.Player_List[room.owner_index].Address, 51002);
                    }
                    catch
                    {
                        Game1.MessageBox(new IntPtr(0), "Disconected from host", "Disconnected", 0);
                        ScreenEvent.Invoke(this, new SivEventArgs(0));
                    }
                }
            }
        }

        public void End_Receive()
        {
            receiverRun = false;
            connect_to_host = false;
            StoppedTcp = true;
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
            panelTexture = Content.Load<Texture2D>("Resource/graphic/Panel");
            imageBorderTexture = Content.Load<Texture2D>("Resource/graphic/base2");
            #endregion

            #region Player Content
            div_char[0] = new RectangleF(55, 40, 180, 180);
            div_char[1] = new RectangleF(265, 40, 180, 180);
            div_char[2] = new RectangleF(475, 40, 180, 180);
            div_char[3] = new RectangleF(685, 40, 180, 180);
            div_char[4] = new RectangleF(55, 270, 180, 180);
            div_char[5] = new RectangleF(265, 270, 180, 180);
            div_char[6] = new RectangleF(475, 270, 180, 180);
            div_char[7] = new RectangleF(685, 270, 180, 180);

            //div_border = new Border("player_border", borderColor, 2
            //   , new RectangleF(20, 20, 900, 480), this);
            Image playerPanel = new Image("Player Panel", panelTexture, new RectangleF(10, 10, 920, 500), 0.3f, this);
            #endregion

            #region Room Information
            //div_info_border = new Border("div_info", borderColor, 2
            //    , new RectangleF(930, 20, 250, 480), this);
            //roomInfoDiv =Image info_pane new Div("Room Info", new RectangleF(932, 22, 246, 476), Color.DarkRed, this);
            Image infoPanel = new Image("Info Panel", panelTexture, new RectangleF(932, 22, 246, 476), 0.3f, this);
            roomInfoContent = new Label("Room Info", Game1.gautami12Regular, "", 960, 80, 300, Color.White, this);
            roomInfoTitle = new Label("Info Label", Game1.gautami14Bold, "Room Information", 970, 50, 300, Color.White, this);
            #endregion

            #region Player Name Label
            for (int i = 0; i < div_char.Length; i++)
            {
                //String name = "div_char" + i;
                //div_char_border[i] = new Border(name, borderColor, 2, div_char[i], this);
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
            MainPlayer_Boder = new Border("MainPlayer_Border", Color.Purple, 2, new RectangleF(0, 0, 0, 0), this);
            #endregion

            #region Button
            cancelButtonTexture = Content.Load<Texture2D>("Resource/button/cancel_button");
            readyButtonTexture = Content.Load<Texture2D>("Resource/button/ready");
            startButtonTexture = Content.Load<Texture2D>("Resource/button/start_button");

            start_button = new ImageButton("Start", startButtonTexture
                , new RectangleF(980, 540, 180, 70), this);
            start_button.OnClick += Start_button_clicked;

            quit_button = new ImageButton("Quit", Content.Load<Texture2D>("Resource/button/quit_button")
                , new RectangleF(980, 620, 180, 70), this);
            quit_button.OnClick += Quit_button_clicked;
            #endregion

            #region Chat
            //Textbox
            chat = new TextBox("Chat Input"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.font, new RectangleF(22, 662, 946, 20), this);
            chat.OnEnterPressed += ChatBox_EnterPressed;

            chatDisplay = new TextBox("Chat Display"
                , Game1.transparentTextBox, Game1.highlightedTextbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar
                , Game1.font, new RectangleF(22, 522, 946, 126), this);
            chatDisplay.Color = Color.White;
            chatDisplay.ReadOnly = true;
            chatDisplay.vscrollable = true;

            //Border
            chat_box_border = new Border("chat_screen", Color.White, 2, chatDisplay.Rect, this);
            chat_input_border = new Border("chat_input", Color.White, 2, chat.Rect, this);
            #endregion

            #region RoomScreen_RegisterHandler
            OnKeysDown += RoomScreen_OnKeysDown;
            #endregion
        }

        #endregion

        #region Start-Stop
        public override void Start(Command command)
        {
            chatDisplay.Text = "";
            if (command.Value_Int == 0)
            {
                Start_Broadcast();
                StartSynch();
                InitializeReceiver();
                UpdateRoom();
            }
            else if (command.Value_Int == 1)
            {
                InitializeReceiver();
                StartSynch();
                UpdateRoom();
            }
        }

        public override void End(Command command)
        {
            End_Broadcast();
            EndSynch();
            End_Receive();
            this.room = null;
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
            bool isAllReady = true;
            foreach (Player p in room.Player_List)
            {
                if (!p.Status)
                {
                    isAllReady = false;
                    break;
                }
            }
            if (isAllReady)
            {
                try
                {
                    int Player_Index = room.findByID(Player_ID);
                    Command c = new Command(CommandCode.Start_Game, room);
                    byte[] data2 = c.Serialize();
                    for (int i = 0; i < tcpServerClient.Count; i++)
                    {
                        try
                        {
                            if (i == Player_Index) continue;
                            tcpServerClient[i].GetStream().Write(data2, 0, data2.Length);
                        }
                        catch { }
                    }
                }
                catch { }
                ScreenEvent.Invoke(this, new SivEventArgs(1));
            }
            else
            {
                Game1.MessageBox(new IntPtr(0), "All Players in this room are not ready!", "Warning!", 0);
            }
        }

        private void Ready_Button_Clicked(object sender, FormEventData e)
        {
            try
            {
                Command c = new Command(CommandCode.Ready, this.Player_ID);
                byte[] data = c.Serialize();
                tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
                tcpClient.Close();
            }
            catch { }
        }

        private void Cancel_button_clicked(object sender, FormEventData e)
        {
            try
            {
                Command c = new Command(CommandCode.Cancel, this.Player_ID);
                byte[] data = c.Serialize();
                tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
                tcpClient.Close();
            }
            catch { }
        }

        private void Quit_button_clicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
        }

        private void ChatBox_EnterPressed(object sender)
        {
            string s = room.findPlayerByID(Player_ID).Player_name + ": " + chat.Text + "\n";
            if (room.isHost(Player_ID))
            {
                SendChatMessage(s);
            }
            else
            {
                Command c = new Command(CommandCode.Chat_Message, s);
                byte[] data = c.Serialize();

                tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
                tcpClient.Close();
            }
            this.chat.Text = "";
        }

        private void SendChatMessage(string message)
        {
            Command c = new Command(CommandCode.Chat_Message, message);
            byte[] data = c.Serialize();
            for (int i = 0; i < tcpServerClient.Count; i++)
            {
                try
                {
                    if (room.Player_List[i].id == this.Player_ID) continue;
                    tcpServerClient[i].GetStream().Write(data, 0, data.Length);
                }
                catch
                {
                }
            }
            chatDisplay.Text += message;
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
        private void play_animation(ref Rectangle rec)
        {
            //int x = 255;
            //int y = 270;
            //int speed = 2;
            //if (rec.X == x && rec.Y == y)
            //    play_animation_state = false;
            //else
            //{
            //    My_Extension.move_rec(ref rec, x, y, speed, speed);
            //}
        }

        public void UpdateRoom()
        {
            String s = "Owner index: " + room.owner_index + "\n";
            //s += "Main Player: " + room.Player_List[Player_Index].Player_name + "\n";
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
            foreach (var item in borderAvatarList) item.Delete();
            borderAvatarList.Clear();
            foreach (var item in playerName)
            {
                item.Text = "";
                item.Visible = false;
                item.Color = Color.White;
            }
            for (int i = 0; i < room.Player_List.Count; i++)
            {
                string status;
                if (room.Player_List[i].Status)
                {
                    status = "Ready";
                }
                else
                {
                    status = "Not Ready";
                }
                String playerNameStr = room.Player_List[i].Player_name;
                playerName[i].Text = playerNameStr + " - " + status;
                playerName[i].Visible = true;
                if (room.Player_List[i].id == this.Player_ID)
                {
                    playerName[i].Color = Color.Aquamarine;
                }
                
                Image borderAvatar = new Image(playerNameStr + "border", imageBorderTexture, div_char[i], 0.4f, this);
                borderAvatar.Priority = 0.41f;
                
                Image newAvatar = new Image(playerNameStr, avatarDefault,
                    new RectangleF(div_char[i].X + 18, div_char[i].Y + 16, div_char[i].Width - 38, div_char[i].Height - 38), 0.41f, this);
                newAvatar.Priority = 0.4f;

                avatar_img.Add(newAvatar);
                borderAvatarList.Add(borderAvatar);
            }
            numberOfPlayer = room.Player_List.Count;
            ButtonChange();
        }

        public void ButtonChange()
        {
            int Player_Index = room.findByID(Player_ID);
            if (Player_Index == room.owner_index)
            {
                start_button.Texture = startButtonTexture;
                start_button.OnClick = Start_button_clicked;
            }
            else
            {
                if (!room.Player_List[Player_Index].Status)
                {
                    start_button.Texture = readyButtonTexture;
                    start_button.OnClick = Ready_Button_Clicked;
                }
                else
                {
                    start_button.Texture = cancelButtonTexture;
                    start_button.OnClick = Cancel_button_clicked;
                }
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            //if (play_animation_state) play_animation(ref avatar_img.rec);            
            base.Update(theTime);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, main_game.rz);
            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }

    
}
