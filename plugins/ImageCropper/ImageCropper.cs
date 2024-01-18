using System.IO;

namespace ImageCropper.Maui
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

        public async void Crop_Image(ImageSource image_file)
        {
            string image_directory = "error";
            // Create temporary file from stream
            if (image_file is StreamImageSource streamImageSource)
            {
                // Get strem from image
                using (Stream stream = await streamImageSource.Stream(new CancellationToken()))
                {
                    // Null check
                    if (stream != null)
                    {
                        // Getting temporary file
                        string tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        image_directory = Path.Combine(tempDirectory, "temp_image.png");

                        // Saving stream into file
                        using (FileStream fs = new FileStream(image_directory, FileMode.Create))
                        {
                            await stream.CopyToAsync(fs);
                        }
                    }
                }
            }
            // Create temporary file from file
            else if (image_file is FileImageSource fileImageSource)
            {
                // Get file path
                string filePath = fileImageSource.File;

                // Check null
                if (!string.IsNullOrEmpty(filePath))
                {
                    // Get temporary path
                    string tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    image_directory = Path.Combine(tempDirectory, "temp_image.png");

                    // Add file to temporary path
                    if (File.Exists(filePath))
                    {
                        File.Copy(filePath, image_directory, true);
                    }
                }
            }
            // Create temporary file from uri
            else if (image_file is UriImageSource uriImageSource)
            {
                // Get image URL
                string imageUrl = uriImageSource.Uri?.AbsoluteUri;

                // Null Check
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Get image with http
                    using (HttpClient client = new HttpClient())
                    {
                        // Getting image bytes
                        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

                        // Getting temporary path
                        string tempDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        image_directory = Path.Combine(tempDirectory, "temp_image.jpg");

                        // Saving image to temporary path
                        File.WriteAllBytes(image_directory, imageBytes);
                    }
                }
            }

            // Native call to crop image
            DependencyService.Get<IImageCropperWrapper>().Show_From_File(this, image_directory);
        }
    }
}