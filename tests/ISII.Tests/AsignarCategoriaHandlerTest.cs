using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Domain.ProductAggregate;
using ISII.Web.ProductFeatures.AsignarCategoria;
using NSubstitute;

namespace ISII.Tests;

public class AsignarCategoriaHandlerTest
{
    private readonly IRepository<Product> _productRepository = Substitute.For<IRepository<Product>>();
    private readonly IReadRepository<Categoria> _categoriaRepository = Substitute.For<IReadRepository<Categoria>>();
    private readonly AsignarCategoriaHandler _handler;

    public AsignarCategoriaHandlerTest()
    {
        _handler = new AsignarCategoriaHandler(_productRepository, _categoriaRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        _productRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Product>>(), Arg.Any<CancellationToken>())
            .Returns((Product?)null);
        var command = new AsignarCategoriaCommand(ProductId.From(1), CategoriaId.From(1));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(ResultStatus.NotFound, result.Status);
        await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundAndNotSave_WhenCategoriaDoesNotExist()
    {
        var product = new Product(ProductId.From(1), "Laptop", 999.99m);
        _productRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Product>>(), Arg.Any<CancellationToken>())
            .Returns(product);
        _categoriaRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns((Categoria?)null);
        var command = new AsignarCategoriaCommand(ProductId.From(1), CategoriaId.From(1));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(ResultStatus.NotFound, result.Status);
        await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldAssignCategoriaAndSave_WhenProductAndCategoriaExist()
    {
        var product = new Product(ProductId.From(1), "Laptop", 999.99m);
        var categoria = new Categoria(CategoriaId.From(1), "Electronica", null);
        _productRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Product>>(), Arg.Any<CancellationToken>())
            .Returns(product);
        _categoriaRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns(categoria);
        var command = new AsignarCategoriaCommand(ProductId.From(1), CategoriaId.From(1));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(CategoriaId.From(1), result.Value.CategoriaId);
        await _productRepository.Received(1).UpdateAsync(
            Arg.Is<Product>(p => p.CategoriaId == CategoriaId.From(1)), Arg.Any<CancellationToken>());
    }
}
