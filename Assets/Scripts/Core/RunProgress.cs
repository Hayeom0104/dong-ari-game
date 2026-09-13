using System;

namespace DongAriGame.Core
{
    public sealed class RunProgress
    {
        public const int TotalRooms = 10;
        public int CurrentRoom { get; private set; } = 1;
        public int Gold { get; private set; }
        public bool IsShopRoom => CurrentRoom == 4 || CurrentRoom == 9;
        public bool IsComplete => CurrentRoom > TotalRooms;

        public void CompleteCurrentRoom()
        {
            if (IsComplete) return;
            if (!IsShopRoom) Gold += 40 + CurrentRoom * 10;
            CurrentRoom++;
        }

        public bool TrySpendGold(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (IsComplete || !IsShopRoom || Gold < amount) return false;
            Gold -= amount;
            return true;
        }

        public void Reset()
        {
            CurrentRoom = 1;
            Gold = 0;
        }
    }
}

