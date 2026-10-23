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
    var nombreRecortado = nombre.Trim();
    Guard.Against.StringTooLong(nombreRecortado, NombreLongitudMaxima, nameof(nombre));

    var descripcionRecortada = descripcion?.Trim();
    if (descripcionRecortada != null)
    {
      Guard.Against.StringTooLong(descripcionRecortada, DescripcionLongitudMaxima, nameof(descripcion));
    }

    Id = id;
    Nombre = nombreRecortado;
    Descripcion = descripcionRecortada;
  }

  public static Categoria Crear(string nombre, string? descripcion)
    => new Categoria(CategoriaId.Nueva, nombre, descripcion);

  public string Nombre { get; private set; } = string.Empty;
  public string? Descripcion { get; private set; }
}
