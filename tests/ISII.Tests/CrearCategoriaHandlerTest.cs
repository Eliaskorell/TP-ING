using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using ISII.Web.CategoriaFeatures.Crear;
using ISII.Web.Domain.CategoriaAggregate;
using NSubstitute;

namespace ISII.Tests;

public class CrearCategoriaHandlerTest
{
    private readonly IRepository<Categoria> _repository = Substitute.For<IRepository<Categoria>>();
    private readonly CrearCategoriaHandler _handler;

    public CrearCategoriaHandlerTest()
    {
        _handler = new CrearCategoriaHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldAddCategoriaAndReturnOk_WhenNombreDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns((Categoria?)null);
        var command = new CrearCategoriaCommand("Bebidas", "Gaseosas, jugos y aguas");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Bebidas", result.Value.Nombre);
        Assert.Equal("Gaseosas, jugos y aguas", result.Value.Descripcion);
        await _repository.Received(1).AddAsync(
            Arg.Is<Categoria>(c => c.Nombre == "Bebidas"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnConflictAndNotAdd_WhenNombreAlreadyExists()
    {
        var existente = new Categoria(CategoriaId.From(1), "Bebidas", null);
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns(existente);
        var command = new CrearCategoriaCommand("bebidas", null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        await _repository.DidNotReceive().AddAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnConflictAndNotAdd_WhenNombreAlreadyExistsWithExtraSpaces()
    {
        var existente = new Categoria(CategoriaId.From(1), "Bebidas", null);
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Categoria>>(), Arg.Any<CancellationToken>())
            .Returns(existente);
        var command = new CrearCategoriaCommand(" Bebidas ", null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        await _repository.DidNotReceive().AddAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }
}
