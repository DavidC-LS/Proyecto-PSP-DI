using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Incentivos")]
[PrimaryKey(nameof(IdReparacion), nameof(IdMecanico))]
public class Incentivo
{
    [Column("id_reparacion")]
    public int IdReparacion { get; set; }

    [Column("id_mecanico")]
    public int IdMecanico { get; set; }

    [Column("id_peticion", TypeName = "char(36)")]
    public Guid IdPeticion { get; set; }

    [Column("fecha", TypeName = "date")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Column("importe"), Precision(11, 2)]
    public decimal Importe { get; set; }

    [Column("estado"), MaxLength(30)]
    public string Estado { get; set; } = "";

    [Column("detalle"), MaxLength(255)]
    public string Detalle { get; set; } = "";
}
