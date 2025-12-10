using FakeID.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeID.Application.Contexts
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<PersonaEntity> Personas { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientEntity>().Create();
            modelBuilder.Entity<PersonaEntity>().Create();
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

        public static void Create(this EntityTypeBuilder<PersonaEntity> builder)
        {
            builder.ToTable("personas");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired();

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired();

            builder.Property(e => e.Role)
                .HasColumnName("role")
                .IsRequired();

            builder.HasOne(e => e.Client)
                .WithMany(e => e.Personas)
                .HasForeignKey("id_client")
                .HasConstraintName("fk_personas_client")
                .IsRequired();
        }
    }
}
