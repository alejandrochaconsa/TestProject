using TestProject.Models;

public class DirectoryListing
{
    public IEnumerable<Item> Items { get; set; } = [];
    public int FileCount { get; set; }
    public int FolderCount { get; set; }
    public long TotalSize { get; set; }

}