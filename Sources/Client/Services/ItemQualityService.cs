using System;
using System.Linq;
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
        else if (IsRepairable(item, out RepairableComponent repairableComponent) && !item.TryGetItemComponent(out ArmorHolderComponent _))
        {
            result = GetRepairableItemQualityValue(repairableComponent);
        }
        else if (item.TryGetItemComponent(out ArmorHolderComponent armorHolderComponent) && armorHolderComponent.ArmorPlates.Any())
        {
            result = GetArmorHolderQualityValue(item, armorHolderComponent);
        }
        else if (item is FoodDrinkItemClass foodDrinkItem)
        {
            result = (foodDrinkItem.FoodDrinkComponent?.HpPercent / foodDrinkItem.FoodDrinkComponent?.MaxResource) ?? 1d;
        }
        else if (IsKey(item, out KeyComponent keyComponent) && keyComponent?.NumberOfUsages > 0 && keyComponent?.Template?.MaximumNumberOfUsage > 0)
        {
            double numberOfUsages = keyComponent.NumberOfUsages!;
            double maximumNumberOfUsage = keyComponent.Template.MaximumNumberOfUsage!;

            result = (maximumNumberOfUsage - numberOfUsages) / maximumNumberOfUsage;
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
        if (repairableComponent.TemplateDurability > 0)
            maxPossibleDurability = repairableComponent.TemplateDurability;

        float durability = repairableComponent.Durability / maxPossibleDurability;

        if (durability == 0f)
            return 1d;

        return Math.Sqrt(durability);
    }

    private static double GetArmorHolderQualityValue(Item rootItem, ArmorHolderComponent armorHolderComponent)
    {
        double qualityModifier = 0d;
        double itemsWithQualityCount = 0d;

        if (armorHolderComponent.MoveAbleArmorSlots.Any() || rootItem.GetItemComponent<HelmetComponent>() != null)
        {
            qualityModifier = 1d;
            itemsWithQualityCount++;
        }

        foreach (ArmorPlateItemClass armorPlateItemClass in armorHolderComponent.ArmorPlates)
        {
            RepairableComponent repairableComponent = armorPlateItemClass.GetItemComponent<RepairableComponent>();
            if (repairableComponent != null)
            {
                double plateQuality = GetRepairableItemQualityValue(armorPlateItemClass.GetItemComponent<RepairableComponent>());
                if (Math.Abs(plateQuality - (-1)) < 0.001)
                {
                    continue;
                }

                qualityModifier += plateQuality;
                itemsWithQualityCount++;
            }
        }

        double result = qualityModifier / itemsWithQualityCount;
        if (double.IsNaN(result))
            result = 1d;

        return Math.Min(result, 1d);
    }
}