using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace JojofiedMonk;

[Serializable]
public class Configuration : IPluginConfiguration
{
    private Plugin? plugin;

    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    public bool Enabled { get; set; } = true;
    public SoundOption SoundOption { get; set; } = SoundOption.Ora;
    public int Version { get; set; } = 0;

    public void Initialize(DalamudPluginInterface plInterface, Plugin? pl)
    {
        pluginInterface = plInterface;
        plugin = pl;
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);

        if (!Enabled) plugin?.StopSound();
    }
}
