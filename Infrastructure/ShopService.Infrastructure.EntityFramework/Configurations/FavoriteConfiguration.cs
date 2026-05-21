using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property<Guid>("CustomerId")
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property<Guid>("ProductId")
            .HasColumnName("product_id")
            .IsRequired();

        builder.HasIndex("CustomerId", "ProductId").IsUnique();

        builder.HasOne(x => x.Customer)
            .WithMany("_favorites")
            .HasForeignKey("CustomerId");

        builder.HasOne(x => x.Product)
            .WithMany("_favorites")
            .HasForeignKey("ProductId");
    }
}
