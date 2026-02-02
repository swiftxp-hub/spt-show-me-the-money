using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;
using System.Reflection;
using UnityEngine.EventSystems;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class GridItemOnPointerExitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(GridItemView), x => x.Name == nameof(GridItemView.OnPointerExit));

    [PatchPrefix]
    static void PatchPrefix(GridItemView __instance, PointerEventData eventData)
    {
        PluginContextDataHolder.SetHoveredItem(hoveredItem: null);
        SimpleTooltipShowPatch.OnClose();
    }
}