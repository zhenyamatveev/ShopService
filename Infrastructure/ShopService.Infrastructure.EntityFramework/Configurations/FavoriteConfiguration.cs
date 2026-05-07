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
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.CustomerId)
            .HasColumnName("customer_id");

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id");

        builder.HasIndex(x => new { x.CustomerId, x.ProductId })
            .IsUnique();

        builder.HasOne(x => x.Customer)
            .WithMany("_favorites")
            .HasForeignKey(x => x.CustomerId);

        builder.HasOne(x => x.Product)
            .WithMany("_favorites")
            .HasForeignKey(x => x.ProductId);
    }
}

