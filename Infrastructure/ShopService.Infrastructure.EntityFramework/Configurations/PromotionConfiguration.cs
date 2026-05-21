using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;
using ShopService.ValueObjects;
using ShopService.ValueObjects.Validators;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("promotions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasConversion(v => v.Value, v => new Title(v))
            .HasMaxLength(TitleValidator.MAX_LENGTH);

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.Discount)
            .HasColumnName("discount")
            .HasPrecision(5, 2)
            .HasConversion(
                v => v == null ? (decimal?)null : v.Value,
                v => v == null ? null : new Discount(v.Value));

        builder.Property<Guid>("SellerId")
            .HasColumnName("seller_id")
            .IsRequired();

        builder.Property(x => x.StartDate).HasColumnName("start_date");
        builder.Property(x => x.EndDate).HasColumnName("end_date");

        builder.Ignore(x => x.ProductPromotions);

        builder.HasMany<ProductPromotion>("_productPromotions")
            .WithOne(x => x.Promotion)
            .HasForeignKey("PromotionId");

        builder.HasOne(x => x.Seller)
            .WithMany("_promotions")
            .HasForeignKey("SellerId");

        builder.HasMany<ProductPromotion>("_productPromotions")
            .WithOne(x => x.Promotion)
            .HasForeignKey("PromotionId");
    }
}
