namespace FacialRecognition.MAUI
{
    // All the code in this file is included in all platforms.
    static public class Facial
    {
        public static byte[]? imageRecognition;
        public static IFacial GetFacialRecognition()
        {
            return DependencyService.Get<IFacial>();
        }
    }
}
