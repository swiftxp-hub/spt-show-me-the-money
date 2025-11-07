using System;
using EFT.InventoryLogic;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public static class ItemQualityService
{
    public static double GetItemQualityModifier(Item item)
    {
        double result = 1d;

        if (item is MedsItemClass medsItem)
        {
            result = (medsItem.MedKitComponent?.HpResource / medsItem.MedKitComponent?.MaxHpResource) ?? 1d;
        }
        else if (IsRepairable(item, out RepairableComponent repairableComponent))
        {
            result = GetRepairableItemQualityValue(repairableComponent);
        }
        else if (item is FoodDrinkItemClass foodDrinkItem)
        {
            result = (foodDrinkItem.FoodDrinkComponent?.HpPercent / foodDrinkItem.FoodDrinkComponent?.MaxResource) ?? 1d;
        }
        else if (IsKey(item, out KeyComponent keyComponent))
        {
            result = ((keyComponent.Template?.MaximumNumberOfUsage - keyComponent.NumberOfUsages) / keyComponent.Template?.MaximumNumberOfUsage) ?? 1d;
        }
        else if (IsResource(item, out ResourceComponent resourceComponent))
        {
            result = resourceComponent.Value / resourceComponent.MaxResource;
        }
        else if (item is RepairKitsItemClass repairKitsItemClass)
        {
            result = repairKitsItemClass.Resource / repairKitsItemClass.MaxRepairResource;
        }

        if (double.IsNaN(result))
            result = 1d;

        if (result == 0d)
            result = 0.01d;

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

        if (durability == 0f)
            return 1d;

        return Math.Sqrt(durability);
    }
}