using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace JojofiedMonk.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly TextureWrap jojoImage;
    private readonly Plugin plugin;

    public MainWindow(Plugin plugin, TextureWrap jojoImage) : base(
        "Jojofied", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(280, 340);

        this.jojoImage = jojoImage;
        this.plugin = plugin;
    }

    public void Dispose()
    {
        jojoImage.Dispose();
    }

    public override void Draw()
    {
        ImGui.Text($"The plugin is currently {(plugin.Configuration.Enabled ? "enabled" : "disabled")}");

        if (ImGui.Button("Show Settings")) plugin.DrawConfigUI();

        ImGui.Spacing();

        ImGui.Image(jojoImage.ImGuiHandle, new Vector2(jojoImage.Width, jojoImage.Height));
        ImGui.Text("Plugin by Cidolfus Highwind (Cactuar)\n" +
                   "wfournier on GitHub");
    }
}
