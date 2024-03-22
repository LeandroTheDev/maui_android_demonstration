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
    /// <para></para>
    /// The basic orientation if accelerator is not enabled is the one from
    /// DeviceOrientation.MAUI.Orientator.DeviceOrientation, only accept Landscape
    /// and ReverseLandscape, everthing else will be considered Landscape
    /// </summary>
    private bool enableAccelerator = true;
    public bool EnableAccelerator { set => enableAccelerator = value; }
    /// <summary>
    /// When the camera dispose the orientation will be set to the portrait
    /// and accelerator will be disposed.
    /// </summary>
    private bool disposeAccelerator = false;
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
    private bool cameraOrientationBusy = false;
    private string lastOrientation = "landscape";
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
        // If accelerator is true enable the auto orientation change
        if (enableAccelerator)
        {
            // First load we dont need the dynamic orientation update
            DeviceOrientation.MAUI.Orientator.StartAccelerator();
            cameraOrientationBusy = true;
            Task.Delay(1000).ContinueWith((_) => cameraOrientationBusy = false);

            DeviceOrientation.MAUI.Orientator.OnDeviceOrientationChanged += async (sender, args) =>
            {
                if (cameraOrientationBusy) return;
                cameraOrientationBusy = true;

                // Change the orientation if is landscape or reversed
                if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.ReverseLandscape && lastOrientation != "reverselandscape")
                {
                    lastOrientation = "reverselandscape";
                    DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.ReverseLandscape);
                }
                else if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.Landscape && lastOrientation != "landscape")
                {
                    lastOrientation = "landscape";
                    DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.Landscape);
                }
                // Just return if not
                else
                {
                    cameraOrientationBusy = false;
                    return;
                }
                await Task.Delay(100);

                // Refresh screen
                await StartCameraView();

                await Task.Delay(500);
                cameraOrientationBusy = false;
            };
        }
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

    private async void CameraViewLoad(object sender, EventArgs e)
    {
#if ANDROID || IOS
        // Change orientation before the camera loads
        if (DeviceOrientation.MAUI.Orientator.DeviceOrientation == DeviceOrientation.MAUI.Orientation.ReverseLandscape)
        {
            lastOrientation = "reverselandscape";
            DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.ReverseLandscape);
        }
        else
        {
            lastOrientation = "landscape";
            DeviceOrientation.MAUI.Orientator.Get().SetOrientation(DeviceOrientation.MAUI.Orientation.Landscape);
        }
        // Wait a few ticks before for compatibility reasons
        await Task.Delay(100);

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
#endif
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
        //Verifiy if is not frontal side and not busy
        if (cameraPosition == 0 && !cameraBusy)
        {
            cameraView.TorchEnabled = !cameraView.TorchEnabled;
        }
    }

    private void CameraPositionSwitch(object sender, EventArgs e)
    {
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
    }

    private async Task CameraBusyDelay(int ticks)
    {
        if (cameraBusy) return;
        cameraBusy = true;
        await Task.Delay(ticks);
        cameraBusy = false;
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
#if ANDROID || IOS
        History.LastCameraPosition = cameraPosition;
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
