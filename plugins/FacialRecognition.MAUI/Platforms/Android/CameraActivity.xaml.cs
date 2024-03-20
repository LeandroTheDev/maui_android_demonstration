using Android.Content;
using Android.Graphics;
using Android.Widget;
using Camera.MAUI;
using Media = Android.Media;

namespace FacialRecognition.MAUI.Platforms.Android;

public partial class CameraActivity : ContentPage
{
    /// <summary>
    /// Register:
    /// Returns any object with the parameters:
    /// <para>- recognitionImage: byte[]</para>
    /// <para>- bruteImage: string (base64)</para>
    /// Analyzes:
    /// Returns true if Recognition is the same from registred and false if not the same
    /// </summary>
    public event EventHandler<object>? Closed;
    /// <summary>
    /// Android application context
    /// </summary>
    private readonly Context context;
    /// <summary>
    /// Camera position (frontal, back)
    /// </summary>
    private int cameraPosition = 1;
    /// <summary>
    /// If cameraBusy is true all functions will do nothing
    /// </summary>
    private bool cameraBusy = false;
    public CameraActivity(Context _context, string type)
    {

        InitializeComponent();
        context = _context;
        cameraView.CamerasLoaded += new EventHandler(CameraLoaded);
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
    }

    /// <summary>
    /// Starts the camera
    /// </summary>
    private void StartCamera()
    {
        cameraView.Camera = cameraView.Cameras[cameraPosition];
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            await cameraView.StartCameraAsync();
            Console.WriteLine("[FacialRecognition] Camera started");
        });
    }

    /// <summary>
    /// Called when camera is loaded
    /// </summary>
    private void CameraLoaded(object? obj, EventArgs args)
    {
        cameraView.Camera = cameraView.Cameras[cameraPosition];
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await cameraView.StopCameraAsync();
            // The delay is necessary for compatibility
            _ = Task.Delay(200).ContinueWith(async (_) =>
            {
                await cameraView.StartCameraAsync();
            });
        });
    }

    /// <summary>
    /// Close button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CameraClose(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    /// <summary>
    /// Default function for register method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NullReferenceException"></exception>
    private void CameraTakePhotoRegister(object? sender, EventArgs e)
    {
        // Running in secondary thread to not freeze the application while processing
        _ = Task.Run(async () =>
        {
            //Stop function if camera is busy
            if (cameraBusy)
            {
                Console.WriteLine("[FacialRecognition] Register cancelled, the camera is busy");
                return;
            }
            else
            {
                cameraBusy = true;
                //Enable the timer for busy delay
                _ = Task.Delay(1000).ContinueWith((_) =>
                {
                    cameraBusy = false;
                });
            }
            Console.WriteLine("[FacialRecognition] Take photo called");

            //If instance doesnt exist
            if (MainActivity.facialRecognitionInstance == null)
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
            IList<Com.Ttv.Face.FaceResult>? faceResults = MainActivity.facialRecognitionInstance.DetectFaceFromBitmap(imageBitmap);

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
            MainActivity.facialRecognitionInstance.ExtractFeatureFromBitmap(imageBitmap, faceResults);
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

            // Close the camera
            await Navigation.PopModalAsync();

            // Return the values
            Closed?.Invoke(this, new
            {
                recognitionImage = faceFeature,
                bruteImage = Convert.ToBase64String(imageBytes),
            });
        });
    }

    /// <summary>
    /// Default function for analyze method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NullReferenceException"></exception>
    private void CameraTakePhotoAnalyze(object? sender, EventArgs e)
    {
        // Running in secondary thread to not freeze the application while processing
        _ = Task.Run(async () =>
        {
            //Stop function if camera is busy
            if (cameraBusy)
            {
                Console.WriteLine("[FacialRecognition] Analyze cancelled, the camera is busy");
                return;
            }
            else
            {
                cameraBusy = true;
                //Enable the timer for busy delay
                _ = Task.Delay(1000).ContinueWith((_) =>
                {
                    cameraBusy = false;
                });
            }
            Console.WriteLine("[FacialRecognition] Take photo called");

            //If instance doesnt exist
            if (MainActivity.facialRecognitionInstance == null)
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
            IList<Com.Ttv.Face.FaceResult>? faceResults = MainActivity.facialRecognitionInstance.DetectFaceFromBitmap(imageBitmap);

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
            MainActivity.facialRecognitionInstance.ExtractFeatureFromBitmap(imageBitmap, faceResults);
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
            float score = MainActivity.facialRecognitionInstance.CompareFeature(face.Feature.ToArray(), Facial.imageRecognition);
            float faceScore = face.Liveness; // Indicates how the face is real

            // Close the camera
            await Navigation.PopModalAsync();

            if (score > 78)
            {
                Console.WriteLine($"[FacialRecognition] Score: {score} success");
                // Return the values
                Closed?.Invoke(this, true);
            }
            else
            {
                Console.WriteLine($"[FacialRecognition] Score: {score} failed");
                // Return the values
                Closed?.Invoke(this, false);
            }
        });
    }

    /// <summary>
    /// Change the camera position (frontal, back)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CameraPositionSwitch(object? sender, EventArgs e)
    {
        //Stop function if camera is busy
        if (cameraBusy)
        {
            Console.WriteLine("[FacialRecognition] Switch cancelled, the camera is busy");
            return;
        }
        else
        {
            cameraBusy = true;
            //Enable the timer for busy delay
            Task.Delay(1000).ContinueWith((_) =>
            {
                cameraBusy = false;
            });
        }

        //Troca as cameras das variaveis locais
        switch (cameraPosition)
        {
            case 0: cameraPosition = 1; StartCamera(); break;
            case 1: cameraPosition = 0; StartCamera(); break;
        }
    }

    // Close Camera
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraView.ClearLogicalChildren();
        _ = cameraView.StopCameraAsync();
    }
}