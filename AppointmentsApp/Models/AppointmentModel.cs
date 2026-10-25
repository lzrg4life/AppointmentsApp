using System.Text.Json.Serialization;

namespace AppointmentsApp.Models;

public class AppointmentModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public DateOnly AppointmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public TimeOnly AppointmentTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
    public string Details { get; set; } = "";
    public DateTime? WhenArchived { get; set; }

    [JsonIgnore]
    public bool HasDetails => !string.IsNullOrWhiteSpace(Details);
}