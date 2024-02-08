# Android Native Demonstrations

A application made with MAUI demonstration the possibilities of native functions in android like: Camera, Crop Images, Facial Recognition.

### Necessary plugins for compile
- dotnet add package Borescope.MAUI --source ..\plugins\Borescope\bin\Release\
- dotnet add package Camera.MAUI --source ..\plugins\Camera\bin\Release\
- dotnet add package ImageCropper.MAUI --source ..\plugins\ImageCropper\bin\Release\
- dotnet add package LocalNotification --source ..\plugins\LocalNotification\bin\Release\

### Android Compile
- dotnet build -f net7.0-android