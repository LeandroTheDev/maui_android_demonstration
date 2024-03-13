using Android.Content;
using Android.Graphics;
using Android.Widget;
using Media = Android.Media;

namespace FacialRecognition.MAUI.Platforms.Android;

public partial class CameraActivity : ContentPage
{
    /// <summary>
    /// Instance for the FacialRecognition SDK
    /// </summary>
    private readonly Com.Ttv.Face.FaceEngine? facialRecognitionInstance;
    /// <summary>
    /// Returns true if Recognition is the same from registred and false if not the same
    /// </summary>
    public event EventHandler<object>? Closed;
    private readonly Context context;
    public CameraActivity(Context _context, string type)
    {

        InitializeComponent();
        context = _context;
        //Adding the functions to the buttons
        closeFacial.Clicked += CameraClose;
        if (type == "register")
        {
            takePhoto.Clicked += CameraTakePhotoRegister;
        }
        else if (type == "analyze")
        {
            takePhoto.Clicked += CameraTakePhotoAnalyze;
        }
        else
        {
            throw new ArgumentException($"Invalid type {type}");
        }
        switchCameras.Clicked += CameraPositionSwitch;

        Console.WriteLine("[FacialRecognition] Instanciating SDK");
        facialRecognitionInstance = Com.Ttv.Face.FaceEngine.CreateInstance(context);
        if (facialRecognitionInstance == null)
        {
            throw new Exception("The SDK Instance returned null");
        }
        facialRecognitionInstance.Init();


        Console.WriteLine("[FacialRecognition] Loading Facial Camera");
        // Create a timer for checking the camera initialization
        Timer? timer = null;
        timer = new Timer((state) =>
        {
            // Check if camera is initialized
            if (cameraView.Cameras.Count >= 1)
            {
                Console.WriteLine("[FacialRecognition] Camera loaded, starting...");
                // We need a delay here because
                // starting the camera after before the initialization
                // will come with a black screen instead the camera
                Task.Delay(500).ContinueWith((_) =>
                {
                    StartCamera();
                });
                timer?.Dispose();
            }
        }, null, 0, 100);
    }

