# Facial Recognition for MAUI
### Compiling
- dotnet build -c Release
- dotnet pack -c Release
### Adding to your project
- dotnet add package FacialRecognition.MAUI --source ..\plugins\FacialRecognition.MAUI\bin\Release\

### Debuging and Building
- For debugging and building you will need to download the [ttvface.aar](https://github.com/FaceOnLive/Face-Recognition-SDK-Android)


### Android
- RegisterNewImage and PerformAnalyze methods needs Camera.MAUI dependencie, consider installing it and injecting on builder
```
builder.UseMauiCameraView();
```
> If you will use the RegisterFromData/RegisterFromImageBytes and PerformAnalyzeFromImageBytes the Camera.MAUI is not necessary
- In MainActivity Android instanciate the interfaces
```
protected override void OnCreate(Bundle savedInstanceState) {
    base.OnCreate(savedInstanceState);
    //Facial Recognition Plugin
    FacialRecognition.MAUI.Platforms.Android.AndroidInterface.GenerateFacialRecognitionInterface();
}
```

- Using the plugin
```
var facial = FacialRecognition.MAUI.Facial.GetFacialRecognition();
//Open camera, taks a photo and register the image
facial.RegisterNewImage(Navigation);
//Register image from bytes
facial.RegisterFromData(data);
//Open camera, takes a photo and test it with the image registred
facial.PerformAnalyze(Navigation);
//Perform anylizes with the data in parameter and the registred image
facial.PerformAnalyzeFromImageBytes(data);
```