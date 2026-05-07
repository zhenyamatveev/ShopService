using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class ProductPromotionConfiguration : IEntityTypeConfiguration<ProductPromotion>
{
    public void Configure(EntityTypeBuilder<ProductPromotion> builder)
    {
        builder.ToTable("product_promotions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id");

        builder.Property(x => x.PromotionId)
            .HasColumnName("promotion_id");

        builder.Ignore(x => x.Product);
        builder.Ignore(x => x.Promotion);

        builder.HasIndex(x => new { x.ProductId, x.PromotionId })
            .IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany("_productPromotions")
            .HasForeignKey(x => x.ProductId);

        builder.HasOne(x => x.Promotion)
            .WithMany("_productPromotions")
            .HasForeignKey(x => x.PromotionId);
    }
}

