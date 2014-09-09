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
        float cardWidth = 105;
        float cardHeight = 150;
        float handWitdh = 530;
        float padding = 5;
        float handOrder = 0.5f;
        int hand_hovered_index = -1, characterHoverIndex = -1, selected_card_index = -1;
        int[] playerRandomChar = new int[2];

        public Room room;
        public Guid Player_ID;
        int Player_Index;
        //Player myPlayer;

        private ContentManager Content;
        Random rand = new Random();
        Texture2D borderTexture, characterBackTexture, shirou, masterTexture, servantTexture,
            view_detail_button_textture, back_button_texture, foward_button_texture, panelTexture,
            healthUnitTexture;
        RectangleF[,] oppPlayerRectangle;
        Border chatInputBorder, chatDisplayBorder, handZoneBorder, equipZoneBorder;
        Border[] playerCharacterBorder = new Border[2];
        List<Label> OtherPlayerNameLabel = new List<Label>();
        Color borderColor = Color.MediumAquamarine;
        XmlDocument xml = new XmlDocument();
        Character[] masterList, servantList;
        List<CardDeck> deck = new List<CardDeck>();
        List<CardDeck> handList = new List<CardDeck>();
        List<CardForm> Hand_Image_List = new List<CardForm>();
        List<Image> otherPlayerMasterImage = new List<Image>();
        List<Image> otherPlayerServantImage = new List<Image>();
        Image[] playerHealth;
        //List<Rectangle> hand_area_list = new List<Rectangle>();
        Image masterImg, servantImg, Card_Detail_Image;
        Image infoPanel;
        ImageButton drawButton, handBackButton, handFowardButton, discardButton, useButton, endButton;
        TextBox chatInputTextbox, chatDisplayTextbox, usernameTextbox, ipTextbox;
        Label deckStatistic, handStatistic, cardStatic;
        Div playerControlPanel;

        RasterizerState rz = new RasterizerState();   

        #endregion

        #region Card class
        /*public class Card
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
        }*/
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

        public bool isHost()
        {
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        string s = room.Player_List[i].Player_name + " had left the room!" + "\n";
                        //this.tcpServerClient.RemoveAt(i);
                        //this.room.Player_List.RemoveAt(i);
                        //i--;
                        room.Player_List[i].Status = false;
                        UpdatePlayer(room.findByID_ExcludeMainPlayer(room.Player_List[i].id, Player_ID), i);
                        update_room = true;
                        SendChatMessage(s);
                    }
                }

                if (update_room)
                {
                    Command c2 = new Command(CommandCode.Update_Room, room);
                    sendDataToClient(c2);

                }
                Thread.Sleep(1000);
            }
        }

        public void sendDataToClient(Command c2)
        {
            byte[] data2 = c2.Serialize();
            for (int i = 0; i < tcpServerClient.Count; i++)
            {
                try
                {
                    if (room.Player_List[i].id == Player_ID) continue;
                    //if (!room.Player_List[i].Status) continue;
                    tcpServerClient[i].GetStream().Write(data2, 0, data2.Length);
                }
                catch
                {
                }
            }
        }

        public void sendDataToHost(Command c)
        {
            try
            {
                int Player_Index = room.findByID(Player_ID);
                byte[] data = c.Serialize();
                tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                        switch (c.Command_Code)
                        {
                            case CommandCode.Chat_Message:
                                SendChatMessage(c.Message);
                                break;
                            case CommandCode.Update_Room:
                                Room r = (Room)c.Data1;
                                int PlayerIndex = r.findByID(Player_ID);
                                for (i = 0; i < r.Player_List.Count; i++)
                                {
                                    if (i == PlayerIndex) continue;
                                    if (!r.Player_List[i].Status)
                                    {
                                        UpdatePlayer(r.findByID_ExcludeMainPlayer(r.Player_List[i].id, Player_ID), Player_Index);
                                    }
                                }
                                break;
                            case CommandCode.Character_Change:
                                Character character1 = (Character)c.Data1;
                                Character character2 = (Character)c.Data2;
                                Guid id = (Guid)c.Data3;
                                Thread.Sleep(200);
                                CharacterChange(character1, character2, id);
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
                                break;
                            case CommandCode.CardList_Synchronize:
                                deck = (List<CardDeck>)c.Data1;
                                Command deckSyn = new Command(CommandCode.CardList_Synchronize, deck);
                                sendDataToClient(deckSyn);
                                break;
                            case CommandCode.End_Turn:
                                int PlayerEndTurn = c.Value_Int;
                                ChangeTurn(PlayerEndTurn);
                                break;
                            case CommandCode.Draw_Card:
                                try
                                {
                                    Guid PlayerDrawdCardId = (Guid)c.Data1;
                                    Player player = room.findPlayerByID(PlayerDrawdCardId);
                                    int numberOfCardDraw = (int)c.Data2;

                                    CardDeck drawCard = deck.Last();
                                    Command draw = new Command(CommandCode.Draw_Card_Result, PlayerDrawdCardId, drawCard);
                                    sendDataToClient(draw);

                                    room.findPlayerByID(PlayerDrawdCardId).HandCard.Add(drawCard);
                                    deck.RemoveAt(deck.Count - 1);
                                    //Some effect
                                    chatDisplayTextbox.Text += room.findPlayerByID(PlayerDrawdCardId).Player_name
                                        + " has drawn one card";
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    client.Close();
                }
            }
        }

        private void ClientReceiver()
        {
            
            while (receiverRun)
            {
                if (StoppedTcp)
                {
                    break;
                }
                else if (receiveTcp.Pending())
                {
                    try
                    {
                        LastReceiveTimeFromHost = DateTime.Now;
                        TcpClient client = receiveTcp.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();
                        int i;
                        Byte[] bytes = new Byte[1024 * 16];
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
                            else if (c.Command_Code == CommandCode.CardList_Synchronize)
                            {
                                deck = (List<CardDeck>)c.Data1;
                            }
                            else if (c.Command_Code == CommandCode.Synchronize_All_Player)
                            {
                                this.room.Player_List = (List<Player>)c.Data1;
                            }
                            else if (c.Command_Code == CommandCode.Draw_Card_Result)
                            {
                                //Determine who draws a card and the number of drawn card
                                Guid id = (Guid)c.Data1;
                                Player player = room.findPlayerByID(id);
                                CardDeck drawCard = (CardDeck)c.Data2;
                                //int numberOfCardDraw = (int)c.Data2;

                                if (id == Player_ID)
                                {
                                    //DrawCard();
                                    player.HandCard.Add(drawCard);
                                    deck.RemoveAt(deck.Count - 1);
                                    CardForm card = new CardForm(player.HandCard.Last()
                                     , new RectangleF(190 + (cardWidth + padding) * Hand_Image_List.Count, 567, cardWidth, cardHeight)
                                     , handOrder, main_game.Content, this);

                                    Hand_Image_List.Add(card);
                                    resizeHand();
                                }
                                else
                                {
                                    room.findPlayerByID(id).HandCard.Add(drawCard);
                                    deck.RemoveAt(deck.Count - 1);
                                    //Some effect
                                    chatDisplayTextbox.Text += room.findPlayerByID(id).Player_name
                                        + " has drawn one card.\n";
                                }
                            }
                            else if (c.Command_Code == CommandCode.Change_Turn)
                            {
                                int PlayerEndTurn = (int)c.Data1;
                                int NextPlayer = (int)c.Data2;
                                room.Player_List[PlayerEndTurn].Turn.phase = Turn.Phase.OtherPlayerTurn;
                                room.Player_List[NextPlayer].Turn.phase = Turn.Phase.Beginning;
                            }
                            else if (c.Command_Code == CommandCode.Attack)
                            {
                                Guid usingCard = (Guid)c.Data1;
                                Guid sourcePlayer = (Guid)c.Data2;
                                Guid targetPlayer = (Guid)c.Data3;
                                //chatDisplayTextbox.Text += "You has been attacked by " + room.findPlayerByID(targetPlayer).Player_name + "\n";
                                if (sourcePlayer == Player_ID)
                                {
                                    foreach (CardForm cf in Hand_Image_List)
                                    {
                                        if (cf.CardDeck.CardId == usingCardId)
                                        {
                                            cf.MoveBySpeed(400, 200, 700);
                                            Hand_Image_List.Remove(cf);
                                            break;
                                        }
                                    }
                                    CardDeck carduse = null;
                                    foreach (CardDeck cd in room.findPlayerByID(Player_ID).HandCard)
                                    {
                                        if (cd.CardId == usingCard)
                                        {
                                            carduse = cd;
                                            break;
                                        }
                                    }
                                    room.findPlayerByID(Player_ID).HandCard.Remove(carduse);
                                    chatDisplayTextbox.Text += "You has attacked " + room.findPlayerByID(targetPlayerId).Player_name + "\n";
                                }
                                else if (targetPlayer == Player_ID)
                                {
                                    
                                    int index = room.findByID(sourcePlayer);
                                    CardDeck carduse = null;
                                    for (int j = 0; j < room.Player_List[index].HandCard.Count; j++)
                                    {
                                        if (room.Player_List[index].HandCard[j].CardId == usingCard)
                                        {
                                            carduse = room.Player_List[index].HandCard[j];
                                            room.Player_List[index].HandCard.RemoveAt(j);
                                            break;
                                        }
                                    }
                                    if (carduse != null)
                                    {
                                        chatDisplayTextbox.Text += "You has been attacked by " + room.findPlayerByID(sourcePlayer).Player_name + "\n";
                                        CardForm dummy = new CardForm(carduse, new RectangleF(oppPlayerRectangle[index, 0].X, oppPlayerRectangle[index, 0].Y, cardWidth, cardHeight), 0.3f, main_game.Content, this);
                                        dummy.MoveBySpeed(400, 200, 700);
                                    }

                                    //Asking for Dodge later
                                }
                                else
                                {
                                    int index = room.findByID(sourcePlayer);
                                    CardDeck carduse = null;
                                    for (int j = 0; j < room.Player_List[index].HandCard.Count; j++)
                                    {
                                        if (room.Player_List[index].HandCard[j].CardId == usingCard)
                                        {
                                            carduse = room.Player_List[index].HandCard[j];
                                            room.Player_List[index].HandCard.RemoveAt(j);
                                            break;
                                        }
                                    }
                                    if (carduse != null)
                                    {
                                        CardForm dummy = new CardForm(carduse, new RectangleF(oppPlayerRectangle[index, 0].X, oppPlayerRectangle[index, 0].Y, cardWidth, cardHeight), 0.3f, main_game.Content, this);
                                        dummy.MoveBySpeed(400, 200, 700);
                                        chatDisplayTextbox.Text += room.findPlayerByID(Player_ID).Player_name
                                            + " has attacked " + room.findPlayerByID(targetPlayerId).Player_name + "\n";
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        Game1.MessageBox(new IntPtr(0), "Connection Error!", "Connection Error!", 0);
                        ScreenEvent.Invoke(this, new SivEventArgs(0));
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
        public InGameScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theScreenEvent, Game1 parent)
            : base("InGameScreen", theScreenEvent, parent)
        {
            rz.ScissorTestEnable = true;

            #region Load Resource
            this.Content = Content;
            borderTexture = Content.Load<Texture2D>("Resource/Untitled-1");
            characterBackTexture = Content.Load<Texture2D>("Resource/character_back");
            shirou = Content.Load<Texture2D>("Resource/character1");
            view_detail_button_textture = Content.Load<Texture2D>("Resource/view_detail_button");
            back_button_texture = Content.Load<Texture2D>("Resource/graphic/Actions-arrow-left-double-icon");
            foward_button_texture = Content.Load<Texture2D>("Resource/graphic/Actions-arrow-right-double-icon");
            panelTexture = Content.Load<Texture2D>("Resource/graphic/Panel");
            healthUnitTexture = Content.Load<Texture2D>("Resource/graphic/HealthUnit");
            #endregion

            #region Player Control Panel
            playerCharacterBorder[0] = new Border("Character Border 1", Color.Red,
                2, new RectangleF(731, 564, 111, 156), this);
            playerCharacterBorder[1] = new Border("Character Border 2", Color.Red,
                2, new RectangleF(842, 564, 111, 156), this);

            playerControlPanel = new Div("PlayerControlPanel",
                new RectangleF(0, 564, 1000, 156), Color.White, this);
            masterImg = new Image("Player Master Image", characterBackTexture,
                new RectangleF(734, 567, 105, 150), 0.48f, this);
            servantImg = new Image("Player Servant Image", characterBackTexture,
                new RectangleF(845, 567, 105, 150), 0.48f, this);

            handZoneBorder = new Border("Hand Zone", Color.Red, 2,
                new RectangleF(170, 564, 565, 156), this);
            equipZoneBorder = new Border("Equip Zone", Color.Red, 2,
                new RectangleF(0, 564, 168, 156), this);

            Card_Detail_Image = new Image("Card_Detail_Image", Game1.whiteTexture
                , new RectangleF(200, 80, 298, 416), 0.1f, this);
            Card_Detail_Image.Priority = 0.6f;
            Card_Detail_Image.Visible = false;

            infoPanel = new Image("Card Static Panel", panelTexture, new RectangleF(525, 200, 300, 200), 0.3f, this);
            cardStatic = new Label("Card Static", Game1.arial12Bold, "Test Test Test", 550, 220, 300, Color.White, this);
            infoPanel.Visible = false;
            cardStatic.Visible = false;
            #endregion

            #region Button
            drawButton = new ImageButton("Draw Button", Content.Load<Texture2D>("Resource/button/draw")
                , new RectangleF(600, 520, 130, 35), 0.5f, this);
            drawButton.OnClick += draw_button_clicked;

            useButton = new ImageButton("Use Button", Content.Load<Texture2D>("Resource/button/use")
                , new RectangleF(460, 520, 130, 35), 0.5f, this);
            useButton.Visible = false;
            useButton.OnClick += UseCardClick;
            discardButton = new ImageButton("Discard Button", Content.Load<Texture2D>("Resource/button/discard")
                , new RectangleF(320, 520, 130, 35), 0.5f, this);
            discardButton.Visible = false;

            endButton = new ImageButton("Use Button", Content.Load<Texture2D>("Resource/button/end")
                , new RectangleF(740, 520, 130, 35), 0.5f, this);
            endButton.OnClick += endButton_OnClick;

            //handBackButton = new ImageButton("HLBP", back_button_texture
            //    , new RectangleF(155, 600, 40, 40), 0.47f, this);
            //handFowardButton = new ImageButton("HLFP", foward_button_texture
            //    , new RectangleF(720, 600, 40, 40), 0.47f, this);
            #endregion

            #region Chat
            chatDisplayBorder = new Border("chatDisplayBorder", borderColor, 2
                , new RectangleF(1008, 10, 190, 404), this);

            chatDisplayTextbox = new TextBox("chatDisplayTextbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar, Game1.font
                , new RectangleF(1010, 12, 186, 400), this);
            chatDisplayTextbox.hscrollable = true;
            chatDisplayTextbox.vscrollable = true;
            chatDisplayTextbox.ReadOnly = true;

            chatInputBorder = new Border("chatInputBorder", borderColor, 2
               , new RectangleF(1008, 438, 190, 104), this);

            chatInputTextbox = new TextBox("chatInputTextbox"
                , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar, Game1.font
                , new RectangleF(1010, 440, 186, 100), this);
            chatInputTextbox.hscrollable = true;
            chatInputTextbox.vscrollable = true;
            chatInputTextbox.OnEnterPressed += chat_textbox_onEnterPressed;
            chatInputTextbox.OnShift_EnterPressed += textbox_onShiftEnterPressed;
            #endregion

            #region Label
            deckStatistic = new Label("DeckCount Label", Game1.font, "Deck: "
                , 890, 530, 200, Color.White, this);
            handStatistic = new Label("HandCount Label", Game1.font, "Hand: "
                , 890, 510, 200, Color.White, this);

            //ipLabel = new Label("label_IP", Game1.font, "IP Address"
            //    , 1010, 630, 1198 - 1010, Color.White, this);

            //usernameTextbox = new TextBox("textbox_username"
            //    , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
            //    , Game1.font, new Rectangle(1010, 600, 188, 20), this);
            //usernameTextbox.Text = "Siv";

            //ipTextbox = new TextBox("textbox_IP"
            //    , Game1.whiteTextbox, Game1.highlightedTextbox, Game1.caret
            //    , Game1.font, new Rectangle(1010, 650, 188, 20), this);
            //ipTextbox.Text = "255.255.255.255";
            #endregion

            #region inGameScreen_RegisterHandler
            OnKeysDown += InGameScreen_OnKeysDown;
            #endregion

            #region XML loading
            //Character's data load
            xml.Load("Data/Character.xml");
            XmlNodeList xml_master_list = xml.GetElementsByTagName("Master")[0].ChildNodes;
            XmlNodeList xml_servant_list = xml.GetElementsByTagName("Servant")[0].ChildNodes;
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


            #endregion
        }


        public override void Start(Command command)
        {
            Player_Index = room.findByID(Player_ID);
            StartSynch();
            InitializeReceiver();

            #region InitializeGameData
            oppPlayerRectangle = new RectangleF[room.Player_List.Count - 1, 2];
            foreach (Player player in room.Player_List)
            {
                player.MaxHealth = player.Character1.MaxHealth + player.Character2.MaxHealth;
                player.CurrentHealth = Convert.ToInt32(Math.Floor(player.MaxHealth));
                player.HandLimit = player.CurrentHealth;
            }

            #region Draw Main Player
            masterImg.Texture = GetTexture(room.Player_List[Player_Index].Character1.CharAsset);
            servantImg.Texture = GetTexture(room.Player_List[Player_Index].Character2.CharAsset);
            playerHealth = new Image[room.Player_List[Player_Index].CurrentHealth];
            if (room.Player_List[Player_Index].CurrentHealth == 4)
            {
                playerHealth[0] = new Image("health1", healthUnitTexture,
                    new RectangleF(960, 575, 28, 29), 0.3f, this);
                playerHealth[1] = new Image("health1", healthUnitTexture,
                    new RectangleF(960, 610, 28, 29), 0.3f, this);
                playerHealth[2] = new Image("health1", healthUnitTexture,
                    new RectangleF(960, 645, 28, 29), 0.3f, this);
                playerHealth[3] = new Image("health1", healthUnitTexture,
                    new RectangleF(960, 680, 28, 29), 0.3f, this);
            }
            else if (room.Player_List[Player_Index].CurrentHealth == 5)
            {
                playerHealth[0] = new Image("health1", healthUnitTexture,
                    new RectangleF(965, 570, 24, 26), 0.3f, this);
                playerHealth[1] = new Image("health1", healthUnitTexture,
                    new RectangleF(965, 600, 24, 26), 0.3f, this);
                playerHealth[2] = new Image("health1", healthUnitTexture,
                    new RectangleF(965, 630, 24, 26), 0.3f, this);
                playerHealth[3] = new Image("health1", healthUnitTexture,
                    new RectangleF(965, 660, 24, 26), 0.3f, this);
                playerHealth[4] = new Image("health1", healthUnitTexture,
                    new RectangleF(965, 690, 24, 26), 0.3f, this);
            }
            #endregion

            #region Synchronize Deck
            if (room.owner_index == Player_Index)
            {
                //Load Deck
                deck = Card.LoadDeckFromXML(Content);
                //End card's data load
                deck.shuffle<CardDeck>();
                Command deckSyn = new Command(CommandCode.CardList_Synchronize, deck);
                sendDataToClient(deckSyn);
            }
            #endregion

            if (isHost())
            {
                room.Player_List[Player_Index].Turn.phase = Turn.Phase.Beginning;
            }
            #endregion

            #region Define Other Player Area
            float width = 100, height = 75;
            switch (room.Player_List.Count)
            {
                case 2:
                    oppPlayerRectangle[0, 0] = new RectangleF(435, 30, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(435, 105, width, height);
                    break;
                case 3:
                    oppPlayerRectangle[0, 0] = new RectangleF(180, 30, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(180, 105, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(690, 30, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(690, 105, width, height);
                    break;
                case 4:
                    oppPlayerRectangle[0, 0] = new RectangleF(20, 100, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(20, 175, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(435, 30, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(435, 105, width, height);
                    oppPlayerRectangle[2, 0] = new RectangleF(860, 100, width, height);
                    oppPlayerRectangle[2, 1] = new RectangleF(860, 175, width, height);
                    break;
                case 5:
                    oppPlayerRectangle[0, 0] = new RectangleF(20, 100, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(20, 175, width, height);
                    oppPlayerRectangle[0, 0] = new RectangleF(180, 30, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(180, 105, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(690, 30, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(690, 105, width, height);
                    oppPlayerRectangle[2, 0] = new RectangleF(860, 100, width, height);
                    oppPlayerRectangle[2, 1] = new RectangleF(860, 175, width, height);
                    break;
                case 6:
                    oppPlayerRectangle[0, 0] = new RectangleF(20, 100, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(70, 100, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(180, 30, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(230, 30, width, height);
                    oppPlayerRectangle[2, 0] = new RectangleF(435, 30, width, height);
                    oppPlayerRectangle[2, 1] = new RectangleF(485, 30, width, height);
                    oppPlayerRectangle[3, 0] = new RectangleF(690, 30, width, height);
                    oppPlayerRectangle[3, 1] = new RectangleF(740, 30, width, height);
                    oppPlayerRectangle[4, 0] = new RectangleF(860, 100, width, height);
                    oppPlayerRectangle[4, 1] = new RectangleF(910, 100, width, height);
                    break;
                case 7:
                    oppPlayerRectangle[0, 0] = new RectangleF(20, 100, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(70, 100, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(180, 30, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(230, 30, width, height);
                    oppPlayerRectangle[2, 0] = new RectangleF(350, 30, width, height);
                    oppPlayerRectangle[2, 1] = new RectangleF(400, 30, width, height);
                    oppPlayerRectangle[3, 0] = new RectangleF(520, 30, width, height);
                    oppPlayerRectangle[3, 1] = new RectangleF(570, 30, width, height);
                    oppPlayerRectangle[4, 0] = new RectangleF(690, 30, width, height);
                    oppPlayerRectangle[4, 1] = new RectangleF(740, 30, width, height);
                    oppPlayerRectangle[5, 0] = new RectangleF(860, 100, width, height);
                    oppPlayerRectangle[5, 1] = new RectangleF(910, 100, width, height);
                    break;
                case 8:
                    oppPlayerRectangle[0, 0] = new RectangleF(20, 320, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(70, 320, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(20, 100, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(70, 100, width, height);
                    oppPlayerRectangle[2, 0] = new RectangleF(180, 30, width, height);
                    oppPlayerRectangle[2, 1] = new RectangleF(230, 30, width, height);
                    oppPlayerRectangle[3, 0] = new RectangleF(435, 30, width, height);
                    oppPlayerRectangle[3, 1] = new RectangleF(485, 30, width, height);
                    oppPlayerRectangle[4, 0] = new RectangleF(690, 30, width, height);
                    oppPlayerRectangle[4, 1] = new RectangleF(740, 30, width, height);
                    oppPlayerRectangle[5, 0] = new RectangleF(860, 100, width, height);
                    oppPlayerRectangle[5, 1] = new RectangleF(910, 100, width, height);
                    oppPlayerRectangle[6, 0] = new RectangleF(860, 320, width, height);
                    oppPlayerRectangle[7, 1] = new RectangleF(910, 320, width, height);
                    break;
                case 9:
                    oppPlayerRectangle[0, 0] = new RectangleF(20, 320, width, height);
                    oppPlayerRectangle[0, 1] = new RectangleF(70, 320, width, height);
                    oppPlayerRectangle[1, 0] = new RectangleF(20, 100, width, height);
                    oppPlayerRectangle[1, 1] = new RectangleF(70, 100, width, height);
                    oppPlayerRectangle[2, 0] = new RectangleF(180, 30, width, height);
                    oppPlayerRectangle[2, 1] = new RectangleF(230, 30, width, height);
                    oppPlayerRectangle[3, 0] = new RectangleF(350, 30, width, height);
                    oppPlayerRectangle[3, 1] = new RectangleF(400, 30, width, height);
                    oppPlayerRectangle[4, 0] = new RectangleF(520, 30, width, height);
                    oppPlayerRectangle[4, 1] = new RectangleF(570, 30, width, height);
                    oppPlayerRectangle[5, 0] = new RectangleF(690, 30, width, height);
                    oppPlayerRectangle[5, 1] = new RectangleF(740, 30, width, height);
                    oppPlayerRectangle[6, 0] = new RectangleF(860, 100, width, height);
                    oppPlayerRectangle[6, 1] = new RectangleF(910, 100, width, height);
                    oppPlayerRectangle[7, 0] = new RectangleF(860, 320, width, height);
                    oppPlayerRectangle[7, 1] = new RectangleF(910, 320, width, height);
                    break;
                default:
                    break;
            }
            #endregion

            #region Draw other Area
            for (int i = 0, i2 = 0; i < room.Player_List.Count; i++, i2++)
            {
                if (i == Player_Index)
                {
                    i2--;
                    continue;
                }
                Label labelTemp = new Label("OtherPlayerNameLbel", Game1.font, room.Player_List[i].Player_name
                    , oppPlayerRectangle[i2, 0].X
                    , oppPlayerRectangle[i2, 0].Y - 20
                    , oppPlayerRectangle[i2, 1].Width
                    , Color.White, this);
                labelTemp.CenterAlign = true;
                labelTemp.Value = room.Player_List[i].id;
                OtherPlayerNameLabel.Add(labelTemp);

                Image masterTemp = new Image("Opp Master Image", GetTexture(room.Player_List[i].Character1.CharAsset),
                    oppPlayerRectangle[i2, 0], 0.3f, this);
                masterTemp.Source_Rectangle = new Rectangle(0, 0, characterBackTexture.Width, characterBackTexture.Height / 2);
                masterTemp.Scale.Y = 2;
                masterTemp.playerOwner = room.Player_List[i];
                //masterTemp.OnClick = new FormEventHandler(Character_OnClick);
                otherPlayerMasterImage.Add(masterTemp);

                /*Border temp = new Border("", Color.Red, 2, oppPlayerRectangle[i2, 0], this);
                System.Console.WriteLine(masterTemp.Rect.ToString());
                System.Console.WriteLine(masterTemp.Source_Rectangle.ToString());
                System.Console.WriteLine(temp.Rect.ToString());*/

                Image servantTemp = new Image("Opp Servant Image", GetTexture(room.Player_List[i].Character2.CharAsset),
                    oppPlayerRectangle[i2, 1], 0.3f, this);
                servantTemp.Source_Rectangle = new Rectangle(0, 0, characterBackTexture.Width, characterBackTexture.Height / 2);
                servantTemp.Scale.Y = 2;
                servantTemp.playerOwner = room.Player_List[i];
                //servantTemp.OnClick = new FormEventHandler(Character_OnClick);
                otherPlayerServantImage.Add(servantTemp);
            }
            #endregion
        }

        public override void End(Command command)
        {
            //End_Chat();
            EndSynch();
            End_Receive();
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
            for (int i = 0; i < OtherPlayerNameLabel.Count; i++)
            {
                Guid id = (Guid)OtherPlayerNameLabel[i].Value;
                OtherPlayerNameLabel[i].Text = room.Player_List[i].Player_name + " - " + room.findPlayerByID(id).HandCard.Count ;
            }
            resizeHand();
            updateRoom();
            checkHoverHandCard();
            checkHoverCharacter();
            base.Update(theTime);
        }
        #endregion

        #region Update's Function
        private void resizeHand()
        {
            if (Hand_Image_List.Count > 0)
            {
                //float net_width = (Hand_Image_List.Last().Rect.X2 - Hand_Image_List[0].Rect.X);
                float net_width = cardWidth * Hand_Image_List.Count;
                float oversize = net_width - handWitdh;
                float handOrder = 0.5f;
                if (oversize > 0)
                {
                    //padding = padding - (oversize / Hand_Image_List.Count);
                    padding = (oversize / (Hand_Image_List.Count - 1));
                    Hand_Image_List[0].Rect = new RectangleF(190, 567, cardWidth, cardHeight);
                    for (int i = 1; i < Hand_Image_List.Count; i++)
                    {
                        handOrder += 0.01f;
                        Hand_Image_List[i].Rect = new RectangleF
                            ((190 + (cardWidth * i)) - padding * i, 567, cardWidth, cardHeight);
                        if (i == hand_hovered_index) break;
                        Hand_Image_List[i].DrawOrder = handOrder;
                    }
                }
                else
                {
                    padding = 5;
                    Hand_Image_List[0].Rect = new RectangleF(190, 567, cardWidth, cardHeight);
                    for (int i = 1; i < Hand_Image_List.Count; i++)
                    {
                        handOrder += 0.01f;
                        Hand_Image_List[i].Rect = new RectangleF
                            (190 + (cardWidth + padding) * i, 567, cardWidth, cardHeight);
                        if (i == hand_hovered_index) break;
                        Hand_Image_List[i].DrawOrder = handOrder;
                        //hand_area_list[i] = new Rectangle(175 + (cardWidth + padding) * i, 567, cardWidth, cardHeight);
                    }
                }
            }
        }

        private void CharacterChange(Character character1, Character character2, Guid id)
        {
            int Index = room.findByID_ExcludeMainPlayer(id, Player_ID);
            int Player = room.findByID(id);
            room.Player_List[Player].Character1 = character1;
            room.Player_List[Player].Character2 = character2;

            UpdatePlayer(Index, Player);
        }

        private void UpdatePlayer(int index, int player)
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

        private Texture2D GetTexture(string asset)
        {
            return Content.Load<Texture2D>("Resource/character/" + asset);
        }

        private void updateRoom()
        {
            if (room.Player_List[Player_Index].Turn.phase == Turn.Phase.OtherPlayerTurn)
            {
                endButton.Visible = false;
                drawButton.Visible = false;
            }
            else
            {
                endButton.Visible = true;
                drawButton.Visible = true;
            }
            handStatistic.Text = "Hand: " + room.findPlayerByID(Player_ID).HandCard.Count();
            deckStatistic.Text = "Deck: " + deck.Count.ToString();
        }

        private void checkHoverHandCard()
        {
            if (hand_hovered_index == -1)
            {
                for (int i = Hand_Image_List.Count - 1; i >= 0; i--)
                {
                    if (main_game.mouse_hover(Hand_Image_List[i].Rect))
                    {
                        hand_hovered_index = i;

                        Hand_Image_List[hand_hovered_index].DrawOrder = 0.49f;
                        Hand_Image_List[hand_hovered_index].OnClick += CardImage_Onlick;
                        break;
                    }
                    hand_hovered_index = -1;
                }
            }
            else
            {
                if (!main_game.mouse_hover(Hand_Image_List[hand_hovered_index].Rect))
                {
                    for (int i = Hand_Image_List.Count - 1; i >= 0; i--)
                    {
                        if (main_game.mouse_hover(Hand_Image_List[i].Rect))
                        {
                            Hand_Image_List[hand_hovered_index].DrawOrder = 0.5f;
                            Hand_Image_List[hand_hovered_index].OnClick -= CardImage_Onlick;
                            //if (View_Detail_Button != null)
                            //    ClearDetailButtonAndImage();

                            hand_hovered_index = i;

                            Hand_Image_List[hand_hovered_index].DrawOrder = 0.49f;
                            Hand_Image_List[hand_hovered_index].OnClick += CardImage_Onlick;
                            break;
                        }
                    }
                    Hand_Image_List[hand_hovered_index].DrawOrder = 0.5f;
                    Hand_Image_List[hand_hovered_index].OnClick -= CardImage_Onlick;
                    //if (View_Detail_Button != null)
                    //    ClearDetailButtonAndImage();

                    hand_hovered_index = -1;
                }
            }
        }

        private void checkHoverCharacter()
        {
            if (characterHoverIndex == -1)
            {
                for (int i = otherPlayerMasterImage.Count - 1; i >= 0; i--)
                {
                    if (main_game.mouse_hover(otherPlayerMasterImage[i].Rect))
                    {
                        characterHoverIndex = i;
                        break;
                    }
                    characterHoverIndex = -1;
                }
                for (int i = otherPlayerServantImage.Count - 1; i >= 0; i--)
                {
                    if (main_game.mouse_hover(otherPlayerServantImage[i].Rect))
                    {
                        characterHoverIndex = i;
                        break;
                    }
                    characterHoverIndex = -1;
                }
            }
            else
            {
                if (!main_game.mouse_hover(otherPlayerMasterImage[characterHoverIndex].Rect) ||
                    !main_game.mouse_hover(otherPlayerServantImage[characterHoverIndex].Rect))
                {
                    if (Card_Detail_Image.Visible == true)
                    {
                        Card_Detail_Image.Visible = false;
                        cardStatic.Visible = true;
                        infoPanel.Visible = true;
                    }
                    characterHoverIndex = -1;
                }
            }
        }

        private String getStaticCard(Card card)
        {
            String s = "Name: " + card.CardName + "\n";
            s += "Number: " + card.CardNumber + "\n";
            s += "Suit: " + card.CardSuit + "\n";
            s += "Type: " + card.CardType + "\n";
            s += "Ability: " + card.CardDescription;
            return s;
        }

        Guid usingCardId, targetPlayerId;
        private void UseBasicCard(CardForm cardForm)
        {
            if (cardForm.CardDeck.Card.CardName == "ATTACK")
            {
                usingCardId = cardForm.CardDeck.CardId;
                foreach (Image item in otherPlayerMasterImage)
                {
                    item.OnClick += TargetPlayer_OnClick;
                }
                foreach (Image item in otherPlayerServantImage)
                {
                    item.OnClick += TargetPlayer_OnClick;
                }
            }
            //cardForm.MoveBySpeed(400, 200, 700);
        }

        private void DrawCard()
        {
            Player me = room.findPlayerByID(Player_ID);
            handOrder += 0.01f;
            //handList.Add(deck[0]);
            CardDeck cardDraw = deck.Last();
            Command draw = new Command(CommandCode.Draw_Card_Result, Player_ID, cardDraw);
            sendDataToClient(draw);
            me.HandCard.Add(cardDraw);
            deck.RemoveAt(deck.Count - 1);

            //if (handList.Count <= 5)
            //{
            CardForm card = new CardForm(me.HandCard.Last()
                    , new RectangleF(190 + (cardWidth + padding) * Hand_Image_List.Count, 567, cardWidth, cardHeight)
                    , handOrder, main_game.Content, this);
            //Image temp_image = new Image("", handList.Last().texture, new RectangleF(175 + (cardWidth + padding) * Hand_Image_List.Count, 567, cardWidth, cardHeight), 0.5f, this);

            Hand_Image_List.Add(card);
            //hand_area_list.Add(new Rectangle(175 + (cardWidth + padding) * hand_area_list.Count, 567, cardWidth, cardHeight));
            //}
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
            try
            {
                if (isHost())
                {
                    //Send command draw card to all clients
                    DrawCard();
                    resizeHand();
                }
                else
                {
                    //Send request draw card to host
                    Command draw = new Command(CommandCode.Draw_Card, Player_ID, 1);
                    sendDataToHost(draw);
                }
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        private void endButton_OnClick(object sender, FormEventData e)
        {
            if (isHost())
            {
                ChangeTurn(Player_Index);
            }
            else
            {
                Command endTurn = new Command(CommandCode.End_Turn, Player_Index);
                sendDataToHost(endTurn);
            }
        }

        private void UseCardClick(object sender, FormEventData e)
        {
            foreach (CardForm c in Hand_Image_List)
            {
                if (c.Border)
                {
                    c.Border = false;
                    //Hand_Image_List.Remove(c);
                    switch (c.CardDeck.Card.CardType)
                    {
                        case CardType.Tool:
                            //c.MoveBySpeed(400, 200, 700);
                            break;
                        case CardType.Weapon:
                            //c.MoveBySpeed(10, 567, 700);
                            break;
                        case CardType.PlusVehicle:
                            //c.MoveBySpeed(10, 567, 700);
                            break;
                        case CardType.MinusVehicle:
                            //c.MoveBySpeed(10, 567, 700);
                            break;
                        case CardType.Armour:
                            //c.MoveBySpeed(10, 567, 700);
                            break;
                        case CardType.Basic:
                            UseBasicCard(c);
                            break;
                        default:
                            break;
                    }
                    selected_card_index = -1;
                    ClearDetailButtonAndImage();
                    break;
                }
            }
            //CardForm card = Hand_Image_List[selected_card_index];
            //card.MoveBySpeed(0, 0, 300);
            //card.MoveBySecond(0, 0, 3);
            //Hand_Image_List.RemoveAt(selected_card_index);
        }

        private void TargetPlayer_OnClick(object sender, FormEventData e)
        {
            Image image = (Image)sender;
            Player player = image.playerOwner;
            targetPlayerId = player.id;
            if (!isHost())
            {
                Command c = new Command(CommandCode.Attack, usingCardId, Player_ID, targetPlayerId);
                sendDataToHost(c);
            }
            else
            {
                Command c = new Command(CommandCode.Attack, usingCardId, Player_ID, targetPlayerId);
                sendDataToClient(c);
                foreach (CardForm cf in Hand_Image_List)
                {
                    if (cf.CardDeck.CardId == usingCardId)
                    {
                        cf.MoveBySpeed(400, 200, 700);
                        Hand_Image_List.Remove(cf);
                        break;
                    }
                }
                //foreach (CardDeck cd in room.findPlayerByID(Player_ID).HandCard)
                 for (int i = 0; i < room.findPlayerByID(Player_ID).HandCard.Count; i++)
                {
                    if (room.findPlayerByID(Player_ID).HandCard[i].CardId == usingCardId)
                    {
                        room.findPlayerByID(Player_ID).HandCard.RemoveAt(i);
                        break;
                    }
                }
                chatDisplayTextbox.Text += room.findPlayerByID(Player_ID).Player_name
                    + " has attacked " + room.findPlayerByID(targetPlayerId).Player_name + "\n";
                //Command c2 = new Command(CommandCode.Update_Room, room);
                //sendDataToClient(c2);
            }

        }

        private void ChangeTurn(int playerEndTurn)
        {
            int NextPlayer;
            if (room.Player_List.Last() != room.Player_List[playerEndTurn]) NextPlayer = playerEndTurn + 1;
            else
            {
                NextPlayer = 0;
                if (!room.Player_List[NextPlayer].Status) NextPlayer++;
            }
            room.Player_List[playerEndTurn].Turn.phase = Turn.Phase.OtherPlayerTurn;
            room.Player_List[NextPlayer].Turn.phase = Turn.Phase.Beginning;
            Command changeTurn = new Command(CommandCode.Change_Turn, playerEndTurn, NextPlayer);
            sendDataToClient(changeTurn);
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

        bool Card_Clicked = false;
        ImageButton View_Detail_Button;
        private void CardImage_Onlick(object sender, FormEventData e)
        {
            if (!Card_Clicked)
            {
                CardForm image = (CardForm)sender;
                selected_card_index = hand_hovered_index;
                //View_Detail_Button = new ImageButton("view_detail_button"
                //    , view_detail_button_textture
                //    , new RectangleF(image.Rect.X, image.Rect.Y, 87, 20), this);
                //View_Detail_Button.DrawOrder = 0.48f;
                //View_Detail_Button.Value = image;
                //View_Detail_Button.OnClick += ViewDetailButton_Onclick;
                image.Border = true;
                useButton.Visible = true;
                discardButton.Visible = true;
                Card_Clicked = true;
            }
            else
            {
                MouseState ms = (MouseState)e.args;
                CardForm image = (CardForm)sender;
                //if (View_Detail_Button.Rect.Contains(new Point(ms.X, ms.Y))) return;
                //if (image.Rect.Contains(new Point(ms.X, ms.Y))) return;
                ClearDetailButtonAndImage();
            }
        }

        private void Character_OnClick(object sender, FormEventData e)
        {
            if (Card_Detail_Image.Visible == false)
            {
                Image image = (Image)(sender);
                Card_Detail_Image.Texture = image.Texture;
                Card_Detail_Image.Visible = true;
                cardStatic.Visible = true;
                infoPanel.Visible = true;
            }
        }

        private void ClearDetailButtonAndImage()
        {
            //View_Detail_Button.Delete();
            //Card_Detail_Image.Visible = false;
            //cardStatic.Visible = false;
            //infoPanel.Visible = false;
            if (selected_card_index > -1) Hand_Image_List[selected_card_index].Border = false;
            selected_card_index = -1;
            Card_Clicked = false;
            discardButton.Visible = false;
            useButton.Visible = false;
        }

        private void ViewDetailButton_Onclick(object sender, FormEventData e)
        {
            CardForm image = (CardForm)((ImageButton)sender).Value;
            Card_Detail_Image.Texture = image.Texture;
            Card_Detail_Image.Visible = true;
            cardStatic.Text = getStaticCard(image.CardDeck.Card);
            cardStatic.Visible = true;
            infoPanel.Visible = true;
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, rz);
            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            /*if (hand_hovered_index == -1)
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
                        spriteBatch.Draw(handList[i].texture, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.301f);
                        spriteBatch.Draw(handList[i].texture, hand_area_list[i], null, new Color(0xFF, 0xFF, 0xFF, 0xCC), 0f, new Vector2(0, 0), SpriteEffects.None, 0.3f);
                    }
                    else
                        spriteBatch.Draw(handList[i].texture, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f - i * 0.001f);
                }
            }*/
            base.Draw(graphics, spriteBatch, gameTime);
            //spriteBatch.Draw(card_list[0].texture, player_card_area, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
        #endregion
    }
}
