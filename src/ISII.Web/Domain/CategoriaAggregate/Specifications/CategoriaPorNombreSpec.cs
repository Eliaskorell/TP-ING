namespace ISII.Web.Domain.CategoriaAggregate.Specifications;

public class CategoriaPorNombreSpec : Specification<Categoria>
{
  public CategoriaPorNombreSpec(string nombre) =>
    Query
        .Where(categoria => categoria.Nombre.ToLower() == nombre.ToLower());
}
