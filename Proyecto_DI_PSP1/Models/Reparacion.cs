using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Reparaciones")]
public class Reparacion
{
    [Key, Column("id_reparacion")]
    public int IdReparacion { get; set; }

    [Column("id_vehiculo")]
    public int IdVehiculo { get; set; }

    [Column("fecha", TypeName = "date")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Column("descripcion"), MaxLength(255)]
    public string Descripcion { get; set; } = "";

    [Column("estado"), MaxLength(30)]
    public string Estado { get; set; } = "";
}
