
using System;
using EFT.Communications;

namespace SwiftXP.ShowMeTheMoney.Notifications;

public class NotificationsService
{
    public static NotificationsService Instance => instance.Value;

    private static readonly Lazy<NotificationsService> instance = new(() => new NotificationsService());

    private NotificationsService() { }

    public void SendLongAlert(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Alert);
    }

    public void SendLongNotice(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Default);
    }

    public void Send(string message, ENotificationDurationType duration, ENotificationIconType icon)
    {
        GClass2314 updatedPricesMessage = new GClass2314(
            message,
            duration,
            icon
        );

        NotificationManagerClass.DisplayNotification(updatedPricesMessage);
    }
}