using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace UltrakillStyleEditor.Patches
{
    [HarmonyPatch(typeof(StyleCalculator), nameof(StyleCalculator.HitCalculator))]
    public static class StyleCalculator_HitCalculator
    {
        [HarmonyTranspiler]
        public static IEnumerable Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr && code[i].OperandIs("FRIED"))
                {
                    code[i].operand = "ultrakill.fried";
                    break;
                }
            }

            return code.AsEnumerable();
        }
    }
}
