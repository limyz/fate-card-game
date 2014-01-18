using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Xml;


namespace WindowsGame1
{
    public class CharacterSelectScreen : Screen
    {
        #region Variable Declaration
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
                new Rectangle(100, 200, 149, 208), this);
            charSelectBorder[1] = new Border("Char 2 Border", Color.Black, 2,
                new Rectangle(250, 200, 149, 208), this);
            charSelectBorder[2] = new Border("Char 3 Border", Color.Black, 2,
                new Rectangle(400, 200, 149, 208), this);
            charSelectBorder[3] = new Border("Char 4 Border", Color.Black, 2,
                new Rectangle(550, 200, 149, 208), this);
            charSelectBorder[4] = new Border("Char 5 Border", Color.Black, 2,
                new Rectangle(700, 200, 149, 208), this);
            charSelectBorder[5] = new Border("Char 6 Border", Color.Black, 2,
                new Rectangle(850, 200, 149, 208), this);
            charSelectBorder[6] = new Border("Char 7 Border", Color.Black, 2,
                new Rectangle(1000, 200, 149, 208), this);

            characterImage[0] = new Image("Char 1 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(102, 202, 147, 206), 0.3f, this);
            characterImage[1] = new Image("Char 2 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(252, 202, 147, 206), 0.3f, this);
            characterImage[2] = new Image("Char 3 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(402, 202, 147, 206), 0.3f, this);
            characterImage[3] = new Image("Char 4 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(552, 202, 147, 206), 0.3f, this);
            characterImage[4] = new Image("Char 5 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(702, 202, 147, 206), 0.3f, this);
            characterImage[5] = new Image("Char 6 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(852, 202, 147, 206), 0.3f, this);
            characterImage[6] = new Image("Char 7 Image", Content.Load<Texture2D>("Resource/character_back"),
                new Rectangle(1002, 202, 147, 206), 0.3f, this);
            #endregion

            #region Button
            okButton = new ImageButton("OK Button", Content.Load<Texture2D>("Resource/ok_button"),
                new Rectangle(500, 450, 180, 70), this);
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
            for (int i = 0; i < 7; i++)
            {
                playerRandomChar[0] = rand.Next(characterList.Count);
                characterPlayerList.Add(characterList[playerRandomChar[0]]);
                characterList.RemoveAt(playerRandomChar[0]);
            }

            #endregion
        }

        #endregion

        #region Start-End Game
        public override void Start(Command command)
        {
            UpdateSelectImageList();
            mainPlayer = new Player("test player", "0.0.0.0");
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
            if (count<2)
            {
                Game1.MessageBox(new IntPtr(0), "Please select 2 Characters", "Warning!", 0);
            }else if (count == 2)
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
                    Game1.MessageBox(new IntPtr(0), s, "Your Character", 0);
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
