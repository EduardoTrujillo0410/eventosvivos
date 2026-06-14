using FluentValidation;
using EventosVivos.Application.Common;

namespace EventosVivos.Application.Features.Eventos.Commands.CrearEvento;

public class CrearEventoValidator : AbstractValidator<CrearEventoCommand>
{
    public CrearEventoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .Length(5, 100).WithMessage("El título debe tener entre 5 y 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .Length(10, 500).WithMessage("La descripción debe tener entre 10 y 500 caracteres.");

        RuleFor(x => x.VenueId)
            .GreaterThan(0).WithMessage("El venue es obligatorio.");

        RuleFor(x => x.CapacidadMaxima)
            .GreaterThan(0).WithMessage("La capacidad máxima debe ser un entero positivo.");

        RuleFor(x => x.FechaInicio)
            .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de inicio debe ser futura.");

        RuleFor(x => x.FechaFin)
            .GreaterThan(x => x.FechaInicio).WithMessage("La fecha de fin debe ser posterior al inicio.");

        RuleFor(x => x.PrecioEntrada)
            .GreaterThan(0).WithMessage("El precio debe ser un decimal positivo.");

        RuleFor(x => x.Tipo)
            .NotEmpty()
            .Must(t => new[] { "conferencia", "taller", "concierto" }.Contains(t.ToLower()))
            .WithMessage("El tipo debe ser: conferencia, taller o concierto.");

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .Length(5, 100)
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ0-9\s\-_.,!?()]+$")
            .WithMessage("El título contiene caracteres no permitidos.");

        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .Length(10, 500)
            .Must(d => !SecurityValidations.ContienePatronesSQL(d))
            .WithMessage("La descripción contiene contenido no permitido.");
    }
}