using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.Hooks.Rebirth2;
using Reloaded.Hooks.Definitions;


namespace Nep3ArchipelagoClient.Hooks.Rebirth2
{
    public class Hooks
    {
        public static void SetupHooks(IReloadedHooks hooks)
        {
            RB2ItemCollectionHooks.SetupHooks(hooks);
            RB2TextHooks.SetupHooks(hooks);
        }
    }
}
