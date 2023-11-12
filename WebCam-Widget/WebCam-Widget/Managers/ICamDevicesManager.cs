using CommunityToolkit.Mvvm.ComponentModel;
using FlashCap;
using WebCam_Widget.Models;

namespace WebCam_Widget.Managers;

public interface ICamDevicesManager : IDisposable
{
    /// <summary>
    /// Load all the available camera devices.
    /// </summary>
    /// <returns></returns>
    Task<List<CamDevice>> LoadCamDevicesAsync();

    public List<CaptureDeviceDescriptor>? CaptureDevices { get; set; }
}

public partial class CamDevicesManager : ObservableObject, ICamDevicesManager
{
    [ObservableProperty] private List<CaptureDeviceDescriptor>? _captureDevices;

    /// <summary>
    /// Load all the available camera devices.
    /// </summary>
    /// <returns></returns>
    public async Task<List<CamDevice>> LoadCamDevicesAsync()
    {
        var camDevices = new List<CamDevice>();

        await Task.Run(() =>
        {
            CaptureDevices = new CaptureDevices().EnumerateDescriptors().ToList();
            foreach (var descriptor in CaptureDevices)
            {
                var device = new CamDevice
                {
                    Id = Convert.ToString(descriptor.Identity)!,
                    Name = descriptor.Name,
                    Description = descriptor.Description,
                    Type = descriptor.DeviceType.ToString(),
                    CamDeviceDetails = new()
                };

                foreach (var ch in descriptor.Characteristics)
                {
                    if (ch.PixelFormat != PixelFormats.Unknown)
                    {
                        device.CamDeviceDetails.Add(new CamDeviceDetail
                        {
                            Description = ch.Description,
                            Height = ch.Height,
                            Width = ch.Width,
                            FramesPerSecond = ch.FramesPerSecond.ToString(),
                            RawPixFormat = ch.RawPixelFormat,
                            PixelFormat = ch.PixelFormat.ToString(),
                        });
                    }
                }

                camDevices.Add(device);
            }
        });

        return camDevices;
    }

    public void Dispose()
    {
        CaptureDevices?.Clear();
    }
}
