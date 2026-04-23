using TestProject.Models;

namespace TestProject.Services;

public interface IItemService
{
    void DeleteItem(string path);
    Task<Stream> DownloadFileAsync(string path);
    DirectoryListing GetItems(string path);
    IEnumerable<Item> SearchItems(string path, string query);
    Task<Item> UploadFileAsync(string path, IFormFile file);
    
}