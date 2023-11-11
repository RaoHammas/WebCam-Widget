using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCap;
using WebCam_Widget.Models;

namespace WebCam_Widget.ViewModels;

public interface ICamControlViewModel : IAsyncDisposable
{
    public List<CamDevice> CamDevices { get; set; }
    public CamDevice? SelectedCamDevice { get; set; }
    public CamDeviceDetail? SelectedCamDetail { get; set; }
    Task LoadDevicesAsync();
}

public partial class CamControlViewModel : ObservableObject, ICamControlViewModel
{
    [ObservableProperty] private byte[] _imageData;
    [ObservableProperty] private List<CamDevice> _camDevices;
    [ObservableProperty] private CamDevice? _selectedCamDevice;
    [ObservableProperty] private CamDeviceDetail? _selectedCamDetail;

    private List<CaptureDeviceDescriptor> _captureDevices;
    private CaptureDevice? _captureDevice;

    public CamControlViewModel()
    {
    }

    public async Task LoadDevicesAsync()
    {
        CamDevices = new List<CamDevice>();
        SelectedCamDevice = null;

        await Task.Run(() =>
        {
            var devicesFound = new CaptureDevices();
            _captureDevices = devicesFound.EnumerateDescriptors().ToList();
            foreach (var descriptor in _captureDevices)
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

                CamDevices.Add(device);
            }

            if (CamDevices.Any())
            {
                SelectedCamDevice = CamDevices[0];
            }
        });
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            await StartCapturing(SelectedCamDevice, SelectedCamDetail);
        }
        catch (Exception ex)
        {
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        try
        {
            await DisposeAsync();
        }
        catch (Exception ex)
        {
        }
    }

    public async Task StartCapturing(CamDevice? device, CamDeviceDetail? detail)
    {
        if (device != null && detail != null)
        {
            if (_captureDevice is { IsRunning: true })
            {
                await _captureDevice.StopAsync();
            }

            var (capDev, capDevDetail) = GetInnerDevice(device, detail);
            if (capDev != null && capDevDetail != null)
            {
                _captureDevice = await capDev.OpenAsync(capDevDetail, true, true, 10, PixelBufferArrived);
                await _captureDevice.StartAsync();
            }
        }
    }

    private void PixelBufferArrived(PixelBufferScope bufferScope)
    {
        ImageData = bufferScope.Buffer.ExtractImage();
    }

    private (CaptureDeviceDescriptor? device, VideoCharacteristics? details) GetInnerDevice(CamDevice? device,
        CamDeviceDetail? detail)
    {
        if (device != null && detail != null)
        {
            var capDev = _captureDevices.First(x => Convert.ToString(x.Identity) == device.Id && x.Name == device.Name);
            var capDevDetail = capDev.Characteristics.First(x =>
                x.Description == detail.Description
                && x.FramesPerSecond.ToString() == detail.FramesPerSecond
                && x.RawPixelFormat == detail.RawPixFormat
                && x.Width == detail.Width
                && x.Height == detail.Height
                && x.PixelFormat.ToString() == detail.PixelFormat
            );
            return (capDev, capDevDetail);
        }

        return (null, null);
    }


    public async ValueTask DisposeAsync()
    {
        if (_captureDevice != null)
        {
            if (_captureDevice.IsRunning)
            {
                await _captureDevice.StopAsync();
            }
        }
    }
}