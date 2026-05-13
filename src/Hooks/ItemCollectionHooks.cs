using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.Hooks
{
    public class ItemCollectionHooks
    {
        public static List<IAsmHook> _asmHooks = new();

        public static IReverseWrapper<GetGatherSpot> _onGatherSpot;
        public static IReverseWrapper<CollectGatherSpot> _onCollectionGatherSpot;
        public static IReverseWrapper<GetDungeonTresureId> _onGetDungeonTresureId;
        public static IFunction<AddItemToInventory> _addItemFunction;


        [Function(CallingConventions.Stdcall)]
        public delegate int AddItemToInventory(uint itemID, uint qunatity, char dunno);

        public static void SetupHooks(IReloadedHooks hooks)
        {
            if (hooks == null) return;
            // Game functions
            nuint offset = 0;
            if(FunctionScanner.FindFunction("Add Item", "55 8B EC 83 EC 08 57 8B 7D ?? 85 FF 0F 84 ?? ?? ?? ?? F7 C7 00 00 FF FF",out offset))
                _addItemFunction = hooks.CreateFunction<AddItemToInventory>((int)(Mod.ModuleBase + offset));

            Console.WriteLine("Test function location {0:X}", _addItemFunction.Address);
            //string[] loadGatheringSpot = {
            //    "use32",
            //    "push edx",
            //    "mov edx,[esp+24]",
            //    "pushad",
            //    "pushfd",
            //    $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetGatherSpot, out _onGatherSpot)}",
            //    "popfd",
            //    "popad",
            //    "pop edx",
            //};
            //// this function exit 3 times <.<
            //_asmHooks.Add(hooks.CreateAsmHook(loadGatheringSpot, (int)(Mod.ModuleBase + 0xC32DE), AsmHookBehaviour.ExecuteFirst).Activate());

            string[] lootGather = {
                "use32",
                "pushad",
                "pushfd",
                "mov eax,[esp+0x24]",
                "mov edx,[esp+0x28]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetGatherSpot, out _onGatherSpot)}",
                "popfd",
                "popad",
            };
            if(FunctionScanner.FindFunction("Loot Gather Spot", "55 8B EC FF 75 ?? A1 ?? ?? ?? ?? FF 75 ?? FF 70", out offset))
                _asmHooks.Add(hooks.CreateAsmHook(lootGather, (int)(Mod.ModuleBase + offset + 17), AsmHookBehaviour.ExecuteFirst).Activate());

            string[] getTreasureId = {
                "use32",
                "pushad",
                "pushfd",
                "mov ecx,[esp+0x24]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetDungeonTresureId, out _onGetDungeonTresureId)}",
                "popfd",
                "popad",
            };
            if(FunctionScanner.FindFunction("Collect Treasure", "E8 ?? ?? ?? ?? 83 C4 08 C7 47 ?? 00 00 00 00 C7 03 00 00 00 00", out offset))
                _asmHooks.Add(hooks.CreateAsmHook(getTreasureId, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());

            string[] removeDungeonCreation = {
                "use32",
                "ret",
            };
            if(FunctionScanner.FindFunction("Create Dungeon", "55 8B EC 57 8B 7D ?? 85 FF 75 ?? 32 C0 5F 5D C3 A1 ?? ?? ?? ?? 53 8B 98 ?? ?? ?? ?? 83 FB 50 73 ?? 57 E8 ?? ?? ?? ?? 0F B7 C8 83 C4 04 66 85 C9 75",out offset))
                _asmHooks.Add(hooks.CreateAsmHook(removeDungeonCreation, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
        }

        public static bool allowOrignalLoot = true;
        //get dungeon and spot id to send it to the ap server
        [Function(new[] { FunctionAttribute.Register.eax,FunctionAttribute.Register.edx }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int GetGatherSpot(int dungeonID,int dungeoFlag);
        public static unsafe int OnGetGatherSpot(int eax,int edx)
        {
            eax = Memory.Instance.Read<short>((nuint)eax);
            Console.WriteLine($"Dungeon ID = {eax}, Gather Flag ID = {edx}");
            allowOrignalLoot = true;
            if (Mod.APClient.IsConnected)
            {
                long GatherspotID = (eax * 10) + edx + 1;
                if (Mod.APClient.CheckedLocation.Contains(GatherspotID)) return eax;
                Mod.APClient.SendLocation(GatherspotID);
                TextHooks.NewText(Mod.APClient.GetItemName(GatherspotID));
                TextHooks.DoReplaceText = true;
                allowOrignalLoot = false;
            }
            return eax;
        }

        //determines if an item need to be set in the players inventory
        [Function(new FunctionAttribute.Register[] { FunctionAttribute.Register.eax,FunctionAttribute.Register.edx }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int CollectGatherSpot(uint item,uint quantity);
        public static unsafe int OnCollectGatherSpot(uint eax,uint edx)
        {
            Console.WriteLine($"item id:{eax} quantity:{edx}");
                //non randomized item
            if(allowOrignalLoot)
                _addItemFunction.GetWrapper()(eax,edx,(char)1);

            return (int)eax;
        }

        [Function(new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.ecx }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int GetDungeonTresureId(int eax,int dungeonTreasureID);
        public static unsafe int OnGetDungeonTresureId(int eax,int ecx)
        {
            Console.WriteLine($"Dungeon Tresure ID:{ecx}");
            long DungeonTreasureId = APClient.TreasureBaseID+ecx;
            TextHooks.DoReplaceText = true; 
            Mod.APClient.SendLocation(DungeonTreasureId);
            TextHooks.NewText(Mod.APClient.GetItemName(DungeonTreasureId));
            return eax;
        }

    }
}
