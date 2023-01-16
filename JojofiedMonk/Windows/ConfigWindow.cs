using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace JojofiedMonk.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly ChatGui chatGui;
    private readonly Configuration Configuration;
    private readonly Plugin plugin;

    public ConfigWindow(Plugin plugin, ChatGui chatGui) : base(
        "Jojofied Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 120);
        SizeCondition = ImGuiCond.Always;

        this.plugin = plugin;
        Configuration = plugin.Configuration;
        this.chatGui = chatGui;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var configEnabled = Configuration.SoundEnabled;
        if (ImGui.Checkbox("Enabled", ref configEnabled))
        {
            Configuration.SoundEnabled = configEnabled;
            Configuration.Save();
        }

        var soundOptionIndex = (int)Configuration.SoundOption;
        if (ImGui.ListBox("Sound options", ref soundOptionIndex, plugin.soundOptionsDict.Values.ToArray(),
                          plugin.soundOptionsDict.Count))
        {
            Configuration.SoundOption = (SoundOption)soundOptionIndex;
            chatGui.Print($"[JojofiedMonk] {plugin.soundOptionsDict[Configuration.SoundOption]} will now be played");
            Configuration.Save();
        }

        ImGui.Spacing();
    }
}
