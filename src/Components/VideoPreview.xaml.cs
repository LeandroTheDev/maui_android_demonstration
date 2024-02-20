using Android_Native_Demonstration.Utils;

namespace Android_Native_Demonstration.Components;

public partial class VideoPreview : ContentPage
{
    private int Camera_Position = 0;
    private bool Camera_Busy = false;
    private bool Playing = false;
    private string Save_Video_Directory;
    private string Video_Description = "Template";
    public event EventHandler<string> Closed;

    private readonly DeviceOrientation.MAUI.IOrientator orientator = DeviceOrientation.MAUI.Orientator.Get_Orientator();
    public VideoPreview(string directory)
    {
        //Make orientation to landscape
        orientator.Set_Orientation("landscape");
        InitializeComponent();
        Description.Text = Video_Description;
        Save_Video_Directory = directory;
    }

    private void Camera_View_Load(object sender, EventArgs e)
    {
        // Starting in Frontal Camera
        cameraView.Camera = cameraView.Cameras[0];
        // Enabling Microphone
        cameraView.Microphone = cameraView.Microphones[0];
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            await cameraView.StartCameraAsync();
        });
    }

    private async void Camera_StartStop_Recording(object sender, EventArgs e)
    {
        // Start Recording
        if (!Playing)
        {
            Playing = true;
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            //Permission Treatment
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "Permission denied for saving the file", "OK");
                Playing = false;
                return;
            }
            Save_Video_Directory = Storage.Create_Directory(Save_Video_Directory);
            await cameraView.StartRecordingAsync(Save_Video_Directory, new Size(1920, 1080));
        }
        // Stop Recording
        else
        {
            await cameraView.StopRecordingAsync();
            //Send video directory throught pop
            await Navigation.PopModalAsync();
            Closed?.Invoke(this, Save_Video_Directory);
        }
    }

    private void Camera_Flashlight_Switch(object sender, EventArgs e)
    {
        //Verifiy if is not frontal side and not busy
        if (Camera_Position == 0 && !Camera_Busy)
        {
            cameraView.TorchEnabled = !cameraView.TorchEnabled;
        }
    }

    private void Camera_Position_Switch(object sender, EventArgs e)
    {
        //Stop function if camera is busy
        if (Camera_Busy || Playing)
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

    private async Task Camera_Busy_Delay(int ticks)
    {
        Camera_Busy = true;
        await Task.Delay(ticks);
        Camera_Busy = false;
    }

    private void Camera_Close(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //Reset orientation to portrait
        orientator.Set_Orientation("portrait");
    }
}