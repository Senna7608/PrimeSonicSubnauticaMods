﻿namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(CyclopsHolographicHUD))]
    [HarmonyPatch("RefreshUpgradeConsoleIcons")]
    internal class CyclopsHolographicHUD_Patcher
    {
        [HarmonyPostfix]
        internal static bool Prefix(ref CyclopsHolographicHUD __instance)
        {
            return false; // Should now be handled by SetCyclopsUpgrades
        }

    }
}
