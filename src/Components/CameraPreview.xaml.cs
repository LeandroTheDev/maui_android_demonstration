using System.Numerics;

namespace Android_Native_Demonstration.Components;

public partial class CameraPreview : ContentPage
{
    /// <summary>
    /// The actual position from the camera, 0 = back, 1 = frontal
    /// </summary>
    private int Camera_Position = 0;
    /// <summary>
    /// If camera is busy then you cannot perform camera actions
    /// </summary>
    private bool Camera_Busy = false;
    public event EventHandler<ImageSource> Closed;
    /// <summary>
    /// If image is busy the temporary image cannot be deleted in dispose
    /// </summary>
    private bool Temp_Busy = false;
    public CameraPreview(string description)
    {
        InitializeComponent();
        Description.Text = description;
        DeviceOrientation.MAUI.Orientator.StartAccelerator();
    }

    private void CameraViewLoad(object? sender, EventArgs e)
    {
        // Starting in Frontal Camera
        cameraView.Camera = cameraView.Cameras[0];
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            _ = Task.Delay(200).ContinueWith(async (_) =>
            {
                await cameraView.StartCameraAsync();
            });
        });
    }

    private async void Camera_Take_Photo(object? sender, EventArgs e)
    {
        try
        {
            //Save image in a temporary storage and returns the image path
            static async Task<string> Save_Temporary_Image(ImageSource image_source)
            {
                // Creating the directory
                var directory = "Pictures/Demonstration/";
                directory += "temp.png";
                //Removing old temps
                var result = Utils.Storage.RemoveFile(directory);
                if (result == "success" || result == "no_file_found")
                {
                    Console.WriteLine($"[File] removed: {result}");
                    //Saving the image and returning
                    return await Utils.Storage.SaveImageSourceToDirectory(directory, image_source);
                }
                else
                {
                    throw new Exception($"Error while removing temporary image: {result}");
                }
            }

            //Rotate the image depending on the accelerator
            static void Rotate_Image(string directory, DeviceOrientation.MAUI.Orientation orientation)
            {
#if ANDROID
                var orientation_tag = 1;
                switch (orientation)
                {
                    case DeviceOrientation.MAUI.Orientation.Landscape: orientation_tag = 2; break;
                    case DeviceOrientation.MAUI.Orientation.ReverseLandscape: orientation_tag = 4; break;
                }
                Android.Media.ExifInterface exifInterface = new(directory);
                exifInterface.SetAttribute(Android.Media.ExifInterface.TagOrientation, orientation_tag.ToString());
                exifInterface.SaveAttributes();
                Console.Write($"[File] Image rotated to {orientation} in {directory}");
#else
                throw new Exception("Cannot rotate image, system operational not suported");
#endif
            }

            //Stream Creation
            Stream stream = await cameraView.TakePhotoAsync();

            //Getting ImageSource
            ImageSource image_source = ImageSource.FromStream(() => stream);

            //Saving temporary image
            Temp_Busy = true;
            var temp_directory = await Save_Temporary_Image(image_source);

            if (DeviceOrientation.MAUI.Orientator.Accelerator[0] > 0.8)
            {
                //Flipping image to left
                Rotate_Image(temp_directory, DeviceOrientation.MAUI.Orientation.Landscape);
            }
            else if (DeviceOrientation.MAUI.Orientator.Accelerator[0] < -0.8)
            {
                //Flipping image to right
                Rotate_Image(temp_directory, DeviceOrientation.MAUI.Orientation.ReverseLandscape);
            }

            //Send image throught pop
            await Navigation.PopModalAsync();
            Closed?.Invoke(this, ImageSource.FromFile(temp_directory));
            Temp_Busy = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Cannot take the photo: {ex.Message}", "OK");
        }
    }

    private void Camera_Flashlight_Switch(object? sender, EventArgs e)
    {
        //Verifiy if is not frontal side and not busy
        if (Camera_Position == 0 && !Camera_Busy)
        {
            cameraView.TorchEnabled = !cameraView.TorchEnabled;
        }
    }

    private void Camera_Position_Switch(object? sender, EventArgs e)
    {
        //Stop function if camera is busy
        if (Camera_Busy)
        {
            return;
        }
        else
        {
            //Enable the timer for busy delay
            _ = Camera_Busy_Delay(1000);
        }

        //Changing the camera position
        void SwitchCamera()
        {
            cameraView.Camera = cameraView.Cameras[Camera_Position];
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();

            });
        }
        //Troca as cameras das variaveis locais
        switch (Camera_Position)
        {
            case 0: Camera_Position = 1; SwitchCamera(); break;
            case 1: Camera_Position = 0; cameraView.TorchEnabled = false; SwitchCamera(); break;
        }
    }

    private void Camera_Close(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Desactivating the Device Moviment Detector
        DeviceOrientation.MAUI.Orientator.DisposeAccelerator();
        // Removing old temps
        Task.Delay(1000).ContinueWith((task) =>
        {
            if (!Temp_Busy)
            {
                Utils.Storage.RemoveFile("Pictures/Demonstration/temp.png");
                Console.WriteLine("[File] Temporary image has been deleted");
            }
            else
            {
                Console.WriteLine("[File] Temporary image cannot be deleted, the image is busy");

            }
        });
        // Closing camera
        cameraView.ClearLogicalChildren();
        _ = cameraView.StopCameraAsync();
    }

    private async Task Camera_Busy_Delay(int ticks)
    {
        Camera_Busy = true;
        await Task.Delay(ticks);
        Camera_Busy = false;
    }
}