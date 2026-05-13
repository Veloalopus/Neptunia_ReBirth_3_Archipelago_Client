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
    internal class TextHooks
    {
        public static bool DoReplaceText = false;
        public static List<IAsmHook> _asmHooks = new();
        public static byte[] ReplacementText = new byte[255];

        public static IReverseWrapper<GetEnemyDropString> _onGetEnemyDropString;
        public static IReverseWrapper<ChangeText> _onTextRead;

        public static void ClearReplacementText()
        {
            for (int i = 0; i < ReplacementText.Length; i++)
            {
                ReplacementText[i] = 0;
            }
        }
        public static void NewText(string text)
        {
            ClearReplacementText();
            int idx = 0;
            foreach(char c in text)
            {
                if (idx >= ReplacementText.Length) break;
                ReplacementText[idx] = (byte)c;
                idx++;
            }
        }

        //change text for item
        //TODO target only on gather / killed enemy
        [Function(new[] { FunctionAttribute.Register.eax }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int ChangeText(int originalText);
        public static unsafe int OnChangeText(int eax)
        {
            if (DoReplaceText)
                fixed (byte* p = ReplacementText)
                    return (int)p;
            DoReplaceText = false;
            return eax;
        }

        public static int counter = 0;
        [Function(new[] { FunctionAttribute.Register.eax }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int GetEnemyDropString(int dungeonID);
        public static unsafe int OnGetEnemyDropString(int eax)
        {
            Console.WriteLine($"Item number {counter}");
            ClearReplacementText();
            ReadOnlySpan<byte> text = Encoding.UTF8.GetBytes($"Item {counter}");
            text.ToArray().CopyTo(ReplacementText, 0);
            counter++;
            DoReplaceText = false;
            return eax;
        }
        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0;
            //text replacement test
            string[] testReplacement =
{
                "use32",
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
                //=
            }; 

            if (FunctionScanner.FindFunction("Item Text 1", "E8 ?? ?? ?? ?? 50 89 45 ?? E8 ?? ?? ?? ?? F3 0F 10 0D",out offset))
                _asmHooks.Add(hooks.CreateAsmHook(testReplacement, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteAfter).Activate());
            if(FunctionScanner.FindFunction("Item Text 2", "E8 ?? ?? ?? ?? 83 C4 04 50 57 E8 ?? ?? ?? ?? 83 C4 04 50 57 E8 ?? ?? ?? ?? 50", out offset))
                _asmHooks.Add(hooks.CreateAsmHook(testReplacement, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteAfter).Activate());





            string[] enemyDrop = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetEnemyDropString, out _onGetEnemyDropString)}",
                "popfd",
                "popad",
            };
            if(FunctionScanner.FindFunction("Enemy Drop Text", "E8 ?? ?? ?? ?? 8B 85 ?? ?? ?? ?? 46 83 C4 0C 83 C7 08",out offset))
                _asmHooks.Add(hooks.CreateAsmHook(enemyDrop, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());

        }
    }
}
