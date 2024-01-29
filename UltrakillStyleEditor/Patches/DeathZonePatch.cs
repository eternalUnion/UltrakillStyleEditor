using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace UltrakillStyleEditor.Patches
{
    [HarmonyPatch(typeof(DeathZone), nameof(DeathZone.GotHit))]
    public static class DeathZone_GotHit
    {
        private static string MakeGrey(string id)
		{
            string corpseId = $"customcorpse.{id}";

            FormattedStringField idField = ConfigManager.GetOrCreateField(id);
            if (StyleHUD.Instance != null && idField != null)
                StyleHUD.Instance.idNameDict[corpseId] = $"<color=grey>{idField.rawString}</color>";

            return corpseId;
        }

		[HarmonyTranspiler]
        public static IEnumerable Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			MethodInfo MakeGreyInfo = ReflectionUtils.StaticMethod(typeof(DeathZone_GotHit), nameof(DeathZone_GotHit.MakeGrey));
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr)
                {
                    if (code[i].OperandIs("<color=grey>"))
                    {
                        code[i].operand = "";
                    }
                    else if (code[i].OperandIs("</color>"))
                    {
                        code[i].operand = "";

                        // Try to make the text grey. Skip the concat call
                        i += 2;
                        code.Insert(i, new CodeInstruction(OpCodes.Call, MakeGreyInfo));
                    }
                }
            }

            return code.AsEnumerable();
        }
    }
}
