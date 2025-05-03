using CommunityToolkit.Mvvm.ComponentModel;

namespace Ngaq.Ui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
