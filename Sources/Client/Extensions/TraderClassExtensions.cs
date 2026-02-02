using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using SwiftXP.SPT.Common.Sessions;
using SwiftXP.SPT.ShowMeTheMoney.Client.Data;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Extensions;

public static class TraderClassExtensions
{
    private static readonly FieldInfo s_supplyDataField =
        typeof(TraderClass).GetField("SupplyData_0", BindingFlags.Public | BindingFlags.Instance);

    public static SupplyData? GetSupplyData(this TraderClass trader) =>
        s_supplyDataField?.GetValue(trader) as SupplyData;

    public static async void UpdateSupplyData(this TraderClass trader)
    {
        try
        {
            if (s_supplyDataField.GetValue(trader) is null)
            {
                Result<SupplyData> result = await SptSession.Session.GetSupplyData(trader.Id);
                if (result.Failed)
                {
                    PluginContextDataHolder.Current.SptLogger?
                        .LogError("Unable to update supply data for trader(s)! Plug-in will not work properly without that data");

                    return;
                }

                s_supplyDataField.SetValue(trader, result.Value);
            }
        }
        catch (Exception exception)
        {
            PluginContextDataHolder.Current.SptLogger?
                .LogException(exception);
        }
    }
}