using System.Text;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using static Android.Graphics.Bitmap;
namespace HoWiFi;
public class PathConfig
{
    private const string TAG = "[Borescope Path]";
    public enum SdcardSelector
    {
        BUILT_IN
    }
    public static SdcardSelector sdcardItem = SdcardSelector.BUILT_IN;

    public static string getRootPath()
    {
        return SdCardUtils.getFirstExternPath();
    }

    public static string savePhoto(string str, string str2, Context context, Bitmap bitmap, Handler handler)
    {
        if (getRootPath() != null)
        {
            try
            {
                Java.IO.File file = new(str);
                if (!file.Exists())
                {
                    file.Mkdirs();
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("create folder ");
                    stringBuilder.Append(str);
                    Log.Debug(TAG, stringBuilder.ToString());
                }
                file = new Java.IO.File(str, str2);
                if (!file.Exists())
                {
                    file.CreateNewFile();
                }
                str = file.AbsolutePath;
                Log.Debug(TAG, str);
                FileStream fileOutputStream = new(str, FileMode.Create);
                bitmap.Compress(CompressFormat.Jpeg, 80, fileOutputStream);
                fileOutputStream.Close();
                Message message = new Message
                {
                    What = HandlerParams.TAKE_PICTURE
                };
                handler.SendMessage(message);
                return str;
            }
            catch (Java.IO.FileNotFoundException e)
            {
                e.PrintStackTrace();
            }
            catch (Java.IO.IOException e2)
            {
                e2.PrintStackTrace();
            }
        }
        return null;
    }

    [Obsolete]
    public static int getSdcardAvilibleSize()
    {
        StatFs statFs = (StatFs)new Java.IO.File(getRootPath()).Path;
        return (int)(((((long)statFs.AvailableBlocks) * ((long)statFs.BlockSize)) / 1024) / 1024);
    }
}