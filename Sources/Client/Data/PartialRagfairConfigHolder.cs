namespace SwiftXP.SPT.ShowMeTheMoney.Client.Data;

public static class PartialRagfairConfigHolder
{
    private static volatile PartialRagfairConfig? s_currentData;

    public static PartialRagfairConfig? Current
    {
        get { return s_currentData; }
    }

    public static void UpdateData(PartialRagfairConfig newPartialRagfairConfig)
    {
        System.Threading.Interlocked.Exchange(ref s_currentData, newPartialRagfairConfig);
    }
}