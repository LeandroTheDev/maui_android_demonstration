namespace Android_Native_Demonstration.Components;

public partial class CameraPreview : ContentPage
{
    private int Camera_Position = 0;
    private bool Camera_Busy = false;
    private string Image_Description = "Template";
    public event EventHandler<ImageSource> Closed;
    public CameraPreview()
    {
        InitializeComponent();
        Description.Text = Image_Description;
    }

    private void Camera_View_Load(object sender, EventArgs e)
    {
        // Starting in Frontal Camera
        cameraView.Camera = cameraView.Cameras[0];
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            await cameraView.StartCameraAsync();
        });
    }

    private async void Camera_Take_Photo(object sender, EventArgs e)
    {
        //Stream Creation
        Stream stream = await cameraView.TakePhotoAsync();

        //Image throught the stream
        ImageSource imageSource = ImageSource.FromStream(() => stream);

        //Send image throught pop
        await Navigation.PopModalAsync();
        Closed?.Invoke(this, imageSource);
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
}