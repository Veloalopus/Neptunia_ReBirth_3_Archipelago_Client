using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.MemoryInterface;
using Nep3ArchipelagoClient.Neptunia_2_Data;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient
{
    internal class RB2SaveGame : SaveGame
    {
        static Memory memory = Memory.Instance;
        Inventory Inventory;

        public RB2SaveGame()
        {
            SaveGameOffest = 0x443310;
            Inventory = new RB2Inventory(this);
            APSaveLocation = 0x1032c;
            PlanOffset = 0x443310;
            EventFlagOffset = 0x91c;
        }
        public int CurrentItemCount()
        {
            return memory.Read<int>(SaveGamePointer + 0x443310);
        }
        public override int CurrentDungeon()
        {
            return memory.Read<int>(SaveGamePointer - 0x12B6F4);
        }

        public override void SetupSaveFile()
        {
            if (!IsInit && IsEventFlagSet(658))
            {
                //debug stuff
                memory.Write<byte>(SaveGamePointer + APSaveLocation - 17, 1);
                for (int i = 1; i < 55; i++)
                    SetEventFlag(i, false);
#if DEBUG
                Test_CharacterUnlock();
                Test_DungeonUnlock();
                Test_VGMRun();
                Test_Goal();
                #endif
            }
        }
        public override void AddDungeon(short dungeonId)
        {
            nuint dungeonOffset = 0x10330;
            nuint dungeonLenghtOffset = 0x1032c;
            var dungeonListLength = memory.Read<byte>(SaveGamePointer + dungeonLenghtOffset);
            var writeInto = SaveGamePointer + dungeonOffset + (nuint)(0x203c * dungeonListLength);
            memory.Write<byte>(writeInto, 0x0F);
            memory.Write<short>(writeInto + 2, dungeonId);
            memory.Write<byte>(writeInto + 4, (byte)1);
            memory.Write<byte>(SaveGamePointer + dungeonLenghtOffset, ++dungeonListLength);
        }
        public unsafe override void AddPartyMember(int characterID)
        {
            CharacterHooks._addNewCharacter.GetWrapper()((uint)characterID);
            var character = (Character*)CharacterHooks.GetCharacter(characterID);
            if (character == null) return;
            character->Armor = 1632;
        }
        public override void RemovePartyMember(int characterId) => CharacterHooks._removePartyMember.GetWrapper()(characterId);

        public override void ShowCharacter(int characterId)
        {
            var characterPoint = CharacterHooks._findCharacter.GetWrapper()(characterId);
            if (characterPoint == 0) return;
            var currentVal = memory.Read<byte>(characterPoint);
            currentVal &= 0xff - 0x80;
            memory.Write<byte>(characterPoint, currentVal);
        }
        public override void CheckUnlockGoalCondition()
        {
            bool old_sword = Inventory.FindItem(254, out int _);
            if (!old_sword) return;
            SetEventFlag(522, true);
            SetEventFlag(523, true);
            
        }
        void Test_VGMRun()
        {
            Inventory.AddItem(1655, 4);
            Inventory.AddItem(1305, 1);
            Inventory.AddItem(40, 99);

        }
        void Test_DungeonUnlock()
        {
            for(short i = 1; i < 37; i++)
            {
                AddDungeon(i);
            }
        }
        void Test_CharacterUnlock()
        {
            for(short i = 1; i < 25; i++)
            {
                RemovePartyMember(i);
            }
            Thread.Sleep(5000);
            for(short i = 1; i < 25; i++)
            {
                AddPartyMember(i);
            }
        }
        void Test_Goal()
        {
            Inventory.AddItem(254, 1);
        }

        public override bool IsGoalAchieved(long APLocation)
        {
            return APLocation == APClient.EnemyBaseID + 1055;
        }

    }
}
