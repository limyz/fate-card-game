using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using EventTextInput;

namespace WindowsGame1
{
    public class InGameScreen : Screen
    {
        #region chat_stuff
        //delegate void AddMessage(string message);

        //string userName = "";

        //const int send_port = 51000;
        //const int receive_port = 51000;
        //string Address = "255.255.255.255";

        //UdpClient receivingClient;
        //UdpClient sendingClient;

        //Thread receivingThread;

        //public void SetUsername()
        //{
        //    userName = usernameTextbox.Text.Replace("\n", "");
        //}

        //public void SetIP()
        //{
        //    if (string.IsNullOrEmpty(ipTextbox.Text))
        //    {
        //        Address = "255.255.255.255";
        //    }
        //    /*else if (textbox_IP.Text == "all")
        //    {
        //        Address = IPAddress.Broadcast.ToString();
        //    }*/
        //    else
        //    {
        //        Address = ipTextbox.Text.Replace("\n", "");
        //    }
        //}

        //public void InitializeSender()
        //{
        //    sendingClient = new UdpClient();
        //    sendingClient.EnableBroadcast = true;
        //}

        //public void InitializeReceiver()
        //{
        //    receivingClient = new UdpClient(receive_port);
        //    receivingClient.EnableBroadcast = true;

        //    ThreadStart start = new ThreadStart(Receiver);
        //    receivingThread = new Thread(start);
        //    receivingThread.IsBackground = true;
        //    receivingThread.Start();
        //}

        //private void Receiver()
        //{
        //    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, receive_port);
        //    AddMessage messageDelegate = MessageReceived;

        //    while (true)
        //    {
        //        byte[] data = receivingClient.Receive(ref endPoint);
        //        string message = Encoding.Unicode.GetString(data);
        //        //messageDelegate.Invoke(endPoint.Address.ToString()+":"+endPoint.Port+">>"+message);
        //        messageDelegate.Invoke(message);
        //        //Invoke(messageDelegate, message);
        //    }
        //}

        //private void MessageReceived(string message)
        //{
        //    chatDisplayTextbox.Text += (message + "\n");
        //}
        //public void End_Chat()
        //{
        //    receivingThread.Abort();
        //    sendingClient.Close();
        //    receivingClient.Close();
        //}

        #endregion

        #region Variable Decleration
        int cardWidth = 105;
        int cardHeight = 150;
        int handWitdh = 535;
        int padding = 5;
        int hand_hovered_index = -1;
        int[] playerRandomChar = new int[2];

        public Room room;
        public Guid Player_ID;
        Player myPlayer;

        private ContentManager Content;
        Random rand = new Random();
        Texture2D borderTexture, characterBackTexture, shirou, masterTexture, servantTexture;
        Rectangle[,] oppPlayerRectangle;
        Border chatInputBorder, chatDisplayBorder, handZoneBorder, equipZoneBorder;
        Border[] playerCharacterBorder = new Border[2];
        List<Label> OtherPlayerNameLabel = new List<Label>();
        Color borderColor = Color.MediumAquamarine;
        XmlDocument xml = new XmlDocument();
        Character[] masterList, servantList;
        List<Card> cardList = new List<Card>();
        List<Card> handList = new List<Card>();
        List<Image> otherPlayerMasterImage = new List<Image>();
        List<Image> otherPlayerServantImage = new List<Image>();
        List<Rectangle> hand_area_list = new List<Rectangle>();
        Image masterImg, servantImg;
        ImageButton drawButton;
        TextBox chatInputTextbox, chatDisplayTextbox, usernameTextbox, ipTextbox;
        Label usernameLabel, ipLabel;
        Div playerControlPanel;
        #endregion

        #region Character class
        //public class Character
        //{
        //    private ContentManager Content;
        //    public string CharTag;
        //    public string CharName;
        //    public string CharAsset;
        //    public Texture2D CharTexture;
        //    public enum char_type
        //    {
        //        master = 0,
        //        servant = 1
        //    };
        //    char_type type;
        //    public Character(ContentManager Content, string char_tag, string char_name, string char_asset, char_type type)
        //    {
        //        this.Content = Content;
        //        this.CharTag = char_tag;
        //        this.CharName = char_name;
        //        this.CharAsset = char_asset;
        //        this.type = type;
        //    }
        //    public void load_texture()
        //    {
        //        CharTexture = Content.Load<Texture2D>("Resource/character/" + CharAsset);
        //    }
        //}
        #endregion

