using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.MemoryInterface;
using Nep3ArchipelagoClient.Neptunia_3_Data;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.Hooks
{
    internal class CharacterHooks
    {
        public static List<IAsmHook> _asmHooks = new();

        public static IFunction<AddNewCharater> _addNewCharacter;
        public static IFunction<FindCharacterPointer> _findCharacter;
        public static IFunction<RemovePartyMember> _removePartyMember;

        [Function(CallingConventions.Stdcall)]
        public delegate int AddNewCharater(uint characterID);
        [Function(CallingConventions.Stdcall)]
        public delegate nuint FindCharacterPointer(int characterID);
        [Function(CallingConventions.Stdcall)]
        public delegate nuint RemovePartyMember(int characterID);

        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0;
            if (FunctionScanner.FindFunction("Add Character", "55 8B EC 83 EC 08 56 8B 75 ?? 57 56 E8 ?? ?? ?? ?? 83 C4 04",out offset))
                _addNewCharacter = hooks.CreateFunction<AddNewCharater>((int)(Mod.ModuleBase + offset));
            if(FunctionScanner.FindFunction("Find Character", "55 8B EC 57 8B 7D ?? 85 FF 75 ?? 33 C0 5F 5D C3 57 E8 ?? ?? ?? ?? 83 C4 04 85 C0 74 ?? A1 ?? ?? ?? ?? 53 56 8D B0 ?? ?? ?? ?? 8D 98 ?? ?? ?? ?? 3B F3 73 ?? 56 E8 ?? ?? ?? ?? 83 C4 04 3B C7 75", out offset))
                _findCharacter = hooks.CreateFunction<FindCharacterPointer>((int)(Mod.ModuleBase + offset));
            if(FunctionScanner.FindFunction("Remove Character", "55 8B EC 51 53 56 8B 75 ?? 56 E8 ?? ?? ?? ?? 8B D8", out offset))
                _removePartyMember = hooks.CreateFunction<RemovePartyMember>((int)(Mod.ModuleBase + offset));
        }

        public static unsafe Character* GetCharacter(CharacterId character)
        {
            var chara = (Character*)_findCharacter.GetWrapper()((int)character);
            return chara;
        }
    }
}
