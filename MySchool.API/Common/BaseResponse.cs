using MySchool.API.Interfaces;
using System.Net;
using System.Text.Json.Serialization;

namespace MySchool.API.Common
{
    public class BaseResponse : BaseResponse<object>
    {
        public BaseResponse() : base() { }
        public BaseResponse(HttpStatusCode status, string? message = null, object? data = null, IEnumerable<object>? errors = null)
            : base(status, message, data, errors) { }
    }

    public class BaseResponse<T> : IBaseResponse<T>
    {
        public bool IsSuccess => (int)Status < 400;
        [JsonConverter(typeof(JsonNumberEnumConverter<HttpStatusCode>))]
        public HttpStatusCode Status { get; private set; } = HttpStatusCode.OK;
        private string? _message;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message => _message ?? Status.ToString();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<object>? Errors { get; private set; }

        public BaseResponse() { }
        public BaseResponse(HttpStatusCode status = HttpStatusCode.OK, string? message = default, T? data = default, IEnumerable<object>? errors = default)
        {
            Status = status;
            _message = message;
            Data = data;
            Errors = errors;
        }

        public IBaseResponse<T> SetStatus(HttpStatusCode statusCode)
        {
            this.Status = statusCode;
            return this;
        }

        public IBaseResponse<T> SetMessage(string message)
        {
            this._message = message;
            return this;
        }
        public IBaseResponse<T> SetData(T data)
        {
            this.Data = data;
            return this;
        }

        public IBaseResponse<T> SetErrors(IEnumerable<object> errors)
        {
            this.Errors = errors;
            return this;
        }
    }
}
