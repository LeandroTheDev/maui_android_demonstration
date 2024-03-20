namespace ImageCropper.MAUI.Platforms.IOS
{
    public class Platform
    {
        public void Init()
        {
            DependencyService.Register<IImageCropperWrapper, PlatformImageCropper>();
        }
    }
}
