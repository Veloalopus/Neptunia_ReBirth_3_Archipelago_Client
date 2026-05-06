using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Nep3ArchipelagoClient.Template;
using Nep3ArchipelagoClient.Configuration;
using Nep3ArchipelagoClient.MemoryInterface;
using Nep3ArchipelagoClient.src;
using Nep3ArchipelagoClient.src.Hooks;
using System.Diagnostics;






namespace Nep3ArchipelagoClient;

/// <summary>
/// Your mod logic goes here.
/// </summary>
public class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    public static UIntPtr ModuleBase = 0x400000;
    public static Archipelago.APClient APClient = new();

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

#if DEBUG
        // Attaches debugger in debug mode; ignored in release.
        Debugger.Launch();
#endif

        ModuleBase = (UIntPtr)Process.GetCurrentProcess().MainModule!.BaseAddress;

        Hooks.SetupAllHooks(_hooks);
        // For more information about this template, please see
        // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

        // If you want to implement e.g. unload support in your mod,
        // and some other neat features, override the methods in ModBase.

        // TODO: Implement some mod logic


        APClient.ConnectToServer(_configuration.Server, _configuration.Port, _configuration.Player);

        var t = new Thread(start: MainLoop);
        t.Start();
    }

    internal static SaveGame SaveGame;
    internal static Inventory Inventory;
    static void MainLoop()
    {
        bool test = true;
        SaveGame = new(ModuleBase);
        Inventory = new();
        while (true)
        {
            Thread.Sleep(100);
            APClient.update();
            SaveGame.SetupSaveFile();
            if(!SaveGame.DoOnceAfterChapter1Start && test)
            {
                Thread.Sleep(1_000);
                test = false;
                for (int i = 1; i < 68; i++)
                {
                    //SaveGame.AddDungeon((byte)i);
                }
            }
        }
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}