using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.SPT.Common.EFT;
using SwiftXP.SPT.ShowMeTheMoney.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class InventoryScreenShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(InventoryScreen), x => x.Name == nameof(InventoryScreen.Show));

    [PatchPostfix]
    public static void PatchPostfix(InventoryScreen __instance)
    {
        if (!EFTHelper.IsInRaid)
            RagfairPriceTableService.Instance.UpdatePrices();
    }
}