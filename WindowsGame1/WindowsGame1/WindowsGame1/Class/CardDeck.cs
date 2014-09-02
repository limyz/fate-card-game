using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    [Serializable]
    public class CardDeck
    {
        public Guid CardId;
        public Card Card;
        public CardDeck(Card card)
        {
            this.Card = card;
            CardId = Guid.NewGuid();
        }
    }
}
