using Nep3ArchipelagoClient.src.Hooks;
using Nep3ArchipelagoClient.src.Neptunia_3_Data;
using Nep3ArchipelagoClient.src.Neptunia_3_Data.ProgressiveGear;
using Reloaded.Memory;
using System;
using System.Collections.Generic;

namespace Nep3ArchipelagoClient
{
    internal class SaveGame
    {
        public static UIntPtr SaveGamePointer = 0;
        uint DungeonCountOffset = 0x103B0;
        static Memory memory = Memory.Instance;

        public SaveGame(UIntPtr baseAddress)
        {
            while (SaveGamePointer == 0)
            {
                SaveGamePointer = Memory.Instance.Read<uint>(Mod.ModuleBase + 0x4F6ED8);
                Thread.Sleep(100);
            }
        }
        public int CurrentItemCount()
        {
            return memory.Read<int>(SaveGamePointer + 0xC7CC);
        }
        public int CurrentDungeon()
        {
            return memory.Read<int>(SaveGamePointer - 0x1EA8FA);
        }

        public bool DoOnceAfterChapter1Start => memory.Read<byte>(SaveGamePointer + DungeonCountOffset - 17) == 0;
        public void SetupSaveFile()
        {
            if (DoOnceAfterChapter1Start && (memory.Read<byte>(SaveGamePointer + 0x91C) & 1 << 4) > 0)
            {
                memory.Write<byte>(SaveGamePointer + DungeonCountOffset - 17, 1);
                SetupAllNations();
                InitGear();
                UnlockStuff();
                AddPartyMember(Mod.APClient.GetStartingCharacter());
                RemovePartyMember(CharacterId.nepgear);
                RemovePartyMember(CharacterId.neptune);
                DeleteChap0Flags();
                //debug stuff
                Test_Unlocks();
            }
        }
        public void SetupAllNations()
        {
            var worldMapThing = memory.Read<byte>(SaveGamePointer + 0xE04);
            worldMapThing |= 1 << 4;
            memory.Write<byte>(SaveGamePointer + 0xE04, worldMapThing);
            memory.Write<byte>(SaveGamePointer + 0xfe44, 9);
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
        public void AddDungeon(byte dungeonId)
        {
            if (dungeonId == 66) return;
            var dungeonListLength = memory.Read<byte>(SaveGamePointer + 0x103b0);
            var writeInto = SaveGamePointer + (nuint)(0x103b4 + 0x203c * dungeonListLength);
            memory.Write<byte>(writeInto, 0x0F);
            memory.Write<byte>(writeInto + 2, dungeonId);
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
        public int GetCurrentApItemCount()
        {
            return Memory.Instance.Read<int>(SaveGamePointer + DungeonCountOffset - 16);
        }
        public void IncrementCurrentApItemCount()
        {
            var value = Memory.Instance.Read<int>(SaveGamePointer + DungeonCountOffset - 16)+1;
            Memory.Instance.Write<int>(SaveGamePointer + DungeonCountOffset - 16,value);
        }
        public void SetTrueEndFlag()
        {
            var reiEvent = memory.Read<byte>(SaveGamePointer + 0x999);
            reiEvent |= 0xFE;
            memory.Write<byte>(SaveGamePointer + 0x999, reiEvent);
        }
        public static void AddItem(int id, int quantity)
        {
            ItemCollectionHooks._addItemFunction.GetWrapper()((uint)id, (uint)quantity, (char)1);

            //goal condition
            bool pudding = Mod.Inventory.FindItem(203, out int _);
            bool syringe = Mod.Inventory.FindItem(204, out int _);
            bool notebook = Mod.Inventory.FindItem(205, out int _);
            bool doll = Mod.Inventory.FindItem(206, out int _);
            bool drawing = Mod.Inventory.FindItem(210, out int _);
            if (pudding && syringe && notebook && doll && drawing)
            {
                Mod.SaveGame.SetTrueEndFlag();
            }
        }
        public static void AddPartyMember(CharacterId character) => AddPartyMember((int)character);
        public static void AddPartyMember(int characterID) => CharacterHooks._addNewCharacter.GetWrapper()((uint)characterID);

        public static void RemovePartyMember(CharacterId character) => RemovePartyMember((int)character);
        public static void RemovePartyMember(int characterId) => CharacterHooks._removePartyMember.GetWrapper()(characterId);

        public static void ShowCharacter(CharacterId character) => ShowCharacter((int)character);
        public static void ShowCharacter(int characterId)
        {
            var characterPoint = CharacterHooks._findCharacter.GetWrapper()(characterId);
            if (characterPoint == 0) return;
            var currentVal = memory.Read<byte>(characterPoint);
            currentVal &= 0xff - 0x80;
            memory.Write<byte>(characterPoint, currentVal);
        }
        public static void UnlockStuff()
        {
            var flagPionter = CharacterHooks._findCharacter.GetWrapper()(1); // getNeptune save pointer
            flagPionter -= 305; // move to the flags
            for(nuint i = 0; i<5;i ++)
                memory.Write<byte>(flagPionter+i, 0xFF);
            if (PlanHooks.ReadPlan(53) == 1)
                PlanHooks.FrocePlan(53, PlanFlags.Build);
        }
        public static void DeleteChap0Flags()
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
                    ProgressiveGear.ProgressiveGears[(CharacterId)character].IncreaseGearTier();
                if (character == 11)
                    continue;
                AddPartyMember(character);
            }
            foreach(var dungeon in DungeonToNation.link.Keys)
                AddDungeon(dungeon);
            
        }
    }
}
