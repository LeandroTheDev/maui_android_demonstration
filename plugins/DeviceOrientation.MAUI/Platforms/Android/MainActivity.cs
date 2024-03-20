using Android.Content.PM;

namespace DeviceOrientation.MAUI.Platforms.Android
{
    // All the code in this file is only included on Android.
    public class MainActivity : IOrientator
    {
        /// <summary>
        /// Change orientation from device,
        /// this functions receives 2 types of orientation
        /// "portrait" and "landscape"
        /// </summary>
        public void SetOrientation(Orientation orientation)
        {
            var activity = ActivityStateManager.Default.GetCurrentActivity();
            if (activity != null)
            {
                switch (orientation)
                {
                    case Orientation.Portrait: activity.RequestedOrientation = ScreenOrientation.Portrait; break;
                    case Orientation.ReversePortrait: activity.RequestedOrientation = ScreenOrientation.ReversePortrait; break;
                    case Orientation.Landscape: activity.RequestedOrientation = ScreenOrientation.Landscape; break;
                    case Orientation.ReverseLandscape: activity.RequestedOrientation = ScreenOrientation.ReverseLandscape; break;
                }
                Console.WriteLine("[Orientator] Success changing orientation");
            }
            else
            {
                Console.WriteLine("[Orientator] Error changing orientation, activity is null");
            }
        }
    }

    public class Interface
    {
        /// <summary>
        /// Generetas the interface for the android devices
        /// </summary>
        static public void Generate(object activity)
        {
            Console.WriteLine("[Orientator Interface] Registering the Orientator Interface");
            DependencyService.Register<IOrientator, MainActivity>();
            Console.WriteLine("[Orientator Interface] Registration Completed");
            if (activity is MauiAppCompatActivity androidActivity)
            {
                androidActivity.RequestedOrientation = ScreenOrientation.Portrait;
                Console.WriteLine("[Orientator Interface] Orientation set");
            }
        }
    }
}
