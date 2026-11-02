namespace Lamsura.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync ( IFormFile file, string folderName );
        void DeleteFile ( string fileName, string folderName );
    }
}
