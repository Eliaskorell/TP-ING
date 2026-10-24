using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Domain.CategoriaAggregate.Specifications;

namespace ISII.Tests;

public class CategoriaPorNombreSpecTest
{
    private readonly List<Categoria> _categorias =
    [
        new Categoria(CategoriaId.From(1), "Bebidas", null),
        new Categoria(CategoriaId.From(2), "Limpieza", null)
    ];

    [Theory]
    [InlineData("Bebidas")]
    [InlineData("bebidas")]
    [InlineData("BEBIDAS")]
    [InlineData("bEbIdAs")]
    public void Evaluate_ShouldFindCategoria_IgnoringCase(string nombre)
    {
        var spec = new CategoriaPorNombreSpec(nombre);

        var resultado = spec.Evaluate(_categorias).ToList();

        Assert.Single(resultado);
        Assert.Equal(CategoriaId.From(1), resultado[0].Id);
    }

    [Fact]
    public void Evaluate_ShouldReturnEmpty_WhenNombreDoesNotExist()
    {
        var spec = new CategoriaPorNombreSpec("Almacen");

        var resultado = spec.Evaluate(_categorias);

        Assert.Empty(resultado);
    }
}
