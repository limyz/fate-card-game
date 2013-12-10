﻿using System;
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
        SpriteFont font;
        Background bg;
        ImageButton back_button, ok_button, host_button;
        Image saber, gameList;
        #endregion

        #region receiver Thread
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

        private void Receiver()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 51001);

            while (true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                BinaryFormatter bin = new BinaryFormatter();
                MemoryStream mem = new MemoryStream(data);
                Room room = (Room)bin.Deserialize(mem);

                String s = "Owner index: " + room.owner_index + "\n";
                s += "Player List Count: " + room.Player_List.Count + "\n";
                s += "Room name: " + room.Room_name + "\n";
                s += "Number of Player: " + room.Number_of_Player + "\n";
                s += "Player List:\n";
                foreach (Player p in room.Player_List)
                {
                    s += "+ " + p.Player_name + " - " + p.Address + "\n";
                }
                Game1.MessageBox(new IntPtr(0), s, endPoint.Address.ToString() + ":" + endPoint.Port, 0);
            }
        }
        public void End_Receive()
        {
            receivingThread.Abort();
            receivingClient.Close();
        }
        #endregion

        #region load content
        public JoinScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("JoinScreen",theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");
            bg = new Background(Content.Load<Texture2D>("Resource/background"),this);

            saber = new Image("saber"
                , Content.Load<Texture2D>("Resource/SaberLily_Trans")
                , new Rectangle(720, 110, 480, 615), 0.99f, this);

            gameList = new Image("gameList"
                , Content.Load<Texture2D>("Resource/gamelist")
                , new Rectangle(50, 50, 700, 650), 0.97f, this);

            ok_button = new ImageButton("Ok"
                , Content.Load<Texture2D>("Resource/ok_button")
                , new Rectangle(170, 650, 120, 42), this);

            host_button = new ImageButton("Host"
                , Content.Load<Texture2D>("Resource/host_button")
                , new Rectangle(320, 650, 120, 42), this);
            host_button.OnClick += HostClicked;

            back_button = new ImageButton("Back"
                , Content.Load<Texture2D>("Resource/back_button")
                , new Rectangle(470, 650, 120, 42), this);
            back_button.OnClick += BackClicked;

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
