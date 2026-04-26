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
        _logger.LogInformation($"Executing Service: [ItemService] Method: [DeleteItem]");

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("path cannot be empty");
        }

        string fullPath = Path.Combine(_baseStorageDirectory, path);
        if (!Path.GetFullPath(fullPath).StartsWith(_baseStorageDirectory))
        {
            throw new ArgumentException("Invalid file to delete");
        }

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
            else
            {
                throw new FileNotFoundException();
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Service: [ItemService] Method: [DeleteItem] Exception: [{ex}] InnerException: [{ex.InnerException}] Message: [{ex.Message}] StackTrace: [{ex.StackTrace}] ");
            throw;
        }
    }

    public Stream DownloadFile(string path)
    {
        _logger.LogInformation($"Executing Service: [ItemService] Method: [DownloadFile]");

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("path for the file to be downloaded is invalid");
        }

        Stream fileStreamResult;

        try
        {
            string fullPath = Path.Combine(_baseStorageDirectory, path);
            if (!Path.GetFullPath(fullPath).StartsWith(_baseStorageDirectory))
            {
                throw new ArgumentException("Invalid path");
            }

            if (File.Exists(fullPath))
            {
                fileStreamResult = File.OpenRead(fullPath);
            }
            else
            {
                throw new FileNotFoundException(path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Service: [ItemService] Method: [DownloadFile] Exception: [{ex}] InnerException: [{ex.InnerException}] Message: [{ex.Message}] StackTrace: [{ex.StackTrace}] ");
            throw;
        }
        _logger.LogInformation($"Execution of Service: [ItemService] Method: [DownloadFile] completed succesfully.");

        return fileStreamResult;
    }

    public DirectoryListing GetItems(string? path)
    {
        DirectoryListing directoryListing = new DirectoryListing();
        List<Item> itemsResult = new List<Item>();
        try
        {
            path ??= ""; // if path is null handle gracefully to root
            _logger.LogInformation($"Executing Service: [ItemService] Method: [GetItems]");

            string fullPath = Path.Combine(_baseStorageDirectory, path);
            if (!Path.GetFullPath(fullPath).StartsWith(_baseStorageDirectory))
            {
                throw new ArgumentException("Invalid path");
            }

            if (Directory.Exists(fullPath))
            {
                IEnumerable<string> filePaths = Directory.EnumerateFiles(fullPath);
                IEnumerable<string> folderPaths = Directory.EnumerateDirectories(fullPath);

                directoryListing.FileCount = filePaths.Count();
                directoryListing.FolderCount = folderPaths.Count();

                long totalSize = 0;
                foreach (string filePath in filePaths)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    totalSize += fileInfo.Length;

                    Item newFile = new Item();
                    newFile.Name = Path.GetFileName(filePath);
                    // I'm avoiding exposure of the system's absolute path for security
                    newFile.Path = Path.GetRelativePath(_baseStorageDirectory, filePath);
                    newFile.Type = ItemType.File;

                    itemsResult.Add(newFile);
                }

                directoryListing.TotalSize = totalSize;

                foreach (string folderPath in folderPaths)
                {
                    Item newFolder = new Item();
                    newFolder.Name = Path.GetFileName(folderPath);
                    newFolder.Path = Path.GetRelativePath(_baseStorageDirectory, folderPath);
                    newFolder.Type = ItemType.Folder;

                    itemsResult.Add(newFolder);
                }
                directoryListing.Items = itemsResult;

            }
            else
            {
                throw new DirectoryNotFoundException(fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Service: [ItemService] Method: [GetItems] Exception: [{ex}] InnerException: [{ex.InnerException}] Message: [{ex.Message}] StackTrace: [{ex.StackTrace}] ");
            throw;
        }

        _logger.LogInformation($"Execution of Service: [ItemService] Method: [GetItems] completed succesfully. Returning {itemsResult.Count} items");
        return directoryListing;
    }

    public DirectoryListing SearchItems(string? path, string query)
    {
        _logger.LogInformation($"Executing Service: [ItemService] Method: [SearchItems]");

        path ??= "";
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("query cannot be empty");
        }

        string fullPath = Path.Combine(_baseStorageDirectory, path);
        if (!Path.GetFullPath(fullPath).StartsWith(_baseStorageDirectory))
        {
            throw new ArgumentException("Invalid path");
        }

        DirectoryListing directoryListing = new DirectoryListing();
        try
        {
            IEnumerable<string> itemPaths = Directory.EnumerateFileSystemEntries(fullPath, $"*{query}*", SearchOption.AllDirectories);
            List<Item> searchItemsResult = new List<Item>();
            int fileCount = 0;
            int folderCount = 0;
            long totalSize = 0;

            foreach (string itemPath in itemPaths)
            {
                Item item = new Item();
                if (File.Exists(itemPath))
                {
                    FileInfo fileInfo = new FileInfo(itemPath);
                    totalSize += fileInfo.Length;

                    item.Name = Path.GetFileName(itemPath);
                    item.Path = Path.GetRelativePath(_baseStorageDirectory, itemPath);
                    item.Type = ItemType.File;
                    fileCount++;
                }
                else
                {
                    item.Name = Path.GetFileName(itemPath);
                    item.Path = Path.GetRelativePath(_baseStorageDirectory, itemPath);
                    item.Type = ItemType.Folder;
                    folderCount++;
                }
                searchItemsResult.Add(item);
            }
            directoryListing.Items = searchItemsResult;
            directoryListing.FileCount = fileCount;
            directoryListing.FolderCount = folderCount;
            directoryListing.TotalSize = totalSize;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Service: [ItemService] Method: [SearchItems] Exception: [{ex}] InnerException: [{ex.InnerException}] Message: [{ex.Message}] StackTrace: [{ex.StackTrace}] ");
            throw;
        }

        _logger.LogInformation($"Execution of Service: [ItemService] Method: [SearchItems] completed succesfully. Items found: [{directoryListing.Items.Count()}]");
        return directoryListing;
        
    }

    public async Task<Item> UploadFileAsync(string? path, IFormFile file)
    {
        _logger.LogInformation($"Executing Service: [ItemService] Method: [UploadFileAsync]");

        path ??= "";
        Item itemResult = new Item();

        if (file == null)
        {
            throw new ArgumentException("Invalid file");
        }

        string fileName = Path.GetFileName(file.FileName);
        string fullPath = Path.Combine(_baseStorageDirectory, path, fileName);

        if (!Path.GetFullPath(fullPath).StartsWith(_baseStorageDirectory))
        {
            throw new ArgumentException("Invalid path");
        }

        try
        {
            using FileStream fileStream = File.Create(fullPath);
            await file.CopyToAsync(fileStream);

            itemResult.Name = fileName;
            itemResult.Path = Path.GetRelativePath(_baseStorageDirectory, fullPath);
            itemResult.Type = ItemType.File;

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Service: [ItemService] Method: [UploadFileAsync] Exception: [{ex}] InnerException: [{ex.InnerException}] Message: [{ex.Message}] StackTrace: [{ex.StackTrace}] ");
            throw;
        }

        return itemResult;

    }
}