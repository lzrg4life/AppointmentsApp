using System.Text.Json;
using AppointmentsApp.Models;

namespace AppointmentsApp.Services;

public sealed class AppointmentStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "appointments.json");

    public async Task<IReadOnlyList<AppointmentModel>> GetAppointmentsAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            return await ReadAppointmentsAsync();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task SaveAppointmentAsync(AppointmentModel appointment)
    {
        await _fileLock.WaitAsync();
        try
        {
            var appointments = await ReadAppointmentsAsync();
            var existingIndex = appointments.FindIndex(item => item.Id == appointment.Id);
            if (existingIndex >= 0)
            {
                appointments[existingIndex] = appointment;
            }
            else
            {
                appointments.Add(appointment);
            }

            var temporaryPath = $"{_filePath}.tmp";
            await using (var stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, appointments, JsonOptions);
            }

            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task<List<AppointmentModel>> ReadAppointmentsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<AppointmentModel>>(stream, JsonOptions) ?? [];
    }
}
