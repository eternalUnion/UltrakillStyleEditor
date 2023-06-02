using BepInEx;

namespace UltrakillStyleEditor
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_NAME = "StyleEditor";
        public const string PLUGIN_GUID = "eternalUnion.ultrakill.styleEditor";
        public const string PLUGIN_VERSION = "1.0.0";

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        }
    }
}
