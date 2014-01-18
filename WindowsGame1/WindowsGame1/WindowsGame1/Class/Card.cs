using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    [Serializable]
    public class Card
    {
        //private ContentManager Content;
        public string CardType;
        public string CardName;
        public string CardAsset;
        public string CardEff;
        public enum Suit
        {
            Heart = 0,//♥♡
            Diamond = 1,//♦♢
            Club = 2,//♣♧
            Spade = 3//♠♤
        };
        Suit cardSuit;
        public enum Number
        {
            Ace = 1,
            Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, 
            Seven = 7, Eight = 8, Nine = 9, Ten = 10,
            Jack = 11, Queen = 12, King = 13
        }
        Number cardNumber;
        //public Texture2D texture;
        public Card(string type, string name, string asset, string effect,
            Suit suit, Number number)
        {
            //this.Content = Content;
            this.CardType = type;
            this.CardName = name;
            this.CardAsset = asset;
            this.CardEff = effect;
            this.cardSuit = suit;
            this.cardNumber = number;
        }
        //public void load_texture()
        //{
        //    this.texture = Content.Load<Texture2D>("Resource/card/" + CardAsset);
        //}
    }
}
