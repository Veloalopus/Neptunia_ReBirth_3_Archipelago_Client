using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.Hooks
{
    internal class RB2TextHooks
    {
        public static List<IAsmHook> _asmHooks = new();
        public static byte[] ReplacementText => TextHooks.ReplacementText;

        public static IReverseWrapper<ChangeText> _onTextRead;

        [Function(new[] { FunctionAttribute.Register.eax }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int ChangeText(int originalText);
        public static unsafe int OnChangeText(int eax)
        {
            if (TextHooks.DoReplaceText)
                fixed (byte* p = ReplacementText)
                    return (int)p;
            TextHooks.DoReplaceText = false;
            return eax;
        }

        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0;
            //text replacement test
            string[] testReplacement =
            {
                "use32",
                "push eax",
                "mov eax,[esp+0x4]",
                "push ecx",
                "push edx",
                "push ebp",
                "push esi",
                "push edi",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnChangeText, out _onTextRead)}",
                "popfd",
                "pop edi",
                "pop esi",
                "pop ebp",
                "pop edx",
                "pop ecx",
                "mov [esp+0x4],eax",
                "pop eax",

                //=
            };
            if (FunctionScanner.FindFunction("Gather Item Text", "E8 ?? ?? ?? ?? 50 8D 85 ?? ?? ?? ?? 68 ?? ?? ?? ?? 50 FF 15 ?? ?? ?? ?? 8D 95 ?? ?? ?? ?? 83 C4 10 8B F2 8A 02 42 84 C0 75 ?? 8D BD ?? ?? ?? ?? 2B D6 4F 8A 47 ?? 47 84 C0 75 ?? 8B 9D", out offset))
                _asmHooks.Add(hooks.CreateAsmHook(testReplacement, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteAfter).Activate());


        }
    }
}
