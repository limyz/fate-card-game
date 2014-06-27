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

        //No data, used to asked the host is it ok to join the room
        Can_I_Join = 3,

        //No data, used to signal the client that it is ok to join the room
        OK_to_Join = 4,

        //No data, used to signal the client that it is NOT ok to join the room because it is full
        Cant_Join_Room_Full = 5,
        
        //String as Message, used to send chat
        Chat_Message = 6,

        //GUID as Data1,used to set ready_status to true for player
        Ready = 7,

        //GUID as Data1,used to set ready_status to false for player
        Cancel = 8,

        //Room as Data1,used to Start the Game
        Start_Game = 9,

        //Character Change
        Character_Change = 10,

        //Character 1 as Data1, Character2 as Data2, Guid as Data3, used to send Selected Character to host
        Character_Select = 11,

        //Character List as Data1, used to send Character List to client
        Character_Distribute = 12,

        //Synchronize Deck send a List of Card as Data1, used to synchronize any ListCard
        CardList_Synchronize = 13,

        //Player as Data1
        Change_Turn = 14,

        End_Turn = 15,

        Check_Connect
    };
}
