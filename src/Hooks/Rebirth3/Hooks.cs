using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.Hooks.Rebirth2;
using Reloaded.Hooks.Definitions;


namespace Nep3ArchipelagoClient.Hooks.Rebirth3
{
    public class Hooks
    {
        public static void SetupHooks(IReloadedHooks hooks)
        {
            RB3ItemCollectionHooks.SetupHooks(hooks);
            RB3TextHooks.SetupHooks(hooks);
        }
    }
}
