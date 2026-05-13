using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.Neptunia_3_Data;
using Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient
{
    internal abstract class SaveGame
    {
        public UIntPtr SaveGamePointer = 0;
        protected uint APSaveLocation;
        public nuint PlanOffset;

        Memory memory => Memory.Instance;

        protected SaveGame(UIntPtr baseAddress,uint offset)
        {
            while (SaveGamePointer == 0)
            {
                SaveGamePointer = Memory.Instance.Read<uint>(Mod.ModuleBase + offset);
                Thread.Sleep(100);
            }
        }

        public abstract int CurrentDungeon();

        public bool IsInit => memory.Read<byte>(SaveGamePointer + APSaveLocation - 17) == 1;

        public abstract void SetupSaveFile();

        public abstract void AddDungeon(short dungeonId);

        public int GetCurrentApItemCount()
        {
            return Memory.Instance.Read<int>(SaveGamePointer + APSaveLocation - 16);
        }

        public void IncrementCurrentApItemCount()
        {
            var value = Memory.Instance.Read<int>(SaveGamePointer + APSaveLocation - 16) + 1;
            Memory.Instance.Write<int>(SaveGamePointer + APSaveLocation - 16, value);
        }
        protected abstract void GoalCondition();
        
        public void AddPartyMember(CharacterId character) => AddPartyMember((int)character);
        public unsafe abstract void AddPartyMember(int characterID);

        public  void RemovePartyMember(CharacterId character) => RemovePartyMember((int)character);
        public abstract void RemovePartyMember(int characterId);

        public void ShowCharacter(CharacterId character) => ShowCharacter((int)character);
        public abstract void ShowCharacter(int characterId);
    }
}
