using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;
using ShopService.ValueObjects;
using ShopService.ValueObjects.Validators;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("sellers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(v => v.Value, v => new Name(v))
            .HasMaxLength(NameValidator.MAX_LENGTH);

        builder.Ignore(x => x.Products);
        builder.Ignore(x => x.Promotions);

        builder.HasMany<Product>("_products")
            .WithOne(x => x.Seller)
            .HasForeignKey("SellerId");

        builder.HasMany<Promotion>("_promotions")
            .WithOne(x => x.Seller)
            .HasForeignKey("SellerId");
    }
}
