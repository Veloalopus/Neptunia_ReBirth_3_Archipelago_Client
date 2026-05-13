using Nep3ArchipelagoClient.Hooks.Rebirth2;
using Reloaded.Hooks.Definitions;


namespace Nep3ArchipelagoClient.Hooks
{
    internal class Hooks
    {
        public static void SetupAllHooks(IReloadedHooks hooks)
        {
            CharacterHooks.SetupHooks(hooks);
            EnemyHooks.SetupHooks(hooks);
            ItemCollectionHooks.SetupHooks(hooks);
            TextHooks.SetupHooks(hooks);
            PlanHooks.SetupHooks(hooks);
            ShopHook.SetupHooks(hooks);
            switch (Mod.Game)
            {
                case NeptuniaGame.Neptunia_ReBirth_2:
                    Rebirth2.Hooks.SetupHooks(hooks);
                    break;
                case NeptuniaGame.Neptunia_ReBirth_3:
                    Rebirth3.Hooks.SetupHooks(hooks);
                    break;
            }
        }
    }
}
