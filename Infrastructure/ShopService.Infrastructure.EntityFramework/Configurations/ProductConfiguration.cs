using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;
using ShopService.ValueObjects;
using ShopService.ValueObjects.Validators;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(v => v.Value, v => new Name(v))
            .HasMaxLength(NameValidator.MAX_LENGTH);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired(false)
            .HasConversion(
                d => d == null ? null : d.Value,
                s => s == null ? null : new Description(s))
            .HasMaxLength(DescriptionValidator.MAX_LENGTH);

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .IsRequired()
            .HasPrecision(10, 2)
            .HasConversion(v => v.Value, v => new Price(v));

        builder.Property<Guid>("SellerId")
            .HasColumnName("seller_id")
            .IsRequired();

        builder.Ignore(x => x.Favorites);
        builder.Ignore(x => x.ProductPromotions);

        builder.HasOne(x => x.Seller)
            .WithMany("_products")
            .HasForeignKey("SellerId");

        builder.HasMany<Favorite>("_favorites")
            .WithOne(x => x.Product)
            .HasForeignKey("ProductId");

        builder.HasMany<ProductPromotion>("_productPromotions")
            .WithOne(x => x.Product)
            .HasForeignKey("ProductId");
    }
}
