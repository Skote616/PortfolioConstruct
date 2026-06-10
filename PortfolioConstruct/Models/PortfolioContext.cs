using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Models;

public partial class PortfolioContext : DbContext
{
    public PortfolioContext() { }
    public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options) { }

    public virtual DbSet<Block>          Blocks          { get; set; }
    public virtual DbSet<BlockType>      BlockTypes      { get; set; }
    public virtual DbSet<DesignSetting>  DesignSettings  { get; set; }
    public virtual DbSet<GalleryImage>   GalleryImages   { get; set; }
    public virtual DbSet<Portfolio>      Portfolios      { get; set; }
    public virtual DbSet<Role>           Roles           { get; set; }
    public virtual DbSet<Section>        Sections        { get; set; }
    public virtual DbSet<StudentProfile> StudentProfiles { get; set; }
    public virtual DbSet<User>           Users           { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Block =====
        modelBuilder.Entity<Block>(e =>
        {
            e.ToTable("Block");
            e.HasIndex(b => b.SectionId);
            e.HasIndex(b => b.BlockTypeId);
            e.HasOne(b => b.Section)
             .WithMany(s => s.Blocks)
             .HasForeignKey(b => b.SectionId);
            e.HasOne(b => b.BlockType)
             .WithMany(t => t.Blocks)
             .HasForeignKey(b => b.BlockTypeId);
            // Type — вычисляемое свойство, не маппим в колонку
            e.Ignore(b => b.Type);
        });

        // ===== BlockType (справочник) =====
        modelBuilder.Entity<BlockType>(e =>
        {
            e.ToTable("BlockType");
            e.HasIndex(t => t.Code).IsUnique();
            // Заполняем справочник при создании БД
            e.HasData(
                new BlockType { Id = 1, Code = "text", Icon = "📝", IsActive = true },
                new BlockType { Id = 2, Code = "image", Icon = "🖼", IsActive = true },
                new BlockType { Id = 3, Code = "link", Icon = "🔗", IsActive = true },
                new BlockType { Id = 4, Code = "gallery", Icon = "🖼🖼", IsActive = true },
                new BlockType { Id = 5, Code = "document", Icon = "📎", IsActive = true }
            );
        });

        // ===== GalleryImage =====
        modelBuilder.Entity<GalleryImage>(e =>
        {
            e.ToTable("GalleryImage");
            e.HasIndex(g => g.BlockId);
            e.HasOne(g => g.Block)
             .WithMany(b => b.GalleryImages)
             .HasForeignKey(g => g.BlockId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== DesignSetting =====
        modelBuilder.Entity<DesignSetting>(e =>
        {
            e.HasIndex(d => d.PortfolioId).IsUnique();
            e.HasOne(d => d.Portfolio)
             .WithOne(p => p.DesignSetting)
             .HasForeignKey<DesignSetting>(d => d.PortfolioId);
        });

        // ===== Portfolio =====
        modelBuilder.Entity<Portfolio>(e =>
        {
            e.HasIndex(p => p.UserId).IsUnique();
            e.HasOne(p => p.User)
             .WithOne(u => u.Portfolio)
             .HasForeignKey<Portfolio>(p => p.UserId);
        });

        // ===== Role (справочник) =====
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Role");
            e.HasIndex(r => r.Code).IsUnique();
            e.HasData(
                new Role { Id = 1, Code = "User"},
                new Role { Id = 2, Code = "Admin"}
            );
        });

        // ===== Section =====
        modelBuilder.Entity<Section>(e =>
        {
            e.ToTable("Section");
            e.HasIndex(s => s.PortfolioId);
            e.HasOne(s => s.Portfolio)
             .WithMany(p => p.Sections)
             .HasForeignKey(s => s.PortfolioId);
        });

        // ===== StudentProfile =====
        modelBuilder.Entity<StudentProfile>(e =>
        {
            e.ToTable("StudentProfile");
            e.HasIndex(p => p.UserId).IsUnique();
            e.HasOne(p => p.User)
             .WithOne(u => u.StudentProfile)
             .HasForeignKey<StudentProfile>(p => p.UserId);
        });

        // ===== User =====
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id).HasName("PK_Users");
            e.ToTable("User");
            e.Property(u => u.Email).HasMaxLength(200);
            e.Property(u => u.PasswordHash).HasMaxLength(200);
            e.HasIndex(u => u.RoleId);
            e.HasOne(u => u.Role)
             .WithMany(r => r.Users)
             .HasForeignKey(u => u.RoleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
