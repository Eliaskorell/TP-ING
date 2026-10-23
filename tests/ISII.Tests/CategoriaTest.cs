using ISII.Web.Domain.CategoriaAggregate;

namespace ISII.Tests;

public class CategoriaTest
{
    [Fact]
    public void Constructor_ShouldAssignProvidedValues()
    {
        var id = CategoriaId.From(1);
        const string nombre = "Bebidas";
        const string descripcion = "Gaseosas, jugos y aguas";

        var categoria = new Categoria(id, nombre, descripcion);

        Assert.Equal(id, categoria.Id);
        Assert.Equal(nombre, categoria.Nombre);
        Assert.Equal(descripcion, categoria.Descripcion);
    }

    [Fact]
    public void Crear_ShouldReturnCategoriaWithNuevaId()
    {
        var categoria = Categoria.Crear("Limpieza", "Productos de limpieza");

        Assert.Equal(CategoriaId.Nueva, categoria.Id);
        Assert.Equal("Limpieza", categoria.Nombre);
        Assert.Equal("Productos de limpieza", categoria.Descripcion);
    }

    [Fact]
    public void Crear_ShouldAllowNullDescripcion()
    {
        var categoria = Categoria.Crear("Limpieza", null);

        Assert.Null(categoria.Descripcion);
    }

    [Fact]
    public void Crear_ShouldTrimNombreAndDescripcion()
    {
        var categoria = Categoria.Crear(" Bebidas ", " Gaseosas, jugos y aguas ");

        Assert.Equal("Bebidas", categoria.Nombre);
        Assert.Equal("Gaseosas, jugos y aguas", categoria.Descripcion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_ShouldThrow_WhenNombreIsEmpty(string nombre)
    {
        Assert.Throws<ArgumentException>(() => Categoria.Crear(nombre, null));
    }

    [Fact]
    public void Crear_ShouldThrow_WhenNombreIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Categoria.Crear(null!, null));
    }

    [Fact]
    public void Crear_ShouldThrow_WhenNombreIsTooLong()
    {
        var nombre = new string('a', Categoria.NombreLongitudMaxima + 1);

        Assert.Throws<ArgumentException>(() => Categoria.Crear(nombre, null));
    }

    [Fact]
    public void Crear_ShouldAllowNombreWithMaxLength()
    {
        var nombre = new string('a', Categoria.NombreLongitudMaxima);

        var categoria = Categoria.Crear(nombre, null);

        Assert.Equal(nombre, categoria.Nombre);
    }

    [Fact]
    public void Crear_ShouldThrow_WhenDescripcionIsTooLong()
    {
        var descripcion = new string('a', Categoria.DescripcionLongitudMaxima + 1);

        Assert.Throws<ArgumentException>(() => Categoria.Crear("Bebidas", descripcion));
    }

    [Fact]
    public void CategoriaId_ShouldThrow_WhenValueIsNegative()
    {
        Assert.ThrowsAny<Exception>(() => CategoriaId.From(-1));
    }
}
