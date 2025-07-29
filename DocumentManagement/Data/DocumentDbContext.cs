using DocumentManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Data;

public partial class DocumentDbContext : DbContext
{
    public DocumentDbContext(DbContextOptions<DocumentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<DocumentAttachment> DocumentAttachments { get; set; }
    public virtual DbSet<DocumentLog> DocumentLogs { get; set; }
    public virtual DbSet<DocumentRecipient> DocumentRecipients { get; set; }
    public virtual DbSet<DocumentStatus> DocumentStatus { get; set; }
    public virtual DbSet<UrgencyLevel> UrgencyLevels { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core sẽ tự động cấu hình các mối quan hệ dựa trên ForeignKey,
        // nhưng bạn có thể thêm các cấu hình phức tạp hơn ở đây nếu cần.

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
