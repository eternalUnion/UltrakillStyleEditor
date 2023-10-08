using HarmonyLib;
using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace UltrakillStyleEditor.Patches
{
    [HarmonyPatch(typeof(StyleHUD), MethodType.Constructor)]
    public static class StyleHUD_CtorPatch
    {
        [HarmonyPostfix]
        [HarmonyAfter]
        public static void Postfix(StyleHUD __instance)
        {
            foreach (KeyValuePair<string, FormattedStringField> pair in ConfigManager.styleDic)
            {
                __instance.idNameDict[pair.Key] = pair.Value.formattedString;
            }
        }
    }

    [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.RegisterStyleItem))]
    public static class StyleHUD_RegisterStyleItem
    {
        [HarmonyPrefix]
        public static bool Prefix(StyleHUD __instance, string __0)
        {
            if (__instance.idNameDict.ContainsKey(__0))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.GetLocalizedName))]
    public static class StyleHUD_GetLocalizedName
    {
        [HarmonyPrefix]
        public static bool Prefix(StyleHUD __instance, string __0, ref string __result)
        {
            if (ConfigManager.styleDic.TryGetValue(__0, out FormattedStringField field))
            {
                __result = field.formattedString;
                return false;
            }

            if (__instance.idNameDict.TryGetValue(__0, out string text))
            {
                var configField = new FormattedStringField(ConfigManager.unknownStylePanel, __0, __0, Utils.FormattedStringFromFormattedText(text), true);
                ConfigManager.styleDic.Add(__0, configField);
                ConfigManager.AddValueChangeListener(__0, configField);
            }
            else
            {
                var configField = new FormattedStringField(ConfigManager.unknownStylePanel, __0, __0, Utils.FormattedStringFromFormattedText(__0), true);
                ConfigManager.styleDic.Add(__0, configField);
                ConfigManager.AddValueChangeListener(__0, configField);
            }

            __result = ConfigManager.styleDic[__0].formattedString;
            return false;
        }
    }
}
