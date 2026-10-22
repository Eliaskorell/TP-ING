using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using ISII.Web.CategoriaFeatures.ObtenerPorId;
using ISII.Web.Domain.CategoriaAggregate;
using NSubstitute;

namespace ISII.Tests;

public class ObtenerCategoriaHandlerTest
{
    private readonly IReadRepository<Categoria> _repository = Substitute.For<IReadRepository<Categoria>>();
    private readonly ObtenerCategoriaHandler _handler;

    public ObtenerCategoriaHandlerTest()
    {
        _handler = new ObtenerCategoriaHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoria_WhenItExists()
    {
        var categoria = new Categoria(CategoriaId.From(1), "Bebidas", "Gaseosas, jugos y aguas");
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns(categoria);

        var result = await _handler.Handle(new ObtenerCategoriaQuery(CategoriaId.From(1)), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(CategoriaId.From(1), result.Value.Id);
        Assert.Equal("Bebidas", result.Value.Nombre);
        Assert.Equal("Gaseosas, jugos y aguas", result.Value.Descripcion);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns((Categoria?)null);

        var result = await _handler.Handle(new ObtenerCategoriaQuery(CategoriaId.From(99)), CancellationToken.None);

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }
}
