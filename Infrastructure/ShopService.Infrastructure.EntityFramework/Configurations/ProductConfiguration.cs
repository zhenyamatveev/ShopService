using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;
using ShopService.ValueObjects;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .HasConversion(
                v => v.Value,
                v => new Name(v)
            );

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2)
            .HasConversion(
                v => v.Value,
                v => new Price(v)
            );

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id");

        builder.Ignore(x => x.Favorites);
        builder.Ignore(x => x.ProductPromotions);

        builder.HasOne(x => x.Seller)
            .WithMany("_products")
            .HasForeignKey(x => x.SellerId);

        builder.HasMany<Favorite>("_favorites")
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);

        builder.HasMany<ProductPromotion>("_productPromotions")
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);
    }
}

