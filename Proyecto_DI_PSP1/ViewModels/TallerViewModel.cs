using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using Comunicacion;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Proyecto_DI_PSP1.Models;
using Proyecto_DI_PSP1.Servicios;

namespace Proyecto_DI_PSP1.ViewModels;

public class TallerViewModel : ViewModelBase
{
    private string? conexion;
    private string mensaje = "Introduce el usuario y la contraseña de MySQL.";
    private bool conectado;
    private bool calculando;

    public string Servidor { get; set; } = "localhost";
    public uint Puerto { get; set; } = 3306;
    public string BaseDatos { get; set; } = "taller_di_psp";
    public string Usuario { get; set; } = "root";
    public string Clave { private get; set; } = "";

    public bool Conectado
    {
        get => conectado;
        private set { conectado = value; AvisarCambio(); }
    }

    public bool Calculando
    {
        get => calculando;
        private set { calculando = value; AvisarCambio(); }
    }

    public string Mensaje
    {
        get => mensaje;
        private set { mensaje = value; AvisarCambio(); }
    }

    public CrudViewModel<Cliente> Clientes { get; }
    public CrudViewModel<Vehiculo> Vehiculos { get; }
    public CrudViewModel<Mecanico> Mecanicos { get; }
    public CrudViewModel<Reparacion> Reparaciones { get; }
    public CrudViewModel<Trabajo> Trabajos { get; }
    public CrudViewModel<Factura> Facturas { get; }
    public ObservableCollection<Incentivo> Incentivos { get; } = new();

    public string[] EstadosReparacion { get; } = ["Pendiente", "En curso", "Finalizada"];
    public string[] EstadosFactura { get; } = ["Pendiente", "Pagada"];
    public Comando Conectar => new(ConectarBase);
    public Comando Recargar => new(RecargarDatos);
    public Comando Calcular => new(async () => await CalcularIncentivoAsync());

    public TallerViewModel()
    {
        Clientes = CrearCrud<Cliente>(x => [x.IdCliente], (_, x) =>
            Texto(x.Nombre, "el nombre", 100) ?? Opcional(x.Telefono, "El teléfono", 20) ??
            Opcional(x.Email, "El correo", 100) ??
            (!string.IsNullOrWhiteSpace(x.Email) && !new EmailAddressAttribute().IsValid(x.Email)
                ? "Escribe un correo electrónico válido." : null));
        Vehiculos = CrearCrud<Vehiculo>(x => [x.IdVehiculo], (db, x) =>
            Texto(x.Matricula, "la matrícula", 20) ?? Texto(x.Marca, "la marca", 50) ??
            Texto(x.Modelo, "el modelo", 50) ??
            (!db.Clientes.Any(c => c.IdCliente == x.IdCliente) ? "Selecciona un cliente." : null) ??
            (db.Vehiculos.Any(v => v.Matricula == x.Matricula && v.IdVehiculo != x.IdVehiculo)
                ? "Ya existe un vehículo con esa matrícula." : null));
        Mecanicos = CrearCrud<Mecanico>(x => [x.IdMecanico], (_, x) =>
            Texto(x.Nombre, "el nombre", 100) ?? Texto(x.Especialidad, "la especialidad", 100));
        Reparaciones = CrearCrud<Reparacion>(x => [x.IdReparacion], (db, x) =>
            Texto(x.Descripcion, "la descripción", 255) ??
            (!db.Vehiculos.Any(v => v.IdVehiculo == x.IdVehiculo) ? "Selecciona un vehículo." : null) ??
            (!EstadosReparacion.Contains(x.Estado) ? "Selecciona el estado de la reparación." : null));
        Trabajos = CrearCrud<Trabajo>(x => [x.IdReparacion, x.IdMecanico], (db, x) =>
            (!db.Reparaciones.Any(r => r.IdReparacion == x.IdReparacion) ? "Selecciona una reparación." : null) ??
            (!db.Mecanicos.Any(m => m.IdMecanico == x.IdMecanico) ? "Selecciona un mecánico." : null) ??
            (x.HorasTrabajadas is <= 0 or > 999.99m ? "Las horas deben estar entre 0,01 y 999,99." : null) ??
            (x.CosteHora is <= 0 or > 999999.99m ? "Introduce un coste por hora válido." : null));
        Facturas = CrearCrud<Factura>(x => [x.IdFactura], (db, x) =>
            (!db.Reparaciones.Any(r => r.IdReparacion == x.IdReparacion) ? "Selecciona una reparación." : null) ??
            (!EstadosFactura.Contains(x.Resultado) ? "Selecciona el estado de la factura." : null) ??
            (x.Importe is <= 0 or > 999.99m ? "El importe debe estar entre 0,01 y 999,99." : null));
    }

