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
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property<Guid>("ProductId")
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property<Guid>("PromotionId")
            .HasColumnName("promotion_id")
            .IsRequired();

        builder.HasIndex("ProductId", "PromotionId").IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany("_productPromotions")
            .HasForeignKey("ProductId");

        builder.HasOne(x => x.Promotion)
            .WithMany("_productPromotions")
            .HasForeignKey("PromotionId");
    }
}
