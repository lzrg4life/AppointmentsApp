using System.Text.Json;
using AppointmentsApp.Services;

namespace AppointmentsApp;

public partial class ArchivedAppointmentsPage : ContentPage
{
    private readonly AppointmentStorageService _storageService;

    public ArchivedAppointmentsPage(AppointmentStorageService storageService)
    {
        _storageService = storageService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var archivedAppointments = (await _storageService.GetAppointmentsAsync())
                .Where(appointment => appointment.WhenArchived is not null)
                .OrderBy(appointment => appointment.AppointmentDate)
                .ThenBy(appointment => appointment.AppointmentTime)
                .ToList();

            AppointmentsList.ItemsSource = archivedAppointments;
            EmptyStateLabel.IsVisible = archivedAppointments.Count == 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            await DisplayAlertAsync("Unable to load archived appointments", ex.Message, "OK");
        }
    }
}
