using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ISII.Web.Domain.CategoriaAggregate;

namespace ISII.Web.Infrastructure.Data.Config;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
  public void Configure(EntityTypeBuilder<Categoria> builder)
  {
    builder.Property(entity => entity.Id)
      .HasVogenConversion()
      .HasSentinel(CategoriaId.Nueva)
      .HasValueGenerator<VogenIntIdValueGenerator<AppDbContext, Categoria, CategoriaId>>()
      .ValueGeneratedOnAdd()
      .IsRequired();

    builder.Property(entity => entity.Nombre)
      .HasMaxLength(Categoria.NombreLongitudMaxima)
      .IsRequired();

    builder.HasIndex(entity => entity.Nombre)
      .IsUnique();

    builder.Property(entity => entity.Descripcion)
      .HasMaxLength(Categoria.DescripcionLongitudMaxima);
  }
}
