using Vogen;

namespace ISII.Web.Domain.CategoriaAggregate;

[ValueObject<int>]
public readonly partial struct CategoriaId
{
  public static CategoriaId Nueva => From(0);

  private static Validation Validate(int valor)
      => valor >= 0 ? Validation.Ok : Validation.Invalid("El Id de la categoría no puede ser negativo.");
}
