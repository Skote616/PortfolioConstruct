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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=Portfolio;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Block>(entity =>
        {
            entity.ToTable("Block");

            entity.HasIndex(e => e.SectionId, "IX_Block_SectionId");

            entity.HasOne(d => d.Section).WithMany(p => p.Blocks).HasForeignKey(d => d.SectionId);
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
