using SPT.Reflection.Utils;

namespace SwiftXP.ShowMeTheMoney.Sessions;

public static class SptSession
{
    public static ISession Session => ClientAppUtils.GetMainApp().GetClientBackEndSession();
}