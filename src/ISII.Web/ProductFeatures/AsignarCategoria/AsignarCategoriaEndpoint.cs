using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Domain.ProductAggregate;
using ISII.Web.Extensions;

namespace ISII.Web.ProductFeatures.AsignarCategoria;

public sealed class AsignarCategoriaRequest
{
  public const string Route = "/Products/{ProductId}/Categoria";

  public int ProductId { get; init; }
  public int CategoriaId { get; init; }
}

public class AsignarCategoriaEndpoint(IMediator mediator)
  : Endpoint<AsignarCategoriaRequest,
             Results<Ok<ProductRecord>,
                     NotFound,
                     ProblemHttpResult>>
{
  public override void Configure()
  {
    Put(AsignarCategoriaRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Asignar una categoria a un producto";
      s.Description = "Asigna la categoria indicada al producto. Falla con 404 si el producto o la categoria no existen.";
      s.ExampleRequest = new AsignarCategoriaRequest { ProductId = 1, CategoriaId = 1 };
      s.ResponseExamples[200] = new ProductRecord(1, "Laptop", 999.99m, 1);

      s.Responses[200] = "Categoria asignada correctamente";
      s.Responses[404] = "El producto o la categoria no existen";
      s.Responses[400] = "Datos invalidos";
    });

    Tags("Products");

    Description(builder => builder
      .Accepts<AsignarCategoriaRequest>()
      .Produces<ProductRecord>(200, "application/json")
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<Ok<ProductRecord>, NotFound, ProblemHttpResult>>
    ExecuteAsync(AsignarCategoriaRequest request, CancellationToken ct)
  {
    var command = new AsignarCategoriaCommand(ProductId.From(request.ProductId), CategoriaId.From(request.CategoriaId));
    var result = await mediator.Send(command, ct);

    return result.ToUpdateResult(
      dto => new ProductRecord(dto.Id.Value, dto.Name, dto.UnitPrice, dto.CategoriaId?.Value));
  }
}

public sealed class AsignarCategoriaValidator : Validator<AsignarCategoriaRequest>
{
  public AsignarCategoriaValidator()
  {
    RuleFor(x => x.ProductId)
      .GreaterThan(0)
      .WithMessage("El Id del producto debe ser mayor a 0");

    RuleFor(x => x.CategoriaId)
      .GreaterThan(0)
      .WithMessage("El Id de la categoria debe ser mayor a 0");
  }
}
