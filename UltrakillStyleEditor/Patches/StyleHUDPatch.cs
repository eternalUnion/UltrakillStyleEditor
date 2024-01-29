using HarmonyLib;
using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
            if (string.IsNullOrEmpty(__0) || __0.StartsWith("customcorpse."))
                return true;

            FormattedStringField field = ConfigManager.GetOrCreateField(__0);
            if (field == null)
                return true;

            __result = field.formattedString;
            return false;
        }
    }
}
