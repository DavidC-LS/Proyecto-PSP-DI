using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Reparacion_Mecanico")]
[PrimaryKey(nameof(IdReparacion), nameof(IdMecanico))]
public class Trabajo
{
    [Column("id_reparacion")]
    public int IdReparacion { get; set; }

    [Column("id_mecanico")]
    public int IdMecanico { get; set; }

    [Column("horas_trabajadas"), Precision(5, 2)]
    public decimal HorasTrabajadas { get; set; }

    [Column("coste_hora"), Precision(8, 2)]
    public decimal CosteHora { get; set; }
}
