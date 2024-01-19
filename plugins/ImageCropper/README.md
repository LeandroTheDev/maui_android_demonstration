# Compiling and using the plugin
### Compiling
-  dotnet build -c Release
### Adding to your project
- dotnet add package ImageCropper.MAUI --source ..\plugins\ImageCropper\bin\Release\

### Android

In AndroidManifest.xml
```xml
<application ...>
	<!--Enabling ImageCropper Theme-->
	<activity android:name="com.canhub.cropper.CropImageActivity"
		android:theme="@style/Base.Theme.AppCompat"/>
</application>
```

In MainActivity.cs file:
```cs
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // ImageCropper Plugin
        new ImageCropper.Maui.Platform().Instanciate_Image_Cropper(this);
    }

```
### iOS

In AppDelegate.cs file:

```cs
     new ImageCropper.Maui.Platform.Init();
```
## Usage

### Crop Image
```cs
    new ImageCropper()
    {
        Success = (imageFile) =>
        {
            Dispatcher.Dispatch(() =>
            {
                imageView.Source = ImageSource.FromFile(imageFile);
            });
        }
    }.Crop_Image(image_source);
```
### Crop Image with additional parameters.
```cs
    new ImageCropper()
    {
        PageTitle = "Test Title",
        AspectRatioX = 1,
        AspectRatioY = 1,
	CropShape = ImageCropper.CropShapeType.Oval,
	SelectSourceTitle = "Select source",
	TakePhotoTitle = "Take Photo",
	PhotoLibraryTitle = "Photo Library",
	CancelButtonTitle = "Cancel",
        Success = (imageFile) =>
        {
            Dispatcher.Dispatch(() =>
            {
                imageView.Source = ImageSource.FromFile(imageFile);
            });
        }
    }.Crop_Image(image_source);
```