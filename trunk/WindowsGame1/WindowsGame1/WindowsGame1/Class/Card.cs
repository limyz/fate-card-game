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
                Suit suit = Suit.Heart;
                Number num = Number.Ace;
                switch (Convert.ToInt32(temp.GetAttribute("suit")))
                {
                    case 1: suit = Suit.Heart; break;
                    case 2: suit = Suit.Diamond; break;
                    case 3: suit = Suit.Club; break;
                    case 4: suit = Suit.Spade; break;
                    case 0: suit = Suit.None; break;
                    default: break;
                }
                switch (Convert.ToInt32(temp.GetAttribute("number")))
                {
                    case 1: num = Number.Ace; break;
                    case 2: num = Number.Two; break;
                    case 3: num = Number.Three; break;
                    case 4: num = Number.Four; break;
                    case 5: num = Number.Five; break;
                    case 6: num = Number.Six; break;
                    case 7: num = Number.Seven; break;
                    case 8: num = Number.Eight; break;
                    case 9: num = Number.Nine; break;
                    case 10: num = Number.Ten; break;
                    case 11: num = Number.Jack; break;
                    case 12: num = Number.Queen; break;
                    case 13: num = Number.King; break;
                    case 0: num = Number.None; break;
                    default: break;
                }
                myReturn.Add(new Card(temp.GetAttribute("type")
                    , xml_card_list[i].Name
                    , temp.GetAttribute("img")
                    , xml_card_list[i].InnerText
                    , suit
                    , num));
            }
            return myReturn;
        }
    }
}
