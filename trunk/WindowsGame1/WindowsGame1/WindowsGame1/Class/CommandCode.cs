using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public enum CommandCode
    {
        //No data, used to check connect status
        Standby = 0,
        //Room as Data1, used to update room state
        Update_Room = 1,
        //Player as Data1, used to send Player object to host
        Join_Game = 2,
        //String as Message, used to send chat
        Chat_Message = 3,
        //GUID as Data1,used to set ready_status to true for player
        Ready = 4,
        //GUID as Data1,used to set ready_status to false for player
        Cancel = 5,
        //Room as Data1,used to Start the Game
        Start_Game = 6,
        //Character Change
        Character_Change = 7
    };
}
