namespace MySchool.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public string? EntityName { get; set; }

        public NotFoundException(string message) : base(message)
        {
        }
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotFoundException(string message, string? entityName) : base(message)
        {
            EntityName = entityName;
        }
    }
}
