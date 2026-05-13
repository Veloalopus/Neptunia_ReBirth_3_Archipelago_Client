using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.MemoryInterface;
using Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.Hooks
{
    public class ShopHook
    {
        public static List<IAsmHook> _asmHooks = new();
        public static IReverseWrapper<CheckSell> _onCheckSell;
        public static IFunction<SellItem> _sellItem;

        [Function(CallingConventions.Stdcall)]
        public delegate int SellItem(nuint itemStackPointer);

        [Function(new[] { FunctionAttribute.Register.eax}, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.None)]
        public delegate int CheckSell(nuint pointer);
        public static unsafe int OnCheckSell(nuint eax)
        {
            var mem = Memory.Instance;
            nuint pointer = mem.Read<nuint>(eax+0x70)+4;
            pointer = mem.Read<nuint>(pointer);
            int itemId = mem.Read<int>(pointer + 4);
            if (ProgressiveGear.UsedItems.Contains((short)itemId))
                return 0;
            return _sellItem.GetWrapper()(eax);
        }

        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0;
            if(FunctionScanner.FindFunction("Sell Item", "55 8B EC 8B 45 ?? 53 32 DB 85 C0 75 ?? 32 C0 5B 5D C3 56 50 E8 ?? ?? ?? ?? 8B F0 83 C4 04 85 F6 75 ?? 5E 32 C0 5B 5D C3 8B 46",out offset))
                _sellItem = hooks.CreateFunction<SellItem>((int)(Mod.ModuleBase + offset));

            string[] soldItem =
            {
                "use32",
                "push ecx",
                "push edx",
                "push ebp",
                "push esi",
                "push edi",
                "pushfd",
                "mov eax,[esi+0x30]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnCheckSell, out _onCheckSell)}",
                "popfd",
                "pop edi",
                "pop esi",
                "pop ebp",
                "pop edx",
                "pop ecx",
            };
            if(FunctionScanner.FindFunction("Check Sold Item", "FF 76 ?? E8 ?? ?? ?? ?? 83 C4 04 84 C0 0F 84 ?? ?? ?? ?? 6A 0E",out offset))
                _asmHooks.Add(hooks.CreateAsmHook(soldItem, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
        }
    }
}
