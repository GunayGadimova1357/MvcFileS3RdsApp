namespace MvcS3Files.Models;

public class FileItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    public string FileKey { get; set; } 

    public DateTime CreatedAt { get; set; } 
}