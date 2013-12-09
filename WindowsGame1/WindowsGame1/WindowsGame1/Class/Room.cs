using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Room
    {
        List<Player> player_List = new List<Player>();
        public List<Player> Player_List
        {
            get { return player_List; }
            set
            {
                if (this.Number_of_Player >= value.Count)
                {
                    player_List = value;
                }
            }
        }
        public string Room_name;
        public int Number_of_Player;
        public int owner_index;

        public Room(Player player, string Room_name, int Number_of_Player)
        {
            this.Player_List.Add(player);
            this.Room_name = Room_name;
            this.Number_of_Player = Number_of_Player;
            this.owner_index = 0;
        }
    }
}
