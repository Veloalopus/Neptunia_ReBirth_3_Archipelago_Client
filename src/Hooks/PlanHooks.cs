using Nep3ArchipelagoClient.MemoryInterface;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;

namespace Nep3ArchipelagoClient.Hooks
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
        public static byte ReadPlan(int planId) => Memory.Instance.Read<byte>(Mod.SaveGame.SaveGamePointer + ((nuint)planId + Mod.SaveGame.PlanOffset) * 8);
        public static void FrocePlan(int planId, PlanFlags flag) => Memory.Instance.Write<byte>(Mod.SaveGame.SaveGamePointer + ((nuint)planId + Mod.SaveGame.PlanOffset) * 8, (byte)flag);

        public static IFunction<TooglePlan> _TogglePlan;

        [Function(CallingConventions.Stdcall)]
        public delegate int TooglePlan(int planID, int enable);

        [Function(new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.esi }, FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
        public delegate int OnPlanChanged(int planID, int enemyID);
        public static IReverseWrapper<OnPlanChanged> _onPlanChanged;


        public static void SetupHooks(IReloadedHooks hooks)
        {
            nuint offset = 0;
            if(FunctionScanner.FindFunction("Toggle Plan", "55 8B EC 80 7D ?? 00 A1 ?? ?? ?? ?? 53",out offset))
                _TogglePlan = hooks.CreateFunction<TooglePlan>((int)(Mod.ModuleBase + offset));

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
            if(FunctionScanner.FindFunction("Plan Status Changed", "80 7D ?? 00 A1 ?? ?? ?? ?? 53",out offset))
                _asmHooks.Add(hooks.CreateAsmHook(planChanged, (int)(Mod.ModuleBase + offset), AsmHookBehaviour.ExecuteFirst).Activate());
        }
        public static void EnablePlan(int planID) => _TogglePlan.GetWrapper()(planID,1);
        public static int PrintPlanID(int planID,int active)
        {
            string toggle = "disabled";
            if (active != 0)
                toggle = "enabled";
            Console.WriteLine($"Plan ID: {planID} {active}");
            return 0;
        }
    }
}
