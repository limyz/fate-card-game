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
        delegate void AddMessage(string message);

        string userName="";

        const int send_port = 51000;
        const int receive_port = 51000;
        string Address = "255.255.255.255";

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
                Address = "255.255.255.255";
            }
            /*else if (textbox_IP.Text == "all")
            {
                Address = IPAddress.Broadcast.ToString();
            }*/
            else
            {
                Address = textbox_IP.Text.Replace("\r\n", "");
            }
        }
        
        public void InitializeSender()
        {            
            sendingClient = new UdpClient();
            sendingClient.EnableBroadcast = true;
        }

        public void InitializeReceiver()
        {
            receivingClient = new UdpClient(receive_port);
            receivingClient.EnableBroadcast = true;

            ThreadStart start = new ThreadStart(Receiver);
            receivingThread = new Thread(start);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        private void Receiver()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, receive_port);
            AddMessage messageDelegate = MessageReceived;

            while (true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                string message = Encoding.Unicode.GetString(data);
                //messageDelegate.Invoke(endPoint.Address.ToString()+":"+endPoint.Port+">>"+message);
                messageDelegate.Invoke(message);
                //Invoke(messageDelegate, message);
            }
        }

        private void MessageReceived(string message)
        {  
            chatDisplayTextbox.Text += (message + "\r\n");
        }
        public void End_Chat()
        {
            receivingThread.Abort();
            sendingClient.Close();
            receivingClient.Close();
        }

        #endregion

        #region variable decleration
        int card_widht = 105;
        int card_height = 150;
        int hand_witdh = 535;
        int padding = 5;
        int opponent_number = 5;
        int hand_hovered_index = -1;
        int[] player_random_char = new int[2];                       
        Random rand = new Random();
        Texture2D border_texture, char_card_texture, player_control_texture;
        Rectangle[,] opponent_area;
        Border chatInputBorder, chatDisplayBorder;
        Border[] playerCharacterBorder = new Border[2];
        Color borderColor = Color.MediumAquamarine;
        XmlDocument xml = new XmlDocument();
        Character[] master_list, servant_list;
        List<Card> card_list = new List<Card>();
        List<Card> hand_list = new List<Card>();
        List<Rectangle> hand_area_list = new List<Rectangle>();
        Image masterImg, servantImg;
        //Rectangle player_card_area = new Rectangle(175,570,105,150);
        ImageButton draw_button;
        TextBox chatInputTextbox, chatDisplayTextbox, textbox_username, textbox_IP;
        Label label_username, label_IP;
        Div PlayerControlPanel;
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

        #region Load Content
        public InGameScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theScreenEvent, Game1 parent)
            : base("InGameScreen",theScreenEvent,parent)
        {
            //parent.keyboard_dispatcher = new KeyboardDispatcher(main_game.Window);
            #region Load Resource
            border_texture = Content.Load<Texture2D>("Resource/Untitled-1");
            char_card_texture = Content.Load<Texture2D>("Resource/character_back");
            player_control_texture = Content.Load<Texture2D>("Resource/controlplayer");
            #endregion

            //content loading code here

            #region Player Control Panel
            playerCharacterBorder[0] = new Border("Character Border 1", Color.Red, 3, new Rectangle(731, 564, 111, 156), this);
            playerCharacterBorder[1] = new Border("Character Border 2", Color.Red, 3, new Rectangle(842, 564, 111, 156), this);
            PlayerControlPanel = new Div("PlayerControlPanel", new Rectangle(0, 564, 1000, 156), Color.White, this);
            masterImg = new Image("Player Master Image", char_card_texture, new Rectangle(734, 567, 105, 150), 0.3f, this);
            servantImg = new Image("Player Servant Image", char_card_texture, new Rectangle(845, 567, 105, 150), 0.3f, this);
            #endregion

            #region Other Player location
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
            #endregion

            #region Button
            draw_button = new ImageButton("draw_button", Content.Load<Texture2D>("Resource/draw")
                , new Rectangle(740, 520, 130, 35), 0.5f, this);
            draw_button.OnClick += draw_button_clicked;
            #endregion

            #region Chat
            chatDisplayBorder = new Border("chatDisplayBorder", borderColor, 2
                , new Rectangle(968, 10, 230, 404), this);

            chatDisplayTextbox = new TextBox("chatDisplayTextbox"
                , Game1.white_textbox, Game1.highlighted_textbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar, Game1.font
                , new Rectangle(970, 12, 226, 400), this);
            chatDisplayTextbox.scrollable = true;  
            chatDisplayTextbox.ReadOnly = true;

            chatInputBorder = new Border("chatInputBorder", borderColor, 2
               , new Rectangle(968, 438, 230, 104), this);

            chatInputTextbox = new TextBox("chatInputTextbox"
                , Game1.white_textbox, Game1.highlighted_textbox, Game1.caret
                , Game1.scrollbarBackground, Game1.scrollbar, Game1.font
                , new Rectangle(970, 440, 226, 100), this);
            chatInputTextbox.scrollable = true;
            chatInputTextbox.OnEnterPressed += chat_textbox_onEnterPressed;
            chatInputTextbox.OnShift_EnterPressed += textbox_onShiftEnterPressed;
            #endregion

            #region IP Test
            label_username = new Label("label_username", Game1.font, "Username"
                , 1010, 550, 1198 - 1010, Color.White, this);

            label_IP = new Label("label_IP", Game1.font, "IP Address"
                , 1010, 630, 1198 - 1010, Color.White, this);

            textbox_username = new TextBox("textbox_username"
                , Game1.white_textbox, Game1.highlighted_textbox, Game1.caret
                , Game1.font, new Rectangle(1010, 570, 188, 20), this);
            textbox_username.Text = "Siv";

            textbox_IP = new TextBox("textbox_IP"
                , Game1.white_textbox, Game1.highlighted_textbox, Game1.caret
                , Game1.font, new Rectangle(1010, 650, 188, 20), this);
            textbox_IP.Text = "255.255.255.255";
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
            try
            {
                hand_list.Add(card_list[0]);
                card_list.RemoveAt(0);
                hand_area_list.Add(new Rectangle(175 + (card_widht + padding) * hand_area_list.Count, 570, card_widht, card_height));
            }
            catch (Exception ex)
            {
                Game1.MessageBox(new IntPtr(0), ex.Message, "Exception", 0);
            }
        }

        public void randomCharacter()
        {
            Texture2D master = master_list[player_random_char[0]].char_texture;
            Texture2D servant = servant_list[player_random_char[1]].char_texture;

            masterImg.texture = master;
            servantImg.texture = servant;
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
            SetIP();
            SetUsername();
            if (!string.IsNullOrEmpty(chatInputTextbox.Text))
            {
                string toSend = userName + ": " + chatInputTextbox.Text;
                byte[] data = Encoding.Unicode.GetBytes(toSend);
                chatDisplayTextbox.Text += toSend + "\r\n";
                sendingClient.Send(data, data.Length, Address, send_port);
                chatInputTextbox.Text = "";
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
            randomCharacter();
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
            //spriteBatch.Draw(player_control_texture, playerControlRectangle, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.5f);

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
            spriteBatch.DrawString(Game1.font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
        }
        #endregion
    }
}
