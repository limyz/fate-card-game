using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using System.Threading;
using System.Net.Sockets;
using System.Net;


namespace WindowsGame1
{
    public class CharacterSelectScreen : Screen
    {
        #region Variable Declaration\
        public Room room;
        public Guid Player_ID;
        int Player_Index;
        Player mainPlayer;
        Background bg;
        ContentManager Content;
        XmlDocument xml = new XmlDocument();
        Random rand = new Random();
        Border[] charSelectBorder = new Border[7];
        int[] playerRandomChar = new int[2];
        List<Character> characterList = new List<Character>();
        List<Character> characterPlayerList = new List<Character>();
        Image[] characterImage = new Image[7];
        ImageButton okButton;
        Label roomInfoLabel;
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
                        tcpServerClient[i].GetStream().Write(data, 0, data.Length);
                    }
                    //couldn't send message because the client has disconnected
                    //so we remove that tcp client and player from the list
                    catch
                    {
                        string s = room.Player_List[i].Player_name + " had left the room!" + "\n";
                        this.tcpServerClient.RemoveAt(i);
                        this.room.Player_List.RemoveAt(i);
                        i--;
                        UpdateRoom();
                        //room.Player_List[i].Status = false;
                        //UpdateRoom(room.findByID_ExcludeMainPlayer(room.Player_List[i].id, Player_ID), i);
                        update_room = true;
                        //SendChatMessage(s);
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

        public void EndSynch()
        {
            synchronizeRun = false;
            foreach (TcpClient tcpclient in tcpServerClient)
            {
                tcpclient.Close();
            }
            tcpServerClient.Clear();
            //if (isHost()) joinResponseThread.Abort();
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
                            //SendChatMessage(c.Message);
                        }
                        else if (c.Command_Code == CommandCode.Update_Room)
                        {
                            //Room room = (Room)c.Data1;
                            //int Player_Index = room.findByID(Player_ID);
                            //for (i = 0; i < room.Player_List.Count; i++)
                            //{
                            //    if (i == Player_Index) continue;
                            //    if (!room.Player_List[i].Status)
                            //    {
                            //        //UpdateRoom(room.findByID_ExcludeMainPlayer(room.Player_List[i].id, Player_ID), Player_Index);
                            //    }
                            //}
                        }
                        else if (c.Command_Code == CommandCode.Character_Select)
                        {
                            Character character1 = (Character)c.Data1;
                            Character character2 = (Character)c.Data2;
                            Guid id = (Guid)c.Data3;

                            int playerIndex = room.findByID(id);
                            room.Player_List[playerIndex].Character1 = character1;
                            room.Player_List[playerIndex].Character2 = character2;
                            room.Player_List[playerIndex].Status = true;
                            UpdateRoom();
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
                                catch { }
                            }
                            CheckStartGame();
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
                    try
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
                            //else if (c.Command_Code == CommandCode.Chat_Message)
                            //{
                            //    chatDisplayTextbox.Text += c.Message;
                            //}
                            else if (c.Command_Code == CommandCode.Update_Room)
                            {
                                this.room = (Room)c.Data1;
                                UpdateRoom();
                            }
                            else if (c.Command_Code == CommandCode.Start_Game)
                            {
                                room = (Room)c.Data1;
                                ScreenEvent.Invoke(this, new SivEventArgs(1));
                            }
                            else if (c.Command_Code == CommandCode.Character_Distribute)
                            {
                                characterPlayerList = (List<Character>)c.Data1;
                                UpdateSelectImageList();
                            }
                        }
                    }
                    catch
                    {
                        ScreenEvent.Invoke(this, new SivEventArgs(0));
                        Game1.MessageBox(new IntPtr(0), "Connection Error!", "Connection Error!", 0);
                    }
                }
                else if ((DateTime.Now - LastReceiveTimeFromHost) > new TimeSpan(0, 0, 3))
                {
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                    Game1.MessageBox(new IntPtr(0), "Host has left the game", "Host has left the game", 0);
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
        public CharacterSelectScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("JoinScreen", theEvent, parent)
        {
            #region Load Resource
            bg = new Background(Content.Load<Texture2D>("Resource/graphic/CharSelectBG"), this);
            this.Content = Content;
            #endregion

            #region Character Select
            charSelectBorder[0] = new Border("Char 1 Border", Color.Black, 2,
                new Rectangle(100, 500, 149, 208), this);
            charSelectBorder[1] = new Border("Char 2 Border", Color.Black, 2,
                new Rectangle(250, 500, 149, 208), this);
            charSelectBorder[2] = new Border("Char 3 Border", Color.Black, 2,
                new Rectangle(400, 500, 149, 208), this);
            charSelectBorder[3] = new Border("Char 4 Border", Color.Black, 2,
                new Rectangle(550, 500, 149, 208), this);
            charSelectBorder[4] = new Border("Char 5 Border", Color.Black, 2,
                new Rectangle(700, 500, 149, 208), this);
            charSelectBorder[5] = new Border("Char 6 Border", Color.Black, 2,
                new Rectangle(850, 500, 149, 208), this);
            charSelectBorder[6] = new Border("Char 7 Border", Color.Black, 2,
                new Rectangle(1000, 500, 149, 208), this);

            characterImage[0] = new Image("Char 1 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(102, 502, 147, 206), 0.3f, this);
            characterImage[1] = new Image("Char 2 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(252, 502, 147, 206), 0.3f, this);
            characterImage[2] = new Image("Char 3 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(402, 502, 147, 206), 0.3f, this);
            characterImage[3] = new Image("Char 4 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(552, 502, 147, 206), 0.3f, this);
            characterImage[4] = new Image("Char 5 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(702, 502, 147, 206), 0.3f, this);
            characterImage[5] = new Image("Char 6 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(852, 502, 147, 206), 0.3f, this);
            characterImage[6] = new Image("Char 7 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(1002, 502, 147, 206), 0.3f, this);
            #endregion

            roomInfoLabel = new Label("Room Info", Game1.arial12Bold, "Test Test Test", 50, 50, 1000, Color.White, this);

            #region Button
            okButton = new ImageButton("OK Button", Content.Load<Texture2D>("Resource/ok_button"),
                new Rectangle(500, 400, 180, 70), this);
            //okButton = new ImageButton("Cancel Button", Content.Load<Texture2D>("Resource/ok_button"),
            //    new Rectangle(500, 450, 180, 70), this);
            #endregion

            #region inGameScreen_RegisterHandler
            OnKeysDown += CharacterSelectScreen_OnKeysDown;

            foreach (var img in characterImage)
            {
                img.OnClick += new FormEventHandler(CharacterClick);
            }

            okButton.OnClick += new FormEventHandler(OkClick);
            #endregion

            #region XML loading
            //Character's data load
            xml.Load("Data/Character.xml");
            XmlNodeList xml_master_list = xml.GetElementsByTagName("Master")[0].ChildNodes;
            XmlNodeList xml_servant_list = xml.GetElementsByTagName("Servant")[0].ChildNodes;
            for (int i = 0; i < xml_master_list.Count; i++)
            {
                XmlElement temp = (XmlElement)xml_master_list[i];
                Character charTemp = new Character(
                    xml_master_list[i].InnerText,
                    temp.GetAttribute("img"),
                    Character.Type.Master);
                characterList.Add(charTemp);

            }
            for (int i = 0; i < xml_servant_list.Count; i++)
            {
                XmlElement temp = (XmlElement)xml_servant_list[i];
                Character charTemp = new Character(
                    xml_servant_list[i].InnerText,
                    temp.GetAttribute("img"),
                    Character.Type.Servant);
                characterList.Add(charTemp);
            }
            //End character's data load
            #endregion
        }

        #endregion

        #region Start-End Game
        public override void Start(Command command)
        {
            Player_Index = room.findByID(Player_ID);
            InitializeReceiver();
            StartSynch();

            foreach (var p in room.Player_List)
            {
                p.Status = false;
            }
            CharacterDistribute();
            mainPlayer = new Player("test player", "0.0.0.0");
            UpdateRoom();
        }

        public override void End(Command command)
        {
            End_Receive();
            EndSynch();
            this.room = null;
        }
        #endregion

        #region Update's Function
        private Texture2D GetTexture(string asset)
        {
            return Content.Load<Texture2D>("Resource/character/" + asset);
        }

        private void UpdateSelectImageList()
        {
            for (int i = 0; i < 7; i++)
            {
                characterImage[i].Texture = GetTexture(characterPlayerList[i].CharAsset);
            }
        }

        private void UpdateRoom()
        {
            String s = "Owner index: " + room.owner_index + "\n";
            //s += "Main Player: " + room.Player_List[Player_Index].Player_name + "\n";
            s += "Player List Count: " + room.Player_List.Count + "\n";
            s += "Room name: " + room.Room_name + "\n";
            s += "Number of Player: " + room.Number_of_Player + "\n";
            s += "Player List:\n";
            foreach (Player p in room.Player_List)
            {
                s += "+ " + p.Player_name + " - " + p.Address + "-" + p.Status + "\n";
            }
            roomInfoLabel.Text = s;
        }
        #endregion

        #region Handler
        private void CharacterSelectScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }

        private void OkClick(object sender, FormEventData e)
        {
            int count = CountSelectChar();
            if (count < 2)
            {
                Game1.MessageBox(new IntPtr(0), "Please select 2 Characters", "Warning!", 0);
            }
            else if (count == 2)
            {
                string s = "";
                List<Character> listTemp = new List<Character>();
                for (int i = 0; i < 7; i++)
                {
                    if (charSelectBorder[i].Width == 4)
                    {
                        listTemp.Add(characterPlayerList[i]);
                        s += characterPlayerList[i].CharName + "\n";
                    }
                }
                if (listTemp[0].CharType == listTemp[1].CharType)
                {
                    Game1.MessageBox(new IntPtr(0), "Please select 2 Characters with different type", "Warning!", 0);
                }
                else
                {
                    //Game1.MessageBox(new IntPtr(0), s, "Your Character", 0);
                    if (listTemp[0].CharType == Character.Type.Servant)
                    {
                        room.Player_List[Player_Index].Character1 = listTemp[1];
                        room.Player_List[Player_Index].Character2 = listTemp[0];
                    }
                    else
                    {
                        room.Player_List[Player_Index].Character1 = listTemp[0];
                        room.Player_List[Player_Index].Character2 = listTemp[1];
                    }
                    SelectCharacter();
                    for (int i = 0; i < 7; i++)
                    {
                        if (charSelectBorder[i].Width == 2)
                        {
                            charSelectBorder[i].Delete();
                            characterImage[i].Delete();
                        }
                        if (charSelectBorder[i].Width == 4)
                        {
                            characterImage[i].OnClick = null;
                        }
                    }
                    okButton.Delete();
                }
            }
        }

        private void CharacterClick(object sender, FormEventData e)
        {
            Image image = (Image)sender;
            for (int i = 0; i < characterImage.Count(); i++)
            {
                if (characterImage[i].Name == image.Name)
                {
                    if (charSelectBorder[i].Width == 2 && charSelectBorder[i].color == Color.Black)
                    {
                        int count = CountSelectChar();
                        if (count < 2)
                        {
                            charSelectBorder[i].Width = 4;
                            charSelectBorder[i].color = Color.BlueViolet;
                        }
                    }
                    else if (charSelectBorder[i].Width == 4 && charSelectBorder[i].color == Color.BlueViolet)
                    {
                        charSelectBorder[i].Width = 2;
                        charSelectBorder[i].color = Color.Black;
                    }
                }
            }

        }

        private int CountSelectChar()
        {
            int count = 0;
            for (int i = 0; i < 7; i++)
            {
                if (charSelectBorder[i].Width == 4) count++;
            }
            return count;
        }

        private void SelectCharacter()
        {
            NetworkStream networkStream2;
            TcpClient tcpClient2;
            if (isHost())
            {
                try
                {
                    int Player_Index = room.findByID(Player_ID);
                    room.Player_List[Player_Index].Status = true;
                    Command c = new Command(CommandCode.Update_Room, this.room);
                    byte[] data2 = c.Serialize();
                    for (int i = 0; i < tcpServerClient.Count; i++)
                    {
                        if (i == Player_Index) continue;
                        tcpServerClient[i].GetStream().Write(data2, 0, data2.Length);
                    }
                    UpdateRoom();
                    CheckStartGame();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                try
                {
                    int Player_Index = room.findByID(Player_ID);
                    Command c = new Command(CommandCode.Character_Select, room.Player_List[Player_Index].Character1,
                        room.Player_List[Player_Index].Character2, this.Player_ID);
                    byte[] data = c.Serialize();
                    tcpClient2 = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                    networkStream2 = tcpClient2.GetStream();
                    networkStream2.Write(data, 0, data.Length);
                    tcpClient2.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void CheckStartGame()
        {
            foreach (var p in room.Player_List)
            {
                if (!p.Status) return;
            }
            try
            {
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

        private void CharacterDistribute()
        {
            for (int i = 0; i < (tcpServerClient.Count); i++)
            {
                List<Character> listTemp = new List<Character>();
                bool flag = true;
                while (flag)
                {
                    listTemp.Clear();
                    for (int j = 0; j < 7; j++)
                    {
                        playerRandomChar[0] = rand.Next(characterList.Count);
                        listTemp.Add(characterList[playerRandomChar[0]]);
                        characterList.RemoveAt(playerRandomChar[0]);
                    }
                    for (int j = 0; j < (listTemp.Count - 1); j++)
                    {
                        if (listTemp[j].CharType != listTemp[j + 1].CharType)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        for (int j = 0; j < (listTemp.Count - 1); j++)
                        {
                            characterList.Add(listTemp[j]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                try
                {
                    Command c = new Command(CommandCode.Character_Distribute, listTemp);
                    byte[] data = c.Serialize();
                    if (i == Player_Index)
                    {
                        characterPlayerList = listTemp;
                        UpdateSelectImageList();
                    }
                    else tcpServerClient[i].GetStream().Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
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
