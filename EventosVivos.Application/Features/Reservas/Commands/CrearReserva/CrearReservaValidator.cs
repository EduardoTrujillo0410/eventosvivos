using FluentValidation;

namespace EventosVivos.Application.Features.Reservas.Commands.CrearReserva;

public class CrearReservaValidator : AbstractValidator<CrearReservaCommand>
{
    public CrearReservaValidator()
    {
        RuleFor(x => x.EventoId)
            .NotEmpty().WithMessage("El eventoId es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThanOrEqualTo(1).WithMessage("La cantidad debe ser 1 o más.");

        RuleFor(x => x.NombreComprador)
            .NotEmpty().WithMessage("El nombre del comprador es obligatorio.");

        RuleFor(x => x.EmailComprador)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");
    }
}