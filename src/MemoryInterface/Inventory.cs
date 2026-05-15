using Nep3ArchipelagoClient.Hooks;
using Reloaded.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.MemoryInterface
{
    internal abstract class Inventory
    {
        Memory Memory => Memory.Instance;
        protected SaveGame SaveGame;
        protected const UIntPtr ItemCountOffsetPointer = 0x02;
        protected UIntPtr InventoryPointer => InventorySizePointer + 0x04;
        protected const int ItemLength = 0x4;
        protected nuint InventorySizeOffset;
        protected UIntPtr InventorySizePointer => SaveGame.SaveGamePointer + InventorySizeOffset;
        protected Inventory(SaveGame savegame)
        {
            SaveGame = savegame;
        }

        public int CurrentInventoryCount => Memory.Instance.Read<short>(InventorySizePointer);
        public bool FindItem(short itemID, out int position)
        {
            position = 0;
            if (CurrentInventoryCount <= 0)
                return false;
            for (int i = 0; i < CurrentInventoryCount; i++)
            {
                if (itemID == GetItemIDAtSlot(i))
                {
                    position = i;
                    return true;
                }
            }
            return false;
        }

        public void AddItem(int itemID, int amount)
        {
            ItemCollectionHooks._addItemFunction.GetWrapper()((uint)itemID, (uint)amount, (char)1);
            SaveGame.CheckUnlockGoalCondition();
        }


        UIntPtr ItemPosition(int slot)
        {
            return InventoryPointer + (nuint)(slot * ItemLength);
        }

        public byte GetItemCountAtSlot(int slot)
        {
            if (slot < 0)
                return 0;
            return Memory.Instance.Read<byte>(ItemPosition(slot) + ItemCountOffsetPointer);
        }
        public short GetItemIDAtSlot(int slot)
        {
            if (slot < 0)
                return 0;
            return Memory.Instance.Read<short>(ItemPosition(slot));
        }
    }
}
