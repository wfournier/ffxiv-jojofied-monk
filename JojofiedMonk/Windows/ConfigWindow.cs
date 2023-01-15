using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using ImGuiNET;

namespace JojofiedMonk.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Plugin plugin;
    private Configuration Configuration;
    private ChatGui chatGui;

    public ConfigWindow(Plugin plugin, ChatGui chatGui) : base(
        "Jojofied Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(300, 120);
        this.SizeCondition = ImGuiCond.Always;

        this.plugin = plugin;
        this.Configuration = plugin.Configuration;
        this.chatGui = chatGui;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var configEnabled = this.Configuration.SoundEnabled;
        if (ImGui.Checkbox("Enabled", ref configEnabled))
        {
            this.Configuration.SoundEnabled = configEnabled;
            this.Configuration.Save();
        }

        var soundOptionIndex = (int)Configuration.SoundOption;
        if (ImGui.ListBox("Sound options", ref soundOptionIndex, plugin.soundOptionsDict.Values.ToArray(), plugin.soundOptionsDict.Count))
        {
            this.Configuration.SoundOption = (SoundOption)soundOptionIndex;
            chatGui.Print($"{plugin.soundOptionsDict[Configuration.SoundOption]} will now be played");
            this.Configuration.Save();
        }

        ImGui.Spacing();
    }
}
