using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    [Serializable]
    public class Turn
    {
        public enum Phase
        {
            OtherPlayerTurn = 0,
            Beginning = 1,
            Judgment = 2,
            Draw = 3,
            Action = 4,
            Discard = 5,
            End = 6
        };
        public Phase phase;
        public Turn()
        {
            this.phase = Phase.OtherPlayerTurn;
        }
    }
}
