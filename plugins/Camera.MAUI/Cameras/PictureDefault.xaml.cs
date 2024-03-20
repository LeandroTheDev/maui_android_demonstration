namespace Camera.MAUI.Cameras;

public partial class PictureDefault : ContentPage
{
    // Options
    bool isOpenedAsModal = false;
    /// <summary>
    /// The actual position from the camera, 0 = back, 1 = frontal
    /// </summary>
    private int cameraPosition = 0;
    public int CameraPosition { set => cameraPosition = value; }
    /// <summary>
    /// Disable or enable the accelerator, if not enable the picture
    /// will not rotate, use this for full bytes compatibility
    /// </summary>
    private bool enableAccelerator = true;
    public bool EnableAccelerator { set => enableAccelerator = value; }
    /// <summary>
    /// When the camera dispose the orientation will be set to the portrait
    /// and accelerator will be disposed
    /// </summary>
    private bool disposeAccelerator = true;
    public bool DisposeAccelerator { set => disposeAccelerator = value; }
    /// <summary>
    /// If camera is busy then you cannot perform camera actions
    /// </summary>
    private bool cameraBusy = false;
    /// <summary>
    /// Description
    /// </summary>
    private string cameraDescription = "";
    public string CameraDescription { set => cameraDescription = value; }

    // Events
    /// <summary>
    /// Called when take a photo
    /// </summary>
    private event EventHandler<byte[]>? pictureTake;
    public EventHandler<byte[]>? PictureTake { set => pictureTake = value; }
    /// <summary>
    /// Called when the camera is closed
    /// </summary>
    private event EventHandler<object?>? cameraClose;
    public EventHandler<object?>? CameraClose { set => cameraClose = value; }

    // Constructor
    public PictureDefault()
    {
#if ANDROID || IOS
        InitializeComponent();

        // Declaring the accelerator
        if (enableAccelerator)
        {
            DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientation = false;
            DeviceOrientation.MAUI.Orientator.StartAccelerator();
            DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.Portrait);
        }

        // Setting description
        Description.Text = cameraDescription;

        _ = CameraBusyDelay(1000);
#endif
    }

    // Private Functions
    private async Task<bool> StartCameraView()
    {
#if ANDROID || IOS
        cameraView.Camera = cameraView.Cameras[cameraPosition];
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            await cameraView.StartCameraAsync();
            tcs.SetResult(true);
        });
        return await tcs.Task;
#else
    return false;
#endif
    }

    private async void CameraViewLoad(object? sender, EventArgs e)
    {
        // This is necessary because camera previous closed in different
        // position can cause black screen
        if (History.LastCameraPosition != cameraPosition)
        {
            var correctPosition = cameraPosition;
            cameraPosition = History.LastCameraPosition;
            await StartCameraView();
            cameraPosition = correctPosition;
            await Task.Delay(100);
            _ = StartCameraView();
        }
        else
        {
            _ = StartCameraView();
        }
    }

    private async void CameraTakePhoto(object? sender, EventArgs e)
    {
#if ANDROID || IOS
        try
        {
            // Rotate the image depending on the accelerator
            static void RotateImage(string directory, DeviceOrientation.MAUI.Orientation orientation)
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
#endif
            }


            // Temporary image
            string tmpDirectory = Path.Combine(FileSystem.Current.CacheDirectory, "tmp_image.png");

            // Using memoryStream to be disposed after use
            using (MemoryStream memoryStream = new())
            {
                // Stream Creation
                using Stream stream = await cameraView.TakePhotoAsync();
                // Copying stream bytes to Memory Stream
                await stream.CopyToAsync(memoryStream);

                // Converting Memory Stream bytes to brute bytes
                byte[] tempBytes = memoryStream.ToArray();

                if (!enableAccelerator)
                {
                    pictureTake?.Invoke(this, tempBytes);
                    return;
                }

                // Saving temporary image
                File.Delete(tmpDirectory);
                File.WriteAllBytes(tmpDirectory, tempBytes);
            }
            if (!enableAccelerator) { return; }

            // Change image orientation based in device orientation
            if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.Landscape)
            {
                //Flipping image to left
                RotateImage(tmpDirectory, DeviceOrientation.MAUI.Orientation.Landscape);
            }
            else if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.ReverseLandscape)
            {
                //Flipping image to right
                RotateImage(tmpDirectory, DeviceOrientation.MAUI.Orientation.ReverseLandscape);
            }

            // Invoke pictureTake event
            pictureTake?.Invoke(this, File.ReadAllBytes(tmpDirectory));

            // Delete temporary image
            File.Delete(tmpDirectory);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Cannot take the photo: {ex.Message}", "OK");
        }
#endif
    }

    private void CameraFlashlightSwitch(object? sender, EventArgs e)
    {
#if ANDROID || IOS
        //Verifiy if is not frontal side and not busy
        if (cameraPosition == 0 && !cameraBusy)
        {
            cameraView.TorchEnabled = !cameraView.TorchEnabled;
        }
#endif
    }

    private void CameraPositionSwitch(object? sender, EventArgs e)
    {
#if ANDROID || IOS
        //Stop function if camera is busy
        if (cameraBusy)
        {
            return;
        }
        else
        {
            //Enable the timer for busy delay
            _ = CameraBusyDelay(1000);
        }
        //Troca as cameras das variaveis locais
        switch (cameraPosition)
        {
            case 0: cameraPosition = 1; _ = StartCameraView(); break;
            case 1: cameraPosition = 0; cameraView.TorchEnabled = false; _ = StartCameraView(); break;
        }
#endif
    }

    private void CameraClosed(object? sender, EventArgs e)
    {
        if (cameraBusy) return;
        if (isOpenedAsModal) Navigation.PopModalAsync();
        else Navigation.PopAsync();
        cameraClose?.Invoke(this, null);
    }

    private async Task CameraBusyDelay(int ticks)
    {
        cameraBusy = true;
        await Task.Delay(ticks);
        cameraBusy = false;
    }

    // Public Functions
    public void ChangeDescription(string value)
    {
#if ANDROID || IOS
        Description.Text = value;
#endif
    }

    // Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        isOpenedAsModal = Navigation.ModalStack.Contains(this);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        History.LastCameraPosition = cameraPosition;
#if ANDROID || IOS
        if (disposeAccelerator) DeviceOrientation.MAUI.Orientator.DisposeAccelerator();
#endif
    }
    protected override bool OnBackButtonPressed()
    {
        // Block back if camera is busy
        if (cameraBusy) return true;
        return base.OnBackButtonPressed();
    }
}