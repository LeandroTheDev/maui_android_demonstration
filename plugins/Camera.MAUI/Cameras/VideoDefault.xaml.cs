namespace Camera.MAUI.Cameras;

public partial class VideoDefault : ContentPage
{
    // Options
    bool isOpenedAsModal = false;
    /// <summary>
    /// The actual position from the camera, 0 = back, 1 = frontal
    /// </summary>
    private int cameraPosition = 0;
    public int CameraPosition { set => cameraPosition = value; }
    /// <summary>
    /// Disable or enable the accelerator, if not
    /// enable the video will be locked at first orientation set
    /// by default is on landscape
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
    /// Disposing the video will be back to portrait
    /// </summary>
    private bool disposePortrait = true;
    public bool DisposePortrait { set => disposePortrait = value; }
    /// <summary>
    /// If camera is busy then you cannot perform camera actions
    /// </summary>
    private bool cameraBusy = false;
    /// <summary>
    /// Check if camera is recording
    /// </summary>
    private bool cameraRecording = false;
    /// <summary>
    /// Description
    /// </summary>
    private string cameraDescription = "";
    public string CameraDescription { set => cameraDescription = value; }
    /// <summary>
    /// Video will be saved in this location, by default will be saved in application cache folder
    /// with name tmpVideo.mp4
    /// </summary>
    private string videoDirectory = Path.Combine(FileSystem.Current.CacheDirectory, "tmp_video.mp4");
    public string VideoDirectory { set => videoDirectory = value; }

    // Events
    /// <summary>
    /// Called when the video finish recording
    /// </summary>
    private event EventHandler<string>? videoTake;
    public EventHandler<string>? VideoTake { set => videoTake = value; }
    /// <summary>
    /// Called when the camera is closed
    /// </summary>
    private event EventHandler<object?>? cameraClose;
    public EventHandler<object?>? CameraClose { set => cameraClose = value; }

    public VideoDefault()
    {
#if ANDROID || IOS
        //Make orientation to landscape
        InitializeComponent();
        Description.Text = cameraDescription;

        _ = CameraBusyDelay(1000);
#endif
    }

    // Private Functions
    private async Task<bool> StartCameraView()
    {
#if ANDROID || IOS
        // Change orientation dynamic
        if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.Landscape)
        {
            DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.Landscape);
        }
        else if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.ReverseLandscape)
        {
            DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.ReverseLandscape);
        }
        else
        {
            DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.Landscape);
        }
        DeviceOrientation.MAUI.Orientator.OnDeviceOrientationChanged += (sender, args) =>
        {
            _ = StartCameraView();
        };
        await Task.Delay(100);
        // If accelerator is true enable the auto orientation change
        if (enableAccelerator)
        {
            DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientation = true;
            DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientationOnlyLand = true;
            DeviceOrientation.MAUI.Orientator.StartAccelerator();
        }
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

    private async void CameraViewLoad(object sender, EventArgs e)
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

    private async void CameraStartStopRecording(object sender, EventArgs e)
    {
#if ANDROID || IOS
        // Start Recording
        if (!cameraRecording)
        {
            if (enableAccelerator) DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientation = false;
            cameraRecording = true;
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            //Permission Treatment
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "Permission denied for saving the file", "OK");
                cameraRecording = false;
                return;
            }
            await cameraView.StartRecordingAsync(videoDirectory, new Size(1920, 1080));
        }
        // Stop Recording
        else
        {
            if (enableAccelerator) DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientation = true;
            await cameraView.StopRecordingAsync();
            videoTake?.Invoke(this, videoDirectory);
            cameraRecording = false;
        }
#endif
    }

    private void CameraFlashlightSwitch(object sender, EventArgs e)
    {
#if ANDROID || IOS
        //Verifiy if is not frontal side and not busy
        if (cameraPosition == 0 && !cameraBusy)
        {
            cameraView.TorchEnabled = !cameraView.TorchEnabled;
        }
#endif
    }

    private void CameraPositionSwitch(object sender, EventArgs e)
    {
#if ANDROID || IOS
        //Stop function if camera is busy
        if (cameraBusy || cameraRecording)
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

    private async Task CameraBusyDelay(int ticks)
    {
#if ANDROID || IOS
        cameraBusy = true;
        await Task.Delay(ticks);
        cameraBusy = false;
#endif
    }

    private void CameraClosed(object sender, EventArgs e)
    {
        if (cameraBusy) return;
        if (isOpenedAsModal) Navigation.PopModalAsync();
        else Navigation.PopAsync();
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
        DeviceOrientation.MAUI.Orientator.OnDeviceOrientationChanged = null;
        if (disposePortrait) DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.Portrait);
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