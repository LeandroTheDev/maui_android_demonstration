using BorescopePlugin.MAUI;
using CommunityToolkit.Mvvm.Messaging;
using Java.Lang;
using Thread = Java.Lang.Thread;

namespace HoWiFi;
public class CmdSocket
{
    public bool threadRunning = false;

    public void startRunKeyThread()
    {
        threadRunning = true;

        new Thread(new Runnable(() =>
        {
            while (threadRunning)
            {
                int nativeCmdGetRemoteKey = NativeLibs.nativeCmdGetRemoteKey();
                if (nativeCmdGetRemoteKey == 1)
                {
                    WeakReferenceMessenger.Default.Send(new IntMessage(21), "HoWiFi");
                }
                else if (nativeCmdGetRemoteKey == 2)
                {
                    WeakReferenceMessenger.Default.Send(new IntMessage(22), "HoWiFi");
                }
                else if (nativeCmdGetRemoteKey == 3)
                {
                    WeakReferenceMessenger.Default.Send(new IntMessage(23), "HoWiFi");
                }
                else if (nativeCmdGetRemoteKey == 4)
                {
                    WeakReferenceMessenger.Default.Send(new IntMessage(24), "HoWiFi");
                }
                msleep(200);
            }
        })).Start();
    }

    public void stopRunKeyThread()
    {
        threadRunning = false;
    }

    public void msleep(int i)
    {
        try
        {
            Thread.Sleep(i);
        }
        catch (InterruptedException e)
        {
            e.PrintStackTrace();
        }
    }
}