    private void StartCamera()
    {
        cameraView.Camera = cameraView.Cameras[1];
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            await cameraView.StartCameraAsync();
            Console.WriteLine("[FacialRecognition] Camera started");
        });
    }

    private void CameraClose(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private async void CameraTakePhotoRegister(object? sender, EventArgs e)
    {
        Console.WriteLine("[FacialRecognition] Take photo called");

        //If instance doesnt exist
        if (facialRecognitionInstance == null)
        {
            Console.WriteLine("[FacialRecognition] ERROR SDK Instance is null");
            Closed?.Invoke(this, false);
            await Navigation.PopModalAsync();
            return;
        }

        Console.WriteLine("[FacialRecognition] Getting image bytes");

        // Stream Creation
        Stream stream = await cameraView.TakePhotoAsync();

        // Memory stream to store bytes
        MemoryStream memoryStream = new();

        // Use stream to copy brute bytes to MemoryStream
        await stream.CopyToAsync(memoryStream);

        Console.WriteLine("[FacialRecognition] Getting original bitmap from image");

        // Getting bytes from image
        byte[] imageBytes = memoryStream.ToArray();

        Console.WriteLine($"[FacialRecognition] Saving temporary file");

        // Saving temporary image
        string tmp_directory = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, "tmp_facial.png");
        File.Delete(tmp_directory);
        File.WriteAllBytes(tmp_directory, imageBytes);

        Console.WriteLine($"[FacialRecognition] Temporary Image: {tmp_directory}");

        // Getting bitmap from file
        Bitmap imageBitmap = await BitmapFactory.DecodeFileAsync(tmp_directory) ?? throw new NullReferenceException("Temporary file returned null");

        Console.WriteLine($"[FacialRecognition] Checking if temporary file exist: {File.Exists(tmp_directory)}");

        // Check faces registred equals from the image
        IList<Com.Ttv.Face.FaceResult>? faceResults = facialRecognitionInstance.DetectFaceFromBitmap(imageBitmap);

        if (faceResults == null)
        {
            Console.WriteLine("[FacialRecognition] Null detected in face detection");
            _ = DisplayAlert("Alert", "Null detected in face detection", "OK");
            return;
        }

        if (faceResults.Count == 0)
        {

            Console.WriteLine("[FacialRecognition] No face detected");
            _ = DisplayAlert("Alert", "No face detected", "OK");
            return;
        }

        Console.WriteLine("[FacialRecognition] Face detected");
        facialRecognitionInstance.ExtractFeatureFromBitmap(imageBitmap, faceResults);
        Console.WriteLine("[FacialRecognition] Recognition from bitmap extracted");

        // Get the face
        Com.Ttv.Face.FaceResult face = faceResults[0];
        if (face.Feature == null)
        {
            Console.WriteLine("[FacialRecognition] Recognition bytes null");
            _ = DisplayAlert("Alert", "Recognition failed", "OK");
            return;
        }
        Console.WriteLine("[FacialRecognition] Recognition bytes received");
        // Get the face score
        float faceLimiar = face.Liveness;

        // Get the encription for recognition
        byte[] faceFeature = face.Feature.ToArray();

        // Saving the registred feature in static
        Facial.imageRecognition = faceFeature;

        Console.WriteLine("[FacialRecognition] Converting images to Base64");

        // Return the values
        Closed?.Invoke(this, new
        {
            recognitionImage = faceFeature,
            bruteImage = Convert.ToBase64String(imageBytes),
        });
        // Close the camera
        await Navigation.PopModalAsync();
    }

    private async void CameraTakePhotoAnalyze(object? sender, EventArgs e)
    {
        Console.WriteLine("[FacialRecognition] Take photo called");

        //If instance doesnt exist
        if (facialRecognitionInstance == null)
        {
            Console.WriteLine("[FacialRecognition] ERROR SDK Instance is null");
            Closed?.Invoke(this, false);
            await Navigation.PopModalAsync();
            return;
        }

        Console.WriteLine("[FacialRecognition] Getting image bytes");

        // Stream Creation
        using Stream stream = await cameraView.TakePhotoAsync();

        // Memory stream to store bytes
        using MemoryStream memoryStream = new();

        // Use stream to copy brute bytes to MemoryStream
        await stream.CopyToAsync(memoryStream);

        // Get bytes from MemoryStream
        byte[] imageBytes = memoryStream.ToArray();

        Console.WriteLine($"[FacialRecognition] Saving temporary file");

        // Saving temporary image
        string tmp_directory = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, "tmp_facial.png");
        File.Delete(tmp_directory);
        File.WriteAllBytes(tmp_directory, imageBytes);

        Console.WriteLine($"[FacialRecognition] Temporary Image: {tmp_directory}");

        // Getting bitmap from file
        Bitmap imageBitmap = await BitmapFactory.DecodeFileAsync(tmp_directory) ?? throw new NullReferenceException("Temporary file returned null");

        Console.WriteLine($"[FacialRecognition] Checking if temporary file exist: {File.Exists(tmp_directory)}");

        // Check faces registred equals from the image
        IList<Com.Ttv.Face.FaceResult>? faceResults = facialRecognitionInstance.DetectFaceFromBitmap(imageBitmap);

        if (faceResults == null)
        {
            Console.WriteLine("[FacialRecognition] Null detected in face detection");
            _ = DisplayAlert("Alert", "Null detected in face detection", "OK");
            return;
        }

        if (faceResults.Count == 0)
        {

            Console.WriteLine("[FacialRecognition] No face detected");
            _ = DisplayAlert("Alert", "No face detected", "OK");
            return;
        }

        Console.WriteLine("[FacialRecognition] Face detected");
        facialRecognitionInstance.ExtractFeatureFromBitmap(imageBitmap, faceResults);
        Console.WriteLine("[FacialRecognition] Recognition from bitmap extracted");

        // Get the face
        Com.Ttv.Face.FaceResult face = faceResults[0];
        // Get the facial recognition limiar
        if (face.Feature == null)
        {
            Console.WriteLine("[FacialRecognition] Recognition failed");
            _ = DisplayAlert("Alert", "Recognition failed", "OK");
            return;
        }
        Console.WriteLine($"[FacialRecognition] Recognition received");

        // Get the score between the 2 images
        float score = facialRecognitionInstance.CompareFeature(face.Feature.ToArray(), Facial.imageRecognition);
        float faceScore = face.Liveness; // Indicates how the face is real

        if (score > 78 && faceScore > 0.7)
        {
            Console.WriteLine($"[FacialRecognition] Score: {score}, Liveness: {faceScore}");
            // Return the values
            Closed?.Invoke(this, true);
        }
        else
        {
            Console.WriteLine($"[FacialRecognition] Score: {score}, Liveness: {faceScore}");
            // Return the values
            Closed?.Invoke(this, false);
        }
    }

    private void CameraPositionSwitch(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}