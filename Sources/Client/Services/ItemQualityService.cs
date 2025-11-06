using System;
using System.Linq;
using EFT.InventoryLogic;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public static class ItemQualityService
{
    public static double GetItemQualityModifier(Item item/*, bool skipArmorItemsWithoutDurability = true*/)
    {
        Plugin.SptLogger!.LogInfo("GetItemQualityModifier entered...");

        var result = 1d;

        /*
        if (
            skipArmorItemsWithoutDurability
            && item is ArmorItemClass armorItemClass
            && armorItemClass.ArmoredEquipmentTemplateClass.MaxDurability == 0
        )
        {
            Plugin.SptLogger!.LogInfo("Early exit due to skipArmorItemsWithoutDurability...");

            return -1d;
        }
        */

        if (item is MedsItemClass medsItem)
        {
            result = (medsItem.MedKitComponent?.HpResource / medsItem.MedKitComponent?.MaxHpResource) ?? 1d;

            Plugin.SptLogger!.LogInfo($"Item is MedsItemClass. Result: {result}");
        }
        else if (IsRepairable(item, out RepairableComponent repairableComponent))
        {
            result = GetRepairableItemQualityValue(repairableComponent);

            Plugin.SptLogger!.LogInfo($"Item is RepairableComponent. Result: {result}");
        }
        else if (item is FoodDrinkItemClass foodDrinkItem)
        {
            result = (foodDrinkItem.FoodDrinkComponent?.HpPercent / foodDrinkItem.FoodDrinkComponent?.MaxResource) ?? 1d;

            Plugin.SptLogger!.LogInfo($"Item is FoodDrinkItemClass. Result: {result}");
        }
        else if (IsKey(item, out KeyComponent keyComponent))
        {
            result = ((keyComponent.Template?.MaximumNumberOfUsage - keyComponent.NumberOfUsages) / keyComponent.Template?.MaximumNumberOfUsage) ?? 1d;

            Plugin.SptLogger!.LogInfo($"Item is KeyComponent. Result: {result}");
        }
        else if (IsResource(item, out ResourceComponent resourceComponent))
        {
            result = resourceComponent.Value / resourceComponent.MaxResource;

            Plugin.SptLogger!.LogInfo($"Item is ResourceComponent. Result: {result}");
        }
        else if (item is RepairKitsItemClass repairKitsItemClass)
        {
            result = repairKitsItemClass.Resource / repairKitsItemClass.MaxRepairResource;

            Plugin.SptLogger!.LogInfo($"Item is RepairKitsItemClass. Result: {result}");
        }

        if (result == 0d)
        {
            Plugin.SptLogger!.LogInfo($"Result was zero. So falling back to 0.01d.");

            result = 0.01d;
        }

        return result;
    }

    private static bool IsRepairable(Item item, out RepairableComponent repairableComponent)
    {
        repairableComponent = item.GetItemComponent<RepairableComponent>();

        return repairableComponent != null;
    }

    private static bool IsKey(Item item, out KeyComponent keyComponent)
    {
        keyComponent = item.GetItemComponent<KeyComponent>();

        return keyComponent != null;
    }

    private static bool IsResource(Item item, out ResourceComponent resourceComponent)
    {
        resourceComponent = item.GetItemComponent<ResourceComponent>();

        return resourceComponent != null;
    }

    private static double GetRepairableItemQualityValue(RepairableComponent repairableComponent)
    {
        if (repairableComponent.Durability > repairableComponent.MaxDurability)
            repairableComponent.MaxDurability = repairableComponent.Durability;

        float maxPossibleDurability = repairableComponent.MaxDurability;
        float durability = repairableComponent.Durability / maxPossibleDurability;

        if (durability == 0)
            return 1;

        return Math.Sqrt(durability);
    }
}