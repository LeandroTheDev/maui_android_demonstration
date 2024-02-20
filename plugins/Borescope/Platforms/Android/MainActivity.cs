#pragma warning disable CS0612
using Android.Content;
using Android.Graphics;
using BorescopePlugin;
using CommunityToolkit.Mvvm.Messaging;

namespace BorescopePlugin.MAUI;
public class MainActivity : IBorescope
{
    private MauiAppCompatActivity app_activity;
    private Context context;

    //HoWiFi Variables
    private HoWiFi.StreamSelf howifi_stream;
    private HoWiFi.CmdSocket howifi_socket;
    private event EventHandler<ImageSource> howifi_callback;

    static public void Generate_Borescope_Interface()
    {
        Console.WriteLine("[Borescope Interface] Registering the Borescope Interface");
        DependencyService.Register<IBorescope, MainActivity>();
        Console.WriteLine("[Borescope Interface] Registration Completed");
    }

    public void Instanciate_Borescope(object activity)
    {
        Console.WriteLine("[Borescope] Getting Activity from MainActivity");
        app_activity = (MauiAppCompatActivity)activity;
        context = app_activity.ApplicationContext;
        Console.WriteLine("[Borescope] Activity Set");
    }

    //HoWiFi
    public void Start_HoWiFi(EventHandler<ImageSource> callback)
    {
        howifi_callback = callback;
        Console.WriteLine("[Borescope HoWiFi] Starting HoWiFiBorescope");
        WeakReferenceMessenger.Default.Register<IntMessage>("HoWiFi", (token, message) =>
        {
            int? i = message.Data;
            if (i == null) { return; }
            Console.WriteLine($"[Borescope HoWiFi] Handler Called: {i}");
            switch (i)
            {
                case HoWiFi.HandlerParams.REMOTE_TAKE_PHOTO: howifi_stream.takePhoto(); break;
                case HoWiFi.HandlerParams.TAKE_PICTURE: Finish_HoWiFi(howifi_stream.imagem); break;
                case HoWiFi.HandlerParams.ERROR_NOT_FOUND: Finish_HoWiFi(null); break;
                case HoWiFi.HandlerParams.SDCARD_FULL: Finish_HoWiFi(null); break;
            }
        });
        Console.WriteLine("[Borescope HoWiFi] Handler Registered");
        howifi_stream = new HoWiFi.StreamSelf(context);
        howifi_socket = new HoWiFi.CmdSocket();
        Console.WriteLine("[Borescope HoWiFi] HoWiFi Created");
        howifi_socket.startRunKeyThread();
        Console.WriteLine("[Borescope HoWiFi] Socket Started");
        howifi_stream.startStream();
        Console.WriteLine("[Borescope HoWiFi] Stream Started");
    }
    public void Destroy_HoWiFi()
    {
        Console.WriteLine("[Borescope HoWiFi] Started Destroying HoWiFi Borescope");
        howifi_stream.destroy();
        howifi_stream.isRunning = false;
        howifi_socket.stopRunKeyThread();
        WeakReferenceMessenger.Default.Unregister<IntMessage>("HoWiFi");
        Console.WriteLine("[Borescope HoWiFi] Finishing Destroying HoWiFi Borescope");
    }
    public void Finish_HoWiFi(byte[] image)
    {
        Console.WriteLine("[Borescope HoWiFi] Finish Callback called");
        Intent resultIntent = new();
        if (image != null)
        {
            Console.WriteLine("[Borescope HoWiFi] Converting Byte Array into ImageSource");
            MemoryStream stream = new(image);
            howifi_callback.Invoke(this, ImageSource.FromStream(() => stream));
            Console.WriteLine("[Borescope HoWiFi] Callback Invoked");
        }
        else
        {
            Console.WriteLine("[Borescope HoWiFi] Canceled");
            howifi_callback.Invoke(this, null);
        }
        Destroy_HoWiFi();
    }
    public ImageSource Get_Stream_Image_HoWiFi()
    {
        return ImageSource.FromStream(() => new MemoryStream(howifi_stream.imagem));
    }
}

public class IntMessage
{
    public int Data { get; }
    public Bitmap Bitmap { get; }

    public IntMessage(int data, Bitmap bitmap = null)
    {
        Data = data;
        Bitmap = bitmap;
    }
}