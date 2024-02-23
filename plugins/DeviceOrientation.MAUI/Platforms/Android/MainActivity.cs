using Android.Content.PM;

namespace DeviceOrientation.MAUI
{
    // All the code in this file is only included on Android.
    public class MainActivity : IOrientator
    {
        static public void Generate_Orientator_Interface()
        {
            Console.WriteLine("[Orientator Interface] Registering the Orientator Interface");
            DependencyService.Register<IOrientator, MainActivity>();
            Console.WriteLine("[Orientator Interface] Registration Completed");
        }
        public void Set_Orientation(string orientation)
        {
            var activity = ActivityStateManager.Default.GetCurrentActivity();
            if (activity != null)
            {
                switch (orientation)
                {
                    case "portrait": activity.RequestedOrientation = ScreenOrientation.Portrait; break;
                    case "landscape": activity.RequestedOrientation = ScreenOrientation.Landscape; break;
                }
                Console.WriteLine("[Orientator] Success changing orientation");
            }
            else
            {
                Console.WriteLine("[Orientator] Error changing orientation, activity is null");
            }
        }
    }
}
