using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Infrastructure.Data;

namespace ISII.Tests;

public class CategoriaIndiceUnicoTest
{
    [Fact]
    public async Task SaveChanges_ShouldReject_WhenNombreAlreadyExistsWithDifferentCase()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.Categorias.Add(new Categoria(CategoriaId.Nueva, "Bebidas", null));
        await context.SaveChangesAsync();

        context.Categorias.Add(new Categoria(CategoriaId.Nueva, "bebidas", null));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}
