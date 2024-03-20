namespace BorescopePlugin.MAUI
{
    public static class Borescope
    {      
        public static IBorescope Get()
        {
            return DependencyService.Get<IBorescope>();
        }
    }
}