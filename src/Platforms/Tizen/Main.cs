using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Android_Native_Demonstration;

class Program : MauiApplication
{
	protected override MauiApp CreateMauiApp() => Init.CriarAplicacao();

    static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
