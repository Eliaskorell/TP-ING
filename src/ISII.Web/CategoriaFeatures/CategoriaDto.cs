using ISII.Web.Domain.CategoriaAggregate;

namespace ISII.Web.CategoriaFeatures;
public record CategoriaDto(CategoriaId Id, string Nombre, string? Descripcion);
