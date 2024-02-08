namespace BorescopePlugin
{
    public interface IBorescope
    {
        void Instanciate_Borescope(object activity);
        void Start_HoWiFi(EventHandler<ImageSource> callback);
    }
}
