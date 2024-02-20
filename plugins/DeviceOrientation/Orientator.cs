namespace DeviceOrientation.MAUI
{
    // All the code in this file is included in all platforms.
    public class Orientator
    {
        public static IOrientator Get_Orientator()
        {
            return DependencyService.Get<IOrientator>();
        }
    }
}
