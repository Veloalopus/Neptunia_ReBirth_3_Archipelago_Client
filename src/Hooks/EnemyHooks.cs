using Nep3ArchipelagoClient.Archipelago;
using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using System.Text;
using static Nep3ArchipelagoClient.Hooks.TextHooks;

namespace Nep3ArchipelagoClient.Hooks
{
    internal class EnemyHooks
    {
        public static IReverseWrapper<OnNewEnemyKilled> _onNewEnemyKilled;
        public static List<IAsmHook> _asmHooks = new();


        [Function(new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.esi }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int OnNewEnemyKilled(int eax, int enemyID);
        public static int SendNewEnemyKilleCheck(int eax, int esi)
        {
            var enemyId = esi & 0xFFFF;
            var EnemyId = APClient.EnemyBaseID + enemyId;
            Console.WriteLine($"Killed new enemy with the ID:{enemyId}");
            Mod.APClient.SendLocation(EnemyId);
            return eax;
        }

        public static void SetupHooks(IReloadedHooks hooks)
        {
            string[] enemyKilled = {
                "use32",
                "pushad",
                "pushfd",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(SendNewEnemyKilleCheck, out _onNewEnemyKilled)}",
                "popfd",
                "popad",
            };
            if(FunctionScanner.FindFunction("Enemy ID", "66 89 31 66 89 41",out var offset))
                _asmHooks.Add(hooks.CreateAsmHook(enemyKilled, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
        }
    }
}
