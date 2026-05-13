using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient.MemoryInterface
{
    internal class RB3Inventory:Inventory
    {
        public RB3Inventory(SaveGame savegame):base(savegame)
        {
            InventorySizeOffset = 0xC7CC;
            
        }
    }
}
