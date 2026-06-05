using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Models;

public partial class PortfolioContext : DbContext
{
    public PortfolioContext()
    {
    }

    public PortfolioContext(DbContextOptions<PortfolioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Block> Blocks { get; set; }
    public virtual DbSet<DesignSetting> DesignSettings { get; set; }
    public virtual DbSet<Portfolio> Portfolios { get; set; }
    public virtual DbSet<Section> Sections { get; set; }
    public virtual DbSet<User> Users { get; set; }

    // Новая таблица для фото галереи
    public virtual DbSet<GalleryImage> GalleryImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Строка подключения берётся из appsettings.json через DI
        // Этот метод остаётся пустым
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Block>(entity =>
        {
            entity.ToTable("Block");
            entity.HasIndex(e => e.SectionId, "IX_Block_SectionId");
            entity.HasOne(d => d.Section).WithMany(p => p.Blocks).HasForeignKey(d => d.SectionId);
        });

        modelBuilder.Entity<GalleryImage>(entity =>
        {
            entity.ToTable("GalleryImage");
            entity.HasIndex(e => e.BlockId, "IX_GalleryImage_BlockId");
            entity.HasOne(d => d.Block)
                  .WithMany(p => p.GalleryImages)
                  .HasForeignKey(d => d.BlockId)
                  .OnDelete(DeleteBehavior.Cascade); // удаляем фото вместе с блоком
        });

        modelBuilder.Entity<DesignSetting>(entity =>
        {
            entity.HasIndex(e => e.PortfolioId, "IX_DesignSettings_PortfolioId").IsUnique();
            entity.HasOne(d => d.Portfolio).WithOne(p => p.DesignSetting).HasForeignKey<DesignSetting>(d => d.PortfolioId);
        });

        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Portfolios_UserId").IsUnique();
            entity.HasOne(d => d.User).WithOne(p => p.Portfolio).HasForeignKey<Portfolio>(d => d.UserId);
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.ToTable("Section");
            entity.HasIndex(e => e.PortfolioId, "IX_Section_PortfolioId");
            entity.HasOne(d => d.Portfolio).WithMany(p => p.Sections).HasForeignKey(d => d.PortfolioId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");
            entity.ToTable("User");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
