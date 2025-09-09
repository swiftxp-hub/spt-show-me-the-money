using System.Reflection;
using Comfort.Common;
using EFT;
using SwiftXP.ShowMeTheMoney.Loggers;
using SwiftXP.ShowMeTheMoney.Sessions;

namespace SwiftXP.ShowMeTheMoney.Patches;

public static class TraderClassExtensions
{
    private static readonly FieldInfo SupplyDataField =
        typeof(TraderClass).GetField("supplyData_0", BindingFlags.Public | BindingFlags.Instance);
        
    public static async void UpdateSupplyData(this TraderClass trader)
    {
        if (SupplyDataField.GetValue(trader) is null)
        {
            SimpleStaticLogger.Instance.LogDebug($"TraderClassExtensions.UpdateSupplyData");

            Result<SupplyData> result = await SptSession.Session.GetSupplyData(trader.Id);
            if (result.Failed)
            {
                SimpleStaticLogger.Instance.LogError("Unable to update supply data for trader(s)! Plug-in will not work properly without that data.");

                return;
            }

            SupplyDataField.SetValue(trader, result.Value);
        }
    }
}