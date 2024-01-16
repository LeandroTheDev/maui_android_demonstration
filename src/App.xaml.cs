namespace Android_Native_Demonstration;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new Pages.Visualization();
	}
}
