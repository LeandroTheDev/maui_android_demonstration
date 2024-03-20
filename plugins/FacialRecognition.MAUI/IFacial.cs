namespace FacialRecognition.MAUI
{
    public interface IFacial
    {
        /// <summary>
        /// Ask for photo and register the new image as Data for the SDK recognition,
        /// returns any object with the parameters:
        /// <para>- recognitionImage: byte[]</para>
        /// <para>- bruteImage: string (base64)</para>
        /// </summary>
        Task<object> RegisterNewImage(INavigation Navigation);

        /// <summary>
        /// Register the data recognition manually,
        /// the data is received from RegisterNewImage function
        /// </summary>
        void RegisterFromData(byte[] data);

        /// <summary>
        /// Check if image has facial and then
        /// translate it to be a Data for the SDK recognition
        /// and save it
        /// <para></para>
        /// Will return a byte array for the data,
        /// and will throw Exception if the image don't have a face
        /// </summary>
        Task<byte[]> RegisterFromImageBytes(byte[] imageBytes);

        /// <summary>
        /// Asks for a photo and
        /// performs analyze to verify the face integrity
        /// of the 2 images, the registred one and the taked photo
        /// </summary>
        Task<bool> PerformAnalyze(INavigation Navigation);

        /// <summary>
        /// Performs a analyze with the registred data and the
        /// image bytes to check the integrity of the 2 images,
        /// will return true if the two faces of the image is the same person
        /// false if not
        /// </summary>
        Task<bool> PerformAnalyzeFromImageBytes(byte[] data);
    }
}
