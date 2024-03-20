# Borescopes for MAUI
### Compiling
- dotnet build -c Release
- dotnet pack -c Release
### Adding to your project
- dotnet add package Borescope.MAUI --source ..\plugins\Borescope\bin\Release\

# Using
### Android
- In MainActivity of your application
```
protected override void OnCreate(Bundle savedInstanceState) {
    base.OnCreate(savedInstanceState);
    BorescopePlugin.MAUI.Platform.Android.Interface.Generate();
}
```

- Take a Photo
```
//Getting the Borescope Instance
BorescopePlugin.MAUI.IBorescope borescope = BorescopePlugin.MAUI.Borescope.Get();
//With borescope instance you can choose the type and run it
```