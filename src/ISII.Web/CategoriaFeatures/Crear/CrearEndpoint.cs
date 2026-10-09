using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ISII.Web.Domain.CategoriaAggregate;
using ISII.Web.Extensions;

namespace ISII.Web.CategoriaFeatures.Crear;

public sealed class CrearCategoriaRequest
{
  public string Nombre { get; init; } = string.Empty;
  public string? Descripcion { get; init; }
}

public class CrearEndpoint(IMediator mediator)
  : Endpoint<CrearCategoriaRequest,
             Results<Created<CategoriaRecord>, ValidationProblem, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post("/Categorias");
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Crear una categoria nueva";
      s.Description = "Crea una categoria con un nombre y una descripcion opcional. El nombre no se puede repetir entre categorias.";
      s.ExampleRequest = new CrearCategoriaRequest { Nombre = "Bebidas", Descripcion = "Gaseosas, jugos y aguas" };
      s.ResponseExamples[201] = new CategoriaRecord(1, "Bebidas", "Gaseosas, jugos y aguas");

      s.Responses[201] = "Categoria creada correctamente";
      s.Responses[400] = "Datos invalidos";
      s.Responses[409] = "Ya existe una categoria con ese nombre";
    });

    Tags("Categorias");

    Description(builder => builder
      .Accepts<CrearCategoriaRequest>()
      .Produces<CategoriaRecord>(201, "application/json")
      .ProducesProblem(400)
      .ProducesProblem(409));
  }

  public override async Task<Results<Created<CategoriaRecord>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CrearCategoriaRequest request, CancellationToken ct)
  {
    var command = new CrearCategoriaCommand(request.Nombre, request.Descripcion);
    var result = await mediator.Send(command, ct);

    return result.ToCreatedResult(
      dto => $"/Categorias/{dto.Id.Value}",
      dto => new CategoriaRecord(dto.Id.Value, dto.Nombre, dto.Descripcion));
  }
}

public sealed class CrearCategoriaValidator : Validator<CrearCategoriaRequest>
{
  public CrearCategoriaValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty()
      .WithMessage("El nombre es obligatorio")
      .MaximumLength(Categoria.NombreLongitudMaxima)
      .WithMessage($"El nombre no puede superar los {Categoria.NombreLongitudMaxima} caracteres");

    RuleFor(x => x.Descripcion)
      .MaximumLength(Categoria.DescripcionLongitudMaxima)
      .WithMessage($"La descripcion no puede superar los {Categoria.DescripcionLongitudMaxima} caracteres")
      .When(x => x.Descripcion != null);
  }
}
