using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WebCam_Widget.ViewModels;

public interface IMainWindowViewModel
{
}

public partial class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    public ICamControlViewModel CamControlViewModel { get; }

    public MainWindowViewModel(ICamControlViewModel camControlViewModel)
    {
        CamControlViewModel = camControlViewModel;
    }

    [RelayCommand]
    private async Task LoadedAsync()
    {
        await CamControlViewModel.LoadDevicesAsync();
    }

}