        #region Card class
        public class Card
        {
            private ContentManager Content;
            public string tag;
            public string name;
            public string asset;
            public Texture2D texture;
            public Card(ContentManager Content, string tag, string name, string asset)
            {
                this.Content = Content;
                this.tag = tag;
                this.name = name;
                this.asset = asset;
            }
            public void load_texture()
            {
                this.texture = Content.Load<Texture2D>("Resource/card/" + asset);
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
            if (isHost())
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
                //ClientJoin();
            }
        }

        private bool isHost()
        {
            int Player_Index = room.findByID(Player_ID);
            if (room.owner_index == Player_Index)
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
                        if (!room.Player_List[i].Status) continue;
                        tcpServerClient[i].GetStream().Write(data, 0, data.Length);
                    }
                    //couldn't send message because the client has disconnected
                    //so we remove that tcp client and player from the list
                    catch
                    {
                        string s = room.Player_List[i].Player_name + " had left the room!" + "\n";
                        //this.tcpServerClient.RemoveAt(i);
                        //this.room.Player_List.RemoveAt(i);
                        //i--;
                        room.Player_List[i].Status = false;
                        UpdateRoom(room.findByID_ExcludeMainPlayer(room.Player_List[i].id, Player_ID), i);
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
                            if (!room.Player_List[i].Status) continue;
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

        #region Receiver
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

            if (isHost())
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
            while (receiverRun)
            {
                if (StoppedTcp)
                {
                    break;
                }
                else if (receiveTcp.Pending())
                {
                    TcpClient client = receiveTcp.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Command c = new Command(bytes);
                        if (c.Command_Code == CommandCode.Chat_Message)
                        {
                            SendChatMessage(c.Message);
                        }
                        else if (c.Command_Code == CommandCode.Character_Change)
                        {
                            Character character1 = (Character)c.Data1;
                            Character character2 = (Character)c.Data2;
                            Guid id = (Guid)c.Data3;
                            CharacterChange(character1, character2, id);
                            int Player_Index = room.findByID(Player_ID);
                            Command c2 = new Command(CommandCode.Character_Change, character1, character2, id);
                            byte[] data2 = c2.Serialize();
                            for (int j = 0; j < tcpServerClient.Count; j++)
                            {
                                try
                                {
                                    if (j == Player_Index) continue;
                                    if (!room.Player_List[j].Status) continue;
                                    if (room.Player_List[j].id == id) continue;
                                    tcpServerClient[j].GetStream().Write(data2, 0, data2.Length);
                                }
                                catch { }
                            }
                        }
                        else if (c.Command_Code == CommandCode.Update_Room)
                        {
                            Room room = (Room)c.Data1;
                            int Player_Index = room.findByID(Player_ID);
                            for (i = 0; i < room.Player_List.Count; i++)
                            {
                                if (i == Player_Index) continue;
                                if (!room.Player_List[i].Status)
                                {
                                    UpdateRoom(room.findByID_ExcludeMainPlayer(room.Player_List[i].id, Player_ID),Player_Index);
                                }
                            }
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
                        else if (c.Command_Code == CommandCode.Chat_Message)
                        {
                            chatDisplayTextbox.Text += c.Message;
                        }
                        else if (c.Command_Code == CommandCode.Character_Change)
                        {
                            Character character1 = (Character)c.Data1;
                            Character character2 = (Character)c.Data2;
                            Guid id = (Guid)c.Data3;
                            CharacterChange(character1, character2, id);
                        }
                    }
                }
                else if ((DateTime.Now - LastReceiveTimeFromHost) > new TimeSpan(0, 0, 3))
                {
                    Game1.MessageBox(new IntPtr(0), "Host has left the game", "Host has left the game", 0);
                    //ScreenEvent.Invoke(this, new SivEventArgs(0));
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
        public InGameScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theScreenEvent, Game1 parent)
            : base("InGameScreen", theScreenEvent, parent)
        {
            #region Load Resource
            this.Content = Content;
            borderTexture = Content.Load<Texture2D>("Resource/Untitled-1");
            characterBackTexture = Content.Load<Texture2D>("Resource/character_back");
            shirou = Content.Load<Texture2D>("Resource/character1");
            //player_control_texture = Content.Load<Texture2D>("Resource/controlplayer");
            #endregion

            #region Player Control Panel
            playerCharacterBorder[0] = new Border("Character Border 1", Color.Red,
                2, new Rectangle(731, 564, 111, 156), this);
            playerCharacterBorder[1] = new Border("Character Border 2", Color.Red,
                2, new Rectangle(842, 564, 111, 156), this);

            playerControlPanel = new Div("PlayerControlPanel",
                new Rectangle(0, 564, 1000, 156), Color.White, this);
            masterImg = new Image("Player Master Image", characterBackTexture,
                new Rectangle(734, 567, 105, 150), 0.3f, this);
            servantImg = new Image("Player Servant Image", characterBackTexture,
                new Rectangle(845, 567, 105, 150), 0.3f, this);

            handZoneBorder = new Border("Hand Zone", Color.Red, 2,
                new Rectangle(170, 564, 565, 156), this);
            equipZoneBorder = new Border("Equip Zone", Color.Red, 2,
                new Rectangle(0, 564, 168, 156), this);
            #endregion

            #region Button
            drawButton = new ImageButton("draw_button", Content.Load<Texture2D>("Resource/draw")
                , new Rectangle(740, 520, 130, 35), 0.5f, this);
            drawButton.OnClick += draw_button_clicked;
            #endregion

            #region Chat
            chatDisplayBorder = new Border("chatDisplayBorder", borderColor, 2
                , new Rectangle(1008, 10, 190, 404), this);

            chatDisplayTextbox = new TextBox("chatDisplayTextbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar, Game1.font
                , new Rectangle(1010, 12, 186, 400), this);
            chatDisplayTextbox.hscrollable = true;
            chatDisplayTextbox.vscrollable = true;
            chatDisplayTextbox.ReadOnly = true;

            chatInputBorder = new Border("chatInputBorder", borderColor, 2
               , new Rectangle(1008, 438, 190, 104), this);

            chatInputTextbox = new TextBox("chatInputTextbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar, Game1.font
                , new Rectangle(1010, 440, 186, 100), this);
            chatInputTextbox.hscrollable = true;
            chatInputTextbox.vscrollable = true;
            chatInputTextbox.OnEnterPressed += chat_textbox_onEnterPressed;
            chatInputTextbox.OnShift_EnterPressed += textbox_onShiftEnterPressed;
            #endregion

            #region IP Test
            usernameLabel = new Label("label_username", Game1.font, "Username"
                , 1010, 580, 1198 - 1010, Color.White, this);

            ipLabel = new Label("label_IP", Game1.font, "IP Address"
                , 1010, 630, 1198 - 1010, Color.White, this);

            usernameTextbox = new TextBox("textbox_username"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.font, new Rectangle(1010, 600, 188, 20), this);
            usernameTextbox.Text = "Siv";

            ipTextbox = new TextBox("textbox_IP"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.font, new Rectangle(1010, 650, 188, 20), this);
            ipTextbox.Text = "255.255.255.255";
            #endregion

            #region inGameScreen_RegisterHandler
            OnKeysDown += InGameScreen_OnKeysDown;
            #endregion

            #region XML loading
            //Character's data load
            xml.Load("Data/Character.xml");
            XmlNodeList xml_master_list = xml.GetElementsByTagName("Master")[0].ChildNodes;
            XmlNodeList xml_servant_list = xml.GetElementsByTagName("Servant")[0].ChildNodes;
            //MessageBox(new IntPtr(0), xml_master_list.Count.ToString(), "", 0);
            masterList = new Character[xml_master_list.Count];
            servantList = new Character[xml_servant_list.Count];
            for (int i = 0; i < masterList.Length; i++)
            {
                XmlElement temp = (XmlElement)xml_master_list[i];
                masterList[i] = new Character(//Content,
                    //xml_master_list[i].Name,
                    xml_master_list[i].InnerText,
                    temp.GetAttribute("img"),
                    Character.Type.Master);
                //MessageBox(new IntPtr(0), master_list[i].char_asset, "", 0);
                //masterList[i].load_texture();
            }
            for (int i = 0; i < servantList.Length; i++)
            {
                XmlElement temp = (XmlElement)xml_servant_list[i];
                servantList[i] = new Character(//Content,
                    // xml_servant_list[i].Name,
                    xml_servant_list[i].InnerText,
                    temp.GetAttribute("img"),
                    Character.Type.Servant);
                //servantList[i].load_texture();
            }
            //End character's data load

            playerRandomChar[0] = rand.Next(masterList.Length);
            playerRandomChar[1] = rand.Next(servantList.Length);

            //Card's data load            
            xml.Load("Data/Card.xml");
            XmlNodeList xml_card_list = xml.GetElementsByTagName("Card")[0].ChildNodes;
            //card_list = new Card[xml_card_list.Count];
            for (int i = 0; i < xml_card_list.Count; i++)
            {
                XmlElement temp = (XmlElement)xml_card_list[i];
                cardList.Add(new Card(Content,
                    xml_card_list[i].Name,
                    xml_card_list[i].InnerText,
                    temp.GetAttribute("img")));
                cardList[i].load_texture();
            }
            //End card's data load
            cardList.shuffle<Card>();
            #endregion
        }

        public override void Start(Command command)
        {
            InitializeReceiver();
            StartSynch();

            int Player_Index = room.findByID(Player_ID);
            myPlayer = room.Player_List[Player_Index];
            oppPlayerRectangle = new Rectangle[room.Player_List.Count - 1, 2];
            #region Define Ohter Player Area
            switch (room.Player_List.Count)
            {
                case 2:
                    oppPlayerRectangle[0, 0] = new Rectangle(435, 30, 100, 75);
                    oppPlayerRectangle[0, 1] = new Rectangle(435, 105, 100, 75);
                    break;
                case 3:
                    oppPlayerRectangle[0, 0] = new Rectangle(180, 30, 100, 75);
                    oppPlayerRectangle[0, 1] = new Rectangle(180, 105, 100, 75);
                    oppPlayerRectangle[1, 0] = new Rectangle(690, 30, 100, 75);
                    oppPlayerRectangle[1, 1] = new Rectangle(690, 105, 100, 75);
                    break;
                case 4:
                    oppPlayerRectangle[0, 0] = new Rectangle(20, 100, 100, 75);
                    oppPlayerRectangle[0, 1] = new Rectangle(20, 175, 100, 75);
                    oppPlayerRectangle[1, 0] = new Rectangle(435, 30, 100, 75);
                    oppPlayerRectangle[1, 1] = new Rectangle(435, 105, 100, 75);
                    oppPlayerRectangle[2, 0] = new Rectangle(860, 100, 100, 75);
                    oppPlayerRectangle[2, 1] = new Rectangle(860, 175, 100, 75);
                    break;
                case 5:
                    oppPlayerRectangle[0, 0] = new Rectangle(20, 100, 100, 75);
                    oppPlayerRectangle[0, 1] = new Rectangle(20, 175, 100, 75);
                    oppPlayerRectangle[0, 0] = new Rectangle(180, 30, 100, 75);
                    oppPlayerRectangle[0, 1] = new Rectangle(180, 105, 100, 75);
                    oppPlayerRectangle[1, 0] = new Rectangle(690, 30, 100, 75);
                    oppPlayerRectangle[1, 1] = new Rectangle(690, 105, 100, 75);
                    oppPlayerRectangle[2, 0] = new Rectangle(860, 100, 100, 75);
                    oppPlayerRectangle[2, 1] = new Rectangle(860, 175, 100, 75);
                    break;
                case 6:
                    oppPlayerRectangle[0, 0] = new Rectangle(20, 100, 50, 150);
                    oppPlayerRectangle[0, 1] = new Rectangle(70, 100, 50, 150);
                    oppPlayerRectangle[1, 0] = new Rectangle(180, 30, 50, 150);
                    oppPlayerRectangle[1, 1] = new Rectangle(230, 30, 50, 150);
                    oppPlayerRectangle[2, 0] = new Rectangle(435, 30, 50, 150);
                    oppPlayerRectangle[2, 1] = new Rectangle(485, 30, 50, 150);
                    oppPlayerRectangle[3, 0] = new Rectangle(690, 30, 50, 150);
                    oppPlayerRectangle[3, 1] = new Rectangle(740, 30, 50, 150);
                    oppPlayerRectangle[4, 0] = new Rectangle(860, 100, 50, 150);
                    oppPlayerRectangle[4, 1] = new Rectangle(910, 100, 50, 150);
                    break;
                case 7:
                    oppPlayerRectangle[0, 0] = new Rectangle(20, 100, 50, 150);
                    oppPlayerRectangle[0, 1] = new Rectangle(70, 100, 50, 150);
                    oppPlayerRectangle[1, 0] = new Rectangle(180, 30, 50, 150);
                    oppPlayerRectangle[1, 1] = new Rectangle(230, 30, 50, 150);
                    oppPlayerRectangle[2, 0] = new Rectangle(350, 30, 50, 150);
                    oppPlayerRectangle[2, 1] = new Rectangle(400, 30, 50, 150);
                    oppPlayerRectangle[3, 0] = new Rectangle(520, 30, 50, 150);
                    oppPlayerRectangle[3, 1] = new Rectangle(570, 30, 50, 150);
                    oppPlayerRectangle[4, 0] = new Rectangle(690, 30, 50, 150);
                    oppPlayerRectangle[4, 1] = new Rectangle(740, 30, 50, 150);
                    oppPlayerRectangle[5, 0] = new Rectangle(860, 100, 50, 150);
                    oppPlayerRectangle[5, 1] = new Rectangle(910, 100, 50, 150);
                    break;
                case 8:
                    oppPlayerRectangle[0, 0] = new Rectangle(20, 320, 50, 150);
                    oppPlayerRectangle[0, 1] = new Rectangle(70, 320, 50, 150);
                    oppPlayerRectangle[1, 0] = new Rectangle(20, 100, 50, 150);
                    oppPlayerRectangle[1, 1] = new Rectangle(70, 100, 50, 150);
                    oppPlayerRectangle[2, 0] = new Rectangle(180, 30, 50, 150);
                    oppPlayerRectangle[2, 1] = new Rectangle(230, 30, 50, 150);
                    oppPlayerRectangle[3, 0] = new Rectangle(435, 30, 50, 150);
                    oppPlayerRectangle[3, 1] = new Rectangle(485, 30, 50, 150);
                    oppPlayerRectangle[4, 0] = new Rectangle(690, 30, 50, 150);
                    oppPlayerRectangle[4, 1] = new Rectangle(740, 30, 50, 150);
                    oppPlayerRectangle[5, 0] = new Rectangle(860, 100, 50, 150);
                    oppPlayerRectangle[5, 1] = new Rectangle(910, 100, 50, 150);
                    oppPlayerRectangle[6, 0] = new Rectangle(860, 320, 50, 150);
                    oppPlayerRectangle[7, 1] = new Rectangle(910, 320, 50, 150);
                    break;
                case 9:
                    oppPlayerRectangle[0, 0] = new Rectangle(20, 320, 50, 150);
                    oppPlayerRectangle[0, 1] = new Rectangle(70, 320, 50, 150);
                    oppPlayerRectangle[1, 0] = new Rectangle(20, 100, 50, 150);
                    oppPlayerRectangle[1, 1] = new Rectangle(70, 100, 50, 150);
                    oppPlayerRectangle[2, 0] = new Rectangle(180, 30, 50, 150);
                    oppPlayerRectangle[2, 1] = new Rectangle(230, 30, 50, 150);
                    oppPlayerRectangle[3, 0] = new Rectangle(350, 30, 50, 150);
                    oppPlayerRectangle[3, 1] = new Rectangle(400, 30, 50, 150);
                    oppPlayerRectangle[4, 0] = new Rectangle(520, 30, 50, 150);
                    oppPlayerRectangle[4, 1] = new Rectangle(570, 30, 50, 150);
                    oppPlayerRectangle[5, 0] = new Rectangle(690, 30, 50, 150);
                    oppPlayerRectangle[5, 1] = new Rectangle(740, 30, 50, 150);
                    oppPlayerRectangle[6, 0] = new Rectangle(860, 100, 50, 150);
                    oppPlayerRectangle[6, 1] = new Rectangle(910, 100, 50, 150);
                    oppPlayerRectangle[7, 0] = new Rectangle(860, 320, 50, 150);
                    oppPlayerRectangle[7, 1] = new Rectangle(910, 320, 50, 150);
                    break;
                default:
                    break;
            }
            #endregion
            for (int i = 0, i2 = 0; i < room.Player_List.Count; i++, i2++)
            {
                if (i == Player_Index)
                {
                    i2--;
                    continue;
                }
                Label labelTemp = new Label("OtherPlayerNameLbel", Game1.font, room.Player_List[i].Player_name
                    , oppPlayerRectangle[i2, 0].X
                    , oppPlayerRectangle[i2, 0].Top - 20
                    , oppPlayerRectangle[i2, 1].Width
                    , Color.White, this);
                labelTemp.CenterAlign = true;
                OtherPlayerNameLabel.Add(labelTemp);

                Image masterTemp = new Image("Opp Master Image", characterBackTexture, oppPlayerRectangle[i2, 0], 0.3f, this);
                masterTemp.Source_Rectangle = new Rectangle(0, 0, characterBackTexture.Width, characterBackTexture.Height / 2);
                //masterTemp.OnMouseEnter = new FormEventHandler(hoverChar);
                //masterTemp.OnMouseLeave = new FormEventHandler(unHoverMasterChar);
                otherPlayerMasterImage.Add(masterTemp);

                Image servantTemp = new Image("Opp Servant Image", characterBackTexture,
                    oppPlayerRectangle[i2, 1], 0.3f, this);
                servantTemp.Source_Rectangle = new Rectangle(0, 0,
                    characterBackTexture.Width, characterBackTexture.Height / 2);
                //servantTemp.OnMouseEnter = new FormEventHandler(hoverChar);
                //servantTemp.OnMouseLeave = new FormEventHandler(unHoverServantChar);
                otherPlayerServantImage.Add(servantTemp);
                //randomCharacter(ref masterTemp, ref servantTemp);
                randomCharacter();
            }
        }
        public override void End(Command command)
        {
            //End_Chat();
            this.chatDisplayTextbox.Text = "";
            this.room = null;
            oppPlayerRectangle = null;
            for (int i = 0; i < OtherPlayerNameLabel.Count; i++)
            {
                OtherPlayerNameLabel[i].Delete();
                otherPlayerMasterImage[i].Delete();
                otherPlayerServantImage[i].Delete();
            }
            OtherPlayerNameLabel.Clear();
            otherPlayerMasterImage.Clear();
            otherPlayerServantImage.Clear();
        }
        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            //randomCharacter();
            if (hand_hovered_index == -1)
            {
                for (int i = hand_area_list.Count - 1; i >= 0; i--)
                {
                    if (main_game.mouse_hover(hand_area_list[i]))
                    {
                        hand_hovered_index = i;
                        break;
                    }
                    hand_hovered_index = -1;
                }
            }
            else
            {
                if (!main_game.mouse_hover(hand_area_list[hand_hovered_index]))
                {
                    for (int i = hand_area_list.Count - 1; i >= 0; i--)
                    {
                        if (main_game.mouse_hover(hand_area_list[i]))
                        {
                            hand_hovered_index = i;
                            break;
                        }
                        hand_hovered_index = -1;
                    }

                }
            }
            base.Update(theTime);
        }
        #endregion

        #region Update's Function
        private void resize_hand()
        {
            int net_width = (hand_area_list[hand_area_list.Count - 1].Right - hand_area_list[0].Left);
            int oversize = net_width - handWitdh;
            if (oversize > 0)
            {
                padding = padding - (oversize / hand_area_list.Count);
            }
            for (int i = 1; i < hand_area_list.Count; i++)
            {
                hand_area_list[i] = new Rectangle(175 + (cardWidth + padding) * i, 567, cardWidth, cardHeight);
            }
        }

        private void draw_card()
        {
            try
            {
                handList.Add(cardList[0]);
                cardList.RemoveAt(0);
                hand_area_list.Add(new Rectangle(175 + (cardWidth + padding) * hand_area_list.Count, 567, cardWidth, cardHeight));
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        public void randomCharacter()
        {
            playerRandomChar[0] = rand.Next(masterList.Length);
            playerRandomChar[1] = rand.Next(servantList.Length);
            myPlayer.Character1 = masterList[playerRandomChar[0]];
            myPlayer.Character2 = servantList[playerRandomChar[1]];
            masterImg.Texture = GetTexture(myPlayer.Character1.CharAsset);
            servantImg.Texture = GetTexture(myPlayer.Character2.CharAsset);
            if (!isHost())
            {
                try
                {
                    Command c = new Command(CommandCode.Character_Change, myPlayer.Character1, myPlayer.Character2, Player_ID);
                    byte[] data = c.Serialize();
                    tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                    networkStream = tcpClient.GetStream();
                    networkStream.Write(data, 0, data.Length);
                    tcpClient.Close();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                try
                {
                    int Player_Index = room.findByID(Player_ID);
                    Command c2 = new Command(CommandCode.Character_Change, myPlayer.Character1, myPlayer.Character2, Player_ID);
                    byte[] data2 = c2.Serialize();
                    for (int j = 0; j < tcpServerClient.Count; j++)
                    {

                        if (j == Player_Index) continue;
                        tcpServerClient[j].GetStream().Write(data2, 0, data2.Length);
                    }
                }
                catch (Exception ex){
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void CharacterChange(Character character1, Character character2, Guid id)
        {
            int Index = room.findByID_ExcludeMainPlayer(id, Player_ID);
            int Player = room.findByID(id);
            room.Player_List[Player].Character1 = character1;
            room.Player_List[Player].Character2 = character2;

            UpdateRoom(Index, Player);
        }

        private void UpdateRoom(int index, int player)
        {
            if (room.Player_List[player].Status)
            {
                otherPlayerMasterImage[index].Texture = GetTexture(room.Player_List[player].Character1.CharAsset);
                otherPlayerServantImage[index].Texture = GetTexture(room.Player_List[player].Character2.CharAsset);
            }
            else
            {
                OtherPlayerNameLabel[index].Text = room.Player_List[player].Player_name + " - Disconnected";
            }
        }
        #endregion

        #region HANDLER
        private void InGameScreen_OnKeysDown(Keys[] keys)
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

        private void chat_textbox_onEnterPressed(TextBox sender)
        {
            string s = room.findPlayerByID(Player_ID).Player_name + ": " + chatInputTextbox.Text + "\n";
            if (isHost())
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
            this.chatInputTextbox.Text = "";
        }

        private void textbox_onShiftEnterPressed(TextBox sender)
        {
            sender.RecieveTextInput('\n');
        }

        private void draw_button_clicked(object sender, FormEventData e)
        {
            draw_card();
            resize_hand();
        }

        private void hoverChar(object sender, FormEventData e)
        {
            Image image = (Image)sender;
            image.Rect.Width = 100;
            image.DrawOrder = 0.29f;
            image.Source_Rectangle = null;
            if (image.Name == "Opp Servant Image")
            {
                image.Rect.X = image.Rect.X - 50;
            }
        }

        private void unHoverMasterChar(object sender, FormEventData e)
        {
            Image image = (Image)sender;
            image.Rect.Width = 50;
            image.DrawOrder = 0.3f;
            image.Source_Rectangle = new Rectangle(0, 0, image.Texture.Width / 2, image.Texture.Height);
        }

        private void unHoverServantChar(object sender, FormEventData e)
        {
            Image image = (Image)sender;
            image.Rect.Width = 50;
            image.DrawOrder = 0.3f;
            image.Rect.X = image.Rect.X + 50;
            image.Source_Rectangle = new Rectangle(image.Texture.Width / 2, 0, image.Texture.Width / 2, image.Texture.Height);
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
            chatDisplayTextbox.Text += message;
        }

        private Texture2D GetTexture(string asset)
        {
            return Content.Load<Texture2D>("Resource/character/" + asset);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            DrawText(spriteBatch);
            //for (int x = 0; x < otherPlayerCount; x++)
            //{
            //    for (int y = 0; y < 2; y++)
            //    {
            //        spriteBatch.Draw(borderTexture, oppPlayerRectangle[x, y], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);
            //    }
            //}
            //spriteBatch.Draw(player_control_texture, playerControlRectangle, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);

            if (hand_hovered_index == -1)
            {
                for (int i = 0; i < handList.Count; i++)
                {
                    spriteBatch.Draw(handList[i].texture, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f - i * 0.001f);
                }
            }
            else
            {
                for (int i = 0; i < handList.Count; i++)
                {
                    if (i == hand_hovered_index)
                    {
                        spriteBatch.Draw(handList[i].texture/*main_game.white_texture*/, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.301f);
                        spriteBatch.Draw(handList[i].texture, hand_area_list[i], null, new Color(0xFF, 0xFF, 0xFF, 0xCC), 0f, new Vector2(0, 0), SpriteEffects.None, 0.3f);
                    }
                    else
                        spriteBatch.Draw(handList[i].texture, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f - i * 0.001f);
                }
            }
            base.Draw(graphics, spriteBatch, gameTime);
            //spriteBatch.Draw(card_list[0].texture, player_card_area, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
        #endregion

        #region Draw's functions
        private void DrawText(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
        }
        #endregion
    }
}
