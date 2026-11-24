using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Vehiculos")]
public class Vehiculo
{
    [Key, Column("id_vehiculo")]
    public int IdVehiculo { get; set; }

    [Column("matricula"), MaxLength(20)]
    public string Matricula { get; set; } = "";

    [Column("marca"), MaxLength(50)]
    public string Marca { get; set; } = "";

    [Column("modelo"), MaxLength(50)]
    public string Modelo { get; set; } = "";

    [Column("id_clientes")]
    public int IdCliente { get; set; }
}
