# Compiling and using the plugin
### Compiling
-  dotnet build -c Release
### Adding to your project
- dotnet add package LocalNotification.MAUI --source ..\plugins\LocalNotification\bin\Release\

[This plugin is forked from Plugin.LocalNotification](https://github.com/thudugala/Plugin.LocalNotification)

# Android

### Update the Android Manifest
```
<uses-permission android:name="android.permission.WAKE_LOCK" />

<!--Required so that the plugin can reschedule notifications upon a reboot-->
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />

<!--optional-->
<uses-permission android:name="android.permission.USE_EXACT_ALARM" />
<uses-permission android:name="android.permission.SCHEDULE_EXACT_ALARM" />
```

### Service Builder
```
public static MauiApp CreateMauiApp()
{
	var builder = MauiApp.CreateBuilder();
	builder
		.UseMauiApp<App>()
		.........
		.UseLocalNotification();
			
	return builder.Build();
}
```

### Show Local Notification
```
if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
{
    await LocalNotificationCenter.Current.RequestNotificationPermission();
}

var notification = new NotificationRequest
{
    NotificationId = 100,
    Title = "Test",
    Description = "Test Description",
    ReturningData = "Dummy data", // Returning data when tapped on notification.
    Schedule = 
    {
        NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification, if not specified notification will show immediately.
    }
};
await LocalNotificationCenter.Current.Show(notification);
```

### Receive Local Notification Tap Event
```
public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Local Notification tap event listener
		LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;

		MainPage = new MainPage();
	}
	
	private void OnNotificationActionTapped(NotificationActionEventArgs e)
    	{
           if (e.IsDismissed)
           {
               // your code goes here
               return;
           }
	   if (e.IsTapped)
           {
               // your code goes here
               return;
           }
           // if Notification Action are setup
           switch (e.ActionId)
           {
               // your code goes here
           }
	}
}
```