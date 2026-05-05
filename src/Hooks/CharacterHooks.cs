using Nep3ArchipelagoClient.Archipelago;
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
        public static IFunction<AddNewCharater> _addNewCharacter;
        public static IFunction<FindCharacterPointer> _findCharacter;
        public static IFunction<RemovePartyMember> _removePartyMember;

        [Function(CallingConventions.Stdcall)]
        public delegate int AddNewCharater(uint characterID);
        [Function(CallingConventions.Stdcall)]
        public delegate nuint FindCharacterPointer(int characterID);
        [Function(CallingConventions.Stdcall)]
        public delegate nuint RemovePartyMember(int characterID);

        public static void SetUpHooks(IReloadedHooks hooks)
        {
            _addNewCharacter = hooks.CreateFunction<AddNewCharater>((int)(Mod.ModuleBase + 0xBADA0));
            _findCharacter = hooks.CreateFunction<FindCharacterPointer>((int)(Mod.ModuleBase + 0xBB2D0));
            _removePartyMember = hooks.CreateFunction<RemovePartyMember>((int)(Mod.ModuleBase + 0xBAF20));
        }
    }
}
