using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.ShowMeTheMoney.Loggers;
using System.Reflection;
using UnityEngine.EventSystems;

namespace SwiftXP.ShowMeTheMoney.Patches;

public class GridItemOnPointerEnterPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(GridItemView), x => x.Name == "OnPointerEnter");
    
    [PatchPrefix]
    static void PatchPrefix(GridItemView __instance, PointerEventData eventData)
    {
        SimpleStaticLogger.Instance.LogDebug($"GridItemOnPointerEnterPatch.PatchPrefix - Item: {__instance?.Item?.TemplateId}");

        Plugin.HoveredItem = __instance?.Item;
    }
}