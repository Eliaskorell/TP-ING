using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Domain.CategoriaAggregate.Specifications;

namespace ISII.Web.CategoriaFeatures.Crear;

public record CrearCategoriaCommand(string Nombre, string? Descripcion) : ICommand<Result<CategoriaDto>>;

public class CrearCategoriaHandler(IRepository<Categoria> repository)
  : ICommandHandler<CrearCategoriaCommand, Result<CategoriaDto>>
{
  public async ValueTask<Result<CategoriaDto>> Handle(CrearCategoriaCommand request, CancellationToken cancellationToken)
  {
    var spec = new CategoriaPorNombreSpec(request.Nombre);
    var existente = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (existente != null)
    {
      return Result.Conflict("Ya existe una categoria con ese nombre");
    }

    var categoria = Categoria.Crear(request.Nombre, request.Descripcion);
    await repository.AddAsync(categoria, cancellationToken);

    return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
  }
}
