using BepInEx;
using HarmonyLib;

namespace UltrakillStyleEditor
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(PluginConfig.PluginConfiguratorController.PLUGIN_GUID, "1.7.0")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_NAME = "StyleEditor";
        public const string PLUGIN_GUID = "eternalUnion.ultrakill.styleEditor";
        public const string PLUGIN_VERSION = "1.0.3";

        public static Harmony harmony;

        private void Awake()
        {
            // Plugin startup logic
            ConfigManager.Init();

            harmony = new Harmony($"{PLUGIN_GUID}");
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        }
    }
}
