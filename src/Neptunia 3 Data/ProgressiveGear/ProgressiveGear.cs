using Nep3ArchipelagoClient.MemoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nep3ArchipelagoClient.src.Neptunia_3_Data;

namespace Nep3ArchipelagoClient.src.Neptunia_3_Data.ProgressiveGear
{
    internal class ProgressiveGear
    {
        protected Dictionary<int, List<short>> GearList = new();
        protected byte Amount = 2;
        static Inventory Inventory => Mod.Inventory;
        public static Dictionary<int,ProgressiveGear> ProgressiveGears = InitList();
        public static HashSet<short> UsedItems;
        static Dictionary<int, ProgressiveGear> InitList()
        {
            Dictionary<int, ProgressiveGear> list = new();
            list.Add((int)CharacterId.neptune, new NeptuneGear());
            list.Add((int)CharacterId.nepgear, new NepgearGear());
            list.Add((int)CharacterId.plutia, new PlutiaGear());
            list.Add((int)CharacterId.peashy, new PeashyGear());
            list.Add((int)CharacterId.blanc, new BlancGear());
            list.Add((int)CharacterId.noire, new NoireGear());
            list.Add((int)CharacterId.ram, new RamGear());
            list.Add((int)CharacterId.uni, new UniGear());
            list.Add((int)CharacterId.vert, new VertGear());
            list.Add((int)CharacterId.rom, new RomGear());
            list.Add(11, new ArmorGear());
            if (UsedItems == null)
                UsedItems = new();
            foreach (var character in list)
                foreach(var item in character.Value.GetAllItems())
                    UsedItems.Add(item);
            return list;
        }
        public void IncreaseGearTier()
        {
            int tier = 0;
            while (tier < GearList.Count && Inventory.FindItem(GearList[tier][0], out int _))
                tier++;
            if(tier<GearList.Count)
                foreach (var item in GearList[tier])
                        Inventory.AddItem(item, Amount);
        }

        public void UnlockTier(int tier)
        {
            if(GearList.ContainsKey(tier))
                foreach (var item in GearList[tier])
                    Inventory.AddItem(item, Amount);
        }
        protected short[] GetAllItems()
        {
            List<short> items = new();
            foreach(var tier in GearList)
            {
                items.AddRange(tier.Value);
            }
            return items.ToArray();
        }
    }
}