    private CrudViewModel<T> CrearCrud<T>(Func<T, object[]> clave, Func<TallerContext, T, string?> validar)
        where T : class, new() => new(AbrirContexto, clave, validar, RecargarDatos, texto => Mensaje = texto);

    private static string? Texto(string? texto, string nombre, int maximo) =>
        string.IsNullOrWhiteSpace(texto) ? $"Escribe {nombre}." : Opcional(texto, nombre, maximo);

    private static string? Opcional(string? texto, string nombre, int maximo) =>
        texto?.Length > maximo ? $"Longitud máxima para {nombre}: {maximo} caracteres." : null;

    private TallerContext AbrirContexto() => new(conexion
        ?? throw new InvalidOperationException("Conecta primero con la base de datos."));

    private void ConectarBase()
    {
        try
        {
            var datos = new MySqlConnectionStringBuilder
            {
                Server = Servidor.Trim(),
                Port = Puerto,
                Database = BaseDatos.Trim(),
                UserID = Usuario.Trim(),
                Password = Clave,
                ConnectionTimeout = 4,
                PersistSecurityInfo = false
            };
            conexion = datos.ConnectionString;
            Conectado = false;
            Mensaje = "Conexión establecida.";
            RecargarDatos();
        }
        catch (ArgumentException)
        {
            Mensaje = "Revisa los datos de conexión.";
        }
    }

    private void RecargarDatos()
    {
        try
        {
            using var db = AbrirContexto();
            Clientes.Cargar(db);
            Vehiculos.Cargar(db);
            Mecanicos.Cargar(db);
            Reparaciones.Cargar(db);
            Trabajos.Cargar(db);
            Facturas.Cargar(db);
            CargarIncentivos(db);
            Conectado = true;
        }
        catch (Exception ex) when (ex is MySqlException or InvalidOperationException)
        {
            Conectado = false;
            Mensaje = "No se pudo cargar el taller. Comprueba el usuario, la contraseña y el script SQL.";
        }
    }

    private void CargarIncentivos(TallerContext db)
    {
        var resultados = db.Incentivos.AsNoTracking().OrderByDescending(i => i.Fecha).ToList();
        Incentivos.Clear();
        foreach (var resultado in resultados)
            Incentivos.Add(resultado);
    }

    private async Task CalcularIncentivoAsync()
    {
        if (Calculando)
            return;

        var seleccionado = Trabajos.Seleccionado;
        if (seleccionado is null)
        {
            Mensaje = "Selecciona una asignación de mecánico a reparación.";
            return;
        }

        Calculando = true;
        try
        {
            using var db = AbrirContexto();
            var trabajo = await db.Trabajos.FindAsync(seleccionado.IdReparacion, seleccionado.IdMecanico);
            var reparacion = await db.Reparaciones.FindAsync(seleccionado.IdReparacion);
            if (trabajo is null || reparacion is null)
            {
                Mensaje = "La asignación ya no existe. Vuelve a cargar los datos.";
                return;
            }

            var facturas = await db.Facturas.Where(f => f.IdReparacion == reparacion.IdReparacion)
                .Select(f => f.Resultado).ToListAsync();
            string estadoFactura = facturas.Count > 0 && facturas.All(f => f == "Pagada") ? "Pagada" : "Pendiente";
            var peticion = new PeticionIncentivo(Guid.NewGuid(), trabajo.IdMecanico, trabajo.IdReparacion,
                trabajo.HorasTrabajadas, trabajo.CosteHora, reparacion.Estado, estadoFactura);

            Mensaje = "Esperando la respuesta del procesador.";
            var respuesta = await ClienteIncentivos.EnviarAsync(peticion);
            var incentivo = await db.Incentivos.FindAsync(trabajo.IdReparacion, trabajo.IdMecanico);
            if (incentivo is null)
            {
                incentivo = new Incentivo { IdReparacion = trabajo.IdReparacion, IdMecanico = trabajo.IdMecanico };
                db.Incentivos.Add(incentivo);
            }
            incentivo.IdPeticion = respuesta.IdPeticion;
            incentivo.Fecha = DateTime.Now;
            incentivo.Importe = respuesta.Importe;
            incentivo.Estado = respuesta.Estado;
            incentivo.Detalle = respuesta.Detalle;
            await db.SaveChangesAsync();
            CargarIncentivos(db);
            Mensaje = $"{respuesta.Estado}. {respuesta.Detalle}";
        }
        catch (OperationCanceledException)
        {
            Mensaje = "El procesador no respondió a tiempo. Comprueba que la App 2 esté abierta.";
        }
        catch (Exception ex) when (ex is IOException or JsonException or MySqlException or DbUpdateException)
        {
            Mensaje = "No se pudo completar la petición. " + ex.Message;
        }
        finally
        {
            Calculando = false;
        }
    }
}
