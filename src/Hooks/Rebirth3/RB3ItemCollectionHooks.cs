using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.Hooks.Rebirth2
{
    public class RB3ItemCollectionHooks
    {
        public static bool IsAPItem = true;
        public static List<IAsmHook> _asmHooks = new();
        static bool allowOrignalLoot => ItemCollectionHooks.allowOrignalLoot;

        public static IReverseWrapper<CollectGatherSpot> _onCollectionGatherSpot;
        public static IFunction<ItemCollectionHooks.AddItemToInventory> _addItemFunction => ItemCollectionHooks._addItemFunction;


        public static void SetupHooks(IReloadedHooks hooks)
        {
            if (hooks == null) return;
            // Game functions
            nuint offset = 0;

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
            if (FunctionScanner.FindFunction("Collect Gather", "E8 ?? ?? ?? ?? 8B 4D ?? 83 C4 0C 33 CD B0 01 5B E8", out offset))
                _asmHooks.Add(hooks.CreateAsmHook(collectGatherSpot, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
        }

        //determines if an item need to be set in the players inventory
        [Function(new FunctionAttribute.Register[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int CollectGatherSpot(uint item, uint quantity);
        public static unsafe int OnCollectGatherSpot(uint eax, uint edx)
        {
            Console.WriteLine($"item id:{eax} quantity:{edx}");
            //non randomized item
            if (allowOrignalLoot)
                _addItemFunction.GetWrapper()(eax, edx, (char)1);

            return (int)eax;
        }


    }
}
