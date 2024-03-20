namespace BorescopePlugin.MAUI.Platforms.Android;
public class MainActivity : IBorescope
{

    //HoWiFi
    public object StartHoWiFi()
    {
        if (Platform.CurrentActivity == null)
        {
            throw new NullReferenceException("Activity is null while starting HOWIFI");
        }
        HoWiFi.Instance howifi = new(Platform.CurrentActivity);
        return howifi;
    }
}

class Interface
{
    static public void Generate()
    {
        DependencyService.Register<IBorescope, MainActivity>();
    }
}