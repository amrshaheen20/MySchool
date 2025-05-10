using MySchool.API.Interfaces;

namespace MySchool.API.Services.Common
{
    public class FileStorageService(IConfiguration configuration) : IServiceInjector
    {
        const string defaultFileDirectory = "Files";

        public string GetFilePath(string fileName)
        {
            return Path.Combine("wwwroot", defaultFileDirectory, fileName);
        }

        public string GetFileUrl(string fileName)
        {
            var baseUrl = configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
            return $"{baseUrl}/{defaultFileDirectory}/{fileName}";
        }

        public string SaveFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Invalid file");

                var safeFileName = Path.GetFileName(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}";

                var filePath = GetFilePath(uniqueFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("File save failed", ex);
            }
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                var filePath = GetFilePath(fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("File delete failed", ex);
            }
        }
    }
}