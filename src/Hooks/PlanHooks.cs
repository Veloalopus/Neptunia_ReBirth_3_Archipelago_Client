using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;

namespace Nep3ArchipelagoClient.src.Hooks
{
    public enum PlanFlags : byte
    {
        NotFound = 0,
        Found = 1,
        Build = 3,
        Active = 7,
    }
    internal class PlanHooks
    {
        public static List<IAsmHook> _asmHooks = new();
        public static byte ReadPlan(int planId) => Memory.Instance.Read<byte>(SaveGame.SaveGamePointer + (nuint)(planId + 0x1e1fc) * 8);
        public static void FrocePlan(int planId, PlanFlags flag) => Memory.Instance.Write<byte>(SaveGame.SaveGamePointer + (nuint)(planId + 0x1e1fc) * 8, (byte)flag);

        public static IFunction<TooglePlan> _TogglePlan;

        [Function(CallingConventions.Stdcall)]
        public delegate int TooglePlan(int planID, bool enable);

        [Function(new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.esi }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int OnPlanChanged(int planID, int enemyID);
        public static IReverseWrapper<OnPlanChanged> _onPlanChanged;


        public static void SetupHooks(IReloadedHooks hooks)
        {
            _TogglePlan = hooks.CreateFunction<TooglePlan>((int)(Mod.ModuleBase + 0xBE6E0));

            string[] planChanged = {
                "use32",
                "pushad",
                "pushfd",
                "mov eax,[ebp+0x8]",
                "mov esi,[ebp+0xC]",
                $"{hooks.Utilities.GetAbsoluteCallMnemonics(PrintPlanID, out _onPlanChanged)}",
                "popfd",
                "popad",
            };
            _asmHooks.Add(hooks.CreateAsmHook(planChanged, (int)(Mod.ModuleBase + 0xBE6E3), AsmHookBehaviour.ExecuteFirst).Activate());
        }
        public static int PrintPlanID(int planID,int active)
        {
            string toggle = "disabled";
            if (active != 0)
                toggle = "enabled";
            Console.WriteLine($"Plan ID: {planID} {toggle}");
            return 0;
        }
    }
}
