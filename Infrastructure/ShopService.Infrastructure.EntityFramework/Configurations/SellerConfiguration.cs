using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;
using ShopService.ValueObjects;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("sellers");

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

        builder.Ignore(x => x.Products);
        builder.Ignore(x => x.Promotions);

        builder.HasMany<Product>("_products")
            .WithOne(x => x.Seller)
            .HasForeignKey(x => x.SellerId);

        builder.HasMany<Promotion>("_promotions")
            .WithOne(x => x.Seller)
            .HasForeignKey(x => x.SellerId);
    }
}

