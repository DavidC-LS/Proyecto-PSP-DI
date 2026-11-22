using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

public class TallerContext(string conexion) : DbContext
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<Mecanico> Mecanicos => Set<Mecanico>();
    public DbSet<Reparacion> Reparaciones => Set<Reparacion>();
    public DbSet<Trabajo> Trabajos => Set<Trabajo>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Incentivo> Incentivos => Set<Incentivo>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseMySql(conexion, new MySqlServerVersion(new Version(8, 0, 0)));
}
