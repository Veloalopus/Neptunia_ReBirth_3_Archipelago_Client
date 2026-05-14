using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient.MemoryInterface
{
    internal class RB2Inventory : Inventory
    {
        public RB2Inventory(SaveGame savegame) : base(savegame)
        {
            InventorySizeOffset = 0xCA4C;

        }
    }
}
