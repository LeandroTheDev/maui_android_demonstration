using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Android_Native_Demonstration;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    // Instanciating activitys for plugins
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // ImageCropper Plugin
        new ImageCropper.MAUI.Platforms.Android.Platform().InstanciateImageCropper(this);

        // Borescope Plugin
        //BorescopePlugin.MAUI.Platforms.Android.Interface.Generate();

        // Orientator Plugin
        DeviceOrientation.MAUI.Platforms.Android.Interface.Generate(this);

        // Facial Recognition Plugin
        FacialRecognition.MAUI.Platforms.Android.Interface.GenerateFacialRecognitionInterface();

        OpticalCharacterRecognition.MAUI.Platforms.Android.MainActivity.Interface.GenerateInterface();
    }
}