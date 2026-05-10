using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.src.Neptunia_3_Data.ProgressiveGear;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.src.Hooks
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
                return 1;
            return _sellItem.GetWrapper()(eax);
        }

        public static void SetupHooks(IReloadedHooks hooks)
        {
            _sellItem = hooks.CreateFunction<SellItem>((int)(Mod.ModuleBase + 0x1805C0));

            string[] loadText =
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
            _asmHooks.Add(hooks.CreateAsmHook(loadText, (int)(Mod.ModuleBase + 0x17E7CC), AsmHookBehaviour.DoNotExecuteOriginal).Activate());
        }
    }
}
