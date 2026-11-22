namespace Comunicacion;

public record PeticionIncentivo(Guid IdPeticion, int IdMecanico, int IdReparacion,
    decimal HorasTrabajadas, decimal CosteHora, string EstadoReparacion, string EstadoFactura);

public record RespuestaIncentivo(Guid IdPeticion, string Estado, decimal Importe, string Detalle);

public static class CanalTaller
{
    public const string Nombre = "Proyecto_DI_PSP_Taller";
}
