using Nep3ArchipelagoClient.src.Hooks;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient.MemoryInterface
{
    internal class Inventory
    {
        UIntPtr InventoryPointer => InventorySizePointer + 0x04;
        const int ItemLength = 0x04;
        const UIntPtr ItemCountOffsetPointer = 0x02;
        UIntPtr InventorySizePointer => SaveGame.SaveGamePointer + 0xC7CC;
        public int CurrentInventoryCount => Memory.Instance.Read<short>(InventorySizePointer);

        UIntPtr ItemPosition(int slot)
        {
            return InventoryPointer + (nuint)(slot * ItemLength);
        }

        public Inventory()
        {
        }

        public byte GetItemCountAtSlot(int slot)
        {
            if (slot < 0)
                return 0;
            return Memory.Instance.Read<byte>(ItemPosition(slot)+ItemCountOffsetPointer);
        }
        public short GetItemIDAtSlot(int slot)
        {
            if (slot < 0)
                return 0;
            return Memory.Instance.Read<short>(ItemPosition(slot));
        }
        public bool FindItem(short itemID,out int position)
        {
            position = 0;
            if (CurrentInventoryCount <= 0)
                return false;
            for(int i = 0; i < CurrentInventoryCount; i++)
            {
                if(itemID == GetItemIDAtSlot(i))
                {
                    position = i;
                    return true;
                }
            }
            return false;
        }

        public void AddItem(short itemID,byte amount)
        {
            ItemCollectionHooks._addItemFunction.GetWrapper()((uint)itemID, amount, (char)1);
        }
    }
}
