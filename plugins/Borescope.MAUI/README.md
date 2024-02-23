# Borescopes for MAUI
### Dependencies
- dotnet add package CommunityToolkit.Mvvm --version 8.2.2
### Compiling
- dotnet build -c Release
### Adding to your project
- dotnet add package Borescope.MAUI --source ..\plugins\Borescope\bin\Release\

# Using
### Android
- In MainActivity of your application
```
protected override void OnCreate(Bundle savedInstanceState) {
    base.OnCreate(savedInstanceState);
    //Generate the android interfaces
    AndroidActivity.MainActivity.Generate_Interface();
    //Generate the Borescope activity
    BorescopePlugin.Instance.Generate_Borescope(this);
}
```

- Take a Photo
```
//Getting the Borescope Instance
BorescopePlugin.IBorescope borescope = BorescopePlugin.Instance.Get_Borescope();
//Taking a photo with HoWiFi borescopes
borescope.Start_HoWiFi(async (sender, image_source) => {
    //Do what you want here
});
```