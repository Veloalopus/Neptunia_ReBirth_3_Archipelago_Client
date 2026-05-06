using Reloaded.Hooks.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nep3ArchipelagoClient.src.Hooks
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
        }
    }
}
