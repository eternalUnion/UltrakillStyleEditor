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
    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.AfterShock))]
    public static class EnemyIdentifier_AfterShock
    {
        [HarmonyTranspiler]
        public static IEnumerable Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr)
                {
                    if (code[i].OperandIs("<color=cyan>CONDUCTOR</color>"))
                    {
                        code[i].operand = "ultrakill.conductor";
                    }
                }
            }

            return code.AsEnumerable();
        }
    }
}
