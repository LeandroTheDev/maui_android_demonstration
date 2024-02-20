# Android Native Demonstrations

A application made with MAUI demonstrating the possibilities of native functions in android like: Camera, Crop Images, Facial Recognition, Storage Saving, Image Manipulation.

### Necessary plugins for compile
- dotnet add package Camera.MAUI --source ..\plugins\Camera\bin\Release\
- dotnet add package ImageCropper.MAUI --source ..\plugins\ImageCropper\bin\Release\
- dotnet add package LocalNotification --source ..\plugins\LocalNotification\bin\Release\
- dotnet add package DeviceOrientation.MAUI --source ..\plugins\DeviceOrientation\bin\Release\

### Android Compile
- dotnet build -f net7.0-android -c Release