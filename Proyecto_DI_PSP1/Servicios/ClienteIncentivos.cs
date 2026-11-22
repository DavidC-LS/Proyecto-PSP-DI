using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using Comunicacion;

namespace Proyecto_DI_PSP1.Servicios;

public static class ClienteIncentivos
{
    public static async Task<RespuestaIncentivo> EnviarAsync(PeticionIncentivo peticion)
    {
        using var limite = new CancellationTokenSource(TimeSpan.FromSeconds(8));
        using var pipe = new NamedPipeClientStream(".", CanalTaller.Nombre, PipeDirection.InOut,
            PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
        await pipe.ConnectAsync(limite.Token);

        using var escritor = new StreamWriter(pipe, leaveOpen: true) { AutoFlush = true };
        using var lector = new StreamReader(pipe, leaveOpen: true);
        await escritor.WriteLineAsync(JsonSerializer.Serialize(peticion).AsMemory(), limite.Token);

        string json = await lector.ReadLineAsync(limite.Token)
            ?? throw new IOException("El procesador cerró la conexión sin responder.");
        var respuesta = JsonSerializer.Deserialize<RespuestaIncentivo>(json);
        if (respuesta is null || respuesta.IdPeticion != peticion.IdPeticion)
            throw new IOException("La respuesta no corresponde a la petición enviada.");

        return respuesta;
    }
}
