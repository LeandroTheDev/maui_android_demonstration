#pragma warning disable CA1422
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Renderscripts;

namespace HoWiFi;
public class BlurBitmapUtil
{
    private const float BITMAP_SCALE = 0.4f;
    public static Bitmap blurBitmap(Context context, Bitmap bitmap, float f)
    {
        bitmap = Bitmap.CreateScaledBitmap(
            bitmap,
            (int)Math.Round(((float)bitmap.Width) * BITMAP_SCALE),
            (int)Math.Round(((float)bitmap.Height) * BITMAP_SCALE),
            false
        );
        Bitmap createBitmap = Bitmap.CreateBitmap(bitmap);
        SystemClock.Sleep(200);
        RenderScript create = RenderScript.Create(context);
        ScriptIntrinsicBlur create2 = ScriptIntrinsicBlur.Create(create, Android.Renderscripts.Element.U8_4(create));
        Allocation createFromBitmap = Allocation.CreateFromBitmap(create, bitmap);
        Allocation createFromBitmap2 = Allocation.CreateFromBitmap(create, createBitmap);
        create2.SetRadius(f);
        create2.SetInput(createFromBitmap);
        create2.ForEach(createFromBitmap2);
        createFromBitmap2.CopyTo(createBitmap);
        return createBitmap;
    }

    internal static object blurBitmap(Context ctx, object value, float v)
    {
        throw new NotImplementedException();
    }
}