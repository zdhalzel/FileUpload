using Microsoft.EntityFrameworkCore;

namespace FileUpload.Models
{
    public class MyFileContext : DbContext
    {
        public MyFileContext(DbContextOptions<MyFileContext> options) : base(options)
        {
        }

        public DbSet<DBEntry> DbEntry { get; set; }
    }
}
