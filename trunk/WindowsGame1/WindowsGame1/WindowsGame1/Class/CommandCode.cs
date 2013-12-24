using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public enum CommandCode
    {
        //No data, used to check connect status
        standby = 0,
        //Room as Data1, used to update room state
        update_room = 1,
        //Player as Data1, used to send Player object to host
        join_game = 2,
        //String as Message, used to send chat
        chat_message = 3
    };
}
