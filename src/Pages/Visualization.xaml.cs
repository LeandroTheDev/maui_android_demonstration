namespace Android_Native_Demonstration.Pages;

public partial class Visualization : ContentPage
{
    private bool enable_crop;
    public Visualization()
    {
        InitializeComponent();
        // Declaring Variables
        enable_crop = false;
    }

    private async void Button_Open_Camera(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                image_preview.Source = photo.FullPath;
            }
        }
        catch (FeatureNotSupportedException)
        {
            // Handle not supported on device exception
            await DisplayAlert("Error", "Camera not supported on this device.", "OK");
        }
        catch (PermissionException)
        {
            // Handle permission exception
            await DisplayAlert("Error", "Permission to access the camera was denied.", "OK");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private void Enable_Crop(object sender, CheckedChangedEventArgs e)
    {
        enable_crop = e.Value;
    }

    private void Button_Open_Borescope(object sender, EventArgs e)
    {
        BorescopePlugin.Borescope.Initialize();
    }
}