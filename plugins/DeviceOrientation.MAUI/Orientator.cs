#pragma warning disable CA2211
using System.Numerics;

namespace DeviceOrientation.MAUI
{
    public enum Orientation
    {
        Portrait,
        Landscape,
        ReversePortrait,
        ReverseLandscape
    }

    public static class Orientator
    {
        /// <summary>
        /// Accelerator is the actual position of the device
        /// </summary>
        public static Vector3 Accelerator = new(0, 0, 0);
        /// <summary>
        /// If accelerator is updating this variable will be true, false if not updating
        /// </summary>
        public static bool AcceleratorUpdating = false;
        /// <summary>
        /// Actual orientation from device, only updates if AcceleratorUpdating is true
        /// </summary>
        public static Orientation DeviceOrientation = Orientation.Portrait;
        /// <summary>
        /// Set this to true if you want the orientation change based in accelerator
        /// </summary>
        public static bool AcceleratorUpdateChangeOrientation = false;
        /// <summary>
        /// Called everytime the DeviceOrientation is changed,
        /// consider making this variable null after using it
        /// </summary>
        public static EventHandler? OnDeviceOrientationChanged;
        /// <summary>
        /// If this is enabled only landscape and reverse landscape will be changed automatically
        /// also need the AcceleratorUpdateChangeOrientation enabled
        /// </summary>
        public static bool AcceleratorUpdateChangeOrientationOnlyLand = false;

        /// <summary>
        /// Receives the orientator to get functionalities for specific OS
        /// </summary>
        /// <returns></returns>
        public static IOrientator Get()
        {
            return DependencyService.Get<IOrientator>();
        }

        /// <summary>
        /// Accelerator will start receiving the actual position of the device and DeviceOrientation will be updated
        /// </summary>
        public static void StartAccelerator()
        {
            if (AcceleratorUpdating) { return; }
            Console.WriteLine("[DeviceOrientation] Accelerator started receiving updates");
            static void OnDeviceMoviment(object? sender, AccelerometerChangedEventArgs e)
            {
                Accelerator = e.Reading.Acceleration;
                //Landscape
                if (Accelerator[0] > 0.8 && DeviceOrientation != Orientation.Landscape)
                {
                    Console.WriteLine("[DeviceOrientation] Device Orientation changed to Landscape");
                    DeviceOrientation = Orientation.Landscape;
                    OnDeviceOrientationChanged?.Invoke("DeviceOrientation", EventArgs.Empty);
                    if (AcceleratorUpdateChangeOrientation && AcceleratorUpdateChangeOrientationOnlyLand) Get().SetOrientation(Orientation.Landscape);
                    return;
                }
                //ReverseLandscape
                if (Accelerator[0] < -0.8 && DeviceOrientation != Orientation.ReverseLandscape)
                {
                    Console.WriteLine("[DeviceOrientation] Device Orientation changed to Reversed Landscape");
                    DeviceOrientation = Orientation.ReverseLandscape;
                    OnDeviceOrientationChanged?.Invoke("DeviceOrientation", EventArgs.Empty);
                    if (AcceleratorUpdateChangeOrientation && AcceleratorUpdateChangeOrientationOnlyLand) Get().SetOrientation(Orientation.ReverseLandscape);
                    return;
                }
                //Portrait
                if (Accelerator[0] <= 0.8 && Accelerator[0] >= -0.8 && DeviceOrientation != Orientation.Portrait)
                {
                    Console.WriteLine("[DeviceOrientation] Device Orientation changed to Portrait");
                    DeviceOrientation = Orientation.Portrait;
                    OnDeviceOrientationChanged?.Invoke("DeviceOrientation", EventArgs.Empty);
                    if (AcceleratorUpdateChangeOrientation && !AcceleratorUpdateChangeOrientationOnlyLand) Get().SetOrientation(Orientation.Portrait);
                    return;
                }
            }
            Accelerometer.ReadingChanged += OnDeviceMoviment;
            Accelerometer.Start(SensorSpeed.UI);
            AcceleratorUpdating = true;
        }

        /// <summary>
        /// Stop receiving the actual position of the device for Accelerator and DeviceOrientation
        /// </summary>
        public static void DisposeAccelerator()
        {
            if (!AcceleratorUpdating) { return; }
            Console.WriteLine("[DeviceOrientation] Accelerator disposed, no more updates");
            Accelerometer.Stop();
            AcceleratorUpdating = false;
        }
    }
}
