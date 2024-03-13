using Android.App;
using Android.Content;
using Java.Lang;

namespace FacialRecognition.MAUI.Platforms.Android
{
    public class MainActivity : IFacial
    {
        Activity? activity;
        Context? context;

        /// <summary>
        /// Ask for photo and register the new image to be used in Facial Recognition
        /// </summary>
        /// <param name="Navigation"></param>
        /// <returns></returns>
        /// <exception cref="NullPointerException"></exception>
        /// <exception cref="System.Exception"></exception>
        public async Task<object> RegisterNewImage(INavigation Navigation)
        {
            Console.WriteLine("[FacialRecognition] Initializing");
            //Check if activity is null
            if (Platform.CurrentActivity == null)
            {
                throw new NullPointerException("Activity is null");
            }
            activity = Platform.CurrentActivity;
            Console.WriteLine("[FacialRecognition] Activity set");
            //Check if context is null
            if (activity.ApplicationContext == null)
            {
                throw new NullPointerException("Context is null");
            }
            context = activity.ApplicationContext;
            Console.WriteLine("[FacialRecognition] Context set");

            Console.WriteLine("[FacialRecognition] Checking camera permission");
            if (!new PermissionHandler(activity).HasCameraPermission())
            {
                Console.WriteLine("[FacialRecognition] No camera permission, requesting...");
                new PermissionHandler(activity).RequestCameraPermission();
                throw new System.Exception("Permission Denied");
            }
            Console.WriteLine("[FacialRecognition] Device has camera permission");

            // Initializes the Facial Camera
            CameraActivity cameraActivity = new(context, "register");

            // Creates the task for waiting the reuslt
            TaskCompletionSource<object> result = new();

            // On camera closed get the result and set it
            cameraActivity.Closed += (sender, facialResult) =>
            {
                // Converting value
                var resultObject = facialResult as dynamic;
                if (resultObject != null)
                {
                    Console.WriteLine("[FacialRecognition] Facial new register finished");
                    byte[] recognitionImage = resultObject.recognitionImage;
                    string bruteImage = resultObject.bruteImage;
                    if (recognitionImage == null || bruteImage == null)
                    {
                        Console.WriteLine("[FacialRecognition] ERROR Facial Register returned invalid values");
                        throw new System.Exception("Facial returned invalid values");
                    }
                    Console.WriteLine("[FacialRecognition] Facial new register success");
                    // Setting the data into reference
                    Facial.imageRecognition = recognitionImage;
                    result.SetResult(new
                    {
                        recognitionImage,
                        bruteImage,
                        message = "Success",
                    });
                    Console.WriteLine(recognitionImage);
                }
                else
                {
                    Console.WriteLine("[FacialRecognition] ERROR Facial Register returned null");
                    throw new System.Exception("Facial returned null");
                }
            };

            // Loads the Facial Camera
            await Navigation.PushModalAsync(cameraActivity);

            // Wait until the facial camera returns
            return await result.Task;
        }

        /// <summary>
        /// Register the data to recognize images manually
        /// </summary>
        /// <param name="data"></param>
        public void RegisterFromData(byte[] data)
        {
            Facial.imageRecognition = data;
        }

        /// <summary>
        /// Asks for a photo them
        /// performs analyze to verify the face integrity
        /// of the 2 images, the registred one and the taked photo
        /// </summary>
        /// <param name="Navigation"></param>
        /// <returns></returns>
        /// <exception cref="NullPointerException"></exception>
        /// <exception cref="System.Exception"></exception>
        public async Task<bool> PerformAnalyze(INavigation Navigation)
        {
            Console.WriteLine("[FacialRecognition] Initializing");
            //Check if activity is null
            if (Platform.CurrentActivity == null)
            {
                throw new NullPointerException("Activity is null");
            }
            activity = Platform.CurrentActivity;
            Console.WriteLine("[FacialRecognition] Activity set");
            //Check if context is null
            if (activity.ApplicationContext == null)
            {
                throw new NullPointerException("Context is null");
            }
            context = activity.ApplicationContext;
            Console.WriteLine("[FacialRecognition] Context set");

            Console.WriteLine("[FacialRecognition] Checking camera permission");
            if (!new PermissionHandler(activity).HasCameraPermission())
            {
                Console.WriteLine("[FacialRecognition] No camera permission, requesting...");
                new PermissionHandler(activity).RequestCameraPermission();
                throw new System.Exception("Permission Denied");
            }
            Console.WriteLine("[FacialRecognition] Device has camera permission");

            // Initializes the Facial Camera
            CameraActivity cameraActivity = new(context, "analyze");

            // Creates the task for waiting the reuslt
            TaskCompletionSource<bool> result = new();

            // On camera closed get the result and set it
            cameraActivity.Closed += (sender, facialResult) =>
            {
                // Converting value
                bool? resultObject = facialResult as dynamic;
                if (resultObject != null)
                {
                    Console.WriteLine($"[FacialRecognition] Facial anaylzes finished {resultObject}");
                    result.SetResult((bool)resultObject);
                }
                else
                {
                    Console.WriteLine("[FacialRecognition] ERROR Facial analyzes returned null");
                    throw new NullPointerException("Facial returned null");
                }
            };

            // Loads the Facial Camera
            await Navigation.PushModalAsync(cameraActivity);

            // Wait until the facial camera returns
            return await result.Task;
        }
    }

    static public class AndroidInterface
    {
        /// <summary>
        /// Creates the facial recognition interfaces for android devices
        /// </summary>
        public static void GenerateFacialRecognitionInterface()
        {
            Console.WriteLine("[FacialRecognition Interface] Registering the Facial Recognition Interface");
            DependencyService.Register<IFacial, MainActivity>();
            Console.WriteLine("[FacialRecognition Interface] Registration Completed");
        }
    }
}
