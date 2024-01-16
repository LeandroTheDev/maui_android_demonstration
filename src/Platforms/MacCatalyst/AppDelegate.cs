using Foundation;

namespace Android_Native_Demonstration;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => Android_Native_Demonstration.Init.CriarAplicacao();
}
