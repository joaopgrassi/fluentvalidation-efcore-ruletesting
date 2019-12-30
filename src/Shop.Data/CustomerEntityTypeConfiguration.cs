using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Data.Entities;

namespace Shop.Data
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Surname).IsRequired().HasMaxLength(255);
            builder.Property(c => c.Forename).IsRequired().HasMaxLength(255);
            builder.Property(c => c.Address).HasMaxLength(250);
        }
    }
}
