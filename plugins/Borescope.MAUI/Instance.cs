namespace BorescopePlugin
{
    public static class Instance
    {
        private static IBorescope borescope;

        public static IBorescope Generate_Borescope(object activity)
        {
            borescope = DependencyService.Get<IBorescope>();
            borescope.Instanciate_Borescope(activity);
            return borescope;
        }

        public static IBorescope Get_Borescope()
        {
            return borescope;
        }
    }
}