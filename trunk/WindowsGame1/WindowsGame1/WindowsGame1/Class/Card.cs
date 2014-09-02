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
    public enum CardType
    {
        Tool = 0,
        Weapon = 1,
        PlusVehicle = 2,
        MinusVehicle = 3,
        Armour = 4,
        Basic = 5,
    }
    [Serializable]
    public class Card
    {
        //public string CardType;
        public string CardName;
        public string CardAsset;
        public string CardDescription;
        public Suit CardSuit;
        public Number CardNumber;
        public CardType CardType;
        public Card(string type, string name, string asset, string description,
            Suit suit, Number number)
        {
            if (type.ToLower() == "basic") this.CardType = CardType.Basic;
            else if (type.ToLower() == "tool") this.CardType = CardType.Tool;
            else if (type.ToLower() == "armour") this.CardType = CardType.Armour;
            else if (type.ToLower() == "plus-vehicle") this.CardType = CardType.PlusVehicle;
            else if (type.ToLower() == "minus-vehicle") this.CardType = CardType.MinusVehicle;
            else if (type.ToLower() == "weapon") this.CardType = CardType.Weapon;
            this.CardName = name;
            this.CardAsset = asset;
            this.CardDescription = description;
            this.CardSuit = suit;
            this.CardNumber = number;
        }

        public static List<CardDeck> LoadDeckFromXML(ContentManager content)
        {
            List<CardDeck> myReturn = new List<CardDeck>();
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
                Card c = new Card(temp.GetAttribute("type")
                    , xml_card_list[i].InnerText
                    , temp.GetAttribute("img")
                    , xml_card_list[i].InnerText
                    , suit
                    , num);
                CardDeck cd = new CardDeck(c);
                myReturn.Add(cd);
            }
            return myReturn;
        }
    }
}
