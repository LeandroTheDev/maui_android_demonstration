using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace Android_Native_Demonstration;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
        MainPage = new Pages.Visualization();
    }

    private void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        if (e.IsDismissed)
        {
            Console.WriteLine("[Notification] Dismissed");
            return;
        }
        if (e.IsTapped)
        {
            Console.WriteLine("[Notification] Tapped");
            return;
        }
        Console.WriteLine($"[Notification] ID: {e.ActionId}");
    }
}
