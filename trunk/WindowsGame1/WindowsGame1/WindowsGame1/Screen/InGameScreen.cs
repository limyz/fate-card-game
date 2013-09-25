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
    class InGameScreen : Screen
    {
        #region chat_stuff
        delegate void AddMessage(string message);

        string userName="";

        const int port = 54545;
        string broadcastAddress = "255.255.255.255";

        UdpClient receivingClient;
        UdpClient sendingClient;

        Thread receivingThread;

        public void SetUsername()
        {
            userName = textbox_username.Text.Replace("\r\n","");
        }

        public void SetIP()
        {
            if (string.IsNullOrEmpty(textbox_IP.Text))
            {
                broadcastAddress = "255.255.255.255";
            }
            else
            {
                broadcastAddress = textbox_IP.Text.Replace("\r\n", "");
            }
        }
        
        private void InitializeSender()
        {            
            sendingClient = new UdpClient();
            sendingClient.EnableBroadcast = true;
        }

        private void InitializeReceiver()
        {
            //NAT.ForwardPort(port, ProtocolType.Udp, "blah blah");
            //NAT.Discover();
            
            receivingClient = new UdpClient(port);

            ThreadStart start = new ThreadStart(Receiver);
            receivingThread = new Thread(start);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        private void Receiver()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            AddMessage messageDelegate = MessageReceived;

            while (true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                string message = Encoding.Unicode.GetString(data);
                messageDelegate.Invoke(message);
                //Invoke(messageDelegate, message);
            }
        }

        private void MessageReceived(string message)
        {
            textbox_chat_show.Text += (message + "\r\n");
        }


        #endregion

        #region variable decleration
        int card_widht = 105;
        int card_height = 150;
        int hand_witdh = 535;
        int padding = 5;

        SpriteFont font;
        Texture2D test_img;                        
        Random rand = new Random();

        int opponent_number = 5;
        Texture2D border_texture, char_card_texture;
        Rectangle[,] opponent_area;

        Rectangle player_control_area = new Rectangle(0, 570, 1000, 150);
        Texture2D player_control_texture;

        Rectangle chat_screen_area = new Rectangle(970, 0, 230, 330);
        Texture2D chat_screen_texture;
        Rectangle chat_input_area = new Rectangle(970, 330, 230, 220);
        Texture2D chat_input_texture;

        XmlDocument xml = new XmlDocument();
        Character[] master_list;
        Character[] servant_list;
        List<Card> card_list = new List<Card>();
        List<Card> hand_list = new List<Card>();
        int hand_hovered_index = -1;

        Rectangle[] player_char_area = new Rectangle[2];
        int[] player_random_char = new int[2];
        List<Rectangle> hand_area_list = new List<Rectangle>();
        //Rectangle player_card_area = new Rectangle(175,570,105,150);
        ImageButton draw_button;
        TextBox chat_textbox;
        Label label_username;
        Label label_IP;
        TextBox textbox_username;
        TextBox textbox_IP;
        TextBox textbox_chat_show;
        #endregion        
        
        #region Character class
        public class Character
        {
            private ContentManager Content;
            public string char_tag;
            public string char_name;
            public string char_asset;
            public Texture2D char_texture;
            public enum char_type
            {
                master = 0,
                servant = 1
            };
            char_type type;
            public Character(ContentManager Content, string char_tag, string char_name, string char_asset, char_type type)
            {
                this.Content = Content;
                this.char_tag = char_tag;
                this.char_name = char_name;
                this.char_asset = char_asset;
                this.type = type;
            }
            public void load_texture()
            {
                char_texture = Content.Load<Texture2D>("Resource/character/" + char_asset);
            }
        }
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

        #region Load content
        public InGameScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theScreenEvent, Game1 parent)
            : base("InGameScreen",theScreenEvent,parent)
        {
            //parent.keyboard_dispatcher = new KeyboardDispatcher(main_game.Window);
            //test_img = Content.Load<Texture2D>("Resource/Test");                      

            opponent_area = new Rectangle[opponent_number, 2];
            opponent_area[0, 0] = new Rectangle(30, 320, 100, 150);
            opponent_area[0, 1] = new Rectangle(130, 320, 100, 150);
            opponent_area[1, 0] = new Rectangle(50, 100, 100, 150);
            opponent_area[1, 1] = new Rectangle(150, 100, 100, 150);
            opponent_area[2, 0] = new Rectangle(370, 40, 100, 150);
            opponent_area[2, 1] = new Rectangle(470, 40, 100, 150);
            opponent_area[3, 0] = new Rectangle(710, 100, 100, 150);
            opponent_area[3, 1] = new Rectangle(810, 100, 100, 150);
            opponent_area[4, 0] = new Rectangle(730, 320, 100, 150);
            opponent_area[4, 1] = new Rectangle(830, 320, 100, 150);

            player_char_area[0] = new Rectangle(734, 570, 105, 150);
            player_char_area[1] = new Rectangle(845, 570, 105, 150);

            //content loading code here

            font = Content.Load<SpriteFont>("SpriteFont1");

            border_texture = Content.Load<Texture2D>("Resource/Untitled-1");
            char_card_texture = Content.Load<Texture2D>("Resource/character_back");

            player_control_texture = Content.Load<Texture2D>("Resource/controlplayer");
            chat_screen_texture = Content.Load<Texture2D>("Resource/chat_screen");
            chat_input_texture = Content.Load<Texture2D>("Resource/chat_input");

            draw_button = new ImageButton("draw_button", Content.Load<Texture2D>("Resource/draw")
                , new Rectangle(740, 520, 130, 35), 0.5f, this);
            draw_button.OnClick += draw_button_clicked;

            Texture2D white_textbox = Content.Load<Texture2D>("Resource/white_textbox");
            Texture2D highlighted_textbox = Content.Load<Texture2D>("Resource/Highlighted_textbox");
            Texture2D caret = Content.Load<Texture2D>("Resource/caret");
            Texture2D scrollbarBackground = Content.Load<Texture2D>("Resource/ScrollbarBackground");
            Texture2D scrollbar = Content.Load<Texture2D>("Resource/Scrollbar");

            chat_textbox = new TextBox("chat_textbox"
                , white_textbox, highlighted_textbox, caret
                , scrollbarBackground, scrollbar, font
                , new Rectangle(979, 345, 219, 200), this);
            chat_textbox.scrollable = true;
            chat_textbox.OnEnterPressed += chat_textbox_onEnterPressed;
            chat_textbox.OnShift_EnterPressed += textbox_onShiftEnterPressed;

            label_username = new Label("label_username", font, "Username"
                , 1010, 550, 1198 - 1010, Color.White, this);

            label_IP = new Label("label_IP", font, "IP Address"
                , 1010, 630, 1198 - 1010, Color.White, this);

            textbox_username = new TextBox("textbox_username"
                , white_textbox, highlighted_textbox, caret
                , font , new Rectangle(1010, 570, 188, 20), this);
            textbox_username.Text = "Siv";

            textbox_IP = new TextBox("textbox_IP"
                , white_textbox, highlighted_textbox, caret
                , font , new Rectangle(1010, 650, 188, 20), this);
            textbox_IP.Text = "255.255.255.255";

            textbox_chat_show = new TextBox("textbox_chat_show"
                , white_textbox, highlighted_textbox, caret
                , scrollbarBackground, scrollbar, font
                , new Rectangle(979, 10, 219, 312), this);
            textbox_chat_show.scrollable = true;
            textbox_chat_show.OnClick = null;              

            InitializeSender();
            InitializeReceiver();

            #region inGameScreen_RegisterHandler
            OnKeysDown += InGameScreen_OnKeysDown;
            #endregion
            #region XML loading
            //Character's data load
            xml.Load("Data/Character.xml");
            XmlNodeList xml_master_list = xml.GetElementsByTagName("Master")[0].ChildNodes;
            XmlNodeList xml_servant_list = xml.GetElementsByTagName("Servant")[0].ChildNodes;
            //MessageBox(new IntPtr(0), xml_master_list.Count.ToString(), "", 0);
            master_list = new Character[xml_master_list.Count];
            servant_list = new Character[xml_servant_list.Count];
            for (int i = 0; i < master_list.Length; i++)
            {
                XmlElement temp = (XmlElement)xml_master_list[i];
                master_list[i] = new Character(Content,
                    xml_master_list[i].Name,
                    xml_master_list[i].InnerText,
                    temp.GetAttribute("img"),
                    Character.char_type.master);
                //MessageBox(new IntPtr(0), master_list[i].char_asset, "", 0);
                master_list[i].load_texture();
            }
            for (int i = 0; i < servant_list.Length; i++)
            {
                XmlElement temp = (XmlElement)xml_servant_list[i];
                servant_list[i] = new Character(Content,
                    xml_servant_list[i].Name,
                    xml_servant_list[i].InnerText,
                    temp.GetAttribute("img"),
                    Character.char_type.servant);
                servant_list[i].load_texture();
            }
            //End character's data load

            player_random_char[0] = rand.Next(master_list.Length);
            player_random_char[1] = rand.Next(servant_list.Length);

            //Card's data load            
            xml.Load("Data/Card.xml");
            XmlNodeList xml_card_list = xml.GetElementsByTagName("Card")[0].ChildNodes;
            //card_list = new Card[xml_card_list.Count];
            for (int i = 0; i < xml_card_list.Count; i++)
            {
                XmlElement temp = (XmlElement)xml_card_list[i];
                card_list.Add(new Card(Content,
                    xml_card_list[i].Name,
                    xml_card_list[i].InnerText,
                    temp.GetAttribute("img")));
                card_list[i].load_texture();
            }
            //End card's data load
            card_list.shuffle<Card>();
            #endregion
        }
        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            base.Update(theTime);

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
        }
        #endregion                

        #region update's function
        private void resize_hand()
        {
            int net_width = (hand_area_list[hand_area_list.Count - 1].Right - hand_area_list[0].Left);
            int oversize = net_width - hand_witdh;
            if (oversize > 0)
            {
                padding = padding - (oversize / hand_area_list.Count);

            }
            for (int i = 1; i < hand_area_list.Count; i++)
            {
                hand_area_list[i] = new Rectangle(175 + (card_widht + padding) * i, 570, card_widht, card_height);
            }
        }

        private void draw_card()
        {
            hand_list.Add(card_list[0]);
            card_list.RemoveAt(0);
            hand_area_list.Add(new Rectangle(175 + (card_widht + padding) * hand_area_list.Count, 570, card_widht, card_height));
        }
        #endregion

        #region HANDLER
        private void InGameScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }
        private void chat_textbox_onEnterPressed(TextBox sender)
        {
            SetIP();
            SetUsername();
            //InitializeSender();
            if (!string.IsNullOrEmpty(chat_textbox.Text))
            {
                string toSend = userName + ": " + chat_textbox.Text;
                byte[] data = Encoding.Unicode.GetBytes(toSend);
                textbox_chat_show.Text += toSend + "\r\n";
                //Game1.MessageBox(new IntPtr(0), broadcastAddress, "", 0);
                sendingClient.Send(data, data.Length, broadcastAddress, port);
                chat_textbox.Text = "";
            }
        }
        private void textbox_onShiftEnterPressed(TextBox sender)
        {
            sender.Text += "\r\n";
        }
        private void draw_button_clicked(object sender,FormEventData e)
        {
            draw_card();
            resize_hand();
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch,GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            DrawText(spriteBatch);
            for (int x = 0; x < opponent_number; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    spriteBatch.Draw(border_texture, opponent_area[x, y], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);
                }
            }
            spriteBatch.Draw(player_control_texture, player_control_area, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);
            spriteBatch.Draw(master_list[player_random_char[0]].char_texture, player_char_area[0], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.25f);
            spriteBatch.Draw(servant_list[player_random_char[1]].char_texture, player_char_area[1], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.25f);
            spriteBatch.Draw(chat_screen_texture, chat_screen_area, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);
            spriteBatch.Draw(chat_input_texture, chat_input_area, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);

            label_username.Draw(spriteBatch, gameTime);
            label_IP.Draw(spriteBatch, gameTime);

            draw_button.Draw(spriteBatch, gameTime);

            if (hand_hovered_index == -1)
            {
                for (int i = 0; i < hand_list.Count; i++)
                {
                    spriteBatch.Draw(hand_list[i].texture, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f - i * 0.001f);
                }
            }
            else
            {
                for (int i = 0; i < hand_list.Count; i++)
                {
                    if (i == hand_hovered_index)
                    {
                        spriteBatch.Draw(hand_list[i].texture/*main_game.white_texture*/, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.301f);
                        spriteBatch.Draw(hand_list[i].texture, hand_area_list[i], null, new Color(0xFF, 0xFF, 0xFF, 0xCC), 0f, new Vector2(0, 0), SpriteEffects.None, 0.3f);
                    }
                    else
                        spriteBatch.Draw(hand_list[i].texture, hand_area_list[i], null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f - i * 0.001f);
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
            spriteBatch.DrawString(font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
        }
        #endregion
    }
}
