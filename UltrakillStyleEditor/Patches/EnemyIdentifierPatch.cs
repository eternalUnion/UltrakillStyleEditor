using HarmonyLib;
using PluginConfig.API.Fields;
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
        private static FormattedStringField conductorField;

        [HarmonyTranspiler]
        public static IEnumerable Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            conductorField = ConfigManager.styleDic["ultrakill.conductor"];
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr)
                {
                    if (code[i].OperandIs("<color=#00ffffff>CONDUCTOR</color>"))
                    {
                        // This dictionary entry was removed, so load it dynamically
                        // code[i].operand = "ultrakill.conductor";

                        PropertyInfo formattedStringValue = ReflectionUtils.InstanceProperty<FormattedStringField>(nameof(FormattedStringField.value));
						PropertyInfo formattedStringFormattedValue = ReflectionUtils.InstanceProperty<FormattedString>(nameof(FormattedString.formattedString));

                        // Push field to the stack
                        code[i].opcode = OpCodes.Ldsfld;
                        code[i].operand = ReflectionUtils.StaticField(typeof(EnemyIdentifier_AfterShock), nameof(EnemyIdentifier_AfterShock.conductorField));

                        // Get formatted string from formatted string field
                        i += 1;
                        code.Insert(i, new CodeInstruction(OpCodes.Call, formattedStringValue.GetMethod));

                        // Get the value from the formatted string
						i += 1;
						code.Insert(i, new CodeInstruction(OpCodes.Call, formattedStringFormattedValue.GetMethod));
					}
                }
            }

            return code.AsEnumerable();
        }
    }
}
