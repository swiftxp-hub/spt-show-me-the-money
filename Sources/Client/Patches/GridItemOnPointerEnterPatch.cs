using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine.EventSystems;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class GridItemOnPointerEnterPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(GridItemView), x => x.Name == nameof(GridItemView.OnPointerEnter));

    [PatchPrefix]
    static void PatchPrefix(GridItemView __instance, PointerEventData eventData)
    {
        Plugin.HoveredItem = __instance?.Item;
    }
}