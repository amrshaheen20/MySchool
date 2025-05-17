using System.Text.Json.Serialization;

namespace MySchool.API.Models.Dtos
{
    public class BaseResponseDto
    {
        [JsonPropertyOrder(-1)]
        public int Id { get; set; }
    }
}
