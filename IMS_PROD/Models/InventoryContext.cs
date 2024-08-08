using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IMS_PROD.Models;

public partial class InventoryContext : DbContext
{
    public InventoryContext()
    {
    }

    public InventoryContext(DbContextOptions<InventoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Update> Updates { get; set; }
    public virtual DbSet<Purchase> Purchase{ get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;Database=Inventory;User Id=HMS_PROD;Password=Vikas@456;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("Items_pkey");
            entity.ToTable("Items");
            entity.Property(e => e.ItemId)
                .ValueGeneratedNever()
                .HasColumnName("ItemID");
            entity.Property(e => e.ItemName).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UId).HasName("Users_pkey");
            entity.ToTable("User");
            entity.Property(e => e.UId)
                .ValueGeneratedNever()
                .HasColumnName("UId");
            entity.Property(e => e.MailId).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.TId).HasName("Stock_pkey");

            entity.ToTable("Stock");

            entity.Property(e => e.TId)
                .ValueGeneratedNever()
                .HasColumnName("T_ID");
            entity.Property(e => e.BatchId)
                .HasMaxLength(255)
                .HasColumnName("BatchID");
            entity.Property(e => e.ExpiryDate).HasColumnType("timestamp without time zone"); 
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Mrp).HasColumnName("MRP");
            entity.Property(e => e.SItems).HasColumnName("SItems");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseID).HasName("Purchase_pkey");
            entity.ToTable("Purchase");
            entity.Property(e => e.PurchaseID)
                .ValueGeneratedNever()
                .HasColumnName("PurchaseID").HasMaxLength(255);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.PDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PItems).HasColumnName("PItems");

            entity.HasOne(d => d.ItemIDNavigation)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("Purchase_ItemID_fkey");

        });

        modelBuilder.Entity<Update>(entity =>
        {
            entity.HasKey(e => e.UpdateToken).HasName("Update_pkey");
            entity.ToTable("Update");
            entity.Property(e => e.UpdateToken)
                .ValueGeneratedNever()
                .HasColumnName("UpdateToken") ;
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ItemRem).HasColumnName("ItemRem");
            entity.HasOne(d => d.ItemIDNavigation)
                   .WithMany(u => u.Updates)
                   .HasForeignKey(d => d.ItemId)
                   .HasConstraintName("Update_ItemID_fkey");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
