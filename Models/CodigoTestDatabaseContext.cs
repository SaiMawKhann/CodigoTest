using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CodigoTest.Models;

namespace CodigoTest.Models;

public partial class CodigoTestDatabaseContext : DbContext
{
    public CodigoTestDatabaseContext()
    {
    }

    public CodigoTestDatabaseContext(DbContextOptions<CodigoTestDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminTable> AdminTables { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberCoupon> MemberCoupons { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }




    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=CodigoTestDatabase;Trusted_Connection=True;Encrypt=false;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminTable>(entity =>
        {
            entity.HasKey(e => e.AdminId);

            entity.ToTable("admin_table");

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .HasColumnName("mobileNumber");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("userName");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("coupon");

            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.CouponName)
                .HasMaxLength(50)
                .HasColumnName("couponName");
            entity.Property(e => e.DiscountAmount)
                .HasMaxLength(50)
                .HasColumnName("discountAmount");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("item");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .HasColumnName("itemName");
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .HasColumnName("itemType");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("member");

            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.MemberName)
                .HasMaxLength(50)
                .HasColumnName("member_Name");
            entity.Property(e => e.MemberTotalPoint).HasColumnName("memberTotalPoint");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .HasColumnName("mobileNumber");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        modelBuilder.Entity<MemberCoupon>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.CouponId, e.OrderId });

            entity.ToTable("member_coupon");

            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.DiscountTotalPrice).HasColumnName("discountTotalPrice");
            entity.Property(e => e.OrderTotalPrice).HasColumnName("orderTotalPrice");
            entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");
            entity.Property(e => e.UsedDate)
                .HasColumnType("datetime")
                .HasColumnName("usedDate");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Desciption)
                .HasMaxLength(50)
                .HasColumnName("desciption");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("orderDate");
            entity.Property(e => e.TotalPoint).HasColumnName("totalPoint");
            entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_member_id");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ItemId }).HasName("PK_order_items");

            entity.ToTable("order_item");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Point).HasColumnName("point");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Total).HasColumnName("total");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.ToTable("RefreshToken");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiry_date");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.Token)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("token");

            entity.HasOne(d => d.Member).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__RefreshTo__member___75A278F5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
