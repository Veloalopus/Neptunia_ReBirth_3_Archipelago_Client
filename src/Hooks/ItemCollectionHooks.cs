using Nep3ArchipelagoClient.Archipelago;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.src.Hooks
{
    public class ItemCollectionHooks
    {
        public static bool IsAPItem = true;
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
            _addItemFunction = hooks.CreateFunction<AddItemToInventory>((int)(Mod.ModuleBase + 0x0BDB90));

            Console.WriteLine("Test function location {0:X}", _addItemFunction.Address);
            string[] loadGatheringSpot = {
                "use32",
                "push edx",
                "mov edx,[esp+24]",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetGatherSpot, out _onGatherSpot)}",
                "popfd",
                "popad",
                "pop edx",
            };
            _asmHooks.Add(hooks.CreateAsmHook(loadGatheringSpot, (int)(Mod.ModuleBase + 0xC32DE), AsmHookBehaviour.ExecuteFirst).Activate());
            string[] collectGatherSpot = {
                "use32",
                "pushad",
                "pushfd",
                "mov edx,[ebp-0x228]",
                "mov eax,[ebp-0x22c]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnCollectGatherSpot, out _onCollectionGatherSpot)}",
                "popfd",
                "popad",
                "mov ecx,[ebp-0x04]",
            };
            _asmHooks.Add(hooks.CreateAsmHook(collectGatherSpot, (int)(Mod.ModuleBase + 0x20C59B), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
            string[] getTreasureId = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetDungeonTresureId, out _onGetDungeonTresureId)}",
                "popfd",
                "popad",
            };
            _asmHooks.Add(hooks.CreateAsmHook(getTreasureId, (int)(Mod.ModuleBase + 0xB8D1D), AsmHookBehaviour.ExecuteFirst).Activate());
            string[] removeDungeonCreation = {
                "use32",
                "ret",
            };
            _asmHooks.Add(hooks.CreateAsmHook(removeDungeonCreation, (int)(Mod.ModuleBase + 0xC2BD0), AsmHookBehaviour.DoNotExecuteOriginal).Activate());


        }

        static bool allowOrignalLoot = true;
        //get dungeon and spot id to send it to the ap server
        [Function(new[] { FunctionAttribute.Register.eax,FunctionAttribute.Register.edx }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int GetGatherSpot(int dungeonID,int dungeoFlag);
        public static unsafe int OnGetGatherSpot(int eax,int edx)
        {
            Console.WriteLine($"Dungeon ID = {eax}, Gather Flag ID = {edx}");
            allowOrignalLoot = true;
            if (Mod.APClient.IsConnected)
            {
                long GatherspotID = (eax * 10) + edx + 1;
                if (Mod.APClient.CheckedLocation.Contains(GatherspotID)) return eax;
                Mod.APClient.SendLocation(GatherspotID);
                Mod.APClient.GetItemName(GatherspotID, ref TextHooks.ReplacementText);
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
            Mod.APClient.GetItemName(DungeonTreasureId, ref TextHooks.ReplacementText);

            return eax;
        }

    }
}
