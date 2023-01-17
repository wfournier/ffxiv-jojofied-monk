using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace JojofiedMonk;

[Serializable]
public class Configuration : IPluginConfiguration
{
    private Plugin plugin;

    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    public bool Enabled { get; set; } = true;
    public SoundOption SoundOption { get; set; } = SoundOption.ORA;
    public int Version { get; set; } = 0;

    public void Initialize(DalamudPluginInterface pluginInterface, Plugin plugin)
    {
        PluginInterface = pluginInterface;
        this.plugin = plugin;
    }

    public void Save()
    {
        PluginInterface!.SavePluginConfig(this);

        if (!Enabled) plugin.StopSound();
    }
}
