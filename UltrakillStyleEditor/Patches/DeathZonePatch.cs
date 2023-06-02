using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace UltrakillStyleEditor.Patches
{
    [HarmonyPatch(typeof(DeathZone), nameof(DeathZone.GotHit))]
    public static class DeathZone_GotHit
    {
        [HarmonyTranspiler]
        public static IEnumerable Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr)
                {
                    if (code[i].OperandIs("<color=grey>"))
                    {
                        // code[i].operand = "death.";

                        // will ignore grey color for now
                        code[i].operand = "";
                    }
                    else if (code[i].OperandIs("</color>"))
                    {
                        code[i].operand = "";
                    }
                }
            }

            return code.AsEnumerable();
        }
    }
}
