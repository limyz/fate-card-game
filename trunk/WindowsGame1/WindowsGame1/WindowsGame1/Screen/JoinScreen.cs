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
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsGame1
{
    public class JoinScreen : Screen
    {
        #region variable decleration
        Background bg;
        ImageButton back_button, ok_button, host_button;
        Image saber, gameList;

        List<Room> List_Room = new List<Room>();
        List<DateTime> List_Room_Recieve_Time = new List<DateTime>();
        List<Label> List_Room_Info = new List<Label>();
        TextBox playerName;
        Label playerNameLabel;
        #endregion

        #region UDP receiver Thread
        UdpClient receivingClient;
        Thread receivingThread;

        public void InitializeReceiver()
        {
            receivingClient = new UdpClient(51001);
            receivingClient.EnableBroadcast = true;

            ThreadStart start = new ThreadStart(Receiver);
            receivingThread = new Thread(start);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        private void Check_Room_Existed(Room room, IPEndPoint endPoint)
        {
            for (int i = 0; i < List_Room.Count; i++)
            {
                if (List_Room[i].Player_List[List_Room[i].owner_index].Address == endPoint.Address.ToString())
                {
                    room.Player_List[room.owner_index].Address = endPoint.Address.ToString();
                    List_Room[i] = room;
                    List_Room_Recieve_Time[i] = DateTime.Now;

                    String s = List_Room[i].Room_name + " (" + List_Room[i].Player_List.Count;
                    s += "/" + List_Room[i].Number_of_Player + ") - ";
                    s += List_Room[i].Player_List[List_Room[i].owner_index].Address;
                    List_Room_Info[i].Text = s;

                    return;
                }
            }
            room.Player_List[room.owner_index].Address = endPoint.Address.ToString();
            List_Room.Add(room);
            List_Room_Recieve_Time.Add(DateTime.Now);

            String s2 = room.Room_name + " (" + room.Player_List.Count + "/";
            s2 += room.Number_of_Player + ") - ";
            s2 += room.Player_List[room.owner_index].Address;
            int i2 = List_Room.Count - 1;

            Label l = new Label(i2.ToString(), Game1.font, s2, 80, 165 + 20 * i2, 680
                , Color.White, this);
            l.Value = i2;
            l.OnClick = RoomClicked;
            l.OnMouseEnter = RoomEntered;
            l.OnMouseLeave = RoomLeft;
            List_Room_Info.Add(l);
        }

        private void Receiver()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Any, 51001);

                while (true)
                {
                    byte[] data = receivingClient.Receive(ref endPoint);
                    BinaryFormatter bin = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream(data);
                    Room room = (Room)bin.Deserialize(mem);
                    //Game1.MessageBox(new IntPtr(0), "here", "", 0);
                    this.Check_Room_Existed(room, endPoint);
                }
            }
            catch (Exception ex)
            {
                //Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        public void End_Receive()
        {
            try
            {
                receivingThread.Abort();
                receivingClient.Close();
            }
            catch (Exception ex)
            {
                //Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        #endregion

        #region load content
        public JoinScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("JoinScreen", theEvent, parent)
        {
            bg = new Background(Content.Load<Texture2D>("Resource/background"), this);

            #region Image
            saber = new Image("saber"
                , Content.Load<Texture2D>("Resource/SaberLily_Trans")
                , new RectangleF(720, 110, 480, 615), 0.99f, this);

            gameList = new Image("gameList"
                , Content.Load<Texture2D>("Resource/gamelist")
                , new RectangleF(50, 50, 700, 650), 0.97f, this);
            #endregion

            #region Button
            ok_button = new ImageButton("Ok"
                , Content.Load<Texture2D>("Resource/button/ok_button")
                , new RectangleF(170, 650, 120, 42), this);

            host_button = new ImageButton("Host"
                , Content.Load<Texture2D>("Resource/button/host_button")
                , new RectangleF(320, 650, 120, 42), this);
            host_button.OnClick += HostClicked;

            back_button = new ImageButton("Back"
                , Content.Load<Texture2D>("Resource/button/back_button")
                , new RectangleF(470, 650, 120, 42), this);
            back_button.OnClick += BackClicked;
            #endregion

            playerName = new TextBox("Player Name", Game1.whiteTextbox, Game1.highlightedTextbox,
                Game1.caret, Game1.font, new RectangleF(600, 65, 150, 20), this);
            playerNameLabel = new Label("PlayerName Label", Game1.arial13Bold, "Name: ",
                540, 65, 100, Color.White, this);
            playerName.Text = "SivCloud";

            #region JoinScreen_RegisterHandler
            OnKeysDown += JoinScreen_OnKeysDown;
            #endregion
        }
        #endregion

        #region HANDLER
        private void JoinScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }
        private void BackClicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(0));
            return;
        }
        private void HostClicked(object sender, FormEventData e)
        {
            ScreenEvent.Invoke(this, new SivEventArgs(2));
            return;
        }
        private void RoomClicked(object sender, FormEventData e)
        {
            End_Receive();
            Label label = (Label)sender;
            int value = (int)label.Value;
            Room room = List_Room[value];

            this.RequestJoin(room);
        }
        private void RoomEntered(object sender, FormEventData e)
        {
            Label label = (Label)sender;
            label.Color = Color.Red;
        }
        private void RoomLeft(object sender, FormEventData e)
        {
            Label label = (Label)sender;
            label.Color = Color.White;
        }
        #endregion

        #region Update's function
        private void Update_Room_List()
        {
            for (int i = 0; i < List_Room_Recieve_Time.Count; i++)
            {
                if ((DateTime.Now - List_Room_Recieve_Time[i]) > new TimeSpan(0, 0, 3))
                {
                    List_Room.RemoveAt(i);
                    List_Room_Recieve_Time.RemoveAt(i);
                    List_Room_Info[i].Delete();
                    List_Room_Info.RemoveAt(i);

                    for (int i2 = i; i2 < List_Room_Info.Count; i2++)
                    {
                        List_Room_Info[i2].Name = i2.ToString();
                        List_Room_Info[i2].Rect.Y = 165 + i2 * 20;
                        List_Room_Info[i2].Value = i2;
                    }

                    i--;
                }
            }

        }
        NetworkStream networkStream;
        TcpClient tcpClient;
        private void RequestJoin(Room room)
        {
            Command c = new Command(CommandCode.Can_I_Join);
            byte[] data = c.Serialize();

            try
            {
                tcpClient = new TcpClient(room.Player_List[room.owner_index].Address, 51002);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);

                Byte[] bytes = new Byte[1024 * 16];
                DateTime dt = DateTime.Now;
                while (true)
                {
                    int i;
                    while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Command c2 = new Command(bytes);
                        if (c2.Command_Code == CommandCode.OK_to_Join)
                        {
                            tcpClient.Close();
                            Player player = new Player(playerName.Text, Game1.getLocalIP());
                            room.Player_List.Add(player);
                            ScreenEvent.Invoke(this, new SivEventArgs(4, room));
                            return;
                        }
                        else if (c2.Command_Code == CommandCode.Cant_Join_Room_Full)
                        {
                            tcpClient.Close();
                            Game1.MessageBox(new IntPtr(0), "Can't join, the room is full.", "Can not Join", 0);
                            return;
                        }
                    }
                    if ((DateTime.Now - dt) > new TimeSpan(0, 0, 3))
                    {
                        tcpClient.Close();
                        Game1.MessageBox(new IntPtr(0), "Host is not responding", "Can not Join", 0);
                        return;
                    }
                }
            }
            catch
            {
                Game1.MessageBox(new IntPtr(0), "Host is not responding", "Can not Join", 0);
            }
        }
        #endregion

        #region update
        public override void Update(GameTime theTime)
        {
            Update_Room_List();

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
