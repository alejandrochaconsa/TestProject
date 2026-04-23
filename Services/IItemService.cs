using TestProject.Models;

namespace TestProject.Services;

public interface IItemService
{
    void DeleteItem(string path);
    Task<Stream> DownloadFileAsync(string path);
    IEnumerable<Item> GetItems(string path);
    Item RenameItem(string path, string name);
    IEnumerable<Item> SearchItems(string path, string query);
    Task<Item> UploadFileAsync(string path, IFormFile file);
    
}