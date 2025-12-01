using FakeID.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeID.Application.Contexts
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ClientEntity> Clients { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientEntity>().Create();
        }
    }

    file static class ApplicationContextExtensions
    {
        public static void Create(this EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable("clients");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired();
        }
    }
}
