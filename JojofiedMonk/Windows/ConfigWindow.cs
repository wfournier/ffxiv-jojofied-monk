using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace JojofiedMonk.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly ChatGui? chatGui;
    private readonly Configuration config;
    private readonly Plugin plugin;

    public ConfigWindow(Plugin plugin, ChatGui? chatGui) : base(
        "Jojofied Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 120);
        SizeCondition = ImGuiCond.Always;

        this.plugin = plugin;
        config = plugin.Configuration;
        this.chatGui = chatGui;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var configEnabled = config.Enabled;
        if (ImGui.Checkbox("Enabled", ref configEnabled))
        {
            config.Enabled = configEnabled;
            config.Save();
        }

        var soundOptionIndex = (int)config.SoundOption;
        if (ImGui.ListBox("Sound options", ref soundOptionIndex, plugin.SoundOptionsDict.Values.ToArray(),
                          plugin.SoundOptionsDict.Count))
        {
            config.SoundOption = (SoundOption)soundOptionIndex;
            chatGui?.Print($"[JojofiedMonk] {plugin.SoundOptionsDict[config.SoundOption]} will now be played");
            config.Save();
        }

        ImGui.Spacing();
    }
}
