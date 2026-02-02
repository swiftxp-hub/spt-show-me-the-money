using System.Collections.Generic;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public static class FleaPriceDataHolder
{
    private static volatile FleaPriceData? s_currentData;

    public static FleaPriceData? Current
    {
        get { return s_currentData; }
    }

    public static void UpdateData(Dictionary<string, double> newPrices)
    {
        FleaPriceData newDataPackage = new(newPrices);
        System.Threading.Interlocked.Exchange(ref s_currentData, newDataPackage);
    }
}