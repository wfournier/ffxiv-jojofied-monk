using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace JojofiedMonk.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Jojofied Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(232, 75);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var configValue = this.Configuration.SoundEnabled;
        if (ImGui.Checkbox("Enabled", ref configValue))
        {
            this.Configuration.SoundEnabled = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }
    }
}
