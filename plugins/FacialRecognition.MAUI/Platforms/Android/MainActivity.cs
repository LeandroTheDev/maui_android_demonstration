using Android.App;
using Android.Content;
using Android.Graphics;
using Java.Lang;

namespace FacialRecognition.MAUI.Platforms.Android
{
    public class MainActivity : IFacial
    {
        Activity? activity;
        Context? context;
        public static Com.Ttv.Face.FaceEngine? facialRecognitionInstance;

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

            if (MainActivity.facialRecognitionInstance == null)
            {
                Console.WriteLine("[FacialRecognition] Instanciating SDK");
                MainActivity.facialRecognitionInstance = Com.Ttv.Face.FaceEngine.CreateInstance(context);
                if (MainActivity.facialRecognitionInstance == null)
                {
                    throw new System.Exception("The SDK Instance returned null");
                }
                MainActivity.facialRecognitionInstance.Init();
            }

            Console.WriteLine("[FacialRecognition] Checking camera permission");
            PermissionStatus permission = await Permissions.RequestAsync<Permissions.Camera>();
            if (permission != PermissionStatus.Granted)
            {
                Console.WriteLine("[FacialRecognition] No camera permission");
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

        public void RegisterFromData(byte[] data)
        {
            Facial.imageRecognition = data;
        }

        public async Task<byte[]> RegisterFromImageBytes(byte[] imageBytes)
        {
            Console.WriteLine("[FacialRecognition] Initializing");
            // Check if activity is null
            if (Platform.CurrentActivity == null)
            {
                throw new NullPointerException("Activity is null");
            }
            activity = Platform.CurrentActivity;
            Console.WriteLine("[FacialRecognition] Activity set");
            // Check if context is null
            if (activity.ApplicationContext == null)
            {
                throw new NullPointerException("Context is null");
            }
            context = activity.ApplicationContext;

            Console.WriteLine("[FacialRecognition] Context set");

            // Inicializing instance if not set
            if (MainActivity.facialRecognitionInstance == null)
            {
                Console.WriteLine("[FacialRecognition] Instanciating SDK");
                MainActivity.facialRecognitionInstance = Com.Ttv.Face.FaceEngine.CreateInstance(context);
                if (MainActivity.facialRecognitionInstance == null)
                {
                    throw new System.Exception("The SDK Instance returned null");
                }
                MainActivity.facialRecognitionInstance.Init();
            }

            // Saving temporary image
            string tmpDirectory = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, "tmp_facial.png");
            File.Delete(tmpDirectory);
            File.WriteAllBytes(tmpDirectory, imageBytes);

            Console.WriteLine($"[FacialRecognition] Temporary Image: {tmpDirectory}");

            // Getting bitmap from file
            Bitmap imageBitmap = await BitmapFactory.DecodeFileAsync(tmpDirectory) ?? throw new NullReferenceException("Temporary file returned null");

            Console.WriteLine($"[FacialRecognition] Checking if temporary file exist: {File.Exists(tmpDirectory)}");

            // Check faces registred equals from the image
            IList<Com.Ttv.Face.FaceResult>? faceResults = MainActivity.facialRecognitionInstance.DetectFaceFromBitmap(imageBitmap);

            if (faceResults == null)
            {
                Console.WriteLine("[FacialRecognition] Null detected in face detection");
                throw new System.Exception("Null detected in face detection");
            }

            if (faceResults.Count == 0)
            {

                Console.WriteLine("[FacialRecognition] No face detected");
                throw new System.Exception("No face detected");
            }

            Console.WriteLine("[FacialRecognition] Face detected");
            MainActivity.facialRecognitionInstance.ExtractFeatureFromBitmap(imageBitmap, faceResults);
            Console.WriteLine("[FacialRecognition] Recognition from bitmap extracted");

            // Get the face
            Com.Ttv.Face.FaceResult face = faceResults[0];
            if (face.Feature == null)
            {
                Console.WriteLine("[FacialRecognition] Recognition bytes null");
                throw new System.Exception("Recognition failed");
            }
            Console.WriteLine("[FacialRecognition] Recognition bytes received");
            // Get the face score
            float faceLimiar = face.Liveness;

            // Get the encription for recognition
            byte[] faceFeature = face.Feature.ToArray();

            // Saving the registred feature in static
            Facial.imageRecognition = faceFeature;

            return faceFeature;
        }

