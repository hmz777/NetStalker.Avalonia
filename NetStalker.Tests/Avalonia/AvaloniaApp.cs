﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace NetStalker.Tests.Avalonia
{
	public static class AvaloniaApp
	{
		public static void Stop()
		{
			var app = GetApp();

			if (app is IDisposable disposable)
			{
				Dispatcher.UIThread.Post(disposable.Dispose);
			}

			Dispatcher.UIThread.Post(() => app?.Shutdown());
		}

		public static Window? GetMainWindow() => GetApp()?.MainWindow;

		public static IClassicDesktopStyleApplicationLifetime? GetApp() =>
			Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

		public static AppBuilder BuildAvaloniaApp() => NetStalkerAvalonia.Windows.Program.BuildAvaloniaApp();
	}
}