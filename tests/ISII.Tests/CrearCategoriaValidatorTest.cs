using ISII.Web.CategoriaFeatures.Crear;
using ISII.Web.Domain.CategoriaAggregate;

namespace ISII.Tests;

public class CrearCategoriaValidatorTest
{
    private readonly CrearCategoriaValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        var request = new CrearCategoriaRequest { Nombre = "Bebidas", Descripcion = "Gaseosas, jugos y aguas" };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenDescripcionIsNull()
    {
        var request = new CrearCategoriaRequest { Nombre = "Bebidas", Descripcion = null };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNombreIsEmpty(string nombre)
    {
        var request = new CrearCategoriaRequest { Nombre = nombre };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CrearCategoriaRequest.Nombre));
    }

    [Fact]
    public void Validate_ShouldFail_WhenNombreIsTooLong()
    {
        var request = new CrearCategoriaRequest { Nombre = new string('a', Categoria.NombreLongitudMaxima + 1) };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CrearCategoriaRequest.Nombre));
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescripcionIsTooLong()
    {
        var request = new CrearCategoriaRequest
        {
            Nombre = "Bebidas",
            Descripcion = new string('a', Categoria.DescripcionLongitudMaxima + 1)
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CrearCategoriaRequest.Descripcion));
    }
}
