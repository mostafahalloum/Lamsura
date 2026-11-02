namespace Lamsura.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public async Task<string> SaveFileAsync ( IFormFile file, string folderName )
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var folderPath = Path.Combine(_basePath, folderName);

            if ( !Directory.Exists(folderPath) )
                Directory.CreateDirectory(folderPath);

            using ( var stream = File.Create(Path.Combine(folderPath, fileName)) ) {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        public void DeleteFile ( string fileName, string folderName )
        {
            var path = Path.Combine(_basePath, folderName, fileName);
            if ( File.Exists(path) )
                File.Delete(path);
        }
    }
}
