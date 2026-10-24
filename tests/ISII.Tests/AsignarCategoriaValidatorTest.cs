using ISII.Web.ProductFeatures.AsignarCategoria;

namespace ISII.Tests;

public class AsignarCategoriaValidatorTest
{
    private readonly AsignarCategoriaValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenBothIdsArePositive()
    {
        var request = new AsignarCategoriaRequest { ProductId = 1, CategoriaId = 1 };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void Validate_ShouldFail_WhenEitherIdIsNotPositive(int productId, int categoriaId)
    {
        var request = new AsignarCategoriaRequest { ProductId = productId, CategoriaId = categoriaId };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
