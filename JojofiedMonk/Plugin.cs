using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using JojofiedMonk.Windows;

namespace JojofiedMonk;

public sealed class Plugin : IDalamudPlugin
{
    private const string jojoCommand = "/jojo";
    private const string jojoSettings = "/jojosettings";

    private readonly SoundPlayer SoundPlayer = new();

    private float pbTimer;

    public Dictionary<SoundOption, string> soundOptionsDict = new()
    {
        { SoundOption.ORA, "Ora Ora" },
        { SoundOption.MUDA, "Muda  Muda" }
    };

    public WindowSystem WindowSystem = new("JojofiedMonk");

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager)
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface, this);

        var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "jojo.png");
        var jojoImage = PluginInterface.UiBuilder.LoadImage(imagePath);

        WindowSystem.AddWindow(new ConfigWindow(this, chatGui));
        WindowSystem.AddWindow(new MainWindow(this, jojoImage));

        CommandManager.AddHandler(jojoCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open main plugin window"
        });

        CommandManager.AddHandler(jojoSettings, new CommandInfo(OnSettingsCommand)
        {
            HelpMessage = "Open plugin settings"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        Framework.Update += CheckPerfectBalanceIsCast;
    }

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    public Configuration Configuration { get; init; }

    [PluginService]
    public ChatGui chatGui { get; private set; }

    [PluginService]
    public static Framework Framework { get; private set; } = null!;

    [PluginService]
    public static ClientState ClientState { get; set; }

    public string Name => "Jojofied Monk";

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        CommandManager.RemoveHandler(jojoCommand);
    }

    private void OnCommand(string command, string args)
    {
        switch (args)
        {
            case "on":
            case "toggle" when !Configuration.Enabled:
            case "t" when !Configuration.Enabled:
                Configuration.Enabled = true;
                Configuration.Save();
                chatGui.Print("[JojofiedMonk] is now enabled");
                break;
            case "off":
            case "toggle" when Configuration.Enabled:
            case "t" when Configuration.Enabled:
                Configuration.Enabled = false;
                Configuration.Save();
                chatGui.Print("[JojofiedMonk] is now disabled");
                break;
            case "ora":
                Configuration.SoundOption = SoundOption.ORA;
                Configuration.Save();
                chatGui.Print($"[JojofiedMonk] {soundOptionsDict[SoundOption.ORA]} will now be played");
                break;
            case "muda":
                Configuration.SoundOption = SoundOption.MUDA;
                Configuration.Save();
                chatGui.Print($"[JojofiedMonk] {soundOptionsDict[SoundOption.MUDA]} will now be played");
                break;
            case "play":
            case "p":
            case "test":
                if (Configuration.Enabled)
                {
                    PlaySound();
                    chatGui.Print("[JojofiedMonk] Playing test sound");
                }

                break;
            case "stop":
            case "s":
                StopSound();
                chatGui.Print("[JojofiedMonk] Stopping sound");
                break;
            case "":
                // in response to the slash command, just display our main ui
                WindowSystem.GetWindow("Jojofied").IsOpen = true;
                break;
            default:
                chatGui.Print("[JojofiedMonk] Invalid usage: Command must be \"/jojo <option>\"\n" +
                              "on / off / toggle - Enables or disables sound\n" +
                              "ora / muda - Changes the sound that will be played\n" +
                              "play / test - Plays the configured sound\n" +
                              "stop - Stop the sound");
                break;
        }
    }

    private void OnSettingsCommand(string command, string args)
    {
        DrawConfigUI();
    }

    private void CheckPerfectBalanceIsCast(Framework framework)
    {
        if (Configuration.Enabled)
        {
            using var enumerator = ClientState.LocalPlayer?.StatusList.GetEnumerator();
            while (enumerator != null && enumerator.MoveNext())
            {
                var status = enumerator.Current;
                if (status.GameData.Name == "Perfect Balance")
                {
                    // For some god forsaken reason, the remaining time starts at -20 instead of 20, so we use Math.Abs()
                    var time = Math.Abs(status.RemainingTime);
                    var stacks = status.StackCount;

                    // If the time remaining is higher than previous update and stack count is 3 (meaning we just cast PB)
                    if (time > pbTimer && stacks == 3)
                        PlaySound();

                    pbTimer = time;
                    return;
                }
            }

            // If Perfect Balance is not in the list of statuses, manually set timer to 0 just in case
            pbTimer = 0f;
        }
    }

    public void PlaySound()
    {
        if (Configuration.Enabled)
        {
            var filename = Configuration.SoundOption switch
            {
                SoundOption.ORA => "ora.wav",
                SoundOption.MUDA => "muda.wav",
                _ => throw new ArgumentOutOfRangeException()
            };

            var path = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, filename);

            SoundPlayer.SoundLocation = path;
            SoundPlayer.Play();
        }
    }

    public void StopSound()
    {
        SoundPlayer.Stop();
    }


    private void DrawUI()
    {
        WindowSystem.Draw();
    }

    public void DrawConfigUI()
    {
        WindowSystem.GetWindow("Jojofied Configuration").IsOpen = true;
    }
}
