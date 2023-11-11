using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using FlashCap;

namespace WebCam_Widget;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private CaptureDevice _device;
 
    private readonly Dictionary<string, List<string>> _allDevices = new();
    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var devices = new CaptureDevices();
        foreach (var descriptor in devices.EnumerateDescriptors().ToList())
        {
           
            var props = new List<string>();
            foreach (var c in descriptor.Characteristics)
            {
                props.Add($"[Info][{c.Description}]-[PF][{c.PixelFormat}]-[FPS][{c.FramesPerSecond}]-[H/W][{c.Height}/{c.Width}]");
            }
            _allDevices.Add(descriptor.Name, props);
        }

        
    }

    private async Task StartRecording(CaptureDeviceDescriptor device)
    {
       
        /*
        _device = await device.OpenAsync(
            device.Characteristics[0], bufferScope =>
            {
                var imageData = bufferScope.Buffer.ExtractImage();

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(imageData);
                bitmap.EndInit();

                this.Dispatcher.BeginInvoke(
                    new Action((() => CamImage.Source = bitmap)));
                return Task.CompletedTask;
            });
            */
            

        await _device.StartAsync();
    }

    private async void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        await _device.StopAsync();
        await _device.DisposeAsync();
    }

    private void ComboDeviceProps_OnDropDownClosed(object? sender, EventArgs e)
    {
        //var descriptor0 = devices.EnumerateDescriptors().ElementAt(0);

    }
}