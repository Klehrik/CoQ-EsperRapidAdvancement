using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using XRL.World;
using XRL.World.Parts;

namespace Klehrik_EsperRapidAdvancement
{
    [HarmonyPatch]
    public class LevelUpPatch
    {
        static MethodInfo targetMethod = typeof(GameObject).GetMethod(nameof(GameObject.IsEsper));

        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Leveler), nameof(Leveler.LevelUp));
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher codeMatcher = new CodeMatcher(instructions, generator)
                .MatchStartForward(new CodeMatch(OpCodes.Callvirt, targetMethod))
                .ThrowIfInvalid("Could not find call to IsEsper")

                // Remove entire set of 4 instructions for 'IsEsper()' (IL_016f to IL_017a)
                .Advance(-2)
                .RemoveInstructions(4);

            // Jump to '3' (IL_017f)
            codeMatcher.InsertBranch(OpCodes.Br_S, codeMatcher.Pos + 2);

            return codeMatcher.Instructions();
        }
    }
}