namespace BorescopePlugin
{
    public class Borescope
    {
        public static void Initialize()
        {
#if ANDROID
            Android.Util.Log.Info("BorescopePlugin", "Borescope Plugin Has been Initialized");
#else
            throw new Exception("System not compatible");
#endif
        }
    }

}