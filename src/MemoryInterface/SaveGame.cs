using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.Neptunia_3_Data;
using Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient
{
    internal abstract class SaveGame
    {
        protected nuint SaveGameOffest;
        public UIntPtr SaveGamePointer => Memory.Instance.Read<uint>(Mod.ModuleBase + SaveGameOffest);
        protected uint APSaveLocation;
        public nuint PlanOffset;
        protected uint EventFlagOffset;
        Memory memory => Memory.Instance;

        protected SaveGame()
        {
        }

        public abstract int CurrentDungeon();

        public bool IsInit => memory.Read<byte>(SaveGamePointer + APSaveLocation - 17) == 1;

        
        public bool IsEventFlagSet(int EventID) => (memory.Read<byte>(SaveGamePointer + EventFlagOffset+(nuint)(EventID/8)) & 1 << (EventID % 8)) > 0;
        public void SetEventFlag(int EventID,bool Active)
        {
            var FlagRegion = memory.Read<byte>(SaveGamePointer + EventFlagOffset + (nuint)(EventID / 8));

            if (Active)
                FlagRegion |= (byte)(1 << (EventID % 8));
            else
                FlagRegion &= (byte)(0xFF - (1 << (EventID % 8)));
            memory.Write<byte>(SaveGamePointer + EventFlagOffset + (nuint)(EventID / 8), FlagRegion);
        }
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
        public abstract void CheckUnlockGoalCondition();
        public abstract bool IsGoalAchieved(long APLocation);
        
        public void AddPartyMember(CharacterId character) => AddPartyMember((int)character);
        public unsafe abstract void AddPartyMember(int characterID);

        public  void RemovePartyMember(CharacterId character) => RemovePartyMember((int)character);
        public abstract void RemovePartyMember(int characterId);

        public void ShowCharacter(CharacterId character) => ShowCharacter((int)character);
        public abstract void ShowCharacter(int characterId);
    }
}
