using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    [Serializable]
    public class Room
    {
        List<Player> player_List = new List<Player>();
        public List<Player> Player_List
        {
            get { return player_List; }
            set
            {
                /*if (this.Number_of_Player >= value.Count)
                {*/
                    player_List = value;
                /*}*/
            }
        }
        public string Room_name;
        public int Number_of_Player;
        public int owner_index;

        public Room(Player player, string Room_name, int Number_of_Player)
        {
            this.Player_List.Add(player);
            this.Room_name = Room_name;
            if (Number_of_Player < 1)
            {
                this.Number_of_Player = 1;
                
            }
            else
            {
                this.Number_of_Player = Number_of_Player;
            }
            this.owner_index = 0;
            this.Player_List.Capacity = Number_of_Player;
        }

        public bool isHost(Guid Player_ID)
        {
            int Player_Index = this.findByID(Player_ID);
            if (this.owner_index == Player_Index)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int findByID(Guid guid)
        {
            for (int i = 0; i < this.player_List.Count; i++)
            {
                if (player_List[i].id == guid)
                {
                    return i;
                }
            }
            return -1;
        }

        public Player findPlayerByID(Guid guid)
        {
            foreach (Player p in this.player_List)
            {
                if (p.id == guid)
                {
                    return p;
                }
            }
            return null;
        }

        public int findByID_ExcludeMainPlayer(Guid guid, Guid MainPlayer_Guid)
        {
            if (guid == MainPlayer_Guid) return -1;
            for (int i = 0, i2 = 0; i < this.player_List.Count; i++, i2++)
            {
                if (player_List[i].id == MainPlayer_Guid)
                {
                    i2--;
                    continue;
                }
                else if (player_List[i].id == guid)
                {
                    return i2;
                }
            }
            return -1;
        }
    }
}
