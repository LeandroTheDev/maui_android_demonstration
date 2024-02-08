using Android.Util;

namespace HoWiFi;
public class Media
{
    private const string TAG = "[Borescope Media]";

    public void playShutter()
    {
        try
        {
            StreamSelf.mTakePhoto = true;
            Log.Debug(TAG, "Take Photo Pressed!");
        }
        catch (Java.Lang.Exception)
        {
            Log.Error(TAG, "Take Photo Exception");
        }
    }
}