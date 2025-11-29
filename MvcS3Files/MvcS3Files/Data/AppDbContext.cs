
using Microsoft.EntityFrameworkCore;
using MvcS3Files.Models;

namespace MvcS3Files.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FileItem> FileItems { get; set; }
    }
}
