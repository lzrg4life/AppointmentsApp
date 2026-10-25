using AppointmentsApp.Services;

namespace AppointmentsApp;

public partial class App : Application
{
    private readonly AppointmentStorageService _storageService;

    public App(AppointmentStorageService storageService)
    {
        _storageService = storageService;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage(_storageService)));
    }
}