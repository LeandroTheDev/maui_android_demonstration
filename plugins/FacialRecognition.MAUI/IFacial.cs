namespace FacialRecognition.MAUI
{
    public interface IFacial
    {
        Task<object> RegisterNewImage(INavigation Navigation);

        void RegisterFromData(byte[] data);

        Task<bool> PerformAnalyze(INavigation Navigation);
    }
}
