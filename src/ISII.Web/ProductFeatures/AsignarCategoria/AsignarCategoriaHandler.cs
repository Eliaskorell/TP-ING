using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Domain.CategoriaAggregate.Specifications;
using ISII.Web.Domain.ProductAggregate;
using ISII.Web.Domain.ProductAggregate.Specifications;

namespace ISII.Web.ProductFeatures.AsignarCategoria;

public record AsignarCategoriaCommand(ProductId ProductId, CategoriaId CategoriaId) : ICommand<Result<ProductDto>>;

public class AsignarCategoriaHandler(
  IRepository<Product> productRepository,
  IReadRepository<Categoria> categoriaRepository)
  : ICommandHandler<AsignarCategoriaCommand, Result<ProductDto>>
{
  public async ValueTask<Result<ProductDto>> Handle(AsignarCategoriaCommand request, CancellationToken cancellationToken)
  {
    var productSpec = new ProductByIdSpec(request.ProductId);
    var product = await productRepository.FirstOrDefaultAsync(productSpec, cancellationToken);
    if (product == null)
    {
      return Result.NotFound("No existe el producto");
    }

    var categoriaSpec = new CategoriaPorIdSpec(request.CategoriaId);
    var categoria = await categoriaRepository.FirstOrDefaultAsync(categoriaSpec, cancellationToken);
    if (categoria == null)
    {
      return Result.NotFound("No existe la categoria");
    }

    product.AsignarCategoria(request.CategoriaId);
    await productRepository.UpdateAsync(product, cancellationToken);

    return new ProductDto(product.Id, product.Name, product.UnitPrice, product.CategoriaId);
  }
}
