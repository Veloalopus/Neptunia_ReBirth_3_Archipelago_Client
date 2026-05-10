using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Nep3ArchipelagoClient.src.Hooks;
using Nep3ArchipelagoClient.src.Neptunia_3_Data;
using Nep3ArchipelagoClient.src.Neptunia_3_Data.ProgressiveGear;
using Newtonsoft.Json.Linq;

namespace Nep3ArchipelagoClient.Archipelago
{
    public class APClient
    {
        //checks
        public const long TreasureBaseID = 1_000_000;
        public const long EnemyBaseID = 2_000_000;
        //items
        public const long DungeonBaseID = 2_000_000;
        const long ChracterBaseID = 3_000_000;
        const long ProgressiveGearID = 3_500_000;
        private ArchipelagoSession? Session;
        private LoginResult? loginResult = null;
        public bool IsConnected => Session != null && Session.Socket.Connected;
        private long PlayerID = 0;
        private Dictionary<long, ScoutedItemInfo> ItemAtLocation = new();
        internal CharacterId StartingCharacter = CharacterId.neptune;
        public bool ConnectToServer(string destination,int port,string user, string password = "")
        {
            if (Session != null && Session.Socket.Connected)
            {
                Session.Socket.DisconnectAsync().Wait();
            }
            Session = ArchipelagoSessionFactory.CreateSession(destination, port);
            try
            {
                loginResult = Session.TryConnectAndLogin("Hyperdimension Neptunia Re;Birth3 V GENERATION", user, ItemsHandlingFlags.AllItems, password: password);
            }
            catch(Exception e)
            {
                loginResult = new LoginFailure(e.GetBaseException().Message);
            }
            if (!loginResult.Successful)
            {
                LoginFailure failure = (LoginFailure)loginResult;
                string errorMessage = $"Failed to Connect as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }
                Console.WriteLine(errorMessage);
                return false;
            }
            LoginSuccessful success = (LoginSuccessful)loginResult;
            InitalizeItemNameLookup();
            InitSlotData(success.SlotData);
            return true;
        }
        private void InitSlotData(Dictionary<string,object> slotData)
        {
            if (slotData.ContainsKey("start_character"))
                StartingCharacter = (CharacterId)(int)(long)slotData["start_character"];
        }

        private async void InitalizeItemNameLookup()
        {
            long[] locations = Session.Locations.AllLocations.ToArray();
            ItemAtLocation = new(await Session.Locations.ScoutLocationsAsync(locations));
        }

        internal CharacterId GetStartingCharacter() => StartingCharacter;
        public void GetItemName(long id,ref byte[] output)
        {
            for (int i = 0; i< output.Length; i++)
            {
                output[i] = 0;
            }
            if (IsConnected)
            {
                if (!ItemAtLocation.ContainsKey(id))
                {
                    "Location not Found"u8.ToArray().CopyTo(output, 0);
                    return;
                }
                var itemName = ItemAtLocation[id].ItemName;
                int idx = 0;
                if (String.IsNullOrEmpty(itemName))
                    "No Itemname Found"u8.ToArray().CopyTo(output, 0);
                else
                    foreach (char c in itemName)
                    {
                        if (!(idx < output.Length)) break;
                        output[idx] = ((byte)c);
                        idx++;
                    }
            }
            else
            {
                "Not Connected"u8.ToArray().CopyTo(output, 0);
            }
        }
        public void CheckIfGoaled(long id)
        {
            //currently only Rei kill
            if (id == EnemyBaseID + 1042)
                Session.SetGoalAchieved();
                
        }
        public bool SendLocation(long id)
        {
            if (IsConnected)
            {
                CheckIfGoaled(id);
                Session.Locations.CompleteLocationChecks(id);
                return true;
            }
            return false;
        }
        public void update()
        {
            if (IsConnected && !Mod.SaveGame.DoOnceAfterChapter1Start)
            {
                int currentItemNr = Mod.SaveGame.GetCurrentApItemCount();
                if (currentItemNr < Session.Items.AllItemsReceived.Count)
                {
                    var itemId = Session.Items.AllItemsReceived[currentItemNr].ItemId;
                    if (itemId > DungeonBaseID && itemId < DungeonBaseID + 1_000_000)
                        Mod.SaveGame.AddDungeon((byte)(itemId - DungeonBaseID));
                    else if (itemId > ChracterBaseID && itemId < ProgressiveGearID)
                        CharacterHooks._addNewCharacter.GetWrapper()((uint)(itemId - ChracterBaseID));
                    else if (itemId > ProgressiveGearID)
                        ProgressiveGear.ProgressiveGears[(int)(itemId - ProgressiveGearID)].IncreaseGearTier();
                    else
                        SaveGame.AddItem((int)itemId, 1);
                    Mod.SaveGame.IncrementCurrentApItemCount();
                }
            }
        }
    }
}
