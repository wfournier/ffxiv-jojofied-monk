using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace JojofiedMonk.Windows;

public class MainWindow : Window, IDisposable
{
    private TextureWrap jojoImage;
    private Plugin Plugin;

    public MainWindow(Plugin plugin, TextureWrap jojoImage) : base(
        "Jojofied", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.jojoImage = jojoImage;
        this.Plugin = plugin;
    }

    public void Dispose()
    {
        this.jojoImage.Dispose();
    }

    public override void Draw()
    {
        ImGui.Text($"The plugin is currently {(this.Plugin.Configuration.SoundEnabled ? "enabled" : "disabled")}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }

        ImGui.Spacing();

        ImGui.Image(this.jojoImage.ImGuiHandle, new Vector2(this.jojoImage.Width, this.jojoImage.Height));
        ImGui.Text("Plugin by wfournier (Cidolfus Highwind)");
    }
}
