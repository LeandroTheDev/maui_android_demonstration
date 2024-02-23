using Android_Native_Demonstration.Components;
using Plugin.LocalNotification;

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
                var image_source = ImageSource.FromFile(photo.FullPath);
                // Check if crop is enabled
                if (enable_crop)
                {
                    new ImageCropper.Maui.ImageCropper()
                    {
                        Success = (image_file) =>
                        {
                            Dispatcher.Dispatch(async () =>
                            {
                                image_preview.Source = ImageSource.FromFile(image_file);
                                // Saving image
                                if (enable_save)
                                {
                                    // Creating the directory
                                    var directory = "Pictures/Demonstration/";
                                    directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                    //Saving the image
                                    try
                                    {
                                        var result = await Utils.Storage.Save_ImageSource_To_Directory(directory, image_file);
                                        await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                                    }
                                    catch (Exception ex)
                                    {
                                        await DisplayAlert("Error", ex.ToString(), "OK");
                                    }
                                }
                            });
                        }
                    }.Crop_Image(image_source);
                }
                // Only check if save is enabled
                else
                {
                    // Saving image
                    if (enable_save)
                    {
                        // Creating the directory
                        var directory = "Pictures/Demonstration/";
                        directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        //Saving the image
                        try
                        {
                            var result = await Utils.Storage.Save_ImageSource_To_Directory(directory, image_source);
                            await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", ex.ToString(), "OK");
                        }
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
        var cameraPreview = new CameraPreview("Template");
        cameraPreview.Closed += async (sender, image_source) =>
        {
            //Delete previous images, this is necessary for some reason
            image_preview.Source = null;
            // Update image
            image_preview.Source = image_source;

            // Check if crop is enabled
            if (enable_crop)
            {
                new ImageCropper.Maui.ImageCropper()
                {
                    Success = (image_file) =>
                    {
                        Dispatcher.Dispatch(async () =>
                        {
                            image_preview.Source = ImageSource.FromFile(image_file);
                            // Saving image
                            if (enable_save)
                            {
                                // Creating the directory
                                var directory = "Pictures/Demonstration/";
                                directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                //Saving the image
                                try
                                {
                                    var result = await Utils.Storage.Save_ImageSource_To_Directory(directory, image_file);
                                    await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                                }
                                catch (Exception ex)
                                {
                                    await DisplayAlert("Error", ex.ToString(), "OK");
                                }
                            }
                        });
                    }
                }.Crop_Image(image_source);
            }
            // Only check if save is enabled
            else
            {
                // Saving image
                if (enable_save)
                {
                    // Creating the directory
                    var directory = "Pictures/Demonstration/";
                    directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    //Saving the image
                    try
                    {
                        var result = await Utils.Storage.Save_ImageSource_To_Directory(directory, image_source);
                        await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.ToString(), "OK");
                    }
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
                await DisplayAlert("Error", "Cannot enable the save, Permission Denied", "OK");
                ((CheckBox)sender).IsChecked = false;
                enable_save = false;
            }
        }
        else
        {
            enable_save = e.Value;
        }
    }

    private async void Button_Open_Custom_Video(object sender, EventArgs e)
    {
        var video_preview = new VideoPreview($"Movies/Demonstration/{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
        // When the video finished then show a message
        video_preview.Closed += (sender, video_directory) =>
        {
            DisplayAlert("Alert", $"Video saved in {video_directory}", "OK");
        };
        await Navigation.PushModalAsync(video_preview);
    }

    private async void Button_Open_Notification(object sender, EventArgs e)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        var notification = new NotificationRequest
        {
            NotificationId = 100,
            Title = "Test",
            Description = "Test Description",
            ReturningData = "Dummy data",
            //Schedule = { NotifyTime = DateTime.Now.AddSeconds(30) }
        };
        await LocalNotificationCenter.Current.Show(notification);
    }

    async private void Button_Get_Location(object sender, EventArgs e)
    {
        try
        {
            //Request
            GeolocationRequest request = new(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            Location location = await Geolocation.Default.GetLocationAsync(request, new CancellationTokenSource().Token);

            if (location != null)
                await DisplayAlert("Alert", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "OK");
            else
                await DisplayAlert("Error", "GPS does not retrieve any location", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"GPS returned a error {ex.Message}", "OK");
        }
    }
}