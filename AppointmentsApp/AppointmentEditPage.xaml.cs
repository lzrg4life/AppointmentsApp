using System.Text.Json;
using AppointmentsApp.Models;
using AppointmentsApp.Services;

namespace AppointmentsApp;

public partial class AppointmentEditPage : ContentPage
{
    private readonly AppointmentStorageService _storageService;
    private readonly AppointmentModel? _appointment;

    public AppointmentEditPage(AppointmentStorageService storageService, AppointmentModel? appointment = null)
    {
        _storageService = storageService;
        _appointment = appointment;
        InitializeComponent();

        if (appointment is null)
        {
            DatePicker.Date = DateTime.Today;
            TimePicker.Time = DateTime.Now.TimeOfDay;
            return;
        }

        Title = "Edit appointment";
        PageHeading.Text = "Edit appointment";
        TitleEntry.Text = appointment.Title;
        DatePicker.Date = appointment.AppointmentDate.ToDateTime(TimeOnly.MinValue);
        TimePicker.Time = appointment.AppointmentTime.ToTimeSpan();
        DetailsEditor.Text = appointment.Details;
        ArchiveButton.IsVisible = appointment.WhenArchived is null;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var title = TitleEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlertAsync("Title required", "Enter a title for this appointment.", "OK");
            return;
        }

        if (DatePicker.Date is not { } selectedDate || TimePicker.Time is not { } selectedTime)
        {
            await DisplayAlertAsync("Date and time required", "Select a date and time for this appointment.", "OK");
            return;
        }

        var appointment = new AppointmentModel
        {
            Id = _appointment?.Id ?? Guid.NewGuid(),
            Title = title,
            AppointmentDate = DateOnly.FromDateTime(selectedDate),
            AppointmentTime = TimeOnly.FromTimeSpan(selectedTime),
            Details = DetailsEditor.Text?.Trim() ?? string.Empty,
            WhenArchived = _appointment?.WhenArchived
        };

        try
        {
            await _storageService.SaveAppointmentAsync(appointment);
            await Navigation.PopAsync();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            await DisplayAlertAsync("Unable to save appointment", ex.Message, "OK");
        }
    }

    private async void OnArchiveClicked(object? sender, EventArgs e)
    {
        if (_appointment is null)
        {
            return;
        }

        var title = TitleEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlertAsync("Title required", "Enter a title for this appointment.", "OK");
            return;
        }

        var shouldArchive = await DisplayAlertAsync(
            "Archive appointment",
            "This appointment will move to the archived appointments list.",
            "Archive",
            "Cancel");
        if (!shouldArchive)
        {
            return;
        }

        if (DatePicker.Date is not { } selectedDate || TimePicker.Time is not { } selectedTime)
        {
            await DisplayAlertAsync("Date and time required", "Select a date and time for this appointment.", "OK");
            return;
        }

        var appointment = new AppointmentModel
        {
            Id = _appointment.Id,
            Title = title,
            AppointmentDate = DateOnly.FromDateTime(selectedDate),
            AppointmentTime = TimeOnly.FromTimeSpan(selectedTime),
            Details = DetailsEditor.Text?.Trim() ?? string.Empty,
            WhenArchived = DateTime.UtcNow
        };

        try
        {
            await _storageService.SaveAppointmentAsync(appointment);
            await Navigation.PopAsync();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            await DisplayAlertAsync("Unable to archive appointment", ex.Message, "OK");
        }
    }
}
