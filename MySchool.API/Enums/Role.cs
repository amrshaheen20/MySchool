using System.Text.Json.Serialization;

namespace MySchool.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eRole
    {
        None = 0,
        Admin,
        Teacher,
        Student,
        Guardian,
    }

}
