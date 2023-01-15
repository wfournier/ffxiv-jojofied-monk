using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using JojofiedMonk;

namespace JojofiedMonk
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool SoundEnabled { get; set; } = true;
        public SoundOption SoundOption { get; set; } = SoundOption.ORA;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
