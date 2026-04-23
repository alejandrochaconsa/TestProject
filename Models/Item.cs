using TestProject.Enums;
namespace TestProject.Models;

public class Item
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public ItemType Type { get; set; }

}