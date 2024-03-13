using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Android_Native_Demonstration;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    // Instanciating activitys for plugins
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // Enabling only Portrait Orientation
        RequestedOrientation = ScreenOrientation.Portrait;

        // ImageCropper Plugin
        new ImageCropper.Maui.Platform().Instanciate_Image_Cropper(this);

        //Borescope Plugin
        BorescopePlugin.MAUI.MainActivity.Generate_Borescope_Interface(); // Generating Interface
        BorescopePlugin.Instance.Generate_Borescope(this); //Generate Borescope Activity

        //Orientator Plugin
        DeviceOrientation.MAUI.MainActivity.Generate_Orientator_Interface(); //Generate Interfaces for Device Orientation

        //Facial Recognition Plugin
        FacialRecognition.MAUI.Platforms.Android.AndroidInterface.GenerateFacialRecognitionInterface(); //Generate Interfaces for Facial Recognition
    }
}