using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WebCam_Widget.Managers;
using WebCam_Widget.ViewModels;
using SeeShark.FFmpeg;

namespace WebCam_Widget;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Props

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    private IMainWindowViewModel _mainWindowViewModel;

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public App()
    {
        Services = ConfigureServices();
    }

    /// <summary>
    /// When application starts set initial view
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainWindowViewModel = Services.GetRequiredService<IMainWindowViewModel>();
        MainWindow shellView = new()
        {
            DataContext = _mainWindowViewModel,
        };

        shellView.Show();
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddTransient<IMainWindowViewModel, MainWindowViewModel>();
        services.AddTransient<ICamControlViewModel, CamControlViewModel>();
        services.AddSingleton<ICamDevicesManager, CamDevicesManager>();
        services.AddSingleton<ICamOperationsManager, CamOperationsManager>();


        return services.BuildServiceProvider();
    }
}