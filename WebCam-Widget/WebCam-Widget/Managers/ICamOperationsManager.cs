using CommunityToolkit.Mvvm.ComponentModel;
using FlashCap;
using WebCam_Widget.Models;

namespace WebCam_Widget.Managers;

public interface ICamOperationsManager : IDisposable
{
    Task StartCapturing(CamDevice device, CamDeviceDetail detail, Action<byte[]> imageBytesReceived);
    Task StopCapturing();
}

public class CamOperationsManager : ObservableObject, ICamOperationsManager
{
    private readonly ICamDevicesManager _camDevicesManager;
    private CaptureDevice? _captureDevice;

    public CamOperationsManager(ICamDevicesManager camDevicesManager)
    {
        _camDevicesManager = camDevicesManager;
    }

    public async Task StartCapturing(CamDevice device, CamDeviceDetail detail, Action<byte[]> imageBytesReceived)
    {
        if (_captureDevice is { IsRunning: true })
        {
            await _captureDevice.StopAsync();
        }

        var (capDev, capDevDetail) = GetCaptureDevice(device, detail);

        _captureDevice = await capDev.OpenAsync(capDevDetail, false, true, 100,
            bufferScope => { imageBytesReceived(bufferScope.Buffer.ExtractImage()); });

        await _captureDevice.StartAsync();
    }

    public async Task StopCapturing()
    {
        if (_captureDevice is { IsRunning: true })
        {
            await _captureDevice.StopAsync();
            _captureDevice = null;
        }
    }

    private (CaptureDeviceDescriptor device, VideoCharacteristics details) GetCaptureDevice(CamDevice device,
        CamDeviceDetail detail)
    {
        var capDev = _camDevicesManager.CaptureDevices?.FirstOrDefault(x =>
            Convert.ToString(x.Identity) == device.Id && x.Name == device.Name);
        if (capDev != null)
        {
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

        return default;
    }

    public void Dispose()
    {
        _captureDevice?.Dispose();
        _captureDevice = null;
    }
}