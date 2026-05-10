using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.src.Neptunia_3_Data;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;

namespace Nep3ArchipelagoClient.src.Hooks
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
            _addNewCharacter = hooks.CreateFunction<AddNewCharater>((int)(Mod.ModuleBase + 0xBADA0));
            _findCharacter = hooks.CreateFunction<FindCharacterPointer>((int)(Mod.ModuleBase + 0xBB2D0));
            _removePartyMember = hooks.CreateFunction<RemovePartyMember>((int)(Mod.ModuleBase + 0xBAF20));
        }

        public static unsafe Character* GetCharacter(CharacterId character)
        {
            var chara = (Character*)_findCharacter.GetWrapper()((int)character);
            return chara;
        }
    }
}
