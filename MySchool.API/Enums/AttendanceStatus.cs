using System.Text.Json.Serialization;

namespace MySchool.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eAttendanceStatus
    {
        Absent = 0,
        Present,
        Late,
        Excused
    }
}
