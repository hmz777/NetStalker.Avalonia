using Avalonia;
using Avalonia.ReactiveUI;
using NetStalkerAvalonia.Services;
using NetStalkerAvalonia.Services.Implementations.BandwidthControl;
using NetStalkerAvalonia.Services.Implementations.BlockingRedirection;
using NetStalkerAvalonia.Services.Implementations.DeviceNameResolving;
using NetStalkerAvalonia.Services.Implementations.DeviceTypeIdentification;
using NetStalkerAvalonia.Services.Implementations.Notifications;
using NetStalkerAvalonia.Services.Implementations.Packets;
using Serilog;
using Splat;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Reflection;
using NetStalkerAvalonia.Configuration;
using NetStalkerAvalonia.Services.Implementations.DeviceScanning;
using ReactiveUI;
using ILogger = Serilog.ILogger;

namespace NetStalkerAvalonia
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        private static AppBuilder BuildAvaloniaApp()
        {
            // Router uses Splat.Locator to resolve views for
            // view models, so we need to register our views.
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

            // Configure logging
            ConfigureAndRegisterLogging();

            // Register app services
            RegisterRequiredServices();

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }

        private static void ConfigureAndRegisterLogging()
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(Assembly.GetExecutingAssembly().Location, "log.txt"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Locator.CurrentMutable.RegisterLazySingleton(() => log, typeof(ILogger));
        }

        private static void RegisterRequiredServices()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() =>
                    new DeviceScanner(),
                typeof(IDeviceScanner));

            Locator.CurrentMutable.RegisterLazySingleton(() =>
                    new BandwidthController(),
                typeof(IBandwidthController));

            Locator.CurrentMutable.RegisterLazySingleton(() =>
                    new BlockerRedirector(),
                typeof(IBlockerRedirector));

            Locator.CurrentMutable.RegisterLazySingleton(() =>
                    new DeviceNameResolver(),
                typeof(IDeviceNameResolver));

            // Read from app configuration
            var notificationOptions = ConfigurationManager.GetSection("Notifications") as NotificationOptions
                                      ?? new NotificationOptions();

            Locator.CurrentMutable.RegisterLazySingleton(() =>
                    new NotificationManager(notificationOptions),
                typeof(INotificationManager));

            Locator.CurrentMutable.RegisterLazySingleton(() =>
                    new PacketManager(),
                typeof(IPacketManager));

            var macLookupApiToken = ConfigurationManager
                .AppSettings[nameof(ConfigKeys.ApiKey)];
            var macLookupServiceUri = ConfigurationManager
                .AppSettings[nameof(ConfigKeys.MacLookupServiceUri)];

            if (string.IsNullOrWhiteSpace(macLookupApiToken) == false &&
                string.IsNullOrWhiteSpace(macLookupServiceUri) == false)
            {
                Locator.CurrentMutable.RegisterLazySingleton(() =>
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization",
                        "Bearer " + ConfigurationManager.AppSettings[nameof(ConfigKeys.ApiKey)]);

                    return client;
                }, contract: nameof(ContractKeys.MacLookupClient));

                Locator.CurrentMutable.RegisterLazySingleton(() =>
                        new DeviceTypeIdentifier(),
                    typeof(IDeviceTypeIdentifier));
            }
        }
    }
}