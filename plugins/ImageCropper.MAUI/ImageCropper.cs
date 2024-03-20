using System.IO;

namespace ImageCropper.MAUI
{
    public class ImageCropper
    {
        public static ImageCropper Current { get; set; }

        public ImageCropper()
        {
            Current = this;
        }

        public enum CropShapeType
        {
            Rectangle,
            Oval
        };

        public CropShapeType CropShape { get; set; } = CropShapeType.Rectangle;

        public int AspectRatioX { get; set; } = 0;

        public int AspectRatioY { get; set; } = 0;

        public string PageTitle { get; set; } = null;

        public string SelectSourceTitle { get; set; } = "Select source";

        public string TakePhotoTitle { get; set; } = "Take Photo";

        public string PhotoLibraryTitle { get; set; } = "Photo Library";

        public string CancelButtonTitle { get; set; } = "Cancel";

        public string CropButtonTitle { get; set; } = "Crop";

        public Action<string> Success { get; set; }

        public Action Failure { get; set; }

        public MediaPickerOptions MediaPickerOptions { get; set; } = new MediaPickerOptions();

        /// <summary>
        /// Crop the image by the ImageSource or string path to the image
        /// <para><para>You need to provide a Success call back to receive the
        /// cropped image
        /// <para><para>Accepts the three ImageSources (Stream,File,Uri)
        /// </summary>
        /// <param name="imageSource"></param>
        /// <param name="directory"></param>
        /// <exception cref="Exception"></exception>
        public async void CropImage(ImageSource? imageSource = null, string? directory = null)
        {
            if (imageSource == null && directory == null){
                throw new Exception("Double nulls, ImageSource and Directory not provided");
            }
            // If Directory is not provided
            if (directory == null)
            {
                byte[]? imageBytes = null;
                // ImageSource from a Stream
                if (imageSource is IStreamImageSource)
                {
                    Stream stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
                    byte[] bytesAvailable = new byte[stream.Length];
                    stream.Read(bytesAvailable, 0, bytesAvailable.Length);

                    imageBytes = bytesAvailable;
                }
                // ImageSource from a File
                else if (imageSource is FileImageSource fileImageSource)
                {
                    var filePath = fileImageSource.File;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        imageBytes = await File.ReadAllBytesAsync(filePath);
                    }
                }
                // ImageSource from a Path
                else if (imageSource is UriImageSource uriImageSource)
                {
                    var uri = uriImageSource.Uri;
                    if (uri != null)
                    {
                        using (var client = new System.Net.Http.HttpClient())
                        {
                            imageBytes = await client.GetByteArrayAsync(uri);
                        }
                    }
                }

                // Null Check
                if (imageBytes == null)
                {
                    throw new Exception("Image bytes is null, did the ImageSource is valid?");
                }
                // Temporary folder
                string imageDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.png");
                // Writing the temporary image
                await File.WriteAllBytesAsync(imageDirectory, imageBytes);
                // Native call to crop image
                DependencyService.Get<IImageCropperWrapper>().ShowFromFile(this, imageDirectory);
            }
            // Directory provided
            else
            {
                // Native call to crop image
                DependencyService.Get<IImageCropperWrapper>().ShowFromFile(this, directory);
            }
        }
    }
}