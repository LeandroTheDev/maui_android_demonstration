# Device Orientation for MAUI
### Compiling
- dotnet build -c Release
- dotnet pack -c Release
### Adding to your project
- dotnet add package DeviceOrientation.MAUI --source ..\plugins\DeviceOrientation\bin\Release\

# Using
### Android
- In MainActivity of your application
```
protected override void OnCreate(Bundle savedInstanceState) {
    base.OnCreate(savedInstanceState);
    //Generate the android interfaces
    DeviceOrientation.MAUI.Platforms.Android.Interface.Generate();
}
```

- Set Orientation
```
//Getting the Borescope Instance
DeviceOrientation.MAUI.IOrientator orientator = DeviceOrientation.MAUI.DeviceOrientator.Orientator.Get();
//Changing orientation of device
orientator.SetOrientation("landscape" /// "portrait");
```