using System.Collections.Generic;
using TestProject.Models;
using TestProject.Enums;
using System.ComponentModel;

namespace TestProject.Services;
public class ItemService : IItemService
{
    private readonly ILogger<ItemService> _logger;
    private readonly IConfiguration _config;
    private readonly string _baseStorageDirectory = "";

    public ItemService(ILogger<ItemService> logger, IConfiguration config, IWebHostEnvironment env)
    {
        _logger = logger;
        _config = config;

        // For this TestProject we are using storage at the same level as the application but in a 
        // production environment that would be a bad practice since the storage could grow to the point that 
        // it could cause the host of our application to crash, instead use an external block or object storage
        _baseStorageDirectory = Path.Combine(env.ContentRootPath, _config["FileStorage:Directory"] ?? "Storage");
    }

    public void DeleteItem(string path)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> DownloadFileAsync(string path)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Item> GetItems(string path)
    {
        List<Item> itemsResult = new List<Item>();
        try
        {
            path ??= ""; // if path is null handle gracefully to root
            _logger.LogInformation($"Executing Service: [ItemService] Method: [GetItemsAsync]");

            string fullPath = Path.Combine(_baseStorageDirectory, path);
            if (!Path.GetFullPath(fullPath).StartsWith(_baseStorageDirectory))
            {
                throw new ArgumentException("Invalid path");
            }

            if (Directory.Exists(fullPath))
            {
                IEnumerable<string> filePaths = Directory.EnumerateFiles(fullPath);
                IEnumerable<string> folderPaths = Directory.EnumerateDirectories(fullPath);

                foreach (string filePath in filePaths)
                {
                    Item newFile = new Item();
                    newFile.Name = Path.GetFileName(filePath);
                    // I'm avoiding exposure of the system's absolute path for security
                    newFile.Path = Path.GetRelativePath(_baseStorageDirectory, filePath);
                    newFile.Type = ItemType.File;

                    itemsResult.Add(newFile);
                }

                foreach (string folderPath in folderPaths)
                {
                    Item newFolder = new Item();
                    newFolder.Name = Path.GetFileName(folderPath);
                    newFolder.Path = Path.GetRelativePath(_baseStorageDirectory, folderPath);
                    newFolder.Type = ItemType.Folder;

                    itemsResult.Add(newFolder);
                }
            }
            else
            {
                throw new DirectoryNotFoundException(fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Service: [ItemService] Method: [GetItemsAsync] Exception: [{ex}] InnerException: [{ex.InnerException}] Message: [{ex.Message}] StackTrace: [{ex.StackTrace}] ");
            throw;
        }

        _logger.LogInformation($"Execution of Service: [ItemService] Method: [GetItemsAsync] completed succesfully. Returning {itemsResult.Count} items");
        return itemsResult;
    }

    public Item RenameItem(string path, string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Item> SearchItems(string path, string query)
    {
        throw new NotImplementedException();
    }

    public Task<Item> UploadFileAsync(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }
}