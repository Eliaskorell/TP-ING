using Ardalis.GuardClauses;

namespace ISII.Web.Domain.CategoriaAggregate;

public class Categoria : EntityBase<Categoria, CategoriaId>, IAggregateRoot
{
  public const int NombreLongitudMaxima = 100;
  public const int DescripcionLongitudMaxima = 500;

  private Categoria() { }

  public Categoria(CategoriaId id, string nombre, string? descripcion)
  {
    Guard.Against.NullOrWhiteSpace(nombre, nameof(nombre));
    Guard.Against.StringTooLong(nombre, NombreLongitudMaxima, nameof(nombre));

    if (descripcion != null)
    {
      Guard.Against.StringTooLong(descripcion, DescripcionLongitudMaxima, nameof(descripcion));
    }

    Id = id;
    Nombre = nombre;
    Descripcion = descripcion;
  }

  public static Categoria Crear(string nombre, string? descripcion)
    => new Categoria(CategoriaId.Nueva, nombre, descripcion);

  public string Nombre { get; private set; } = string.Empty;
  public string? Descripcion { get; private set; }
}
