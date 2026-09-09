using System;

namespace DongAriGame.Core
{
    public sealed class RunProgress
    {
        public const int TotalRooms = 10;
        public int CurrentRoom { get; private set; } = 1;
        public bool IsComplete => CurrentRoom > TotalRooms;

        public void CompleteCurrentRoom()
        {
            if (!IsComplete) CurrentRoom++;
        }

        public void Reset()
        {
            CurrentRoom = 1;
        }
    }
}