        public async Task<bool> PerformAnalyze(INavigation Navigation)
        {
            //Check if exist any data registred
            if (Facial.imageRecognition == null)
            {
                throw new NullPointerException("No images registred");
            }
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

            if (MainActivity.facialRecognitionInstance == null)
            {
                MainActivity.facialRecognitionInstance = Com.Ttv.Face.FaceEngine.CreateInstance(context);
                if (MainActivity.facialRecognitionInstance == null)
                {
                    throw new System.Exception("The SDK Instance returned null");
                }
                MainActivity.facialRecognitionInstance.Init();
            }

            Console.WriteLine("[FacialRecognition] Checking camera permission");
            PermissionStatus permission = await Permissions.RequestAsync<Permissions.Camera>();
            if (permission != PermissionStatus.Granted)
            {
                Console.WriteLine("[FacialRecognition] No camera permission");
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

        public async Task<bool> PerformAnalyzeFromImageBytes(byte[] imageBytes)
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

            if (MainActivity.facialRecognitionInstance == null)
            {
                Console.WriteLine("[FacialRecognition] Instanciating SDK");
                MainActivity.facialRecognitionInstance = Com.Ttv.Face.FaceEngine.CreateInstance(context);
                if (MainActivity.facialRecognitionInstance == null)
                {
                    throw new System.Exception("The SDK Instance returned null");
                }
                MainActivity.facialRecognitionInstance.Init();
            }
            if (Facial.imageRecognition == null)
            {
                throw new NullPointerException("No images registred");
            }

            // Saving temporary image
            string tmpDirectory = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, "tmp_facial.png");
            File.Delete(tmpDirectory);
            File.WriteAllBytes(tmpDirectory, imageBytes);

            Console.WriteLine($"[FacialRecognition] Temporary Image: {tmpDirectory}");

            // Getting bitmap from file
            Bitmap imageBitmap = await BitmapFactory.DecodeFileAsync(tmpDirectory) ?? throw new NullReferenceException("Temporary file returned null");

            // Check faces registred equals from the image
            IList<Com.Ttv.Face.FaceResult>? faceResults = MainActivity.facialRecognitionInstance.DetectFaceFromBitmap(imageBitmap);

            if (faceResults == null)
            {
                Console.WriteLine("[FacialRecognition] Null detected in face detection");
                return false;
            }

            if (faceResults.Count == 0)
            {
                Console.WriteLine("[FacialRecognition] No face detected");
                return false;
            }

            Console.WriteLine("[FacialRecognition] Face detected");
            MainActivity.facialRecognitionInstance.ExtractFeatureFromBitmap(imageBitmap, faceResults);
            Console.WriteLine("[FacialRecognition] Recognition from bitmap extracted");

            // Get the face
            Com.Ttv.Face.FaceResult face = faceResults[0];
            // Get the facial recognition limiar
            if (face.Feature == null)
            {
                Console.WriteLine("[FacialRecognition] Recognition failed");
                return false;
            }
            Console.WriteLine($"[FacialRecognition] Recognition received");

            // Get the score between the 2 images
            float score = MainActivity.facialRecognitionInstance.CompareFeature(face.Feature.ToArray(), Facial.imageRecognition);
            float faceScore = face.Liveness; // Indicates how the face is real

            if (score > 78)
            {
                Console.WriteLine($"[FacialRecognition] Score: {score}");
                return true;
            }
            else
            {
                Console.WriteLine($"[FacialRecognition] Score: {score}");
                return false;
            }
        }
    }

    static public class Interface
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
