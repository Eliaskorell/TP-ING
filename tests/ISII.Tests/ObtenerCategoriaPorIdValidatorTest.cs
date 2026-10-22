using ISII.Web.CategoriaFeatures.ObtenerPorId;

namespace ISII.Tests;

public class ObtenerCategoriaPorIdValidatorTest
{
    private readonly ObtenerCategoriaPorIdValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsPositive()
    {
        var request = new ObtenerCategoriaPorIdRequest { CategoriaId = 1 };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldFail_WhenIdIsNotPositive(int id)
    {
        var request = new ObtenerCategoriaPorIdRequest { CategoriaId = id };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
