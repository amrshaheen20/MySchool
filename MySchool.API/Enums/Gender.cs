using System.Text.Json.Serialization;

namespace MySchool.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eGender
    {
        None,
        Male,
        Female,
    }
}
