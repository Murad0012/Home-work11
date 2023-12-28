using System;
namespace P335_BackEnd.Services
{
    public class FileService
    {
        public string AddFile(IFormFile file, string targetDirectory)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("wwwroot", targetDirectory));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileInfo = new FileInfo(file.FileName);
            string fileName = fileInfo.Name + Guid.NewGuid().ToString() + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

        public void DeleteFile(string fileName, string targetDirectory)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("wwwroot", targetDirectory, fileName));

            if (!Path.Exists(path)) return;

            File.Delete(path);
        }
    }
}

