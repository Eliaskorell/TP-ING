namespace ISII.Web.Domain.CategoriaAggregate.Specifications;

public class CategoriaPorIdSpec : Specification<Categoria>
{
  public CategoriaPorIdSpec(CategoriaId categoriaId) =>
    Query
        .Where(categoria => categoria.Id == categoriaId);
}
