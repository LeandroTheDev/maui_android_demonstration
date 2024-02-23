using System.Runtime.InteropServices;
using Android.Graphics;

namespace HoWiFi;
public class NativeLibs
{
    private const string Library = "libmlcamera-2.5.so";
    public const short FORMAT_VIDEO_H264_I = 2;
    public const short FORMAT_VIDEO_H264_P = 3;
    public const short FORMAT_VIDEO_MJPEG = 1;
    public const short FORMAT_VIDEO_YUYV = 4;
    public const int ML_PORT_SERIES3 = 8030;
    public const int ML_PORT_SERIES5 = 7060;
    public const short STREAM_PASS_DEFAULT = 0;
    public const short STREAM_PASS_ERR = 2;
    public const short STREAM_PASS_OK = 1;
    private long mNativePtr = nativeCreateCamera();

    [DllImport(Library, EntryPoint = "nativeABGR2YUV", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] nativeABGR2YUV(long j, int i);

    [DllImport(Library, EntryPoint = "nativeAVIClose", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAVIClose();

    [DllImport(Library, EntryPoint = "nativeAVIGetFrameAtIndex", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] nativeAVIGetFrameAtIndex(int i);

    [DllImport(Library, EntryPoint = "nativeAVIGetFrameAtTime", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] nativeAVIGetFrameAtTime(double d);

    [DllImport(Library, EntryPoint = "nativeAVIGetTotalFrame", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeAVIGetTotalFrame();

    [DllImport(Library, EntryPoint = "nativeAVIGetTotalTime", CallingConvention = CallingConvention.Cdecl)]
    public static extern double nativeAVIGetTotalTime();

    [DllImport(Library, EntryPoint = "nativeAVIGetVoiceAtTime", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] nativeAVIGetVoiceAtTime(double d);

    [DllImport(Library, EntryPoint = "nativeAVIOpen", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool nativeAVIOpen(String str);

    [DllImport(Library, EntryPoint = "nativeAVIRecAddData", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAVIRecAddData(byte[] bArr, int i);

    [DllImport(Library, EntryPoint = "nativeAVIRecAddWav", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAVIRecAddWav(byte[] bArr, int i);

    [DllImport(Library, EntryPoint = "nativeAVIRecGetTimestamp", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeAVIRecGetTimestamp();

    [DllImport(Library, EntryPoint = "nativeAVIRecSetAudioParams", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAVIRecSetAudioParams(int i, int i2, int i3);

    [DllImport(Library, EntryPoint = "nativeAVIRecSetParams", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAVIRecSetParams(int i, int i2, int i3);

    [DllImport(Library, EntryPoint = "nativeAVIRecStart", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool nativeAVIRecStart(String str);

    [DllImport(Library, EntryPoint = "nativeAVIRecStop", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAVIRecStop();

    [DllImport(Library, EntryPoint = "nativeAddMP4VideoData", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeAddMP4VideoData(byte[] bArr, int i);

    [DllImport(Library, EntryPoint = "nativeCmdClearPassword", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdClearPassword();

    [DllImport(Library, EntryPoint = "nativeCmdGetBattery", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdGetBattery();

    [DllImport(Library, EntryPoint = "nativeCmdGetPWM", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdGetPWM();

    [DllImport(Library, EntryPoint = "nativeCmdGetRemoteKey", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdGetRemoteKey();

    [DllImport(Library, EntryPoint = "nativeCmdGetResolution", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] nativeCmdGetResolution();

    [DllImport(Library, EntryPoint = "nativeCmdGetVideoFormat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdGetVideoFormat();

    [DllImport(Library, EntryPoint = "nativeCmdSendReboot", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSendReboot();

    [DllImport(Library, EntryPoint = "nativeCmdSetChannel", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetChannel(int i);

    [DllImport(Library, EntryPoint = "nativeCmdSetName", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetName(String str);

    [DllImport(Library, EntryPoint = "nativeCmdSetPWM", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetPWM(int i);

    [DllImport(Library, EntryPoint = "nativeCmdSetPassword", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetPassword(String str);

    [DllImport(Library, EntryPoint = "nativeCmdSetResolution", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetResolution(int i, int i2, int i3);

    [DllImport(Library, EntryPoint = "nativeCmdSetStreamPasswd", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetStreamPasswd(String str);

    [DllImport(Library, EntryPoint = "nativeCmdSetSwitchMode", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetSwitchMode();

    [DllImport(Library, EntryPoint = "nativeCmdSetVideoFormat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSetVideoFormat(int i);

    [DllImport(Library, EntryPoint = "nativeCmdSwitchCam", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeCmdSwitchCam();

    [DllImport(Library, EntryPoint = "nativeCreateCamera", CallingConvention = CallingConvention.Cdecl)]
    private static extern long nativeCreateCamera();

    [DllImport(Library, EntryPoint = "nativeDestroyCamera", CallingConvention = CallingConvention.Cdecl)]
    private static extern void nativeDestroyCamera(long j);

    [DllImport(Library, EntryPoint = "nativeGetAccelerometer", CallingConvention = CallingConvention.Cdecl)]
    private static extern int nativeGetAccelerometer(long j);

    [DllImport(Library, EntryPoint = "nativeGetFrameBuffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte[] nativeGetFrameBuffer(long j);

    [DllImport(Library, EntryPoint = "nativeGetPassErrorBuf", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] nativeGetPassErrorBuf();

    [DllImport(Library, EntryPoint = "nativeGetVideoFormat", CallingConvention = CallingConvention.Cdecl)]
    private static extern int nativeGetVideoFormat(long j, VideoParams videoParams);

    [DllImport(Library, EntryPoint = "nativeGetVideoFrameRate", CallingConvention = CallingConvention.Cdecl)]
    private static extern int nativeGetVideoFrameRate(long j);

    [DllImport(Library, EntryPoint = "nativePushBmpData", CallingConvention = CallingConvention.Cdecl)]
    private static extern void nativePushBmpData(long j, Bitmap bitmap);

    [DllImport(Library, EntryPoint = "nativeSetStreamPasswd", CallingConvention = CallingConvention.Cdecl)]
    private static extern int nativeSetStreamPasswd(long j, String str);

    [DllImport(Library, EntryPoint = "nativeStartMP4Record", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeStartMP4Record(String str, int i, int i2);

    [DllImport(Library, EntryPoint = "nativeStartPreview", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool nativeStartPreview(long j, int i, int i2);

    [DllImport(Library, EntryPoint = "nativeStopMP4Record", CallingConvention = CallingConvention.Cdecl)]
    public static extern void nativeStopMP4Record();

    [DllImport(Library, EntryPoint = "nativeStopPreview", CallingConvention = CallingConvention.Cdecl)]
    private static extern void nativeStopPreview(long j);

    [DllImport(Library, EntryPoint = "nativeUsbDecodeData", CallingConvention = CallingConvention.Cdecl)]
    public static extern int nativeUsbDecodeData(long j, byte[] bArr, int i);

    [DllImport(Library, EntryPoint = "nativeYUV2ABGR", CallingConvention = CallingConvention.Cdecl)]
    private static extern int nativeYUV2ABGR(long j, Bitmap bitmap);

    public void destoryCamera()
    {
        nativeDestroyCamera(this.mNativePtr);
        this.mNativePtr = 0;
    }

    public int decodeUsbData(byte[] bArr, int i)
    {
        return nativeUsbDecodeData(this.mNativePtr, bArr, i);
    }

    public bool startPreview()
    {
        return nativeStartPreview(this.mNativePtr, ML_PORT_SERIES5, ML_PORT_SERIES3);
    }

    public void stopPreview()
    {
        nativeStopPreview(this.mNativePtr);
    }

    public byte[] getOneFrameBuffer()
    {
        return nativeGetFrameBuffer(this.mNativePtr);
    }

    public int drawYUV2ARGB(Bitmap bitmap)
    {
        return nativeYUV2ABGR(this.mNativePtr, bitmap);
    }

    public int getVideoFormat(VideoParams videoParams)
    {
        return nativeGetVideoFormat(this.mNativePtr, videoParams);
    }

    public void pushBmpDataQueue(Bitmap bitmap)
    {
        nativePushBmpData(this.mNativePtr, bitmap);
    }

    public byte[] convertBMP2YUV(int i)
    {
        return nativeABGR2YUV(this.mNativePtr, i);
    }

    public int getVideoFrameRate()
    {
        return nativeGetVideoFrameRate(this.mNativePtr);
    }

    public int getAccelerometer()
    {
        return nativeGetAccelerometer(this.mNativePtr);
    }

    public int setStreamPasswd(String str)
    {
        return nativeSetStreamPasswd(this.mNativePtr, str);
    }
}
