using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    [Serializable]
    public class Player
    {
        #region Variable Declaration
        string player_name;
        string address;
        bool status = false;
        Character character1;
        Character character2;
        public Guid id;
        public bool Gender;
        public int Counter;
        //int graphicIndex;
        public int HandLimit, CurrentHealth;
        public double MaxHealth;
        public Card[] Equipment = new Card[4];
        public Card ExtraEquipment;
        public List<Card> HandCard = new List<Card>();
        public List<Card> PendingCard = new List<Card>();
        public List<Card> DeathCard = new List<Card>();
        #endregion

        #region Property
        public Character Character1
        {
            get { return character1; }
            set { character1 = value; }
        }

        public Character Character2
        {
            get { return character2; }
            set { character2 = value; }
        }

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
        public string Player_name
        {
            get { return player_name; }
            set { player_name = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        #endregion

        public Player(string player_name, string address)
        {
            this.player_name = player_name;
            this.address = address;
            this.id = Guid.NewGuid();
        }
    }
}
