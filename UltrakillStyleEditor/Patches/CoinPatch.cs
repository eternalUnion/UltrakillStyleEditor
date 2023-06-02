using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace UltrakillStyleEditor.Patches
{
    [HarmonyPatch(typeof(Coin), nameof(Coin.RicoshotPointsCheck))]
    public static class Coin_RicoshotPointsCheck
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetUltraPrefix()
        {
            return ConfigManager.styleDic["ultrakill.ultra"].formattedString;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCounterPrefix()
        {
            return ConfigManager.styleDic["ultrakill.counter"].formattedString;
        }

        private static MethodInfo m_GetUltraPrefix = typeof(Coin_RicoshotPointsCheck).GetMethod("GetUltraPrefix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo m_GetCounterPrefix = typeof(Coin_RicoshotPointsCheck).GetMethod("GetCounterPrefix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        [HarmonyTranspiler]
        public static IEnumerable Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr)
                {
                    if (code[i].OperandIs("<color=orange>ULTRA</color>"))
                    {
                        code[i].opcode = OpCodes.Call;
                        code[i].operand = m_GetUltraPrefix;
                    }
                    else if (code[i].OperandIs("<color=red>COUNTER</color>"))
                    {
                        code[i].opcode = OpCodes.Call;
                        code[i].operand = m_GetCounterPrefix;
                    }
                }
            }

            return code.AsEnumerable();
        }
    }
}
