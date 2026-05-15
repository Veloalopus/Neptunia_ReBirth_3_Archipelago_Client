using Nep3ArchipelagoClient.Configuration;
using Nep3ArchipelagoClient.Hooks;
using Nep3ArchipelagoClient.MemoryInterface;
using Nep3ArchipelagoClient.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using System.Reflection;





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
    public static ProcessModule Module = null;
    public static Archipelago.APClient APClient = new();

    internal static SaveGame SaveGame;
    internal static Inventory Inventory;
    internal static NeptuniaGame Game;

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
        Module = Process.GetCurrentProcess().MainModule;
        ModuleBase = (UIntPtr)Module.BaseAddress;
        switch(Module.ModuleName){
            case "NeptuniaReBirth1.exe":
                Game = NeptuniaGame.Neptunia_ReBirth_1;
                APClient.Game = "Hyperdimension Neptunia Re;Birth1";
                SaveGame = new RB1SaveGame();
                Inventory = new RB1Inventory(SaveGame);
                break;
            case "NeptuniaReBirth2.exe":
                Game = NeptuniaGame.Neptunia_ReBirth_2;
                APClient.Game = "Hyperdimension Neptunia Re;Birth2 Sisters Generation";
                SaveGame = new RB2SaveGame();
                Inventory = new RB2Inventory(SaveGame);
                break;
            case "NeptuniaReBirth3.exe":
            default:
                Game = NeptuniaGame.Neptunia_ReBirth_3;
                APClient.Game = "Hyperdimension Neptunia Re;Birth3 V GENERATION";
                SaveGame = new RB3SaveGame();
                Inventory = new RB3Inventory(SaveGame);
                break;
        }
        Console.WriteLine($"Playing: {Game.ToString()}");
        Hooks.Hooks.SetupAllHooks(_hooks);

        APClient.ConnectToServer(_configuration.Server, _configuration.Port, _configuration.Player);

        _loop = Task.Run(MainLoop);
    }
    Task _loop;

    public static void MainLoop()
    {
        while (true)
        {
            if (SaveGame.SaveGamePointer == 0) continue;
            APClient.update();
            SaveGame.SetupSaveFile();
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