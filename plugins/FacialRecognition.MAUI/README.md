# Facial Recognition for MAUI
### Dependencies
- dotnet add package Camera.MAUI --source ..\Camera.MAUI\bin\Release\

### Compiling
- dotnet build -c Release
- dotnet pack -c Release
### Adding to your project
- dotnet add package FacialRecognition.MAUI --source ..\plugins\FacialRecognition.MAUI\bin\Release\

### Debuging and Building
- For debugging and building you will need to buy the ttvface sdk


### Android
- To use this plugin you will need to implement the Camera.MAUI in builder, view the plugin to see how to use it
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
facial.PerformAnalyze(Navigation);
facial.RegisterNewImage(Navigation);
facial.RegisterFromData(data);
```