using Android.App;
using Android.Runtime;

namespace Android_Native_Demonstration;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => Init.CriarAplicacao();
}
