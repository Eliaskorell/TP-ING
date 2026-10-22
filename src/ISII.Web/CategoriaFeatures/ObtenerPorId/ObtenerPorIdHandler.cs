using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Domain.CategoriaAggregate.Specifications;

namespace ISII.Web.CategoriaFeatures.ObtenerPorId;

public record ObtenerCategoriaQuery(CategoriaId CategoriaId) : IQuery<Result<CategoriaDto>>;

public class ObtenerCategoriaHandler(IReadRepository<Categoria> repository)
  : IQueryHandler<ObtenerCategoriaQuery, Result<CategoriaDto>>
{
  public async ValueTask<Result<CategoriaDto>> Handle(ObtenerCategoriaQuery request, CancellationToken cancellationToken)
  {
    var spec = new CategoriaPorIdSpec(request.CategoriaId);
    var categoria = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (categoria == null)
    {
      return Result.NotFound();
    }

    return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
  }
}
