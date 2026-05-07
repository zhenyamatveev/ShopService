using Microsoft.EntityFrameworkCore;
using ShopService.Domain.Entities;

namespace ShopService.Infrastructure.EntityFramework;

/// <summary>
/// Контекст базы данных (EF Core).
/// 
/// Здесь определяются "таблицы" через DbSet, а также подключаются конфигурации
/// из папки Configurations (маппинг сущностей домена на таблицы PostgreSQL).
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>Таблица customers</summary>
    public DbSet<Customer> Customers { get; set; } = null!;
    /// <summary>Таблица sellers</summary>
    public DbSet<Seller> Sellers { get; set; } = null!;
    /// <summary>Таблица products</summary>
    public DbSet<Product> Products { get; set; } = null!;
    /// <summary>Таблица favorites</summary>
    public DbSet<Favorite> Favorites { get; set; } = null!;
    /// <summary>Таблица promotions</summary>
    public DbSet<Promotion> Promotions { get; set; } = null!;
    /// <summary>Таблица product_promotions</summary>
    public DbSet<ProductPromotion> ProductPromotions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Автоматически подключаем все IEntityTypeConfiguration<T>
        // из этой сборки (см. папку Configurations).
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}

