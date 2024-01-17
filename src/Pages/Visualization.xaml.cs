using Android_Native_Demonstration.Components;

namespace Android_Native_Demonstration.Pages;

public partial class Visualization : ContentPage
{
    private bool enable_crop;
    private bool enable_save;
    public Visualization()
    {
        InitializeComponent();
        // Declaring Variables
        enable_crop = false;
        enable_save = false;
    }

    private async void Button_Open_Camera(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                image_preview.Source = photo.FullPath;
                if (enable_save)
                {
                    // Creating the directory
                    var directory = "Pictures/Demonstration/";
                    directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    //Saving the image
                    var result = await Utils.Storage.Save_ImageSource_To_Directory(directory, ImageSource.FromFile(photo.FullPath));
                    if (result == "sucess")
                    {
                        await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", result, "OK");
                    }
                }
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

    private async void Button_Open_Custom_Camera(object sender, EventArgs e)
    {
        var cameraPreview = new CameraPreview();
        cameraPreview.Closed += async (sender, imageSource) =>
        {
            //Delete previous images, this is necessary for some reason
            image_preview.Source = null;
            //Update image
            image_preview.Source = imageSource;
            if (enable_save)
            {
                // Creating the directory
                var directory = "Pictures/Demonstration/";
                directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                //Saving the image
                var result = await Utils.Storage.Save_ImageSource_To_Directory(directory, imageSource);
                if (result == "sucess")
                {
                    await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                } else
                {
                    await DisplayAlert("Error", result, "OK");
                }
            }
        };
        await Navigation.PushModalAsync(cameraPreview);
    }

    private void Enable_Crop(object sender, CheckedChangedEventArgs e)
    {
        enable_crop = e.Value;
    }

    private async void Enable_Save(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {


            // Ask for storage permission
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            // Check if permission is granted
            if (status == PermissionStatus.Granted)
            {
                enable_save = e.Value;
            }
            // If not granted display error
            else
            {
                await DisplayAlert("Error", "Cannot enable the save mod, Permission Denied", "OK");
                ((CheckBox)sender).IsChecked = false;
                enable_save = false;
            }
        }
        else
        {
            enable_save = e.Value;
        }
    }

    private void Button_Open_Borescope(object sender, EventArgs e)
    {
        BorescopePlugin.Borescope.Initialize();
    }

}