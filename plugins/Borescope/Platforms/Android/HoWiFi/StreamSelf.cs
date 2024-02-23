using Android.Content;
using Android.Graphics;
using Android.Util;
using BorescopePlugin.MAUI;
using CommunityToolkit.Mvvm.Messaging;
using Java.Lang;
using Java.Util.Concurrent;
using static Android.Graphics.Bitmap;
namespace HoWiFi;
public class StreamSelf
{
    protected static ThreadPoolExecutor EXECUTER = new ThreadPoolExecutor(3, 5, 10, TimeUnit.Seconds, new LinkedBlockingQueue());
    protected const string TAG = "[Borescope Stream]";
    public static bool mTakePhoto = false;
    public static int mVideoFormat = 0;
    public static int mVideoHeight = 480;
    public static int mVideoTmpHeight = 0;
    public static int mVideoTmpWidth = 0;
    public static int mVideoWidth = 640;
    public Context ctx;
    private bool isNeedTakePhoto = false;
    public bool isRunning = false;
    private Media mMedia;
    public NativeLibs mNativeLibs;
    public VideoParams mVideoParams = new VideoParams();
    public byte[] imagem;

    public StreamSelf(Context context)
    {
        ctx = context;
        mMedia = new Media();
        mNativeLibs = new NativeLibs();
    }

    [Obsolete]
    public void startStream()
    {
        mNativeLibs = new NativeLibs();
        if (!isRunning)
        {
            EXECUTER.Execute(new Runnable(() =>
            {
                mNativeLibs.startPreview();
                int i = 0;
                do
                {
                    mVideoParams.video_format = mNativeLibs.getVideoFormat(mVideoParams);
                    msleep(100);
                    if (i > 10)
                    {
                        Log.Error(TAG, "NOT FOUND");
                        WeakReferenceMessenger.Default.Send(this, HandlerParams.ERROR_NOT_FOUND);
                        break;
                    }
                    else
                    {
                        i++;
                    }
                } while (mVideoParams.video_format <= 0);
                mVideoFormat = mVideoParams.video_format;
                mVideoWidth = mVideoParams.video_width;
                mVideoHeight = mVideoParams.video_height;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("mVideoWidth");
                stringBuilder.Append(mVideoWidth);
                stringBuilder.Append(mVideoHeight);
                Log.Debug(TAG, stringBuilder.ToString());
                doExecuteMJPEG();
                if (mNativeLibs != null)
                {
                    mNativeLibs.stopPreview();
                }
            }));
        }
    }

    public void stopStream()
    {
        isRunning = false;
        NativeLibs.nativeAVIRecStop();
    }

    public void destroy()
    {
        stopStream();
        mNativeLibs.destoryCamera();
        mNativeLibs = null;
    }

    [Obsolete]
    public void takePhoto()
    {
        mMedia.playShutter();
        if (PathConfig.getSdcardAvilibleSize() > 100)
        {
            isNeedTakePhoto = true;
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new IntMessage(18), "HoWiFi");
        }
    }

    public void msleep(int i)
    {
        try
        {
            Java.Lang.Thread.Sleep(i);
        }
        catch (InterruptedException e)
        {
            e.PrintStackTrace();
        }
    }

    private Bitmap scaleBitmap(Bitmap bitmap, int i, int i2)
    {
        int width = bitmap.Width;
        int height = bitmap.Height;
        float f = i / width;
        float f2 = i2 / height;
        Matrix matrix = new();
        matrix.PostScale(f, f2);
        return CreateBitmap(bitmap, 0, 0, width, height, matrix, true);
    }

    [Obsolete]
    public void doExecuteMJPEG()
    {
        Bitmap bitmap = null;
        while (isRunning)
        {
            byte[] oneFrameBuffer = mNativeLibs.getOneFrameBuffer();
            if (oneFrameBuffer == null)
            {
                msleep(5);
            }
            else
            {
                int i;
                bitmap = BitmapFactory.DecodeByteArray(oneFrameBuffer, 0, oneFrameBuffer.Length);
                if (bitmap != null)
                {
                    mVideoWidth = bitmap.Width;
                    i = bitmap.Height;
                    mVideoHeight = i;
                    if (mVideoTmpWidth == 0)
                    {
                        mVideoTmpWidth = mVideoWidth;
                        mVideoTmpHeight = i;
                    }
                    if (!(mVideoTmpWidth == mVideoWidth && mVideoTmpHeight == i))
                    {
                        mVideoTmpWidth = 0;
                    }
                    //Take Photo
                    if (isNeedTakePhoto)
                    {
                        Bitmap imageBitmap = scaleBitmap(bitmap, 1600, 1200);
                        MemoryStream stream = new MemoryStream();
                        imageBitmap.Compress(CompressFormat.Jpeg, 80, stream);
                        byte[] b = stream.ToArray();
                        imagem = b;
                        WeakReferenceMessenger.Default.Send(this, HandlerParams.TAKE_PICTURE);
                        isNeedTakePhoto = false;
                    }
                    //Update Imaage
                    else
                    {
                        Bitmap imageBitmap = scaleBitmap(bitmap, 1600, 1200);
                        MemoryStream stream = new MemoryStream();
                        imageBitmap.Compress(CompressFormat.Jpeg, 80, stream);
                        byte[] b = stream.ToArray();
                        imagem = b;
                    }
                }
            }
        }
        bitmap?.Recycle();
    }
}