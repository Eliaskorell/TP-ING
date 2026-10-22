using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Extensions;

namespace ISII.Web.CategoriaFeatures.ObtenerPorId;

public sealed class ObtenerCategoriaPorIdRequest
{
  public const string Route = "/Categorias/{CategoriaId}";
  public int CategoriaId { get; init; }
}

public class ObtenerPorIdEndpoint(IMediator mediator)
  : Endpoint<ObtenerCategoriaPorIdRequest,
             Results<Ok<CategoriaRecord>, NotFound, ProblemHttpResult>,
             ObtenerCategoriaPorIdMapper>
{
  public override void Configure()
  {
    Get(ObtenerCategoriaPorIdRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Obtener una categoria por su Id";
      s.Description = "Devuelve el Id, el nombre y la descripcion de la categoria indicada.";
      s.ExampleRequest = new ObtenerCategoriaPorIdRequest { CategoriaId = 1 };
      s.ResponseExamples[200] = new CategoriaRecord(1, "Bebidas", "Gaseosas, jugos y aguas");

      s.Responses[200] = "Categoria encontrada";
      s.Responses[400] = "Id invalido";
      s.Responses[404] = "No existe una categoria con ese Id";
    });

    Tags("Categorias");

    Description(builder => builder
      .Accepts<ObtenerCategoriaPorIdRequest>()
      .Produces<CategoriaRecord>(200, "application/json")
      .ProducesProblem(400)
      .ProducesProblem(404));
  }

  public override async Task<Results<Ok<CategoriaRecord>, NotFound, ProblemHttpResult>>
    ExecuteAsync(ObtenerCategoriaPorIdRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(new ObtenerCategoriaQuery(CategoriaId.From(request.CategoriaId)), ct);

    return result.ToGetByIdResult(Map.FromEntity);
  }
}

public sealed class ObtenerCategoriaPorIdValidator : Validator<ObtenerCategoriaPorIdRequest>
{
  public ObtenerCategoriaPorIdValidator()
  {
    RuleFor(x => x.CategoriaId)
      .GreaterThan(0)
      .WithMessage("El Id de la categoria debe ser mayor a 0");
  }
}

public sealed class ObtenerCategoriaPorIdMapper
  : Mapper<ObtenerCategoriaPorIdRequest, CategoriaRecord, CategoriaDto>
{
  public override CategoriaRecord FromEntity(CategoriaDto e)
    => new(e.Id.Value, e.Nombre, e.Descripcion);
}
