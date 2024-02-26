# Android Native Demonstrations

A application made with MAUI demonstrating the possibilities of native functions in android like: 
- Photo.
- Video.
- Crop Images.
- GPS Localization. "Show the Latitude and Longitude"
- Storage Saving. "Save Images/Videos in Pictures or Videos/Demonstration/"
- Image Manipulation. "Change image orientation depending on device accelerator"
- Local Notification.
- Facial Recognition. (Still Working)

### Necessary plugins for compile
- dotnet add package Camera.MAUI --source ..\plugins\Camera.MAUI\bin\Release\
- dotnet add package ImageCropper.MAUI --source ..\plugins\ImageCropper.MAUI\bin\Release\
- dotnet add package LocalNotification.MAUI --source ..\plugins\LocalNotification.MAUI\bin\Release\
- dotnet add package DeviceOrientation.MAUI --source ..\plugins\DeviceOrientation.MAUI\bin\Release\
- dotnet add package Borescope.MAUI --source ..\plugins\Borescope.MAUI\bin\Release\

### Android Compile
- dotnet build -f net8.0-android -c Release