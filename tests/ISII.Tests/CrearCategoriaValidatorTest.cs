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
    [InlineData("\t")]
    public void Validate_ShouldFail_WhenNombreIsEmpty(string nombre)
    {
        var request = new CrearCategoriaRequest { Nombre = nombre };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CrearCategoriaRequest.Nombre) && e.ErrorMessage == "El nombre es obligatorio");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNombreIsNull()
    {
        var request = new CrearCategoriaRequest { Nombre = null! };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CrearCategoriaRequest.Nombre) && e.ErrorMessage == "El nombre es obligatorio");
    }

    [Fact]
    public void Validate_ShouldPass_WhenNombreHasSurroundingWhitespace()
    {
        var request = new CrearCategoriaRequest { Nombre = " Bebidas " };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNombreHasMaxLength()
    {
        var request = new CrearCategoriaRequest { Nombre = new string('a', Categoria.NombreLongitudMaxima) };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNombreIsTooLong()
    {
        var request = new CrearCategoriaRequest { Nombre = new string('a', Categoria.NombreLongitudMaxima + 1) };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CrearCategoriaRequest.Nombre) &&
            e.ErrorMessage == $"El nombre no puede superar los {Categoria.NombreLongitudMaxima} caracteres");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNombreWithSurroundingWhitespaceExceedsMaxLength()
    {
        var nombre = new string('a', Categoria.NombreLongitudMaxima);
        var request = new CrearCategoriaRequest { Nombre = $" {nombre} " };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CrearCategoriaRequest.Nombre) &&
            e.ErrorMessage == $"El nombre no puede superar los {Categoria.NombreLongitudMaxima} caracteres");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(" texto ")]
    public void Validate_ShouldPass_WhenDescripcionIsEmptyWhitespaceOrHasSurroundingWhitespace(string descripcion)
    {
        var request = new CrearCategoriaRequest { Nombre = "Bebidas", Descripcion = descripcion };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenDescripcionHasMaxLength()
    {
        var request = new CrearCategoriaRequest
        {
            Nombre = "Bebidas",
            Descripcion = new string('a', Categoria.DescripcionLongitudMaxima)
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
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
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CrearCategoriaRequest.Descripcion) &&
            e.ErrorMessage == $"La descripcion no puede superar los {Categoria.DescripcionLongitudMaxima} caracteres");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescripcionWithSurroundingWhitespaceExceedsMaxLength()
    {
        var descripcion = new string('a', Categoria.DescripcionLongitudMaxima);
        var request = new CrearCategoriaRequest { Nombre = "Bebidas", Descripcion = $" {descripcion} " };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CrearCategoriaRequest.Descripcion) &&
            e.ErrorMessage == $"La descripcion no puede superar los {Categoria.DescripcionLongitudMaxima} caracteres");
    }
}
