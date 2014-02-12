using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WindowsGame1
{
    public enum Suit
    {
        None = 0,//Joker
        Heart = 1,//♥♡
        Diamond = 2,//♦♢
        Club = 3,//♣♧
        Spade = 4//♠♤
    };
    public enum Number
    {
        None = 0,//Joker
        Ace = 1,
        Two = 2, Three = 3, Four = 4, Five = 5, Six = 6,
        Seven = 7, Eight = 8, Nine = 9, Ten = 10,
        Jack = 11, Queen = 12, King = 13
    }
    [Serializable]
    public class Card
    {
        public string CardType;
        public string CardName;
        public string CardAsset;
        public string CardDescription;
        public Suit CardSuit;
        public Number CardNumber;
        public Card(string type, string name, string asset, string description,
            Suit suit, Number number)
        {
            this.CardType = type;
            this.CardName = name;
            this.CardAsset = asset;
            this.CardDescription = description;
            this.CardSuit = suit;
            this.CardNumber = number;
        }

        public static List<Card> LoadFromXML(ContentManager content)
        {
            List<Card> myReturn = new List<Card>();
            XmlDocument xml = new XmlDocument();
            xml.Load("Data/Card.xml");
            XmlNodeList xml_card_list = xml.GetElementsByTagName("Card")[0].ChildNodes;
            for (int i = 0; i < xml_card_list.Count; i++)
            {
                XmlElement temp = (XmlElement)xml_card_list[i];
                myReturn.Add(new Card(""
                    , xml_card_list[i].Name
                    , temp.GetAttribute("img")
                    , xml_card_list[i].InnerText
                    , Suit.Heart, Number.Ace));
            }
            return myReturn;
        }
    }
}
