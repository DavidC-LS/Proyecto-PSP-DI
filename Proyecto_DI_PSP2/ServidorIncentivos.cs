using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using Comunicacion;

namespace Proyecto_DI_PSP2;

public class ServidorIncentivos(string archivo, List<RegistroPeticion> historial)
{
    private const decimal Porcentaje = 0.10m;
    public event Action<RegistroPeticion>? Registrada;

    public async Task EscucharAsync(CancellationToken cancelacion)
    {
        while (!cancelacion.IsCancellationRequested)
        {
            using var pipe = new NamedPipeServerStream(CanalTaller.Nombre, PipeDirection.InOut,
                1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
            await pipe.WaitForConnectionAsync(cancelacion);
            try
            {
                await ProcesarConexionAsync(pipe, cancelacion);
            }
            catch (IOException)
            {
                // Si el cliente se desconecta, la siguiente iteración acepta otra petición.
            }
        }
    }

    private async Task ProcesarConexionAsync(NamedPipeServerStream pipe, CancellationToken cancelacion)
    {
        using var lector = new StreamReader(pipe, leaveOpen: true);
        using var escritor = new StreamWriter(pipe, leaveOpen: true) { AutoFlush = true };
        string? mensaje = await lector.ReadLineAsync(cancelacion);
        if (mensaje is null)
            return;

        PeticionIncentivo? peticion = null;
        RespuestaIncentivo respuesta;
        string? error = null;
        try
        {
            peticion = JsonSerializer.Deserialize<PeticionIncentivo>(mensaje);
            respuesta = Calcular(peticion);
        }
        catch (JsonException ex)
        {
            respuesta = new(Guid.Empty, "Error de validación", 0, "El JSON de la petición no es válido.");
            error = ex.Message;
        }

        var registro = new RegistroPeticion(DateTime.Now, respuesta.IdPeticion, "Calcular incentivo",
            respuesta.Estado, respuesta.Detalle, error, peticion);

        try
        {
            GuardarRegistro(registro);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            respuesta = new(respuesta.IdPeticion, "Error técnico", 0, "No se pudo guardar el historial.");
            registro = registro with { Estado = respuesta.Estado, Detalle = respuesta.Detalle, ErrorTecnico = ex.Message };
        }
        Registrada?.Invoke(registro);
        await escritor.WriteLineAsync(JsonSerializer.Serialize(respuesta).AsMemory(), cancelacion);
    }

    private static RespuestaIncentivo Calcular(PeticionIncentivo? peticion)
    {
        if (peticion is null || peticion.IdPeticion == Guid.Empty || peticion.IdMecanico <= 0 ||
            peticion.IdReparacion <= 0 || peticion.HorasTrabajadas is <= 0 or > 999.99m ||
            peticion.CosteHora is <= 0 or > 999999.99m ||
            string.IsNullOrWhiteSpace(peticion.EstadoReparacion) || string.IsNullOrWhiteSpace(peticion.EstadoFactura))
        {
            return new(peticion?.IdPeticion ?? Guid.Empty, "Error de validación", 0,
                "Revisa el identificador, las horas, el coste por hora y los estados.");
        }

        bool finalizada = string.Equals(peticion.EstadoReparacion, "Finalizada", StringComparison.OrdinalIgnoreCase);
        bool pagada = string.Equals(peticion.EstadoFactura, "Pagada", StringComparison.OrdinalIgnoreCase);
        if (!finalizada || !pagada)
            return new(peticion.IdPeticion, "Sin incentivo", 0, "La reparación debe estar finalizada y pagada.");

        decimal importe = Math.Round(peticion.HorasTrabajadas * peticion.CosteHora * Porcentaje, 2,
            MidpointRounding.AwayFromZero);
        return new(peticion.IdPeticion, "Calculada", importe, $"10 % de la mano de obra: {importe:C2}");
    }

    private void GuardarRegistro(RegistroPeticion registro)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(archivo)!);
        string temporal = archivo + ".tmp";
        // Se sustituye el historial completo después de escribir el temporal.
        var datos = new List<RegistroPeticion>(historial) { registro };
        File.WriteAllText(temporal, JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporal, archivo, overwrite: true);
        historial.Add(registro);
    }
}
