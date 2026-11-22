using Comunicacion;

namespace Proyecto_DI_PSP2;

public record RegistroPeticion(DateTime Fecha, Guid IdPeticion, string Operacion,
    string Estado, string Detalle, string? ErrorTecnico, PeticionIncentivo? Peticion);
