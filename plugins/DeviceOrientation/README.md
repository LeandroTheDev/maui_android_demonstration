# Device Orientation for MAUI
### Compiling
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
    AndroidActivity.MainActivity.Generate_Orientator_Interface();
}
```

- Set Orientation
```
//Getting the Borescope Instance
DeviceOrientator.IOrientator orientator = DeviceOrientator.Orientator.Get_Orientator();
//Taking a photo with HoWiFi borescopes
orientator.Set_Orientation("landscape" /// "portrait");
```