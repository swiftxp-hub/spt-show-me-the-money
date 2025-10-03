using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.Common.Sessions;

namespace SwiftXP.SPT.ShowMeTheMoney.Extensions;

public static class TraderClassExtensions
{
    private static readonly FieldInfo SupplyDataField =
        typeof(TraderClass).GetField("supplyData_0", BindingFlags.Public | BindingFlags.Instance);

    public static SupplyData? GetSupplyData(this TraderClass trader) =>
        SupplyDataField?.GetValue(trader) as SupplyData;

    public static async void UpdateSupplyData(this TraderClass trader)
    {
        try
        {
            if (SupplyDataField.GetValue(trader) is null)
            {
                Result<SupplyData> result = await SptSession.Session.GetSupplyData(trader.Id);
                if (result.Failed)
                {
                    SimpleSptLogger.Instance.LogError("Unable to update supply data for trader(s)! Plug-in will not work properly without that data");

                    return;
                }

                SupplyDataField.SetValue(trader, result.Value);
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }
    }
}