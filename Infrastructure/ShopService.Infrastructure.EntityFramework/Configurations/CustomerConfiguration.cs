using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Domain.Entities;
using ShopService.ValueObjects;
using ShopService.ValueObjects.Validators;

namespace ShopService.Infrastructure.EntityFramework.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(v => v.Value, v => new Name(v))
            .HasMaxLength(NameValidator.MAX_LENGTH);

        builder.Ignore(x => x.Favorites);

        builder.HasMany<Favorite>("_favorites")
            .WithOne(x => x.Customer)
            .HasForeignKey("CustomerId");
    }
}
