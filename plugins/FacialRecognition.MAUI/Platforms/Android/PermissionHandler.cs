using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Core.App;

namespace FacialRecognition.MAUI.Platforms.Android
{
    public class PermissionHandler(Activity activity)
    {
        private readonly Activity activity = activity;
        private const int CAMERA_PERMISSION_CODE = 10;
        const string CAMERA_PERMISSION = Manifest.Permission.Camera;

        /// <summary>
        /// Request Permission camera permission
        /// </summary>
        public void RequestCameraPermission()
        {
            ActivityCompat.RequestPermissions(activity, [CAMERA_PERMISSION], CAMERA_PERMISSION_CODE);
        }

        /// <summary>
        /// Simple check if the device has camera permission
        /// </summary>
        /// <returns></returns>
        public bool HasCameraPermission()
        {
            return AndroidX.Core.Content.ContextCompat.CheckSelfPermission(activity, CAMERA_PERMISSION) == Permission.Granted;
        }
    }
}
