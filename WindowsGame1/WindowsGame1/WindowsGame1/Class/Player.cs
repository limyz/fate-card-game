﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Player
    {
        string player_name;
        string address;
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

        public Player(string player_name, string address)
        {
            this.player_name = player_name;
            this.address = address;
        }
    }
}