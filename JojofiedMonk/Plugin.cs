using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using JojofiedMonk.Windows;

namespace JojofiedMonk
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Jojofied Monk";

        private const string jojoCommand = "/jojo";
        private const string jojoSettings = "/jojosettings";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("JojofiedMonk");

        [PluginService]
        public ChatGui chatGui { get; private set; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "jojo.png");
            var jojoImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this, jojoImage));

            this.CommandManager.AddHandler(jojoCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open main plugin window"
            });

            this.CommandManager.AddHandler(jojoSettings, new CommandInfo(OnSettingsCommand)
            {
                HelpMessage = "Open plugin settings"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(jojoCommand);
        }

        private void OnCommand(string command, string args)
        {
            switch (args)
            {
                case "on":
                case "toggle" when !Configuration.SoundEnabled:
                case "t" when !Configuration.SoundEnabled:
                    Configuration.SoundEnabled = true;
                    Configuration.Save();
                    chatGui.Print("Jojofied is now enabled");
                    break;
                case "off":
                case "toggle" when Configuration.SoundEnabled:
                case "t" when Configuration.SoundEnabled:
                    Configuration.SoundEnabled = false;
                    Configuration.Save();
                    chatGui.Print("Jojofied is now disabled");
                    break;
                case "ora":
                    Configuration.SoundOption = SoundOption.ORA;
                    Configuration.Save();
                    chatGui.Print("Jojofied sound option is now Ora Ora");
                    break;
                case "muda":
                    Configuration.SoundOption = SoundOption.MUDA;
                    Configuration.Save();
                    chatGui.Print("Jojofied sound option is now Muda Muda");
                    break;
                case "":
                    // in response to the slash command, just display our main ui
                    WindowSystem.GetWindow("Jojofied").IsOpen = true;
                    break;
                default:
                    chatGui.Print("Invalid usage: Command must be \"/jojo <option>\"\n" +
                                  "on / off / toggle - Enables or disables sound\n" +
                                  "ora / muda - Changes the sound that will be played");
                    break;
            }
        }

        private void OnSettingsCommand(string command, string args)
        {
            chatGui.Print("settings");
            DrawConfigUI();
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            this.WindowSystem.GetWindow("Jojofied Configuration").IsOpen = true;
        }
    }
}
