#pragma warning disable CS0612
using Android.Content;
using Android.Hardware.Lights;
using static Android.Icu.Text.ListFormatter;

namespace HoWiFi
{
    public class Instance
    {
        //HoWiFi Variables
        private readonly StreamSelf howifiStream;
        private readonly CmdSocket howifiSocket;
        private readonly Android.App.Activity activity;
        public ImageSource? image;

        public Instance(Android.App.Activity _activity)
        {
            activity = _activity;
            howifiStream = new StreamSelf(activity.ApplicationContext ?? throw new NullReferenceException("Context is null in HoWifi initialization"));
            howifiSocket = new CmdSocket();
            Console.WriteLine("[Borescope HoWiFi] HoWiFi Created");
            howifiSocket.startRunKeyThread();
            Console.WriteLine("[Borescope HoWiFi] Socket Started");
            howifiStream.startStream();
            Console.WriteLine("[Borescope HoWiFi] Stream Started");
        }

        public void Destroy()
        {
            howifiStream.destroy();
            howifiStream.isRunning = false;
            howifiSocket.stopRunKeyThread();
            Console.WriteLine("[Borescope HoWiFi] HoWiFi Borescope Destroyed");
        }

        public void Finish(byte[] image)
        {
            Console.WriteLine("[Borescope HoWiFi] Finishing...");
            if (image != null)
            {
                Console.WriteLine("[Borescope HoWiFi] Converting Byte Array into ImageSource");
                MemoryStream stream = new(image);
                Console.WriteLine("[Borescope HoWiFi] Callback Invoked");
            }
            else
            {
                Console.WriteLine("[Borescope HoWiFi] Canceled");
            }
            Destroy();
        }
    }
}
