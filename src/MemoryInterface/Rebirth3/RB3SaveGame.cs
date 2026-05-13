using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.MemoryInterface;
using Nep3ArchipelagoClient.Neptunia_3_Data;
using Nep3ArchipelagoClient.Neptunia_3_Data.ProgressiveGear;
using Reloaded.Memory;


namespace Nep3ArchipelagoClient
{
    internal class RB3SaveGame:SaveGame
    {
        static Memory memory = Memory.Instance;
        Inventory Inventory;

        public RB3SaveGame(UIntPtr baseAddress):base(baseAddress, 0x4F6ED8)
        {
            Inventory = new RB3Inventory(this);
            APSaveLocation = 0x103B0;
            PlanOffset = 0x1e1fc;
            
        }
        public int CurrentItemCount()
        {
            return memory.Read<int>(SaveGamePointer + 0xC7CC);
        }
        public override int CurrentDungeon()
        {
            return memory.Read<int>(SaveGamePointer - 0x1EA8FA);
        }

        public override void SetupSaveFile()
        {
            if (!IsInit && (memory.Read<byte>(SaveGamePointer + 0x91C) & 1 << 4) > 0)
            {
                memory.Write<byte>(SaveGamePointer + APSaveLocation - 17, 1);
                SetupAllNations();
                InitGear();
                UnlockStuff();
                var startchar = Mod.APClient.GetStartingCharacter();
                AddPartyMember(startchar);
                if(startchar != CharacterId.nepgear)
                    RemovePartyMember(CharacterId.nepgear);
                RemovePartyMember(CharacterId.neptune);
                DeleteChap0Flags();
                //debug stuff
#if DEBUG
                Test_Unlocks();
                Test_CharacterStruct();
                Test_End();
#endif
            }
        }
        public void SetupAllNations()
        {
            var worldMapThing = memory.Read<byte>(SaveGamePointer + 0xE04);
            worldMapThing |= 1 << 4;
            memory.Write<byte>(SaveGamePointer + 0xE04, worldMapThing);
            memory.Write<byte>(SaveGamePointer + 0xfe44, 9);
            memory.Write<byte>(SaveGamePointer + 0x6DC, 1);

            var firstItemAt = SaveGamePointer + 0xfe48 + 6;
            var target = SaveGamePointer + 0xfe48 + 6 * 9;
            byte nationIdx = 1;
            for (var item = SaveGamePointer + 0xfe48+6; item < target; item += 6)
            {
                memory.Write<byte>(item, 0x0A);
                memory.Write<byte>(item + 1, 0x01);
                memory.Write<byte>(item + 2, nationIdx);
                nationIdx++;
            }
            AddDungeon(1);
            
        }
        public void InitGear()
        {
            foreach(var character in ProgressiveGear.ProgressiveGears.Values)
            {
                character.UnlockTier(0);
            }
        }
        public override void AddDungeon(short dungeonId)
        {
            if (dungeonId == 66) return;
            var dungeonListLength = memory.Read<byte>(SaveGamePointer + 0x103b0);
            var writeInto = SaveGamePointer + (nuint)(0x103b4 + 0x203c * dungeonListLength);
            memory.Write<byte>(writeInto, 0x0F);
            memory.Write<short>(writeInto + 2, dungeonId);
            var nation = DungeonToNation.GetNation(dungeonId);
            memory.Write<byte>(writeInto + 4, (byte) nation); // 1 needs to replaced soonish for the nation id
            memory.Write<byte>(SaveGamePointer + 0x103b0, ++dungeonListLength);

        }
        public void test()
        {
            Memory memory = Memory.Instance;
            UIntPtr jmpCounter = SaveGamePointer + 0xE88;
            Console.WriteLine($"Base:{Mod.ModuleBase} Inventory:{SaveGamePointer} JumpCounter:{0xE88} Result:{jmpCounter}");
            Console.WriteLine(Memory.Instance.Read<uint>(Mod.ModuleBase + 0x4F6ED8));

            Console.WriteLine($"Inventory size = {memory.Read<int>(SaveGamePointer+0xC7CC)}");
            Console.WriteLine($"Jump Count = {memory.Read<int>(jmpCounter)}");

        }
        public void SetTrueEndFlag()
        {
            var reiEvent = memory.Read<short>(SaveGamePointer + 0x999);
            reiEvent |= 0x1FE0;
            memory.Write<short>(SaveGamePointer + 0x999, reiEvent);
        }
        public unsafe override void AddPartyMember(int characterID)
        {
            CharacterHooks._addNewCharacter.GetWrapper()((uint)characterID);
            var character = CharacterHooks.GetCharacter((CharacterId)characterID);
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
        public void UnlockStuff()
        {
            var flagPionter = CharacterHooks._findCharacter.GetWrapper()(1); // getNeptune save pointer
            flagPionter -= 305; // move to the flags
            for (nuint i = 0; i < 6; i++)
            {
                memory.Write<byte>(flagPionter + i, 0xFF);
                memory.Write<byte>(flagPionter + i + 17, 0xFF);
            }
            if (PlanHooks.ReadPlan(53) == 1)
                PlanHooks.FrocePlan(53, PlanFlags.Build);
            PlanHooks.FrocePlan(9, PlanFlags.Active);
            PlanHooks.FrocePlan(14, PlanFlags.Active);
            PlanHooks.FrocePlan(2032,PlanFlags.Found);
            PlanHooks.EnablePlan(2032);
            AddDungeon(302);
            Inventory.AddItem(241, 1);
            Inventory.AddItem(244, 1);
            Inventory.AddItem(701, 99);
            Inventory.AddItem(729, 99);
        }
        public void DeleteChap0Flags()
        {
            memory.Write<byte>(SaveGamePointer + 0x91C, 0);
            memory.Write<byte>(SaveGamePointer + 0x91D, 0);
            memory.Write<byte>(SaveGamePointer + 0x91E, 0);
        }

        public void Test_Unlocks()
        {
            foreach(int character in Enum.GetValues(typeof(CharacterId)))
            {
                for(int i  = 0; i<10;i ++)
                    ProgressiveGear.ProgressiveGears[character].IncreaseGearTier();
                if (character == 11)
                    continue;
                AddPartyMember(character);
            }
            foreach(var dungeon in DungeonToNation.link.Keys)
                AddDungeon(dungeon);
        }
        public unsafe void Test_CharacterStruct()
        {
            var character = CharacterHooks.GetCharacter(CharacterId.blanc);
            if (character == null)
                Console.WriteLine("No Character data");
            else
                Console.WriteLine(character->CurrentHP);
        }
        public void Test_End()
        {
            SetTrueEndFlag();
        }

        protected override void GoalCondition()
        {
            bool pudding = Inventory.FindItem(203, out int _);
            bool syringe = Inventory.FindItem(204, out int _);
            bool notebook = Inventory.FindItem(205, out int _);
            bool doll = Inventory.FindItem(206, out int _);
            bool drawing = Inventory.FindItem(210, out int _);
            if (pudding && syringe && notebook && doll && drawing)
            {
                SetTrueEndFlag();
            }
        }
    }
}
