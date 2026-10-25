using AppointmentsApp.Models;
using AppointmentsApp.Services;
using System.Text.Json;

namespace AppointmentsApp;

public partial class MainPage : ContentPage
{
    private readonly AppointmentStorageService _storageService;

    public MainPage(AppointmentStorageService storageService)
    {
        _storageService = storageService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAppointmentsAsync();
    }

    private async Task LoadAppointmentsAsync()
    {
        try
        {
            var appointments = await _storageService.GetAppointmentsAsync();
            var activeAppointments = appointments
                .Where(appointment => appointment.WhenArchived is null)
                .OrderBy(appointment => appointment.AppointmentDate)
                .ThenBy(appointment => appointment.AppointmentTime)
                .ToList();

            AppointmentsList.ItemsSource = activeAppointments;
            EmptyStateLabel.IsVisible = activeAppointments.Count == 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            await DisplayAlertAsync("Unable to load appointments", ex.Message, "OK");
        }
    }

    private async void OnAddAppointmentClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AppointmentEditPage(_storageService));
    }

    private async void OnArchivedAppointmentsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArchivedAppointmentsPage(_storageService));
    }

    private async void OnAppointmentSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not AppointmentModel appointment)
        {
            return;
        }

        AppointmentsList.SelectedItem = null;
        await Navigation.PushAsync(new AppointmentEditPage(_storageService, appointment));
    }
}