using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashCap;
using WebCam_Widget.Managers;
using WebCam_Widget.Models;

namespace WebCam_Widget.ViewModels;

public interface ICamControlViewModel : IDisposable
{
    public List<CamDevice> CamDevices { get; set; }
    public CamDevice? SelectedCamDevice { get; set; }
    public CamDeviceDetail? SelectedCamDetail { get; set; }
    Task LoadDevicesAsync();
}

public partial class CamControlViewModel : ObservableObject, ICamControlViewModel
{
    private readonly ICamDevicesManager _camDevicesManager;
    private readonly ICamOperationsManager _camOperationsManager;
    [ObservableProperty] private byte[] _imageData;
    [ObservableProperty] private List<CamDevice> _camDevices;
    [ObservableProperty] private CamDevice? _selectedCamDevice;
    [ObservableProperty] private CamDeviceDetail? _selectedCamDetail;

    public CamControlViewModel(ICamDevicesManager camDevicesManager, ICamOperationsManager camOperationsManager)
    {
        _camDevicesManager = camDevicesManager;
        _camOperationsManager = camOperationsManager;
    }

    public async Task LoadDevicesAsync()
    {
        CamDevices = new List<CamDevice>();
        SelectedCamDevice = null;

        CamDevices = await _camDevicesManager.LoadCamDevicesAsync();
        if (CamDevices.Any())
        {
            SelectedCamDevice = CamDevices[0];
            SelectedCamDetail = SelectedCamDevice.CamDeviceDetails[0];
        }
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            if (SelectedCamDevice != null && SelectedCamDetail != null)
            {
                await _camOperationsManager.StartCapturing(SelectedCamDevice, SelectedCamDetail, ImageBytesReceived);
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void ImageBytesReceived(byte[] imageBytes)
    {
        ImageData = imageBytes;
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        try
        {
            await _camOperationsManager.StopCapturing();
        }
        catch (Exception ex)
        {
        }
    }


    public void Dispose()
    {
    }
}