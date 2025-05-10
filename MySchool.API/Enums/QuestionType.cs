using System.Text.Json.Serialization;

namespace MySchool.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eQuestionType
    {
        MultipleChoice = 0,
        Essay
    }
}
