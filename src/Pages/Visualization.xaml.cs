using Android_Native_Demonstration.Utils;
using Plugin.LocalNotification;

namespace Android_Native_Demonstration.Pages;

public partial class Visualization : ContentPage
{
    private bool enableCrop;
    private bool enableSave;
    // If timeout is enabled then you cannot perform actions
    private bool timeout = false;

    public Visualization()
    {
        InitializeComponent();
        // Declaring Variables
        enableCrop = false;
        enableSave = false;
    }

    private async void ButtonOpenCamera(object? sender, EventArgs e)
    {
        if (timeout) return;
        timeout = true;
        _ = Task.Delay(1000).ContinueWith((_) => timeout = false);
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                imagePreview.Source = photo.FullPath;
                var imageSource = ImageSource.FromFile(photo.FullPath);
                // Check if crop is enabled
                if (enableCrop)
                {
                    new ImageCropper.MAUI.ImageCropper()
                    {
                        Success = (imageFile) =>
                        {
                            Dispatcher.Dispatch(async () =>
                            {
                                imagePreview.Source = ImageSource.FromFile(imageFile);
                                // Saving image
                                if (enableSave)
                                {
                                    // Creating the directory
                                    var directory = "Pictures/Demonstration/";
                                    directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                    //Saving the image
                                    try
                                    {
                                        var result = await Utils.Storage.SaveImageSourceToDirectory(directory, imageFile);
                                        await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                                    }
                                    catch (Exception ex)
                                    {
                                        await DisplayAlert("Error", ex.ToString(), "OK");
                                    }
                                }
                            });
                        }
                    }.CropImage(imageSource);
                }
                // Only check if save is enabled
                else
                {
                    // Saving image
                    if (enableSave)
                    {
                        // Creating the directory
                        var directory = "Pictures/Demonstration/";
                        directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        //Saving the image
                        try
                        {
                            var result = await Utils.Storage.SaveImageSourceToDirectory(directory, imageSource);
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
    private async void ButtonOpenCustomCamera(object? sender, EventArgs e)
    {
        if (timeout) return;
        timeout = true;
        _ = Task.Delay(1000).ContinueWith((_) => timeout = false);
        var camera = new Camera.MAUI.Cameras.PictureDefault()
        {
            CameraDescription = "Test",
            CameraClose = (sender, _) =>
            {

            },
            PictureTake = async (sender, imageBytes) =>
            {
                // Getting image source from bytes
                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                // Update image
                imagePreview.Source = imageSource;

                // Check if crop is enabled
                if (enableCrop)
                {
                    new ImageCropper.MAUI.ImageCropper()
                    {
                        Success = (imageFile) =>
                        {
                            Dispatcher.Dispatch(async () =>
                            {
                                imagePreview.Source = ImageSource.FromFile(imageFile);
                                // Saving image
                                if (enableSave)
                                {
                                    // Creating the directory
                                    var directory = "Pictures/Demonstration/";
                                    directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                                    //Saving the image
                                    try
                                    {
                                        var result = await Utils.Storage.SaveImageSourceToDirectory(directory, imageFile);
                                        await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                                    }
                                    catch (Exception ex)
                                    {
                                        await DisplayAlert("Error", ex.ToString(), "OK");
                                    }
                                }
                            });
                        }
                    }.CropImage(imageSource);
                }
                // Only check if save is enabled
                else
                {
                    // Saving image
                    if (enableSave)
                    {
                        // Creating the directory
                        var directory = "Pictures/Demonstration/";
                        directory += $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        //Saving the image
                        try
                        {
                            var result = await Utils.Storage.SaveImageSourceToDirectory(directory, imageSource);
                            await DisplayAlert("Alert", "Sucessfully saved the photo in storage", "OK");
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", ex.ToString(), "OK");
                        }
                    }
                }
                // Close
                await Navigation.PopModalAsync();
            }
        };
        // Open
        await Navigation.PushModalAsync(camera);
    }
    private void EnableCrop(object? sender, CheckedChangedEventArgs e)
    {
        enableCrop = e.Value;
    }
    private async void EnableSave(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {


            // Ask for storage permission
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            // Check if permission is granted
            if (status == PermissionStatus.Granted)
            {
                enableSave = e.Value;
            }
            // If not granted display error
            else
            {
                await DisplayAlert("Error", "Cannot enable the save, Permission Denied", "OK");
                ((CheckBox)sender).IsChecked = false;
                enableSave = false;
            }
        }
        else
        {
            enableSave = e.Value;
        }
    }
    private async void ButtonOpenCustomVideo(object? sender, EventArgs e)
    {
        if (timeout) return;
        timeout = true;
        _ = Task.Delay(1000).ContinueWith((_) => timeout = false);
        var video = new Camera.MAUI.Cameras.VideoDefault()
        {
            CameraPosition = 0,
            CameraDescription = "Video Test",
            VideoDirectory = Storage.CreateDirectory($"Movies/Demonstration/{DateTime.Now:yyyyMMdd_HHmmss}.mp4"),
            EnableAccelerator = true,
            VideoTake = (sender, directory) =>
            {
                _ = DisplayAlert("Success", directory, "OK");
            }
        };
        await Navigation.PushModalAsync(video);
    }
    private async void ButtonOpenNotification(object? sender, EventArgs e)
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
    async private void ButtonGetLocation(object? sender, EventArgs e)
    {
        try
        {
            //Request
            GeolocationRequest request = new(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request, new CancellationTokenSource().Token);
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
    private async void ButtonFacialRegisterRecognition(object? sender, EventArgs e)
    {
        if (timeout) return;
        timeout = true;
        _ = Task.Delay(1000).ContinueWith((_) => timeout = false);
        var camera = new Camera.MAUI.Cameras.PictureDefault()
        {
            CameraDescription = "Facial Register",
            CameraClose = (sender, _) =>
            {

            },
            PictureTake = async (sender, imageBytes) =>
            {
                // Registering Image
                var faceSDK = FacialRecognition.MAUI.Facial.GetFacialRecognition();
                byte[]? result = null;
                try
                {
                    result = await faceSDK.RegisterFromImageBytes(imageBytes);
                }
                catch (Exception ex)
                {
                    _ = DisplayAlert("Alert", ex.Message, "OK");
                }
                if (result == null) { return; }

                // Close
                await Navigation.PopModalAsync();
                await DisplayAlert("Success", "Facial has been registered", "OK");
            },
            EnableAccelerator = false,
            CameraPosition = 1,
        };
        await Navigation.PushModalAsync(camera);
    }
    private async void ButtonFacialAnalyzeRecognition(object? sender, EventArgs e)
    {
        if (timeout) return;
        timeout = true;
        _ = Task.Delay(1000).ContinueWith((_) => timeout = false);
        var camera = new Camera.MAUI.Cameras.PictureDefault()
        {
            CameraDescription = "Facial Anaylze",
            CameraClose = (sender, _) => { },
            PictureTake = async (sender, imageBytes) =>
            {
                // Registering Image
                var faceSDK = FacialRecognition.MAUI.Facial.GetFacialRecognition();
                bool? result = null;
                try
                {
                    result = await faceSDK.PerformAnalyzeFromImageBytes(imageBytes);
                }
                catch (Exception ex)
                {
                    _ = DisplayAlert("Alert", ex.Message, "OK");
                }
                if (result == null) { return; }

                if ((bool)result)
                {
                    // Close
                    await Navigation.PopModalAsync();
                    await DisplayAlert("Success", "Facial is the same as the registered", "OK");
                }
                else
                {
                    await DisplayAlert("Fail", "Facial is NOT the same as the registered", "OK");
                }
            },
            EnableAccelerator = false,
            CameraPosition = 1,
        };
        await Navigation.PushModalAsync(camera);
    }
    private void ButtonGetAccelerator(object? sender, EventArgs e)
    {
        _ = DisplayAlert("Accelerator", DeviceOrientation.MAUI.Orientator.Accelerator.ToString(), "OK");
    }
    private void EnableAccelerator(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            DeviceOrientation.MAUI.Orientator.StartAccelerator();
        }
        else
        {
            DeviceOrientation.MAUI.Orientator.DisposeAccelerator();
        }
    }
    private void EnableOrientation(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientation = true;
        }
        else
        {
            DeviceOrientation.MAUI.Orientator.AcceleratorUpdateChangeOrientation = false;
        }
    }
    private async void ButtonOCR(object? sender, EventArgs e)
    {
        if (timeout) return;
        timeout = true;
        _ = Task.Delay(1000).ContinueWith((_) => timeout = false);
        var camera = new Camera.MAUI.Cameras.PictureDefault()
        {
            CameraDescription = "Test",
            CameraClose = (sender, _) =>
            {

            },
            PictureTake = (sender, imageBytes) =>
            {
                // Getting image source from bytes
                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                // Update image
                imagePreview.Source = imageSource;
            }
        };
        // Open
        await Navigation.PushModalAsync(camera);
    }

}